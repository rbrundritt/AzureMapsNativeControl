using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Represent the density of data using different colors (HeatMap).
    /// </summary>
    public class HeatMapLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private HeatMapLayerOptions _options = HeatMapLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// Represent the density of data using different colors (HeatMap).
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public HeatMapLayer(BaseSource source, HeatMapLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.HeatMapLayer", source, id)
        {
            if (options != null)
            {
                HeatMapLayerOptions.Merge(options, _options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override HeatMapLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(HeatMapLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (HeatMapLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}