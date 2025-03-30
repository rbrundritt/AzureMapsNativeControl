using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for the PolygonLayer.
    /// </summary>
    public class PolygonLayerOptions : LayerOptions, IDeepCloneable<PolygonLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// Whether or not the fill should be antialiased. 
        /// </summary>
        [JsonPropertyName("fillAntialias")]
        public bool? FillAntialias { get; set; }

        /// <summary>
        /// Required when the source of the layer is a VectorTileSource. 
        /// A vector source can have multiple layers within it, this identifies which one to render in this layer. 
        /// Prohibited for all other types of sources.
        /// </summary>
        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        /// <summary>
        /// The color to fill the polygons with. 
        /// TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
        /// </summary>
        [JsonPropertyName("fillColor")]
        public Expression<string>? FillColor { get; set; }

        /// <summary>
        /// Name of image in sprite to use for drawing image fills. For seamless patterns, image width must be a factor of two (2, 4, 8, ..., 512).
        /// </summary>
        [JsonPropertyName("fillPattern")]
        public Expression<string>? FillPattern { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the fill will be drawn. 
        /// </summary>
        [JsonPropertyName("fillOpacity")]
        public Expression<double>? FillOpacity { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new PolygonLayerOptions Defaults()
        {
            return new PolygonLayerOptions()
            {
                FillAntialias = true,
                FillColor = Expression<string>.Literal("#1E90FF"),
                FillOpacity = Expression<double>.Literal(0.5),

                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public new PolygonLayerOptions DeepClone()
        {
            return new PolygonLayerOptions
            {
                FillAntialias = FillAntialias,
                SourceLayer = SourceLayer,
                FillColor = FillColor?.DeepClone(),
                FillPattern = FillPattern?.DeepClone(),
                FillOpacity = FillOpacity?.DeepClone(),
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
        public static bool Merge(PolygonLayerOptions source, PolygonLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (source.FillAntialias != null && source.FillAntialias != target.FillAntialias)
                {
                    target.FillAntialias = source.FillAntialias;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(source.SourceLayer) && source.SourceLayer != target.SourceLayer)
                {
                    target.SourceLayer = source.SourceLayer;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.FillColor) && source.FillColor != target.FillColor)
                {
                    target.FillColor = source.FillColor;

                    //Check fill pattern.
                    if(target.FillPattern != null)
                    {
                        if (target.FillPattern.IsUndefined())
                        {
                            //If it was already set to undefined, we can simply set it to null.
                            target.FillPattern = null;
                        } 
                        else
                        {
                            //Otherwise we have to set it to undefined as fill pattern has priority over fill color.
                            target.FillPattern = Expression<string>.Undefined();
                        }
                    }

                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.FillPattern) && source.FillPattern != target.FillPattern)
                {
                    target.FillPattern = source.FillPattern;
                    hasChanges = true;
                }

                if (Expression.IsValueInRange(source.FillOpacity, 0, 1) && source.FillOpacity != target.FillOpacity)
                {
                    target.FillOpacity = source.FillOpacity;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
