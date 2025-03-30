using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for a symbol layer.
    /// </summary>
    public class SymbolLayerOptions : LayerOptions, IDeepCloneable<SymbolLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies the label placement relative to its geometry.
        /// </summary>  
        [JsonPropertyName("placement")]
        public LabelPlacement? Placement { get; set; }

        /// <summary>
        /// Distance in pixels between two symbol anchors along a line. Must be greater or equal to 1. 
        /// </summary>  
        [JsonPropertyName("lineSpacing")]
        public int? LineSpacing { get; set; }

        /// <summary>
        /// Required when the source of the layer is a VectorTileSource. 
        /// A vector source can have multiple layers within it, this identifies which one to render in this layer. 
        /// Prohibited for all other types of sources.
        /// </summary>
        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        /// <summary>
        /// Options used to customize the icons of the symbols.
        /// </summary>
        [JsonPropertyName("iconOptions")]
        public IconOptions? IconOptions { get; set; }

        /// <summary>
        /// Options used to customize the text of the symbols.
        /// </summary>
        [JsonPropertyName("textOptions")]
        public TextOptions? TextOptions { get; set; }

        /// <summary>
        /// Sorts features in ascending order based on this value. Features with lower sort keys are drawn and placed first. 
        /// </summary>
        [JsonPropertyName("sortKey")]
        public Expression<double>? SortKey { get; set; }

        /// <summary>
        /// Determines whether overlapping symbols in the same layer are rendered in the order that they appear in the data source, 
        /// or by their y position relative to the viewport. 
        /// To control the order and prioritization of symbols otherwise, use sortKey.
        /// </summary>
        [JsonPropertyName("zOrder")]
        public ZOrderType Zorder { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new SymbolLayerOptions Defaults()
        {
            return new SymbolLayerOptions()
            {
                //Base symbol options
                LineSpacing = 250,
                Placement = LabelPlacement.Point,

                //Icon options
                IconOptions = IconOptions.Defaults(),

                //Text Options
                TextOptions = TextOptions.Defaults(),

                //Layer Options
                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public new SymbolLayerOptions DeepClone()
        {
            return new SymbolLayerOptions()
            {
                Placement = Placement,
                LineSpacing = LineSpacing,
                SourceLayer = SourceLayer,
                IconOptions = IconOptions?.DeepClone(),
                TextOptions = TextOptions?.DeepClone(),
                SortKey = SortKey?.DeepClone(),
                Filter = Filter?.DeepClone(),
                Visible = Visible,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(SymbolLayerOptions source, SymbolLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (source.IconOptions != null)
                {
                    if (target.IconOptions != null)
                    {
                        hasChanges |= IconOptions.Merge(source.IconOptions, target.IconOptions);
                    }
                    else
                    {
                        target.IconOptions = source.IconOptions;
                        hasChanges = true;
                    }
                }

                if (source.TextOptions != null)
                {
                    if (target.TextOptions != null)
                    {
                        hasChanges |= TextOptions.Merge(source.TextOptions, target.TextOptions);
                    }
                    else
                    {
                        target.TextOptions = source.TextOptions;
                        hasChanges = true;
                    }
                }

                if (source.LineSpacing != null && source.LineSpacing > 0 && source.LineSpacing != target.LineSpacing)
                {
                    target.LineSpacing = source.LineSpacing;
                    hasChanges = true;
                }

                if (source.Placement != null && source.Placement != target.Placement)
                {
                    target.Placement = source.Placement;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(source.SourceLayer) && source.SourceLayer != target.SourceLayer)
                {
                    target.SourceLayer = source.SourceLayer;
                    hasChanges = true;
                }

                if (!Expression.IsNull(target.SortKey) && source.SortKey != target.SortKey)
                {
                    target.SortKey = source.SortKey;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
