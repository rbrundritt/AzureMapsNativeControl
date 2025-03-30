using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for a BubbleLayer.
    /// </summary>
    public class BubbleLayerOptions : LayerOptions, IDeepCloneable<BubbleLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// The amount to blur the circles. A value of 1 blurs the circles such that only the center point if at full opacity. 
        /// </summary>
        [JsonPropertyName("blur")]
        public Expression<double>? Blur { get; set; }

        /// <summary>
        /// A CSS color value that fills the circle symbol with.
        /// TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
        /// </summary>
        [JsonPropertyName("color")]
        public Expression<string>? Color { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the circles will be drawn.
        /// </summary>
        [JsonPropertyName("opacity")]
        public Expression<double>? Opacity { get; set; }

        /// <summary>
        /// Specifies the orientation of circle when map is pitched.
        /// </summary>
        [JsonPropertyName("pitchAlignment")]
        public PitchAlignment? PitchAlignment { get; set; }

        /// <summary>
        /// The radius of the circle symbols in pixels. Must be greater than or equal to 0.
        /// </summary>
        [JsonPropertyName("radius")]
        public Expression<double>? Radius { get; set; }

        /// <summary>
        /// Required when the source of the layer is a VectorTileSource. 
        /// A vector source can have multiple layers within it, this identifies which one to render in this layer. 
        /// Prohibited for all other types of sources.
        /// </summary>
        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        /// <summary>
        /// A CSS color value that outlines the circle symbol with.
        /// TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
        /// </summary>
        [JsonPropertyName("strokeColor")]
        public Expression<string>? StrokeColor { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the circles' outlines will be drawn. 
        /// </summary>
        [JsonPropertyName("strokeOpacity")]
        public Expression<double>? StrokeOpacity { get; set; }

        /// <summary>
        /// The width of the circles' outlines in pixels. 
        /// </summary>
        [JsonPropertyName("strokeWidth")]
        public Expression<int>? StrokeWidth { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new BubbleLayerOptions Defaults()
        {
            return new BubbleLayerOptions()
            {
                Blur = Expression<double>.Literal(0),
                Color = Expression<string>.Literal("#1A73AA"),
                Opacity = Expression<double>.Literal(1),
                PitchAlignment = AzureMapsNativeControl.PitchAlignment.Viewport,
                Radius = Expression<double>.Literal(8),
                StrokeColor = Expression<string>.Literal("#FFFFFF"),
                StrokeOpacity = Expression<double>.Literal(1),
                StrokeWidth = Expression<int>.Literal(2),
                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <summary>
        /// Clones the options.
        /// </summary>
        /// <returns></returns>
        public new BubbleLayerOptions DeepClone()
        {
            return new BubbleLayerOptions
            {
                Blur = Blur?.DeepClone(),
                Color = Color?.DeepClone(),
                Opacity = Opacity?.DeepClone(),
                PitchAlignment = PitchAlignment,
                Radius = Radius?.DeepClone(),
                SourceLayer = SourceLayer,
                StrokeColor = StrokeColor?.DeepClone(),
                StrokeOpacity = StrokeOpacity?.DeepClone(),
                StrokeWidth = StrokeWidth?.DeepClone(),
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
        public static bool Merge(BubbleLayerOptions source, BubbleLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (Expression.IsValueInRange(source.Blur, 0, 1) && source.Blur != target.Blur)
                {
                    target.Blur = source.Blur;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.Color) && source.Color != target.Color)
                {
                    target.Color = source.Color;
                    hasChanges = true;
                }

                if (Expression.IsValueInRange(source.Opacity, 0, 1) && source.Opacity != target.Opacity)
                {
                    target.Opacity = source.Opacity;
                    hasChanges = true;
                }

                if (source.PitchAlignment != null && source.PitchAlignment != target.PitchAlignment)
                {
                    if (source.PitchAlignment == AzureMapsNativeControl.PitchAlignment.Auto)
                    {
                        source.PitchAlignment = AzureMapsNativeControl.PitchAlignment.Map;
                    }
                    target.PitchAlignment = source.PitchAlignment;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.Radius) && source.Radius != target.Radius)
                {
                    target.Radius = source.Radius;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(source.SourceLayer) && source.SourceLayer != target.SourceLayer)
                {
                    target.SourceLayer = source.SourceLayer;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.StrokeColor) && source.StrokeColor != target.StrokeColor)
                {
                    target.StrokeColor = source.StrokeColor;
                    hasChanges = true;
                }

                if (Expression.IsValueInRange(source.StrokeOpacity, 0, 1) && source.StrokeOpacity != target.StrokeOpacity)
                {
                    target.StrokeOpacity = source.StrokeOpacity;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.StrokeWidth) && source.StrokeWidth != target.StrokeWidth)
                {
                    target.StrokeWidth = source.StrokeWidth;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
