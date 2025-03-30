using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Layer;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Base constructor for all layers.
    /// </summary>
    [JsonDerivedType(typeof(BubbleLayer))]
    [JsonDerivedType(typeof(LineLayer))]
    [JsonDerivedType(typeof(PolygonLayer))]
    [JsonDerivedType(typeof(PolygonExtrusionLayer))]
    [JsonDerivedType(typeof(SymbolLayer))]
    [JsonDerivedType(typeof(HeatMapLayer))]
    [JsonDerivedType(typeof(ImageLayer))]
    [JsonDerivedType(typeof(TileLayer))]
    [JsonDerivedType(typeof(AnimatedTileLayer))]
    public abstract class BaseLayer : MapEntity<LayerOptions>
    {
        #region Private Properties

        internal bool _allowManagerAdd = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Base constructor for all layers.
        /// </summary>
        /// <param name="jsNamespace"> The JavaScript namespace of the layer class.</param>
        /// <param name="source">The source of data for the layer.</param>
        /// <param name="id">Unique ID for the layer.</param>
        public BaseLayer(string jsNamespace, BaseSource? source = null, string? id = null) : base(jsNamespace, id)
        {
            Source = source;
        }

        internal BaseLayer(string jsNamespace, BaseSource? source = null, string? id = null, bool allowNonUniqueId = false) : base(jsNamespace, id, allowNonUniqueId)
        {
            Source = source;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the layer that the layer will be inserted before.
        /// Does not trigger an update if changed. Use LayerManager.Move to change the order of layers.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("beforeLayerId")]
        public string? BeforeLayerId { get; set; } = null;

        /// <summary>
        /// Read only. The data source of the layer.
        /// For TileLayer, ImageLayer, and VectorTileLayer, use the SetSource method to set the source.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source")]
        public BaseSource? Source { get; internal set; }

        #endregion

        #region Public Methods

        public override abstract LayerOptions GetOptions();

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(LayerOptions options)
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, options);
            }
        }

        #endregion
    }
}
