using System;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Base control that the legend and layer controls inherit from.
    /// </summary>
    [JsonDerivedType(typeof(LegendControl))]
    [JsonDerivedType(typeof(LayerControl))]
    public abstract class BaseLegendLayerControl : BaseControl
    {
        #region Private Properties

        private ControlLayout _layout = ControlLayout.Carousel;
        private bool _minimized = false;
        private bool _showToggle = true;
        private ControlStyle? _style = ControlStyle.Light;
        private string? _styleColor = null;
        private bool _visible = true;
        private ZoomBehavior _zoomBehavior = Control.ZoomBehavior.Hide;
        private int _maxWidth = 10000;

        #endregion

        #region Constructor

        /// <summary>
        /// Base control that the legend and layer controls inherit from.
        /// </summary>
        /// <param name="jsNamespace">The JavaScript namespace to the control class.</param>
        internal BaseLegendLayerControl(string jsNamespace) : base(jsNamespace, AzureMapsModules.LayerLegendModule)
        {
        }

        #endregion

        #region Public Properties 

        /// <summary>
        /// How multiple items are laid out.
        /// - 'accordion' adds each item or group as an accordion panel.
        /// - 'carousel' allows the user to page through each item.
        /// - 'list' adds items one after another vertically. 
        /// Default: 'carousel'
        /// </summary>
        [JsonPropertyName("layout")]
        public ControlLayout Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                _layout = value;
                OnPropertyChanged("Layout", value);
            }
        }

        /// <summary>
        /// When displayed within the map, specifies if the controls content is minimized or not. 
        /// Only used when showToggle is true. 
        /// Default: false
        /// </summary>
        [JsonPropertyName("minimized")]
        public bool Miminized
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
                _showToggle = value ;
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
        /// Specifies how all legend cards should be treated when the map zoom level falls outside of the items min and max zoom range. 
        /// Can be overridden at the legend type setting level. Default: 'hide'
        /// </summary>
        [JsonPropertyName("zoomBehavior")]
        public ZoomBehavior ZoomBehavior
        {
            get
            {
                return _zoomBehavior;
            }
            set
            {
                _zoomBehavior = value;
                OnPropertyChanged("ZoomBehavior", value);
            }
        }

        /// <summary>
        /// The maximum width of the control in pixels. If not set, the control will automatically adjust its width based on the content.
        /// Will also limit the width of the control to the maps width minus 20 pixels to account for padding.
        /// </summary>
        [JsonPropertyName("maxWidth")]
        public int MaxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                if (value != 0)
                {
                    _maxWidth = Math.Abs(value);
                    OnPropertyChanged("MaxWidth", _maxWidth);
                }
            }
        }

        #endregion
    }
}
