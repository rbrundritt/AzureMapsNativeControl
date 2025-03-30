using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Drawing;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Source;
using AzureMapsNativeControl.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

#if MAUI
using Microsoft.Maui.Platform;

#if WINDOWS || MACCATALYST
using AzureMapsNativeControl.Platforms;
#endif

#elif WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
#elif WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif


namespace AzureMapsNativeControl
{
    /// <summary>
    /// A map control that displays a map.
    /// </summary>
    public class Map : IMapView, IMapEventTarget
    {
        #region Private Methods

        internal bool _isReady = false;
        internal bool _isMapLoaded = false;
        internal bool _pageReloaded = false;

        private TileSource? _elevationSource = null;
        private double _elevationExaggeration = 1;
        private bool _elevationDisabled = true;
        private bool _boolMauiDropRegistered = false;

        AzureMapsConfiguration? _config = null;

        /// <summary>
        /// A unique identifier for the view.
        /// </summary>
        internal string ViewId { get; private set; } = UniqueId.Get("atlas.Map");

        private event EventHandler<MapEventArgs>? OnReadyHandler;
        private event EventHandler<MapEventArgs>? OnLoadedHandler;
        private event EventHandler<MapEventArgs>? OnFilesDroppedHandler;

        internal Dictionary<string, DrawingManager> DrawingManagers = new Dictionary<string, DrawingManager>();
        internal Dictionary<string, IPlayableAnimation> Animations = new Dictionary<string, IPlayableAnimation>();

        #endregion

        #region Constructor

        /// <summary>
        /// A map control that displays a map.
        /// </summary>
        public Map(): this(null, null)
        {
        }

        /// <summary>
        /// A map control that displays a map.
        /// </summary>
        /// <param name="divId">The map control's unique identifier used as the div ID in the HTML page.</param>
        /// <param name="jsInterop">The JS communication channel for the map with a web view.</param>
        internal Map(string? divId = null, MapViewJsInterlop? jsInterop = null): base()
        {
            Id = string.IsNullOrWhiteSpace(divId)? "mainMap": divId;

            if (jsInterop != null)
            {
                JsInterlop = jsInterop;
            }
            else
            {
                //Set the target for JavaScript interop.
                JsInterlop = new MapViewJsInterlop(this);

                //Add the web view to the map container.
                this.Children.Insert(0, JsInterlop._webView);
            }

            //Add event manager.
            Events = new MapEventManager(this);

            //Add content managers.
#if MAUI
            Controls = new ControlManager(this);
#else
            SetValue(ControlManagerProperty, new ControlManager(this));
#endif
            ImageSprite = new ImageSpriteManager(this);
            Layers = new LayerManager(this);
            Markers = new HtmlMarkerManager(this);
            Popups = new PopupManager(this);
            Sources = new SourceManager(this);

#if MAUI && (WINDOWS || MACCATALYST)
            //Logic for supporting drag and drop of files onto the map.
            //Only supported on Windows and Mac Catalyst.

            Loaded += (sender, args) =>
            {
                if(Settings != null && Settings.AllowFileDrop != null && Settings.AllowFileDrop.Value)
                {
                    if (this.Handler?.MauiContext != null)
                    {
                        var view = this.ToPlatform(this.Handler.MauiContext);
                        DragDropHelper.RegisterDrop(view, (files) =>
                        {
                            OnFilesDroppedHandler?.Invoke(this, new MapFilesDroppedEventArgs(this, "drop")
                            {
                                Files = files
                            });
                        });

                        _boolMauiDropRegistered = true;
                    }
                }
            };

            Unloaded += (sender, args) =>
            {
                if (_boolMauiDropRegistered && this.Handler?.MauiContext != null)
                {
                    var view = this.ToPlatform(this.Handler.MauiContext);
                    DragDropHelper.UnRegisterDrop(view);
                }
            };
#endif
        }

#endregion

        #region Managers

        /// <summary>
        /// Manages the communication between the JavaScript Map API and the .NET wrapper.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MapViewJsInterlop JsInterlop { get; private set; }

        /// <summary>
        /// Used to add, remove, and invoke events in the map and map elements (layers, sources, markers, popups...).
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MapEventManager Events { get; internal set; }

