using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Renders extruded filled Polygon and MultiPolygon objects on the map.
    /// </summary>
    public class PolygonExtrusionLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private PolygonExtrusionLayerOptions _options = PolygonExtrusionLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// Renders extruded filled Polygon and MultiPolygon objects on the map.
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public PolygonExtrusionLayer(BaseSource source, PolygonExtrusionLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.PolygonExtrusionLayer", source, id)
        {
            if (options != null)
            {
                PolygonExtrusionLayerOptions.Merge(options, this._options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override PolygonExtrusionLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(PolygonExtrusionLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (PolygonExtrusionLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}