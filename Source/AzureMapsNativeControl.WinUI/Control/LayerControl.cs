using AzureMapsNativeControl.Control.Layers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control for creating a list of layers and actions.
    /// https://github.com/Azure-Samples/azure-maps-layer-legend/blob/main/docs/layer_control.md
    /// </summary>
    public class LayerControl : BaseLegendLayerControl
    {
        #region Private Properties

        private string? _title = null;
        private DynamicLayerGroup? _dynamicLayerGroup = null;
        private ObservableCollection<LayerGroup> _layerGroups;
        private Dictionary<string, string>? _resourceStrings = null; 
        private LegendControl? _legendControl = null;

        #endregion

        #region Constructor

        /// <summary>
        /// A control for creating a list of layers and actions.
        /// </summary>
        public LayerControl() : base("atlas.control.LayerControl")
        {
            _layerGroups = new ObservableCollection<LayerGroup>();
            _layerGroups.CollectionChanged += LayerGroup_CollectionChanged;
        }

        /// <summary>
        /// A control for creating a list of layers and actions.
        /// </summary>
        public LayerControl(LegendControl legendControl) : base("atlas.control.LayerControl")
        {
            _layerGroups = new ObservableCollection<LayerGroup>();
            _layerGroups.CollectionChanged += LayerGroup_CollectionChanged;
            LegendControl = legendControl;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// One or more groups of layers and states.
        /// </summary>
        [JsonPropertyName("layerGroups")]
        public IList<LayerGroup> LayerGroups
        {
            get { return _layerGroups; }
            set
            {
                if (value != null && _layerGroups != value)
                {
                    if (_layerGroups != null)
                    {
                        _layerGroups.CollectionChanged -= LayerGroup_CollectionChanged;
                    }

                    _layerGroups = new ObservableCollection<LayerGroup>(value);

                    if (_layerGroups != null)
                    {
                        _layerGroups.CollectionChanged += LayerGroup_CollectionChanged;
                    }

                    OnPropertyChanged("LayerGroups", value);
                }
            }
        }

        /// <summary>
        /// Options for generating a layer group dynamically based off the user defined layers within the map.
        /// </summary>
        [JsonPropertyName("dynamicLayerGroup")]
        public DynamicLayerGroup? DynamicLayerGroup
        {
            get { return _dynamicLayerGroup; }
            set
            {
                if (value != null && _dynamicLayerGroup != value)
                {
                    _dynamicLayerGroup = value;
                    OnPropertyChanged("DynamicLayerGroup", value);
                }
            }
        }

        /// <summary>
        /// Resource strings. (resx option).
        /// </summary>
        [JsonPropertyName("resx")]
        public Dictionary<string, string>? ResourceStrings
        {
            get { return _resourceStrings; }
            set
            {
                if (_resourceStrings != value)
                {
                    _resourceStrings = (value == null) ? new Dictionary<string, string>() : value;
                    OnPropertyChanged("ResourceStrings", "resx", _resourceStrings);
                }
            }
        }

        /// <summary>
        /// The top level title of the legend control.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title", value);
                }
            }
        }

        /// <summary>
        /// By default the layer control expects the legend control instance. 
        /// The following two properties are used to look up the legend control in the map instance.
        /// Behind the scenes the map interface of this library will correctly connect the legend control to the layer control.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("childControlName")]
        internal string ChildControlName = "legendControl";

        /// <summary>
        /// A legend control to display the layer state legends in.
        /// </summary>
        [JsonPropertyName("childControl")]
        public LegendControl? LegendControl
        {
            get { return _legendControl; }
            set
            {
                if (_legendControl != value)
                {
                    _legendControl = value;
                    OnPropertyChanged("LegendControl", "childControl", value);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Manually triggers a refresh of the control.
        /// </summary>
        public void Refresh()
        {
            CallCustomControlFunction("refresh");
        }

        #endregion

        #region Private Methods

        private void LayerGroup_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //Legend order changed. Refresh the legends.
            OnPropertyChanged("LayerGroups", LayerGroups);
        }

        #endregion
    }
}