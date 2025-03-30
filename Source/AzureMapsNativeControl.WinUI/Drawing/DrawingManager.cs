using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// The primary class that manages all the drawing functionality on the map.
    /// NOTE: Drawing manager does not support multi-geometries currently.
    /// https://learn.microsoft.com/en-us/javascript/api/azure-maps-drawing-tools/atlas.drawing.drawingmanager
    /// </summary>
    public class DrawingManager : IMapEventTarget, IDisposable, INotifyPropertyChanged
    {
        #region Private Properties
                
        private bool _isInitialized = false;

        //Default options
        private HtmlMarkerOptions _dragHandleStyle = HtmlMarkerOptions.Defaults();

        private int _freehandInterval = 3;
        private DrawingInteractionType _interactionType = DrawingInteractionType.Hybrid;
        private DrawingMode _mode = DrawingMode.Idle;
        
        private HtmlMarkerOptions _secondaryDragHandleStyle = HtmlMarkerOptions.Defaults();

        private bool _shapeDraggingEnabled = true;
        private bool _shapeRotationEnabled = true;

        public DrawingToolbarOptions _toolbarOptions = new DrawingToolbarOptions();

        private LineLayerOptions _lineLayerOptions = LineLayerOptions.Defaults();
        private LineLayerOptions _linePreviewLayerOptions = LineLayerOptions.Defaults();
        private PolygonLayerOptions _polygonLayerOptions = PolygonLayerOptions.Defaults();
        private PolygonLayerOptions _polygonPreviewLayerOptions = PolygonLayerOptions.Defaults();
        private LineLayerOptions _polygonOutlineLayerOptions = LineLayerOptions.Defaults();
        private LineLayerOptions _polygonOutlinePreviewLayerOptions = LineLayerOptions.Defaults();
        private DrawingPointLayerOptions _pointLayerOptions = DrawingPointLayerOptions.Defaults();

        [JsonInclude]
        [JsonPropertyName("sourceId")]
        internal string SourceId { get; set; } = string.Empty;

        private event EventHandler? OnInitializedHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DrawingManager class.
        /// </summary>
        /// <param name="map">The map to attach the drawing manager to.</param>
        public DrawingManager(Map map)
        {
            if (!map._isReady)
            {
                throw new Exception("DrawingManager can only be created once the map is ready.");
            }

            Map = map;

            Map.DrawingManagers.Add(Id, this);

            Source = new DataSourceLite();
            SourceId = Source.Id;

            _toolbarOptions.PropertyChanged += ToolbarOptions_PropertyChanged;

            SetDefaultStyleOptions();
            Initialize();
        }

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Event triggered when the map is ready.
        /// </summary>
        public event EventHandler OnInitialized 
        {
            add
            {
                OnInitializedHandler += value;
            }
            remove
            {
                OnInitializedHandler -= value;
            }
        }

        /// <summary>
        /// A unique ID for tracking the drawing manager internally.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; internal set; } = UniqueId.Get("atlas.drawing.DrawingManager");

        [JsonInclude]
        [JsonPropertyName("jsNamespace")]
        public string JsNamespace { get; } = "atlas.drawing.DrawingManager";

        /// <summary>
        /// The map the drawing manager is attached to.
        /// </summary>
        [JsonIgnore]
        public Map Map { get; internal set; }

        /// <summary>
        /// The style options for the primary drag handles
        /// </summary>
        [JsonPropertyName("dragHandleStyle")]
        public HtmlMarkerOptions DragHandleStyle
        { 
            get { return _dragHandleStyle; }
            set
            {
                if (_dragHandleStyle != value)
                {
                    _dragHandleStyle = value;
                    OnPropertyChanged("DragHandleStyle", value);
                }
            }
        }

        /// <summary>
        /// Specifies the number of pixels the mouse or touch must move before another coordinate is added to a shape when in "freehand" or "hybrid" drawing modes. Default is 3.
        /// </summary>
        [JsonPropertyName("freehandInterval")]
        public int FreehandInterval
        {
            get { return _freehandInterval; }
            set
            {
                if (_freehandInterval != value)
                {
                    _freehandInterval = value;
                    OnPropertyChanged("FreehandInterval", value);
                }
            }
        }

        /// <summary>
        /// The type of drawing interaction the manager should adhere to. Default is "hybrid".
        /// </summary>
        [JsonPropertyName("interactionType")]
        public DrawingInteractionType InteractionType
        {
            get { return _interactionType; }
            set
            {
                if (_interactionType != value)
                {
                    _interactionType = value;
                    OnPropertyChanged("InteractionType", value);
                }
            }
        }

        /// <summary>
        /// The drawing mode the manager is in. Default is "idle".
        /// </summary>
        [JsonPropertyName("mode")]
        public DrawingMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged("Mode", value);
                }
            }
        }

        /// <summary>
        /// The style options for the secondary drag handles. These provide handles at mid-points for creating new coordinates between existing coordinates.
        /// </summary>
        [JsonPropertyName("secondaryDragHandleStyle")]
        public HtmlMarkerOptions SecondaryDragHandleStyle
        {
            get { return _secondaryDragHandleStyle; }
            set
            {
                if (_secondaryDragHandleStyle != value)
                {
                    _secondaryDragHandleStyle = value;
                    OnPropertyChanged("SecondaryDragHandleStyle", value);
                }
            }
        }

        /// <summary>
        /// Specifies if shapes can be dragged when in edit or select mode.
        /// </summary>
        [JsonPropertyName("shapeDraggingEnabled")]
        public bool ShapeDraggingEnabled
        {
            get { return _shapeDraggingEnabled; }
            set {
                if (_shapeRotationEnabled != value)
                {
                    _shapeDraggingEnabled = value;
                    OnPropertyChanged("ShapeDraggingEnabled", value);
                }
            }
        }

        /// <summary>
        /// Specifies if shapes can be rotated when in edit mode.
        /// </summary>
        [JsonPropertyName("shapeRotationEnabled")]
        public bool ShapeRotationEnabled
        {
            get { return _shapeRotationEnabled; }
            set
            {
                if (_shapeRotationEnabled != value)
                {
                    _shapeRotationEnabled = value;
                    OnPropertyChanged("ShapeRotationEnabled", value);
                }
            }
        }

        /// <summary>
        /// A data source to add newly created shapes to. If not specified when the drawing manager is constructed one will be created automatically. If the data source is changed the drawing manager will be switched to "idle" mode.
        /// </summary>
        [JsonIgnore]
        public DataSourceLite Source { get; internal set; }

        /// <summary>
        /// The options the drawing toolbar will use when displayed (Visible set to true).
        /// </summary>
        [JsonPropertyName("toolbarOptions")]
        public DrawingToolbarOptions ToolbarOptions
        {
            get { return _toolbarOptions; }
            set
            {
                if (_toolbarOptions != value)
                {
                    _toolbarOptions.PropertyChanged -= ToolbarOptions_PropertyChanged;
                }

                _toolbarOptions = value;

                if (_toolbarOptions != null)
                {
                    _toolbarOptions.PropertyChanged += ToolbarOptions_PropertyChanged;
                }

                OnPropertyChanged("ToolbarOptions", value);
            }
        }

        #region Layer style options

        /// <summary>
        /// The style options used by the lines.
        /// </summary>
        [JsonPropertyName("lineLayerOptions")]
        public LineLayerOptions LineLayerOptions
        {
            get => _lineLayerOptions;
            set
            {
                if (value != null && _lineLayerOptions != value)
                {
                    Layer.LineLayerOptions.Merge(value, _lineLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("LineLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the drawing manager when previewing a new line.
        /// </summary>
        [JsonPropertyName("linePreviewLayerOptions")]
        public LineLayerOptions LinePreviewLayerOptions
        {
            get => _linePreviewLayerOptions;
            set
            {
                if (value != null && _linePreviewLayerOptions != value)
                {
                    Layer.LineLayerOptions.Merge(value, _linePreviewLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("LinePreviewLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the polygons.
        /// </summary>
        [JsonPropertyName("polygonLayerOptions")]
        public PolygonLayerOptions PolygonLayerOptions
        {
            get => _polygonLayerOptions;
            set
            {
                if (value != null && _polygonLayerOptions != value)
                {
                    PolygonLayerOptions.Merge(value, _polygonLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("PolygonLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the drawing manager when previewing a new polygon.
        /// </summary>
        [JsonPropertyName("polygonPreviewLayerOptions")]
        public PolygonLayerOptions PolygonPreviewLayerOptions
        {
            get => _polygonPreviewLayerOptions;
            set
            {
                if (value != null && _polygonPreviewLayerOptions != value)
                {
                    PolygonLayerOptions.Merge(value, _polygonPreviewLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("PolygonPreviewLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used to outline polygons.
        /// </summary>
        [JsonPropertyName("polygonOutlineLayerOptions")]
        public LineLayerOptions PolygonOutlineLayerOptions
        {
            get => _polygonOutlineLayerOptions;
            set
            {
                if (value != null && _polygonOutlineLayerOptions != value)
                {
                    Layer.LineLayerOptions.Merge(value, _polygonOutlineLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("PolygonOutlineLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the drawing manager when previewing an outline of a polygon.
        /// </summary>
        [JsonPropertyName("polygonOutlinePreviewLayerOptions")]
        public LineLayerOptions PolygonOutlinePreviewLayerOptions
        {
            get => _polygonOutlinePreviewLayerOptions;
            set
            {
                if (value != null && _polygonOutlinePreviewLayerOptions != value)
                {
                    Layer.LineLayerOptions.Merge(value, _polygonOutlinePreviewLayerOptions);
                    value.Visible = true; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("PolygonOutlinePreviewLayerOptions", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the points.
        /// </summary>
        [JsonPropertyName("pointLayerOptions")]
        public DrawingPointLayerOptions PointLayerOptions
        {
            get => _pointLayerOptions;
            set
            {
                if (value != null && _pointLayerOptions != value)
                {
                    DrawingPointLayerOptions.Merge(value, _pointLayerOptions);
                    OnPropertyChanged("PointLayerOptions", value);
                }
            }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async void Dispose()
        {
            if (_isInitialized)
            {
                try
                {
                    Map.Events.RemoveTarget(this, true);

                    //Dispose the JS version of the drawing manager.
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "callGenericItemFunction", Id, Constants.GenericCache, "dispose");

                    //Remove the source from the map.
                    Map.Sources.Remove(Source);

                    Map.DrawingManagers.Remove(Id);
                }
                catch { }
            }
        }

        /// <summary>
        /// Puts a shape into edit mode. If the shape is not already in the data source, it adds it to it.
        /// </summary>
        /// <param name="feature">The feature to put into edit mode.</param>
        public async void Edit(Feature feature)
        {
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "drawingManagerEditShape", Id, feature);
        }

        /// <summary>
        /// Puts a shape into edit mode.
        /// </summary>
        /// <param name="featureId">The id of the feature to edit.</param>
        public async void Edit(string featureId)
        {
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "drawingManagerEditShape", Id, featureId);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the details of the drawing manager module.
        /// </summary>
        /// <returns></returns>
        internal static MapModuleInfo GetModuleInfo()
        {
            var config = AzureMapsConfiguration.GetInstance();

            string domain = config.GetDomain();

            return new MapModuleInfo("azure-maps-drawing-tools",
                new List<string> { $"https://{domain}/sdk/javascript/drawing/1/atlas-drawing.js" },
                new List<string> { $"https://{domain}/sdk/javascript/drawing/1/atlas-drawing.min.css" });
        }

        private void SetDefaultStyleOptions()
        {
            HtmlMarkerOptions.Merge(
                new HtmlMarkerOptions()
                {
                    Anchor = PositionAnchor.Center,
                    Draggable = true,
                    HtmlContent = "<svg width='12' height='12' viewBox='0 0 12 12' fill='none' xmlns='http://www.w3.org/2000/svg'> <circle class='dragHandleCircle' cx = '6' cy = '6' r = '5.5' fill='#FFFFFF' stroke='#4E4C4C'/> </svg>"
                },
                _dragHandleStyle
            );

            HtmlMarkerOptions.Merge(
                new HtmlMarkerOptions()
                {
                    Anchor = PositionAnchor.Center,
                    Draggable = true,
                    HtmlContent = "<svg width='12' height='12' viewBox='0 0 12 12' fill='none' xmlns='http://www.w3.org/2000/svg'> <circle class='dragHandleCircle' cx = '6' cy = '6' r = '5.5' fill='#A6A6A6' stroke='#4E4C4C'/> </svg>"
                },
                _secondaryDragHandleStyle
            );

            //Set the default options of the line layer.
            LineLayerOptions.Merge(new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("#000000"),
                Filter = new Expression<bool>("all",
                    new object[] { "==", new object[] { "geometry-type" }, "LineString" },
                    new object[] { "!", new object[] { "has", "outline" } })
            }, _lineLayerOptions);

            //Set the default options of the line preview layer.
            LineLayerOptions.Merge(new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("#2266E3"),
                Filter = _lineLayerOptions.Filter?.DeepClone()
            }, _linePreviewLayerOptions);

            //Set the default options of the polygon layer.
            PolygonLayerOptions.Merge(new PolygonLayerOptions
            {
                FillColor = Expression<string>.Literal("rgba(0, 0, 0, 0.12)"),
                Filter = new Expression<bool>("==", new object[] { "geometry-type" }, "Polygon")
            }, _polygonLayerOptions);

            //Set the default options of the polygon preview layer.
            PolygonLayerOptions.Merge(new PolygonLayerOptions
            {
                FillColor = Expression<string>.Literal("#1E90FF"),
                Filter = _polygonLayerOptions.Filter?.DeepClone()
            }, _polygonPreviewLayerOptions);

            //Set the default options of the polygon outline layer.
            LineLayerOptions.Merge(new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("#000000"),
                Filter = new Expression<bool>("any",
                    new object[] {
                        "all",
                        new object[] { "==", new object[] { "geometry-type" }, "LineString" },
                        new object[] { "has", "outline" }
                    },
                    new object[] { "==", new object[] { "geometry-type" }, "Polygon" })
            }, _polygonOutlineLayerOptions);

            //Set the default options of the polygon outline preview layer.
            LineLayerOptions.Merge(new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("#2266E3"),
                Filter = _polygonOutlineLayerOptions.Filter?.DeepClone()
            }, _polygonOutlinePreviewLayerOptions);
        }

        private async void Initialize()
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        { 
            if (!_isInitialized)
            {
                //Ensure the drawing module is loaded.
                var module = GetModuleInfo();
                await Map.JsInterlop.LoadModule(module);

                //Add the source to the map if it isn't already added.
                if (!Map.Sources.Contains(Source))
                {
                    Map.Sources.Add(Source);
                }

                //Add the drawing manager to the JS map.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateDrawingManager", this);

                //Listen for the mode change event.
                Map.Events.Add("drawingmodechanged", this, EmptyEventHandler);

                _isInitialized = true;

                OnInitializedHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        private void EmptyEventHandler(object? sender, MapEventArgs e)
        {
            //Do nothing here. We listen for the drawing mode changes but don't need to do anything.
        }

        private void ToolbarOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ToolbarOptions", _toolbarOptions);
        }

        private void OnPropertyChanged(string propertyName, object? value)
        {
            string jsPropName = Utils.ToCamelCase(propertyName);
            OnPropertyChanged(propertyName, jsPropName, value);
        }

        private async void OnPropertyChanged(string propertyName, string jsPropName, object? value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (_isInitialized)
            {
                //If value is null, set all options.
                if (value == null)
                {
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateDrawingManager", this);
                }
                else
                {
                    //Only update the individual property
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateDrawingManager", new Dictionary<string, object?>()
                    {
                        { "id", Id },
                        { jsPropName, value }
                    });
            }
            }
        }

        #endregion
    }
}
