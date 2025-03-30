using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Layer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// Manages a pixel based grid for snapping positions at integer based zoom levels.
    /// </summary>
    public class SnapGridManager : IDisposable, INotifyPropertyChanged
    {
        #region Private Properties

        private const string JsNamespace = "atlas.drawing.SnapGridManager";

        private bool _isInitialized = false;
        private Map _map;

        private bool _enabled = true;
        private bool _removeDuplicates = true;
        private int _resolution = 15;
        private bool _showGrid = false;
        private bool _simplify = true;

        private LineLayerOptions _gridLayerOptions = LineLayerOptions.Defaults();

        [JsonInclude]
        [JsonPropertyName("id")]
        internal string Id { get; set; } = UniqueId.Get(JsNamespace);

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the DrawingManager class.
        /// </summary>
        /// <param name="map">The map to attach the drawing manager to.</param>
        public SnapGridManager(Map map)
        {
            if (!map._isReady)
            {
                throw new Exception("SnapGridManager can only be created once the map is ready.");
            }

            _map = map;
            _gridLayerOptions.StrokeColor = Expression<string>.Literal("#c3a77e");
            _gridLayerOptions.StrokeWidth = Expression<int>.Literal(1);

            Initialize();
        }

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Specifies if the snapping functions are enabled. If not, the original shapes and positions will be returned. Default: true
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged("Enabled", value);
                }
            }
        }

        /// <summary>
        /// Specifies duplicate sequential positions should be removed when snapping. Default: true
        /// </summary>
        [JsonPropertyName("removeDuplicates")]
        public bool RemoveDuplicates
        {
            get => _removeDuplicates;
            set
            {
                if (_removeDuplicates != value)
                {
                    _removeDuplicates = value;
                    OnPropertyChanged("RemoveDuplicates", value);
                }
            }
        }

        /// <summary>
        /// Specifies the size of the snapping grid in pixels. The grid will be square and relative to the nearest integer zoom level. 
        /// The grid will scale by a factor of 2 relative to physical real-world area with each zoom level. Default: 15
        /// </summary>
        [JsonPropertyName("resolution")]
        public int Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    OnPropertyChanged("Resolution", value);
                }
            }
        }

        /// <summary>
        /// Specifies if grid lines should be displayed on the map. Default: false
        /// </summary>
        [JsonPropertyName("showGrid")]
        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (_showGrid != value)
                {
                    _showGrid = value;
                    OnPropertyChanged("ShowGrid", value);
                }
            }
        }

        /// <summary>
        /// Specifies if a Douglas-Peucker simplification should occur while snapping to create smoother lines. Default: true
        /// </summary>
        [JsonPropertyName("simplify")]
        public bool Simplify
        {
            get => _simplify;
            set
            {
                if (_simplify != value)
                {
                    _simplify = value;
                    OnPropertyChanged("Simplify", value);
                }
            }
        }

        /// <summary>
        /// The style options used by the grid lines.
        /// </summary>
        [JsonPropertyName("gridLayerOptions")]
        public LineLayerOptions GridLayerOptions
        {
            get => _gridLayerOptions;
            set
            {
                if (value != null && _gridLayerOptions != value)
                {
                    LineLayerOptions.Merge(value, _gridLayerOptions);
                    value.Visible = _showGrid; //Don't let the user set the visibility of the layer.
                    OnPropertyChanged("GridLayerOptions", value);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async void Dispose()
        {
            if (_isInitialized)
            {
                //Dispose the JS version of the manager.
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "callGenericItemFunction", Id, Constants.GenericCache, "dispose");
            }
        }

        /// <summary>
        /// Snaps an array of positions to the grid. If `optimize` is set, duplicate sequential positions will be removed from the shape.
        /// </summary>
        /// <param name="positions">The positions to snap.</param>
        /// <param name="zoom">Optionally specify which zoom level to optimize the snapping for. If not specified, the maps zoom level will be used.</param>
        /// <returns>A new set of snapped positions.</returns>
        public async Task<IList<Position>> SnapPositionsAsync(IList<Position> positions, int? zoom = null)
        {
            if (Enabled)
            {
                var r = await _map.JsInterlop.InvokeJsMethodAsync<List<Position>>(_map, "snapGridManagerSnapPositions", Id, positions, zoom);

                if (r != null)
                {
                    return r;
                }
            }

            return positions;
        }

        /// <summary>
        /// Snaps a GeoJSON feature coordinates to the grid.
        /// </summary>
        /// <param name="feature">The feature to snape</param>
        /// <param name="zoom">Optionally specify which zoom level to optimize the snapping for. If not specified, the maps zoom level will be used.</param>
        /// <returns>A snapped version of the feature (if enabled and successful), or the original feature.</returns>
        public async Task<Feature> SnapFeatureAsync(Feature feature, int? zoom = null)
        {
            if (Enabled)
            {
                //Only need to pass the geometry of the feature. No need to pass properties.
                var f = new Feature(feature.Geometry);
                var snapped = await _map.JsInterlop.InvokeJsMethodAsync<Feature>(_map, "snapGridManagerSnapFeature", Id, f, zoom);

                if (snapped != null)
                {
                    feature.Geometry = snapped.Geometry;
                }
            }

            return feature;
        }

        #endregion

        #region Private Methods

        private async void Initialize()
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                //Ensure the drawing module is loaded.
                var module = DrawingManager.GetModuleInfo();
                await _map.JsInterlop.LoadModule(module);

                //Add the snap grid manager to the JS map.
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "updateSnapGridManager", this);

                _isInitialized = true;
            }
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
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "updateSnapGridManager", this);
                }
                else
                {
                    //Only update the individual property
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "updateSnapGridManager", new Dictionary<string, object?>()
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
