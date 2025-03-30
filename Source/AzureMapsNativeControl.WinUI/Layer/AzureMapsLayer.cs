using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Source;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Represents a non-user added layer in the map control.
    /// Information based on MapLibre Style Specification: https://maplibre.org/maplibre-style-spec/layers/
    /// </summary>
    public class AzureMapsLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties

        public LayerOptions _options = new LayerOptions()
        {
            MinZoom = 0,
            MaxZoom = 24,
            Visible = true
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Represents a non-user added layer in the map control.
        /// </summary>
        /// <param name="layerInfo">Basic information about the layer.</param>
        /// <param name="source">The data source used by the layer.</param>
        /// <param name="map">The map instance the layer is in.</param>
        internal AzureMapsLayer(BasemapLayerInfo layerInfo, AzureMapsSource? source, Map map) : base(layerInfo.LayerType, null, layerInfo.Id, true)
        {
            _options.MinZoom = layerInfo.MinZoom != null ? layerInfo.MinZoom.Value : 0;
            _options.MaxZoom = layerInfo.MaxZoom != null ? layerInfo.MaxZoom.Value : 24;
            _options.Visible = layerInfo.Visible;
            SourceLayer = layerInfo.SourceLayer;

            Source = source;
            _allowManagerAdd = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If the source is a vector tile source, this is the name of the data layer in the tiles.
        /// </summary>
        public string? SourceLayer { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a copy of the base layer options.
        /// </summary>
        /// <returns></returns>
        public override LayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Set the base options of the layer.
        /// </summary>
        /// <param name="options">Options to set.</param>
        public async void SetOptions(LayerOptions options)
        {
            if (options != null && Map != null)
            {
                if (options.Visible != _options.Visible)
                {
                    _options.Visible = options.Visible;

                    var visibility = options.Visible.HasValue && options.Visible.Value ? "visible" : "none";
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayoutProperty", Id, "visibility", visibility);
                }

                if ((options.MinZoom != _options.MinZoom && options.MinZoom >= 0 && options.MinZoom <= 24) || 
                    (options.MaxZoom != _options.MaxZoom && options.MaxZoom >= 0 && options.MaxZoom <= 24))
                {
                    _options.MinZoom = options.MinZoom;
                    _options.MaxZoom = options.MaxZoom;

                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "setInternalLayerZoomRange", Id, options.MinZoom ?? 0, options.MaxZoom ?? 24);
                }

                if (options.Filter != _options.Filter)
                {
                    _options.Filter = options.Filter;

                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerFilter", Id, options.Filter);
                }
            }
        }

        /// <summary>
        /// Set a paint property on the layer.
        /// Based on MapLibre Style Specification: https://maplibre.org/maplibre-style-spec/layers/
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value to set.</param>
        public async Task SetPaintProperty(string name, object value)
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setPaintProperty", Id, name, value);
            }
        }

        /// <summary>
        /// Set a layout property on the layer.
        /// Based on MapLibre Style Specification: https://maplibre.org/maplibre-style-spec/layers/
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value to set.</param>
        public async Task SetLayoutProperty(string name, object value)
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayoutProperty", Id, name, value);
            }
        }

        #endregion
    }
}
