using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// A layer that renders Point objects as scalable circles (bubbles).
    /// </summary>
    public class BubbleLayer: BaseLayer, IMapEventTarget
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private BubbleLayerOptions _options = BubbleLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// A layer that renders Point objects as scalable circles (bubbles).
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public BubbleLayer(BaseSource source, BubbleLayerOptions? options = null, string? id = null) : base("atlas.layer.BubbleLayer", source, id)
        {
            if (options != null)
            {
                BubbleLayerOptions.Merge(options, _options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the bubble layer.
        /// </summary>
        /// <returns></returns>
        public override BubbleLayerOptions GetOptions()
        {
           return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the bubble layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(BubbleLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (BubbleLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}