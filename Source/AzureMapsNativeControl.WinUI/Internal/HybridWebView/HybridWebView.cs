using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using AzureMapsNativeControl.Internal;
using System.Windows;

#if WINUI
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
#elif WPF 
using Microsoft.Web.WebView2.Wpf;
#endif

namespace HybridWebView
{
    internal class HybridWebView : WebView2
    {
        // Using an IP address means that WebView2 doesn't wait for any DNS resolution,
        // making it substantially faster. Note that this isn't real HTTP traffic, since
        // we intercept all the requests within this origin.
        private static readonly string AppHostAddress = "0.0.0.1";

        /// <summary>
        /// Gets the application's base URI. Defaults to <c>https://0.0.0.1/</c>
        /// </summary>
        private static readonly string AppOrigin = $"https://{AppHostAddress}/";

        private static readonly Uri AppOriginUri = new(AppOrigin);

        private CoreWebView2Environment? _coreWebView2Environment;

        internal const string ProxyRequestPath = "proxy";

        private const string HybridWebViewHelperFileName = "HybridWebView.js";

        /// <summary>
        /// Specifies the file within the <see cref="HybridAssetRoot"/> that should be served as the main file. The
        /// default value is <c>index.html</c>.
        /// </summary>
        public string? MainFile { get; set; } = "index.html";

        /// <summary>
        /// Gets or sets the path for initial navigation after the content is finished loading. The default value is <c>/</c>.
        /// </summary>
        public string StartPath { get; set; } = "/";

        /// <summary>
        ///  The path within the app's "Raw" asset resources that contain the web app's contents. For example, if the
        ///  files are located in "ProjectFolder/Resources/Raw/hybrid_root", then set this property to "hybrid_root".
        /// </summary>
        public string? HybridAssetRoot { get; set; }

        /// <summary>
        /// The target object for JavaScript method invocations. When an "invoke" message is sent from JavaScript,
        /// the invoked method will be located on this object, and any specified parameters will be passed in.
        /// </summary>
        public object? JSInvokeTarget { get; set; }

        /// <summary>
        /// Enables web developers tools (such as "F12 web dev tools inspectors")
        /// </summary>
        public bool EnableWebDevTools { get; set; }

        /// <summary>
        /// Gets the user agent of the Web View.
        /// </summary>
        public string UserAgent
        {
            get
            {
                return CoreWebView2.Settings.UserAgent;
            }
        }

        /// <summary>
        /// Raised when a raw message is received from the web view. Raw messages are strings that have no additional processing.
        /// </summary>
        public event EventHandler<HybridWebViewRawMessageReceivedEventArgs>? RawMessageReceived;

        /// <summary>
        /// Async event handler that is called when a proxy request is received from the web view.
        /// </summary>
        public event Func<HybridWebViewProxyEventArgs, Task>? ProxyRequestReceived;

        /// <summary>
        /// Raised after the web view is initialized but before any content has been loaded into the web view. The event arguments provide the instance of the platform-specific web view control.
        /// </summary>
        public event EventHandler<HybridWebViewInitializedEventArgs>? HybridWebViewInitialized;

        public void Navigate(string url)
        {
            NavigateCore(url);
        }

        internal async void OnHandlerChanged()
        {
            await InitializeHybridWebView();

            HybridWebViewInitialized?.Invoke(this, new HybridWebViewInitializedEventArgs()
            {
                WebView = this
            });

            Navigate(StartPath);
        }

        private async Task InitializeHybridWebView()
        {
            WebMessageReceived += Wv2_WebMessageReceived;

            _coreWebView2Environment = await CoreWebView2Environment.CreateAsync();

            await EnsureCoreWebView2Async();

            CoreWebView2.Settings.AreDevToolsEnabled = EnableWebDevTools;
            CoreWebView2.Settings.IsWebMessageEnabled = true;
            CoreWebView2.AddWebResourceRequestedFilter($"{AppOrigin}*", CoreWebView2WebResourceContext.All);
            CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        }

        private void NavigateCore(string url)
        {
            Source = new Uri(new Uri(AppOriginUri, url).ToString());
        }