        /// <summary>
        /// Used to maintain all pin images and layer patterns loaded into the map.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSpriteManager ImageSprite { get; }

#if MAUI
        /// <summary>
        /// Used to maintain all Control’s on the map.
        /// </summary>
        public ControlManager Controls { get; private set; }
#else
        public static readonly DependencyProperty ControlManagerProperty =
            DependencyProperty.Register(nameof(Controls), typeof(ControlManager), typeof(Map), new PropertyMetadata(null));

        /// <summary>
        /// Used to maintain all Control’s on the map.
        /// </summary>
        public ControlManager Controls
        {
            get { return (ControlManager)GetValue(ControlManagerProperty); }
            set { throw new Exception("An attempt ot modify Read-Only property"); }
        }
#endif

        /// <summary>
        /// Used to maintain all Popup’s on the map.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PopupManager Popups { get; private set; }

        /// <summary>
        /// Used to maintain all sources on the map.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SourceManager Sources { get; }

        /// <summary>
        /// Used to maintain all layers on the map.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LayerManager Layers { get; }

        /// <summary>
        /// A manager for the map control's HTML markers.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HtmlMarkerManager Markers { get; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The namespace for the JavaScript object that the map control is attached to.
        /// </summary>
        public string JsNamespace { get; } = "atlas.Map";

        /// <summary>
        /// The map control's unique identifier used as the div ID in the HTML page.
        /// </summary>
        public new string Id { get; private set; } = UniqueId.Get("atlas.Map");

#if MAUI
        /// <summary>
        /// Initial options to set on the map when loading.
        /// </summary>
        public MapLoadOptions? Settings { get; set; }
#else
        public static readonly DependencyProperty SettingsProperty =
          DependencyProperty.Register(nameof(Settings), typeof(MapLoadOptions), typeof(Map), new PropertyMetadata(null));

