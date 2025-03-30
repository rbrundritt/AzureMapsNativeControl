using AzureMapsNativeControl.Layer;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control that displays an overview map of the area the main map is focused on. 
    /// https://github.com/Azure-Samples/azure-maps-overview-map
    /// </summary>
    public sealed class OverviewMapControl: BaseControl
    {
        #region Private Properties

        private int _height = 150;
        private int _width = 150;
        private bool _interactive = true;
        private MapStyle? _mapStyle = AzureMapsNativeControl.MapStyle.Road;
        private HtmlMarkerOptions _markerOptions = new HtmlMarkerOptions();
        private bool _minimized = false;
        private OverviewMapOverlay _overlay = OverviewMapOverlay.Area;
        private bool _showToggle = true;
        private ControlStyle? _style = ControlStyle.Light;
        private string? _styleColor = null;
        private bool _syncBearingPitch = true;
        private bool _syncZoom = true;
        private bool _visible = true;
        private int _zoom = 1;
        private int _zoomOffset = -5;
        private OverviewMapShape _shape = OverviewMapShape.Square;

        private LineLayerOptions _lineLayerOptions = LineLayerOptions.Defaults();      
        private PolygonLayerOptions _polygonLayerOptions = PolygonLayerOptions.Defaults();

        #endregion

        #region Constructor

        /// <summary>
        /// A control that displays an overview map of the area the main map is focused on. 
        /// </summary>
        public OverviewMapControl(): base("atlas.control.OverviewMap", AzureMapsModules.OverviewMapModule)
        {
            _lineLayerOptions.Filter = new Expression<bool>("get", "visible");
            _polygonLayerOptions.Filter = new Expression<bool>("get", "visible");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The height of the overview map in pixels. Default: 150
        /// </summary>
        [JsonPropertyName("height")]
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged("Height", value);
            }
        }

        /// <summary>
        /// The width of the overview map in pixels. Default: 150
        /// </summary>
        [JsonPropertyName("width")]
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged("Width", value);
            }
        }

        /// <summary>
        /// Specifies the type of information to overlay on top of the map.
        /// </summary>
        [JsonPropertyName("overlay")]
        public OverviewMapOverlay Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                OnPropertyChanged("Overlay", value);
            }
        }

        /// <summary>
        /// Options for customizing the marker overlay. If the draggable option of the marker it enabled, the map will center over the marker location after it has been dragged to a new location.
        /// </summary>
        [JsonPropertyName("markerOptions")]
        public HtmlMarkerOptions MarkerOptions
        {
            get
            {
                return _markerOptions;
            }
            set
            {
                _markerOptions = value;
                OnPropertyChanged("MarkerOptions", value);
            }
        }

        /// <summary>
        /// Specifies if the overview map is interactive. Default: true
        /// </summary>
        [JsonPropertyName("interactive")]
        public bool Interactive
        {
            get
            {
                return _interactive;
            }
            set
            {
                _interactive = value;
                OnPropertyChanged("Interactive", value);
            }
        }

        /// <summary>
        /// The name of the style to use when rendering the map. Default: Road
        /// </summary>
        [JsonPropertyName("mapStyle")]
        public MapStyle? MapStyle
        {
            get
            {
                return _mapStyle;
            }
            set
            {
                _mapStyle = value;
                OnPropertyChanged("MapStyle", value);
            }
        }

        /// <summary>
        /// When displayed within the map, specifies if the controls content is minimized or not. 
        /// Only used when showToggle is true. 
        /// Default: false
        /// </summary>
        [JsonPropertyName("minimized")]
        public bool Minimized
        {
            get
            {
                return _minimized;
            }
            set
            {
                _minimized = value;
                OnPropertyChanged("Minimized", value);
            }
        }

        /// <summary>
        /// Specifies if a toggle button for minimizing the controls content should be displayed or not when the control within the map. 
        /// Default: true
        /// </summary>
        [JsonPropertyName("showToggle")]
        public bool ShowToggle
        {
            get
            {
                return _showToggle;
            }
            set
            {
                _showToggle = value;
                OnPropertyChanged("ShowToggle", value);
            }
        }

        /// <summary>
        /// An alternative to the Style property. Uses a CSS3 color value to set the color of the control.
        /// </summary>
        [JsonPropertyName("styleColor")]
        public string? StyleColor
        {
            get
            {
                return _styleColor;
            }
            set
            {
                _styleColor = value;
                _style = null;
                OnPropertyChanged("StyleColor", value);
            }
        }

        /// <summary>
        /// The style of the control.
        /// </summary>
        [JsonPropertyName("style")]
        public new ControlStyle? Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    _styleColor = null;
                    OnPropertyChanged("Style", value);
                }
            }
        }

        /// <summary>
        /// Specifies if bearing and pitch changes should be synchronized. Default: true
        /// </summary>
        [JsonPropertyName("syncBearingPitch")]
        public bool SyncBearingPitch
        {
            get
            {
                return _syncBearingPitch;
            }
            set
            {
                _syncBearingPitch = value;
                OnPropertyChanged("SyncBearingPitch", value);
            }
        }

        /// <summary>
        /// Specifies if zoom level changes should be synchronized. Default: true
        /// </summary>
        [JsonPropertyName("syncZoom")]
        public bool SyncZoom
        {
            get
            {
                return _syncZoom;
            }
            set
            {
                _syncZoom = value;
                OnPropertyChanged("SyncZoom", value);
            }
        }

        /// <summary>
        /// Specifies if the overview map control is visible or not. Default: true
        /// </summary>
        [JsonPropertyName("visible")]
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                OnPropertyChanged("Visible", value);
            }
        }

        /// <summary>
        /// Zoom level to set on overview map when not synchronizing zoom level changes. Default: 1
        /// </summary>
        [JsonPropertyName("zoom")]
        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                OnPropertyChanged("Zoom", value);
            }
        }

        /// <summary>
        /// The number of zoom levels to offset from the parent map zoom level when synchronizing zoom level changes. Default: -5
        /// </summary>
        [JsonPropertyName("zoomOffset")]
        public int ZoomOffset
        {
            get
            {
                return _zoomOffset;
            }
            set
            {
                _zoomOffset = value;
                OnPropertyChanged("ZoomOffset", value);
            }
        }

        /// <summary>
        /// Specifies the shape of the overview map. Default: Square
        /// </summary>
        [JsonPropertyName("shape")]
        public OverviewMapShape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                OnPropertyChanged("Shape", value);
            }
        }

        /// <summary>
        /// Specifies the line layer options used by the overview map when the overlay is set to area.
        /// </summary>
        [JsonIgnore]
        public LineLayerOptions LineLayerOptions
        {
            get { return _lineLayerOptions; }
            set {
                if (value != null)
                {
                    LineLayerOptions.Merge(value, _lineLayerOptions);
                    LayerOptionChanged("LineLayerOptions");
                }
            }
        }

        /// <summary>
        /// Specifies the polygon layer options used by the overview map when the overlay is set to area.
        /// </summary>
        [JsonIgnore]
        public PolygonLayerOptions PolygonLayerOptions
        {
            get { return _polygonLayerOptions; }
            set
            {
                if (value != null)
                {
                    PolygonLayerOptions.Merge(value, _polygonLayerOptions);
                    LayerOptionChanged("PolygonLayerOptions");
                }
            }
        }

        #endregion

        #region Private Methods

        internal void MapAttached()
        {
            CallCustomControlFunction("_layers.lineLayer.setOptions", _lineLayerOptions);
            CallCustomControlFunction("_layers.polygonLayer.setOptions", _polygonLayerOptions);
        }

        private void LayerOptionChanged(string propertyName)
        {
            InvokePropertyChanged(propertyName);

            if (_map != null)
            {
                if (propertyName.Equals("LineLayerOptions"))
                {
                    CallCustomControlFunction("_layers.lineLayer.setOptions", _lineLayerOptions);
                }
                else
                {
                    CallCustomControlFunction("_layers.polygonLayer.setOptions", _polygonLayerOptions);
                }
            }
        }

        #endregion
    }
}
