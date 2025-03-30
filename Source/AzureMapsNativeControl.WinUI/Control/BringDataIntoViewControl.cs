using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control that makes it easy to bring any data loaded on the map into view.
    /// https://github.com/Azure-Samples/azure-maps-bring-data-into-view-control
    /// </summary>
    public sealed class BringDataIntoViewControl : BaseControl
    {
        #region Private Properties

        private bool _includeImageLayers = true;
        private bool _includeMarkers = true;
        private ControlStyle? _style = ControlStyle.Light;
        private string? _styleColor = null;
        private int _padding = 100;
        private ObservableCollection<BaseSource> _sources = new ObservableCollection<BaseSource>();
        private List<string>? _sourcesIds;

        #endregion

        #region Constructor

        /// <summary>
        /// A control that makes it easy to bring any data loaded on the map into view.
        /// </summary>
        public BringDataIntoViewControl() : base("atlas.control.BringDataIntoViewControl", AzureMapsModules.BringDataIntoViewModule)
        {
            _sources.CollectionChanged += Sources_CollectionChanged;
        }

        #endregion

        #region Public Properties

        [JsonInclude]
        [JsonPropertyName("sources")]
        internal List<string>? SourceIds
        {
            get
            {
                return _sourcesIds;
            }
            set
            {
                _sourcesIds = value;
                OnPropertyChanged("Sources", value);
            }
        }

        /// <summary>
        /// An arrary of data source objects or IDs to focus on. By default this control will calculate the coverage area of DataSource imstances in the map.
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<BaseSource> Sources
        {
            get { return _sources; }
            set
            {
                if (value != null && _sources != value)
                {
                    if (_sources != null)
                    {
                        _sources.CollectionChanged -= Sources_CollectionChanged;
                    }

                    _sources = value;

                    if (_sources != null)
                    {
                        _sources.CollectionChanged += Sources_CollectionChanged;
                    }

                    if (_sources != null && _sources.Count > 0)
                    {
                        var ids = new List<string>();


                        foreach (var source in _sources)
                        {
                            ids.Add(source.Id);
                        }

                        SourceIds = ids;
                    }
                }
            }
        }

        /// <summary>
        /// Specifies if image layer should be included in the data view calculation: Default: true
        /// </summary>
        [JsonPropertyName("includeImageLayers")]
        public bool IncludeImageLayers
        {
            get
            {
                return _includeImageLayers;
            }
            set
            {
                _includeImageLayers = value;
                OnPropertyChanged("IncludeImageLayers", value);
            }
        }

        /// <summary>
        /// Specifies if HTML markers should be included in the data view calculation: Default: true
        /// Default: false
        /// </summary>
        [JsonPropertyName("includeMarkers")]
        public bool IncludeMarkers
        {
            get
            {
                return _includeMarkers;
            }
            set
            {
                _includeMarkers = value;
                OnPropertyChanged("IncludeMarkers", value);
            }
        }

        /// <summary>
        /// The amount of pixel padding around the data to account for when setting the map view. Default: 100
        /// </summary>
        [JsonPropertyName("padding")]
        public int Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                _padding = value;
                OnPropertyChanged("Padding", value);
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

        #endregion

        #region Private Methods

        private void Sources_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Sources", _sourcesIds);
        }

        #endregion
    }
}