        /// <summary>
        /// Used to maintain all Control’s on the map.
        /// </summary>
        public MapLoadOptions? Settings
        {
            get { return (MapLoadOptions)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
#endif

#endregion

        #region Module methods

        /// <summary>
        /// Loads a module into the map.
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <returns></returns>
        public override Task LoadModuleAsync(MapModuleInfo moduleInfo)
        {
            return JsInterlop.LoadModule(moduleInfo);
        }

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public override bool IsModuleLoaded(string moduleName)
        {
            return JsInterlop.IsModuleLoaded(moduleName);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the map is ready.
        /// </summary>
        public event EventHandler<MapEventArgs> OnReady
        {
            add
            {
                OnReadyHandler += value;

                //If the map is already ready, trigger the event.
                if (_isReady)
                {
                    OnReadyHandler?.Invoke(this, new MapEventArgs(this, new RawMapMsg()
                    {
                        Type = "ready"
                    }));
                } 
                else
                {
                    Events.Add("ready", value);
                }
            }
            remove
            {
                OnReadyHandler -= value;
                Events.Remove("ready", value);
            }
        }

        /// <summary>
        /// Event triggered when the map is loaded.
        /// </summary>
        public event EventHandler<MapEventArgs> OnLoaded
        {
            add
            {
                OnLoadedHandler += value;

                //If the map is already loaded, trigger the event.
                if (_isMapLoaded)
                {
                    OnLoadedHandler?.Invoke(this, new MapEventArgs(this, new RawMapMsg()
                    {
                        Type = "load"
                    }));
                } 
                else
                {
                    Events.Add("load", value);
                }
            }
            remove
            {
                OnLoadedHandler -= value;
                Events.Remove("load", value);
            }
        }

        /// <summary>
        /// Event handler for when a files is dropped on the map.
        /// </summary>
        public event EventHandler<MapEventArgs> OnFilesDropped
        {
            add
            {
                OnFilesDroppedHandler += value;
                Events.Add("drop", value);
            }
            remove
            {
                OnFilesDroppedHandler -= value;
                Events.Remove("drop", value);
            }
        }

        #endregion

        #region Command Methods

#if WINUI || WPF
        public static DependencyProperty OnReadyCommandProperty = DependencyProperty.Register(nameof(OnReadyCommand), typeof(ICommand), typeof(Map), null);

        /// <summary>
        /// Command to execute when the map is ready.
        /// </summary>
        public ICommand OnReadyCommand
        {
            get
            {
                return (ICommand)GetValue(OnReadyCommandProperty);
            }

            set
            {
                SetValue(OnReadyCommandProperty, value);
            }
        }

        public static DependencyProperty OnLoadedCommandProperty = DependencyProperty.Register(nameof(OnLoadedCommand), typeof(ICommand), typeof(Map), null);

        /// <summary>
        /// Command to execute when the map is loaded.
        /// </summary>
        public ICommand OnLoadedCommand
        {
            get
            {
                return (ICommand)GetValue(OnLoadedCommandProperty);
            }

            set
            {
                SetValue(OnLoadedCommandProperty, value);
            }
        }

        public static DependencyProperty OnFilesDroppedCommandProperty = DependencyProperty.Register(nameof(OnFilesDroppedCommand), typeof(ICommand), typeof(Map), null);

        /// <summary>
        /// Command to execute when files are dropped onto the map.
        /// </summary>
        public ICommand OnFilesDroppedCommand
        {
            get
            {
                return (ICommand)GetValue(OnFilesDroppedCommandProperty);
            }

            set
            {
                SetValue(OnFilesDroppedCommandProperty, value);
            }
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Makes a GET request to an Azure Maps REST service that returns a JSON response.
        /// Will use the same auth at the map and set the "{azMapsDomain}" placeholder in the URL to the Azure Maps domain.
        /// </summary>
        /// <param name="url">A URL that points to an Azure Maps REST service. Can have "{azMapsDomain}" placeholder in it.</param>
        /// <returns>Response from Azure Maps service or null if failed.</returns>
        public async Task<string?> MakeGetRequest(string url)
        {
            return await JsInterlop.InvokeJsMethodAsync(this, "makeGetRequest", url);
        }

        /// <summary>
        /// Makes a GET request to an Azure Maps REST service that returns a JSON response.
        /// Will use the same auth at the map and set the "{azMapsDomain}" placeholder in the URL to the Azure Maps domain.
        /// </summary>
        /// <typeparam name="T">The response type to serialize the response to.</typeparam>
        /// <param name="url">A URL that points to an Azure Maps REST service. Can have "{azMapsDomain}" placeholder in it.</param>
        /// <returns>Response from Azure Maps service or null if failed.</returns>
        public async Task<T?> MakeGetRequest<T>(string url)
        {
            var r = await MakeGetRequest(url);

            if(r != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(r);
                }
                catch { }
            }

            return default(T);
        }

        /// <summary>
        /// Makes a POST request to an Azure Maps REST service that returns a JSON response.
        /// Will use the same auth at the map and set the "{azMapsDomain}" placeholder in the URL to the Azure Maps domain.
        /// </summary>
        /// <param name="url">A URL that points to an Azure Maps REST service. Can have "{azMapsDomain}" placeholder in it.</param>
        /// <param name="body">An object to add as the JSON body.</param>
        /// <returns>Response from Azure Maps service or null if failed.</returns>
        public async Task<string?> MakePostRequest(string url, object? body)
        {
            return await JsInterlop.InvokeJsMethodAsync(this, "makePostRequest", url);
        }

        /// <summary>
        /// Makes a POST request to an Azure Maps REST service that returns a JSON response.
        /// Will use the same auth at the map and set the "{azMapsDomain}" placeholder in the URL to the Azure Maps domain.
        /// </summary>
        /// <typeparam name="T">The response type to serialize the response to.</typeparam>
        /// <param name="url">A URL that points to an Azure Maps REST service. Can have "{azMapsDomain}" placeholder in it.</param>
        /// <param name="body">An object to add as the JSON body.</param>
        /// <returns>Response from Azure Maps service or null if failed.</returns>
        public async Task<T?> MakePostRequest<T>(string url, object? body)
        {
            var r = await MakePostRequest(url, body);

            if (r != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(r);
                }
                catch { }
            }

            return default(T);
        }

        /// <summary>
        /// Sets the cursor style of the mouse.
        /// </summary>
        /// <param name="cursorStyleName"></param>
        public async void SetMouseCursor(string cursorStyleName)
        {
            await JsInterlop.InvokeJsMethodAsync(this, "setMouseCursor", cursorStyleName);
        }

        /// <summary>
        /// Removes all user added sources, layers, markers, and popups from the map. User added images are preserved.
        /// </summary>
        public void ClearMap()
        {
            Layers.Clear();
            Sources.Clear();
            Markers.Clear();
            Popups.Clear();
        }

        /// <summary>
        /// Disables the 3D terrain mesh.
        /// </summary>
        public async void DisableElevation()
        {
            if (!_elevationDisabled && _elevationSource != null)
            {
                await JsInterlop.InvokeJsMethodAsync(this, "disableElevation");
                _elevationDisabled = true;
            }
        }

        /// <summary>
        /// Loads a 3D terrain mesh, based on a "raster-dem" source.
        /// If elevationEncoding property is null, will default to ElevationEncoding.Mapbox
        /// </summary>
        /// <param name="elevationSource">Elevation tile source. ElevationEncoding property MUST be set.</param>
        /// <param name="exaggeration">The elevation exaggeration factor.</param>
        public async void EnableElevation(TileSource? elevationSource = null, double? exaggeration = 1)
        {
            bool needsUpdate = false;

            //Check to see if elevation source was passed in.
            if (elevationSource != null && elevationSource != _elevationSource)
            {
                if (elevationSource.ElevationEncoding == null)
                {
                    elevationSource.ElevationEncoding = ElevationEncoding.Mapbox;
                }

                _elevationSource = elevationSource;

                needsUpdate = true;
            }

            needsUpdate |= exaggeration != _elevationExaggeration;

            _elevationExaggeration = Math.Abs(exaggeration ?? _elevationExaggeration); //Ensure exaggeration is positive.

            if (_elevationSource != null && (_elevationDisabled || needsUpdate))
            {
                await JsInterlop.InvokeJsMethodAsync(this, "enableElevation", _elevationSource.Id, _elevationSource, _elevationExaggeration);
                _elevationDisabled = false;
            }
        }

        /// <summary>
        /// Removes an elevation source, and it's associated tile layers, from the map.
        /// </summary>
        private void RemoveElevationSource()
        {
            if (_elevationSource != null)
            {
                Sources.Remove(_elevationSource);
                _elevationSource = null;
            }
        }

        /// <summary>
        /// Returns the camera's current properties.
        /// </summary>
        /// <returns>The camera's current properties.</returns>
        public async Task<CameraOptions?> GetCamera()
        {
            return await JsInterlop.InvokeJsMethodAsync<CameraOptions>(this, "getCamera");
        }

        /// <summary>
        /// Returns the map camera's current bounding box.
        /// </summary>
        /// <returns>The map camera's current bounding box.</returns>
        public async Task<BoundingBox> GetBounds()
        {
            var bbox = await JsInterlop.InvokeJsMethodAsync<BoundingBox>(this, "getBounds");

            if (bbox == null)
            {
                return BoundingBox.Global();
            }

            return bbox;
        }

        /// <summary>
        /// Returns the service options with which the map control was initialized.
        /// </summary>
        /// <returns>The maps service options.</returns>
        public async Task<StyleOptions?> GetStyle()
        {
            return await JsInterlop.InvokeJsMethodAsync<StyleOptions>(this, "getStyle");
        }

        /// <summary>
        /// Return the map control's current traffic settings.
        /// </summary>
        /// <returns>The maps traffic options.</returns>
        public async Task<TrafficOptions?> GetTrafficOptions()
        {
            return await JsInterlop.InvokeJsMethodAsync<TrafficOptions>(this, "getTraffic");
        }

        /// <summary>
        /// Return the map control's current user interaction handler settings.
        /// </summary>
        /// <returns>The maps user interaction options.</returns>
        public async Task<UserInteractionOptions?> GetUserInteractionOptions()
        {
            return await JsInterlop.InvokeJsMethodAsync<UserInteractionOptions>(this, "getUserInteraction");
        }

        /// <summary>
        /// Converts an array of Pixel objects to an array of geographic Positions objects on the map.
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public async Task<IList<Position>> PixelsToPositions(IList<Pixel> pixels)
        {
            var data = await JsInterlop.InvokeJsMethodAsync<RawMapMsg>(this, "pixelsToPositions", new RawMapMsg { Pixels = pixels });

            return (data != null && data.Positions != null)?  data.Positions : new List<Position>();
        }

        /// <summary>
        /// Converts an array of Positions objects to an array of Pixel objects relative to the map container.
        /// </summary>
        /// <param name="positions"></param>
        /// <returns></returns>
        public async Task<IList<Pixel>> PositionsToPixels(IList<Position> positions)
        {
            var data = await JsInterlop.InvokeJsMethodAsync<RawMapMsg>(this, "positionsToPixels", new RawMapMsg { Positions = positions });

            return (data != null && data.Pixels != null) ? data.Pixels : new List<Pixel>();
        }

        /// <summary>
        /// Resize the map according to the dimensions of its container element.
        /// </summary>
        public async void ResizeMap()
        {
            await JsInterlop.InvokeJsMethodAsync(this, "resize");
        }

        /// <summary>
        /// Sets the maps camera to the specified options.
        /// </summary>
        /// <param name="options">Camera options.</param>
        /// <param name="animationOptions">Options for animating the camera.</param>
        public async void SetCamera(CameraOptions options, CameraAnimationOptions? animationOptions = null)
        {
            await SetCameraAsync(options, animationOptions);
        }

        /// <summary>
        /// Asynchronously set the maps camera and wait for the underlying call to be made.
        /// </summary>
        /// <param name="options">Camera options.</param>
        /// <param name="animationOptions">Options for animating the camera.</param>
        /// <returns></returns>
        public async Task SetCameraAsync(CameraOptions options, CameraAnimationOptions? animationOptions = null)
        {
            if (options.Zoom.HasValue && (options.Zoom < 0 || (options.Zoom > 24)))
            {
                options.Zoom = TileMath.Clip(options.Zoom.Value, 0, 24);
            }

            if (options.MinZoom.HasValue && (options.MinZoom < 0 || (options.MinZoom > 24)))
            {
                options.MinZoom = TileMath.Clip(options.MinZoom.Value, 0, 24);
            }

            if (options.MaxZoom.HasValue && (options.MaxZoom < 0 || (options.MaxZoom > 24)))
            {
                options.MaxZoom = TileMath.Clip(options.MaxZoom.Value, 0, 24);
            }

            if (options.Pitch.HasValue && (options.Pitch < 0 || (options.Pitch > 85)))
            {
                options.Pitch = TileMath.Clip(options.Pitch.Value, 0, 85);
            }

            if (options.MinPitch.HasValue && (options.MinPitch < 0 || (options.MinPitch > 85)))
            {
                options.MinPitch = TileMath.Clip(options.MinPitch.Value, 0, 85);
            }

            if (options.MaxPitch.HasValue && (options.MaxPitch < 0 || (options.MaxPitch > 85)))
            {
                options.MaxPitch = TileMath.Clip(options.MaxPitch.Value, 0, 85);
            }

            await JsInterlop.InvokeJsMethodAsync(this, "setCamera", options, animationOptions);
        }

        /// <summary>
        /// Set the map control's style options. Any options not specified will default to their current values.
        /// </summary>
        /// <param name="options"></param>
        public async void SetStyle(StyleOptions options)
        {
            await JsInterlop.InvokeJsMethodAsync(this, "setStyle", options);
        }

        /// <summary>
        /// Set the traffic options for the map. Any options not specified will default to their current values.
        /// </summary>
        /// <param name="options"></param>
        public async void SetTraffic(TrafficOptions options)
        {
            await JsInterlop.InvokeJsMethodAsync(this, "setTraffic", options);
        }

        /// <summary>
        /// Set the map control's user interaction handlers. Any options not specified will default to their current values.
        /// </summary>
        /// <param name="options"></param>
        public async void SetUserInteraction(UserInteractionOptions options)
        {
            await JsInterlop.InvokeJsMethodAsync(this, "setUserInteraction", options);
        }

        #endregion

        #region Internal Methods

        /// <inerhitdoc />
        internal override async Task InitView(AzureMapsConfiguration? config = null)
        {
            if(config == null)
            {
                _config = AzureMapsConfiguration.GetInstance();
            } 
            else
            {
                _config = config;
            }

            //Web page is loaded and ready. Initialize the map and pass in initial load options and auth.
            await JsInterlop.InvokeJsMethodAsync("loadMap", Id, Settings, _config);
        }

        /// <summary>
        /// When the page is unloaded, clear all map elements.
        /// </summary>
        internal override void WebPageUnloaded()
        {
            ImageSprite.SilentClear();
            Sources.SilentClear();
            Layers.SilentClear();
            Controls.SilentClear();
            Markers.SilentClear();
            Popups.SilentClear();

            //Need to silently remove but the ready/load events.
            Events.RemoveTarget(this, true);

            _pageReloaded = true;
            _isReady = false;
            _isMapLoaded = false;
            _elevationSource = null;
        }

        #endregion
    }
}