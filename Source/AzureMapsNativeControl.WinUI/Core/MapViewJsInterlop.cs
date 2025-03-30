using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Source;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;

#if MAUI
#elif WPF
#elif WINUI
using Microsoft.UI.Xaml.Controls;
#else
#endif

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Interloper class for managing the interaction with a webpage that interacts with the Azure Maps JavaScript map control.
    /// You should only need this if you are extending the Azure Maps control, for example adding custom controls and need to be able to call into the map control from the webpage.
    /// </summary>
    public class MapViewJsInterlop
    {
        #region Private Properties

        //private readonly Map _map;

        internal IMapView _view;

        internal IList<Map> _maps;

        internal bool IsHtmlLoaded = false;

        internal readonly HybridWebView.HybridWebView _webView;

        private Dictionary<string, TaskCompletionSource<string>> asyncTaskCallbacks = new Dictionary<string, TaskCompletionSource<string>>();

        private List<string> _loadedWebResources = new List<string>();

        internal Dictionary<string, bool> _loadedModules = new Dictionary<string, bool>();

        private AzureMapsConfiguration? _azMapsConfig = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Interloper class for managing the interaction with a webpage that interacts with the Azure Maps JavaScript map control.
        /// You sould only need this if you are extending the Azure Maps control, for example adding custom controls and need to be able to call into the map control from the webpage.
        /// </summary>
        /// <param name="view">The map view control to render.</param>
        public MapViewJsInterlop(IMapView view)
        {
            _view = view;

            bool enableWebDevTools = false;

#if DEBUG
            //Enable web dev tools when in debug mode.
            enableWebDevTools = true;
#endif

            //Create a web view control.
            _webView = new HybridWebView.HybridWebView
            {
                HybridAssetRoot = Constants.MapWebViewHybridAssetRoot,
                MainFile = "proxy",
                EnableWebDevTools = enableWebDevTools
            };

#if WINUI
            _webView.OnHandlerChanged();
            _webView.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
            _webView.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch;
#elif WPF
            _webView.OnHandlerChanged();
            _webView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _webView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
#endif

            _webView.JSInvokeTarget = this;
            _webView.ProxyRequestReceived += WebView_ProxyRequestReceived;

#if WINDOWS || WPF || WINUI
            //In Windows, disable manual user zooming of web pages. 
            _webView.HybridWebViewInitialized += (s, e) =>
            {
                //Disable the user manually zooming. Don't want the user accidentally zooming the HTML page.
                e.WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            };
#endif

            if (view is Map map)
            {
                _maps = new List<Map>() { map };
            } 
            else
            {
                _maps = new List<Map>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a Web Resource from a URL into the map view page.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourceType"></param>
        public async void AddWebResource(string url, WebResourceType resourceType = WebResourceType.Script)
        {
            await AddWebResourceAsync(url, resourceType);
        }

        /// <summary>
        /// Loads a Web Resource from a URL into the map view page.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourceType"></param>
        public async Task AddWebResourceAsync(string url, WebResourceType resourceType = WebResourceType.Script)
        {
            if (string.IsNullOrEmpty(url) && !_loadedWebResources.Contains(url))
            {
                return;
            }
            
            _loadedWebResources.Add(url);

            var resourceTypeStr = resourceType.ToString().ToLower();
            await InvokeJsMethodAsync("MapUtils.loadResource", url, resourceTypeStr);
            
        }

        /// <summary>
        /// Adds raw CSS to the map view page.
        /// </summary>
        /// <param name="css"></param>
        public async void AddRawCss(string css)
        {
            await AddRawCssAsync(css);
        }

        /// <summary>
        /// Adds raw CSS to the map view page.
        /// </summary>
        /// <param name="css"></param>
        /// <returns></returns>
        public async Task AddRawCssAsync(string css)
        {
            if (string.IsNullOrEmpty(css))
            {
                return;
            }

            await InvokeJsMethodAsync("MapUtils.injectCss", css);
        }

        /// <summary>
        /// Loads a map module into the map view page.
        /// </summary>
        /// <param name="mapModuleInfo"></param>
        /// <returns></returns>
        public async Task LoadModule(MapModuleInfo mapModuleInfo)
        {
            if (!_loadedModules.ContainsKey(mapModuleInfo.Name))
            {
                //Keep track of which modules have been loaded.
                _loadedModules.Add(mapModuleInfo.Name, false);

                await InvokeJsMethodAsync<bool>("MapUtils.loadModule", mapModuleInfo.Name, mapModuleInfo.JsResources, mapModuleInfo.CssResources);
                
                _loadedModules[mapModuleInfo.Name] = true;
            } 
            else if(!_loadedModules[mapModuleInfo.Name])
            {
                //If we get here, the module is still loading. Wait for it to finish.
                while (!_loadedModules[mapModuleInfo.Name])
                {
                    await Task.Delay(100);
                }
            }
        }

        /// <summary>
        /// Checks to see if a module is loaded.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return _loadedModules.ContainsKey(moduleName) && _loadedModules[moduleName];
        }

        /// <summary>
        /// Handler for when the page is loaded. Do not call this method directly.
        /// </summary>
        public async void PageLoaded()
        {
            IsHtmlLoaded = true;

            if(_azMapsConfig == null)
            {
                _azMapsConfig = AzureMapsConfiguration.GetInstance();
            }

            //The page is loaded. Initialize the view.
            await _view.InitView(_azMapsConfig);
        }

        /// <summary>
        /// Handler for when the page is unloaded. Do not call this method directly.
        /// </summary>
        public void PageUnloaded()
        {
            IsHtmlLoaded = false;

            //The page is unloaded. Let the map view know.
            _view.WebPageUnloaded();

            _loadedWebResources.Clear();

            _loadedModules.Clear();
        }

        /// <summary>
        /// Handler for when the map triggers an event. Do not call this method directly.
        /// </summary>
        /// <param name="eventData"></param>
        public void EventTriggered(string eventData)
        {
            var eventArgs = JsonSerializer.Deserialize<RawMapMsg>(eventData);
            if (eventArgs != null)
            {
                Map map = _maps.Where(m => m.Id == eventArgs.MapId).First();

                map.Events.EventTriggered(eventArgs);
            }
        }

        /// <summary>
        /// Handler for when the an Async JavaScript task has completed and needs to notify .NET.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="result">The JSON string result.</param>
        public void AsyncTaskCompleted(string taskId, string result)
        {
            //Look for the callback in the list of pending callbacks.
            if (!string.IsNullOrEmpty(taskId) && asyncTaskCallbacks.ContainsKey(taskId))
            {
                //Get the callback and remove it from the list.
                var callback = asyncTaskCallbacks[taskId];
                callback.SetResult(result);

                //Remove the callback.
                asyncTaskCallbacks.Remove(taskId);
            }
        }

        /// <summary>
        /// Handler for retrieving the auth token.
        /// </summary>
        /// <returns>string token or empty string</returns>
        public async Task<string> AuthTokenRequested()
        {
            try
            {
                if(_azMapsConfig == null)
                {
                    _azMapsConfig = AzureMapsConfiguration.GetInstance();
                }

                if (_azMapsConfig.GetTokenAsync != null)
                {
                    return await _azMapsConfig.GetTokenAsync(_azMapsConfig);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting auth token: {ex.Message}");
            }

            Debug.WriteLine("Warning: Map does not set GetTokenAsync in AuthOptions.");

            return string.Empty;
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>A string containing the return value of the called method.</returns>
        public async Task<string> InvokeJsMethodAsync(string methodName, params object?[] paramValues)
        {
            return await InvokeJsMethodAsync(Constants.MapJsonSerializerOptions, methodName, paramValues);
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// </summary>
        /// <param name="jsonSerializerOptions">Custom JSON serialization options. </param>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>A string containing the return value of the called method.</returns>
        public async Task<string> InvokeJsMethodAsync(JsonSerializerOptions jsonSerializerOptions, string methodName, params object?[] paramValues)
        {
            try
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    throw new ArgumentException($"The JS method name cannot be null or empty.");
                }

                //Create a callback.
                var callback = new TaskCompletionSource<string>();

                var taskId = UniqueId.Get("asyncMapTask");

                asyncTaskCallbacks.Add(taskId, callback);

                string paramJson = string.Empty;

                if (paramValues != null && paramValues.Length > 0)
                {
                    paramJson = string.Join(", ", paramValues.Select(v => JsonSerializer.Serialize(v, jsonSerializerOptions)));

                    //New line characters do not currently work with EvaluateJavaScriptAsync. https://github.com/dotnet/maui/issues/11905
                    //We don't need formatted JSON, and we don't want new line characters in string properties as that's likely to cause issues anyways.
                    paramJson = paramJson.Replace("\r\n", " ").Replace("\n", " ");

                    //Base64 encode parameters to workaround some known issues in the WebView2 control. (html strings and special characters cause EvaluateJavaScriptAsync to fail silently.
                    paramJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(paramJson));
                }

                await _webView.EvaluateJavaScriptAsync($"(async () => {{ var result; try {{ var paramJson = JSON.parse('[' + atob('{paramJson}') + ']'); result = ({methodName}[Symbol.toStringTag] === 'AsyncFunction') ? await {methodName}(...paramJson) : {methodName}(...paramJson); }} catch(e) {{ console.error(e) }} MapUtils.triggerAsyncCallback('{taskId}', result); }})()");

                return await callback.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error invoking JS method: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value to deserialize from JSON.</typeparam>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>An object of type <typeparamref name="TReturnType"/> containing the return value of the called method.</returns>
        public async Task<TReturnType?> InvokeJsMethodAsync<TReturnType>(string methodName, params object?[] paramValues)
        {
            try
            {
                var stringResult = await InvokeJsMethodAsync(methodName, paramValues);

                if (string.IsNullOrWhiteSpace(stringResult) || stringResult.Equals("null") || stringResult.Equals("{}"))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<TReturnType>(stringResult, Constants.MapJsonSerializerOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error invoking async method: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <param name="map">The map instance to interact with.</param>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>A string containing the return value of the called method.</returns>
        public async Task<string> InvokeJsMethodAsync(Map map, string methodName, params object?[] paramValues)
        {
            return await InvokeJsMethodAsync(map, Constants.MapJsonSerializerOptions, methodName, paramValues);
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// </summary>
        /// <param name="map">The map instance to interact with.</param>
        /// <param name="jsonSerializerOptions">Custom JSON serialization options.</param>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>A string containing the return value of the called method.</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> InvokeJsMethodAsync(Map map, JsonSerializerOptions jsonSerializerOptions, string methodName, params object?[] paramValues)
        {
            if (map != null)
            {
                methodName = $"getMapInterface('{map.Id}').{methodName}";
            }

            return await InvokeJsMethodAsync(jsonSerializerOptions, methodName, paramValues);
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value to deserialize from JSON.</typeparam>
        /// <param name="map">The map instance to interact with.</param>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>An object of type <typeparamref name="TReturnType"/> containing the return value of the called method.</returns>
        public async Task<TReturnType?> InvokeJsMethodAsync<TReturnType>(Map map, string methodName, params object?[] paramValues)
        {
            if (map != null)
            {
                methodName = $"getMapInterface('{map.Id}').{methodName}";
            }

            return await InvokeJsMethodAsync<TReturnType>(methodName, paramValues);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Proxy request handler for the WebView. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task WebView_ProxyRequestReceived(HybridWebView.HybridWebViewProxyEventArgs args)
        {
            //If proxy is called without any parameters and the main map file hasn't been loaded, then load the main html file.
            if (args.QueryParams.Count == 0 && !IsHtmlLoaded)
            {
                await EmbeddedResourceProvider.LoadResource(_view.MapViewFileName, args);
                IsHtmlLoaded = true;
            }
            else if (args.QueryParams.ContainsKey("operation")) //Check to see if an operation is defined.
            {
                switch (args.QueryParams["operation"])
                {
                    case "embeddedResource":  //This is only used internally.

                        if (args.QueryParams.TryGetValue("resourceName", out string? resourceName) && !string.IsNullOrWhiteSpace(resourceName))
                        {
                            try
                            {
                                var thisAssembly = typeof(Map).Assembly;
                                var assemblyName = thisAssembly.GetName().Name;
                                using (var fs = thisAssembly.GetManifestResourceStream($"{assemblyName}.EmbeddedResources.{resourceName.Replace("/", ".")}"))
                                {
                                    if (fs != null)
                                    {
                                        var ms = new MemoryStream();
                                        await fs.CopyToAsync(ms);
                                        ms.Position = 0;

                                        args.ResponseStream = ms;
                                        args.ResponseContentType = Utils.GetMimeType(resourceName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error loading embedded resource via map proxy: " + ex.Message);
                            }
                        }

                        //Since this will run when the map first loads. Use this to update the user agent of the MapHttpClientManager
                        if (!string.IsNullOrWhiteSpace(_webView.UserAgent) && !_webView.UserAgent.Equals(MapHttpClientManager.UserAgent))
                        {
                            MapHttpClientManager.UserAgent = _webView.UserAgent;
                        }
                        break;
                    case Constants.ProxyWebRequestOperation:
                        if (args.QueryParams.TryGetValue("url", out string? url) && !string.IsNullOrWhiteSpace(url))
                        {
                            var response = await MapHttpClientManager.TryGetStreamAsync(url);

                            if (response != null)
                            {
                                args.ResponseStream = response.Stream;
                                args.ResponseContentType = response.MimeType;
                                //TODO: May need to add support for response headers in the future.
                            }
                        }
                        break;
                    case Constants.CustomTileSourceOperation:
                        var tileSourceId = args.QueryParams["tileSourceId"];

                        BaseSource? ts = null;

                        //Search through the maps to find the tile source.
                        for (int i = 0; i < _maps.Count; i++)
                        {
                            ts = _maps[i].Sources.GetById(tileSourceId);

                            if(ts != null)
                            {
                                break;
                            }
                        }

                        if (ts != null && ts is CustomTileSource customTileSource)
                        {
                            var tileInfo = Utils.GetTileInfoFromProxyRequest(args.QueryParams);

                            if (tileInfo != null)
                            {
                                var tileStreamInfo = await customTileSource.GetTileStream(tileInfo);

                                if (tileStreamInfo != null)
                                {
                                    args.ResponseStream = tileStreamInfo.Stream;
                                    args.ResponseContentType = tileStreamInfo.MimeType;
                                    //TODO: May need to add support for response headers in the future.
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        
        #endregion
    }
}