        private async void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs eventArgs)
        {
            // Get a deferral object so that WebView2 knows there's some async stuff going on. We call Complete() at the end of this method.
            var deferral = eventArgs.GetDeferral();

            var requestUri = QueryStringHelper.RemovePossibleQueryString(eventArgs.Request.Uri);

            if (new Uri(requestUri) is Uri uri && AppOriginUri.IsBaseOf(uri))
            {
                PathUtils.GetRelativePathAndContentType(HybridWebView.AppOriginUri, uri, eventArgs.Request.Uri, MainFile, out string relativePath, out string contentType, out string fullUrl);

                Stream? contentStream = null;
                IDictionary<string, string>? customHeaders = null;

                // Check to see if the request is a proxy request
                if (relativePath == ProxyRequestPath)
                {
                    var args = new HybridWebViewProxyEventArgs(fullUrl, contentType);
                    await OnProxyRequestMessage(args);

                    if (args.ResponseStream != null)
                    {
                        contentType = args.ResponseContentType ?? PathUtils.PlanTextMimeType;
                        contentStream = args.ResponseStream;
                        customHeaders = args.CustomResponseHeaders;
                    }
                }

                if (relativePath.Contains("HybridWebView.js"))
                {
                    contentStream = EmbeddedResourceProvider.ReadResource(relativePath);
                }

#if WINUI
                if (contentStream is null)
                {
                    var assetPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", HybridAssetRoot!, relativePath!);
                    contentStream = await GetAssetStreamAsync(assetPath);
                }
#else
                if (contentStream is null)
                {
                    var assetPath = Path.Combine(HybridAssetRoot!, relativePath!);
                    contentStream = await GetAssetStreamAsync(assetPath);
                }
#endif

                if (contentStream is null)
                {
                    var notFoundContent = "Resource not found (404)";
                    eventArgs.Response = _coreWebView2Environment!.CreateWebResourceResponse(
                        Content: null,
                        StatusCode: 404,
                        ReasonPhrase: "Not Found",
                        Headers: GetHeaderString("text/plain", notFoundContent.Length, null)
                    );
                }
                else
                {
#if WINUI
                    eventArgs.Response = _coreWebView2Environment!.CreateWebResourceResponse(
                        Content: await CopyContentToRandomAccessStreamAsync(contentStream),
                        StatusCode: 200,
                        ReasonPhrase: "OK",
                        Headers: GetHeaderString(contentType, (int)contentStream.Length, customHeaders)
                    );

                    if (contentStream != null)
                    {
                        await contentStream.DisposeAsync();
                    }
#else
                    eventArgs.Response = _coreWebView2Environment!.CreateWebResourceResponse(
                       Content: contentStream,
                       StatusCode: 200,
                       ReasonPhrase: "OK",
                       Headers: GetHeaderString(contentType, (int)contentStream.Length, customHeaders)
                   );
#endif
                }
            }

#if WINUI
            async Task<IRandomAccessStream> CopyContentToRandomAccessStreamAsync(Stream content)
            {
                var randomAccessStream = new InMemoryRandomAccessStream();

                using(var ms = new MemoryStream()){
                    await content.CopyToAsync(ms);
                    await randomAccessStream.WriteAsync(ms.GetWindowsRuntimeBuffer());
                }
                return randomAccessStream;
            }
#endif

            // Notify WebView2 that the deferred (async) operation is complete and we set a response.
            deferral.Complete();
        }

