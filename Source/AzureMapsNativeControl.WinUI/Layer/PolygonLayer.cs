using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Renders filled Polygon and MultiPolygon objects on the map.
    /// </summary>
    public class PolygonLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private PolygonLayerOptions _options = PolygonLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// Renders filled Polygon and MultiPolygon objects on the map.
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public PolygonLayer(BaseSource source, PolygonLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.PolygonLayer", source, id)
        {
            if (options != null)
            {
                PolygonLayerOptions.Merge(options, _options);
            }
        }

        internal PolygonLayer(BaseSource source, PolygonLayerOptions? options = null, string? id = null, bool allowNonUniqueId = false) :
            base("atlas.layer.PolygonLayer", source, id, allowNonUniqueId)
        {
            if (options != null)
            {
                PolygonLayerOptions.Merge(options, _options);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override PolygonLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(PolygonLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (PolygonLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}