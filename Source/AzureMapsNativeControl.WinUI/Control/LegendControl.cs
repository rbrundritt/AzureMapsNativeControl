using AzureMapsNativeControl.Control.Legends;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control that displays legend information on the map.
    /// https://github.com/Azure-Samples/azure-maps-layer-legend/blob/main/docs/legend_control.md
    /// </summary>
    public sealed class LegendControl : BaseLegendLayerControl
    {
        #region Private Properties

        private string? _title = null;
        private ObservableCollection<BaseLegend> _legends;
        private Dictionary<string, string>? _resourceStrings = null;

        #endregion

        #region Constructor

        /// <summary>
        /// A control that displays legend information on the map.
        /// </summary>
        public LegendControl() : base("atlas.control.LegendControl")
        {
            _legends = new ObservableCollection<BaseLegend>();
            _legends.CollectionChanged += Legends_CollectionChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of legends to generate.
        /// </summary>
        [JsonPropertyName("legends")]
        public IList<BaseLegend> Legends
        {
            get { return _legends; }
            set
            {
                if (value != null && _legends != value)
                {
                    if (_legends != null)
                    {
                        _legends.CollectionChanged -= Legends_CollectionChanged;
                    }

                    _legends = new ObservableCollection<BaseLegend>(value);

                    if (_legends != null)
                    {
                        _legends.CollectionChanged += Legends_CollectionChanged;
                    }

                    OnPropertyChanged("Legends", value);
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the specified legend index within a carousel or list.
        /// </summary>
        /// <param name="idx">The legend index in the array of legends in the legend control options.</param>
        /// <param name="focus">Specifies if tab focus should move inside of the specified legend.</param>
        public void SetLegendIndex(int idx, bool focus = false)
        {
            CallCustomControlFunction("setLegendIdx", idx, focus);
        }

        /// <summary>
        /// Puts the specified legend in view of the user. If in carousel mode, will switch to that legend.
        /// </summary>
        /// <param name="legend">The legend to focus on.</param>
        public void Focus(BaseLegend legend)
        {
            var idx = _legends.IndexOf(legend);
            if(idx > -1)
            {
                CallCustomControlFunction("setLegendIdx", idx, true);
            }
        }

        #endregion

        #region Private Methods

        private void Legends_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //Legend order changed. Refresh the legends.
            OnPropertyChanged("Legends", Legends);
        }

        #endregion
    }
}
