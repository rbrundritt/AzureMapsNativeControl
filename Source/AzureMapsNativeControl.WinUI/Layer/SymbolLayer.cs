using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Renders point based data as symbols on the map using text and/or icons. Symbols can also be created for line and polygon data as well.
    /// </summary>
    public class SymbolLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private SymbolLayerOptions _options = SymbolLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// Renders point based data as symbols on the map using text and/or icons. Symbols can also be created for line and polygon data as well.
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public SymbolLayer(BaseSource source, SymbolLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.SymbolLayer", source, id)
        {
            if (options != null)
            {
                SymbolLayerOptions.Merge(options, _options);
            }
        }

        internal SymbolLayer(BaseSource source, SymbolLayerOptions? options = null, string? id = null, bool allowNonUniqueId = false) :
            base("atlas.layer.SymbolLayer", source, id, allowNonUniqueId)
        {
            if (options != null)
            {
                SymbolLayerOptions.Merge(options, _options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override SymbolLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(SymbolLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (SymbolLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}