using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// A base class which all other layer options inherit from.
    /// </summary>
    [JsonDerivedType(typeof(BubbleLayerOptions))]
    [JsonDerivedType(typeof(LineLayerOptions))]
    [JsonDerivedType(typeof(PolygonLayerOptions))]
    [JsonDerivedType(typeof(PolygonExtrusionLayerOptions))]
    [JsonDerivedType(typeof(SymbolLayerOptions))]
    [JsonDerivedType(typeof(HeatMapLayerOptions))]
    [JsonDerivedType(typeof(ImageLayerOptions))]   
    public class LayerOptions: IDeepCloneable<LayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// An expression specifying conditions on source features. Only features that match the filter are displayed.
        /// </summary>
        [JsonPropertyName("filter")]
        public Expression<bool>? Filter { get; set; }

        /// <summary>
        /// An integer specifying the maximum zoom level to render the layer at. 
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public int? MaxZoom { get; set; }

        /// <summary>
        /// An integer specifying the minimum zoom level to render the layer at. 
        /// </summary>
        [JsonPropertyName("minZoom")]
        public int? MinZoom { get; set; }

        /// <summary>
        /// Specifies if the layer is visible or not. 
        /// </summary>
        [JsonPropertyName("visible")]
        public bool? Visible { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the default options.
        /// </summary>
        /// <returns></returns>
        public static LayerOptions Defaults()
        {
            return new LayerOptions()
            {
                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public LayerOptions DeepClone()
        {
            return new LayerOptions()
            {
                Filter = Filter?.DeepClone(),
                MaxZoom = MaxZoom,
                MinZoom = MinZoom,
                Visible = Visible
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(LayerOptions source, LayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.Filter != null && source.Filter != target.Filter)
                {
                    target.Filter = source.Filter;
                    hasChanges = true;
                }

                if (source.MinZoom != null && source.MinZoom >= 0 && source.MinZoom <= 24 && source.MinZoom != target.MinZoom)
                {
                    target.MinZoom = source.MinZoom;
                    hasChanges = true;
                }

                if (source.MaxZoom != null && source.MaxZoom >= 0 && source.MaxZoom <= 24 && source.MaxZoom != target.MaxZoom)
                {
                    target.MaxZoom = source.MaxZoom;
                    hasChanges = true;
                }

                if (source.Visible != null && source.Visible != target.Visible)
                {
                    target.Visible = source.Visible;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}