        private protected static string GetHeaderString(string contentType, int contentLength, IDictionary<string, string>? customHeaders)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Content-Type: {contentType}");
            sb.AppendLine($"Content-Length: {contentLength}");

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    // Add custom headers to the response. Skip the Content-Length and Content-Type headers.
                    if (header.Key != "Content-Length" && header.Key != "Content-Type")
                    {
                        sb.AppendLine($"{header.Key}: {header.Value}");
                    }
                }
            }

            // Ensure that the Cache-Control header is not set in the custom headers.
            if (customHeaders == null || !customHeaders.ContainsKey("Cache-Control"))
            {
                // Disable local caching. This will prevent user scripts from executing correctly.
                sb.AppendLine("Cache-Control: no-cache, max-age=0, must-revalidate, no-store");
            }

            return sb.ToString();
        }

        private void Wv2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            OnMessageReceived(args.TryGetWebMessageAsString());
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>A string containing the return value of the called method.</returns>
        public async Task<string> InvokeJsMethodAsync(string methodName, params object[] paramValues)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException($"The method name cannot be null or empty.", nameof(methodName));
            }
            
            return await EvaluateJavaScriptAsync($"{methodName}({(paramValues == null ? string.Empty : string.Join(", ", paramValues.Select(v => JsonSerializer.Serialize(v))))})");
        }

        /// <summary>
        /// Invokes a JavaScript method named <paramref name="methodName"/> and optionally passes in the parameter values specified
        /// by <paramref name="paramValues"/> by JSON-encoding each one.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value to deserialize from JSON.</typeparam>
        /// <param name="methodName">The name of the JavaScript method to invoke.</param>
        /// <param name="paramValues">Optional array of objects to be passed to the JavaScript method by JSON-encoding each one.</param>
        /// <returns>An object of type <typeparamref name="TReturnType"/> containing the return value of the called method.</returns>
        public async Task<TReturnType?> InvokeJsMethodAsync<TReturnType>(string methodName, params object[] paramValues)
        {
            var stringResult = await InvokeJsMethodAsync(methodName, paramValues);

            if (stringResult is null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<TReturnType>(stringResult);
        }

        public virtual void OnMessageReceived(string message)
        {
            var messageData = JsonSerializer.Deserialize<WebMessageData>(message);
            switch (messageData?.MessageType)
            {
                case 0: // "raw" message (just a string)
                    RawMessageReceived?.Invoke(this, new HybridWebViewRawMessageReceivedEventArgs(messageData.MessageContent));
                    break;
                case 1: // "invoke" message
                    if (messageData.MessageContent == null)
                    {
                        throw new InvalidOperationException($"Expected invoke message to contain MessageContent, but it was null.");
                    }
                    var invokeData = JsonSerializer.Deserialize<JSInvokeMethodData>(messageData.MessageContent)!;
                    InvokeDotNetMethod(invokeData);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown message type: {messageData?.MessageType}. Message contents: {messageData?.MessageContent}");
            }

        }

        /// <summary>
        /// Handle the proxy request message.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>A Task</returns>
        public virtual async Task OnProxyRequestMessage(HybridWebViewProxyEventArgs args)
        {
            // Don't let failed proxy requests crash the app.
            try
            {
                // When no query parameters are passed, the SendRoundTripMessageToDotNet JavaScript method is expected to have been called.
                if (args.QueryParams != null && args.QueryParams.TryGetValue("__ajax", out string? jsonQueryString))
                {
                    if (jsonQueryString != null)
                    {
                        var invokeData = JsonSerializer.Deserialize<JSInvokeMethodData>(jsonQueryString);

                        if (invokeData != null && invokeData.MethodName != null)
                        {
                            object? result = InvokeDotNetMethod(invokeData);

                            if (result != null)
                            {
                                args.ResponseContentType = "application/json";

                                DotNetInvokeResult dotNetInvokeResult;

                                var resultType = result.GetType();
                                if (resultType.IsArray || resultType.IsClass)
                                {
                                    dotNetInvokeResult = new DotNetInvokeResult()
                                    {
                                        Result = JsonSerializer.Serialize(result),
                                        IsJson = true,
                                    };
                                }
                                else
                                {
                                    dotNetInvokeResult = new DotNetInvokeResult()
                                    {
                                        Result = result,
                                    };
                                }
                                args.ResponseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dotNetInvokeResult)));
                            }
                        }
                    }
                }
                else if (ProxyRequestReceived != null) //Check to see if user has subscribed to the event.
                {
                    await ProxyRequestReceived(args);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An exception occurred while handling the proxy request: {ex.Message}");
            }
        }

        private object? InvokeDotNetMethod(JSInvokeMethodData invokeData)
        {
            if (JSInvokeTarget is null)
            {
                throw new NotImplementedException($"The {nameof(JSInvokeTarget)} property must have a value in order to invoke a .NET method from JavaScript.");
            }

            var invokeMethod = JSInvokeTarget.GetType().GetMethod(invokeData.MethodName!, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod);
            if (invokeMethod == null)
            {
                throw new InvalidOperationException($"The method {invokeData.MethodName} couldn't be found on the {nameof(JSInvokeTarget)} of type {JSInvokeTarget.GetType().FullName}.");
            }

            if (invokeData.ParamValues != null && invokeMethod.GetParameters().Length != invokeData.ParamValues.Length)
            {
                throw new InvalidOperationException($"The number of parameters on {nameof(JSInvokeTarget)}'s method {invokeData.MethodName} ({invokeMethod.GetParameters().Length}) doesn't match the number of values passed from JavaScript code ({invokeData.ParamValues.Length}).");
            }

            var paramObjectValues =
                invokeData.ParamValues?
                    .Zip(invokeMethod.GetParameters(), (s, p) => s == null ? null : JsonSerializer.Deserialize(s, p.ParameterType))
                    .ToArray();

            return invokeMethod.Invoke(JSInvokeTarget, paramObjectValues);
        }

        private sealed class JSInvokeMethodData
        {
            public string? MethodName { get; set; }
            public string[]? ParamValues { get; set; }
        }

        private sealed class WebMessageData
        {
            public int MessageType { get; set; }
            public string? MessageContent { get; set; }
        }

        /// <summary>
        /// A simple internal class to hold the result of a .NET method invocation, and whether it should be treated as JSON.
        /// </summary>
        private sealed class DotNetInvokeResult
        {
            public object? Result { get; set; }
            public bool IsJson { get; set; }
        }

        internal static async Task<string?> GetAssetContentAsync(string assetPath)
        {
            using var stream = await GetAssetStreamAsync(assetPath);
            if (stream == null)
            {
                return null;
            }
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();

            return contents;
        }

        internal static async Task<Stream?> GetAssetStreamAsync(string assetPath)
        {
            try
            { 
                var file = new FileInfo(assetPath);
                if (file.Exists)
                {
                    return file.OpenRead();
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

#if !MAUI
        internal async Task<string> EvaluateJavaScriptAsync(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                var result = await CoreWebView2.ExecuteScriptAsync(v);

                if (!string.IsNullOrEmpty(result))
                {
                    if (result.StartsWith("\"") && result.EndsWith("\""))
                    {
                        result = result[1..^1];
                    }

                    return result;
                }
            }

            return string.Empty;
        }
    }
#endif
}
