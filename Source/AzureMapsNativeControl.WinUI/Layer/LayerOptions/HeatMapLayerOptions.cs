using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for the HeatMapLayer.
    /// </summary>
    public class HeatMapLayerOptions : LayerOptions, IDeepCloneable<HeatMapLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies the color gradient used to colorize the pixels in the heatmap. This is defined using an expression that uses ["heatmap-density"] as input. 
        /// </summary>
        [JsonPropertyName("color")]
        public Expression<string>? Color { get; set; }

        /// <summary>
        /// Similar to heatmap-weight but specifies the global heatmap intensity. The higher this value is, the more ‘weight’ each point will contribute to the appearance. 
        /// </summary>
        [JsonPropertyName("intensity")]
        public double? Intensity { get; set; }

        /// <summary>
        /// The opacity at which the heatmap layer will be rendered defined as a number between 0 and 1.
        /// </summary>
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }

        /// <summary>
        /// The radius in pixels used to render a data point on the heatmap. The radius must be a number greater or equal to 1.
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
        /// Specifies how much an individual data point contributes to the heatmap. 
        /// Must be a number greater than 0. A value of 5 would be equivalent to having 5 points of weight 1 in the same spot. 
        /// This is useful when clustering points to allow heatmap rendering or large datasets. 
        /// </summary>
        [JsonPropertyName("weight")]
        public Expression<double>? Weight { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new HeatMapLayerOptions Defaults()
        {
            return new HeatMapLayerOptions()
            {
                Color = new Expression<string>() { "interpolate", new object[] { "linear" }, new object[] { "heatmap-density" }, 0, "rgba(0,0, 255,0)", 0.1, "royalblue", 0.3, "cyan", 0.5, "lime", 0.7, "yellow", 1, "red" },
                Intensity = 1,
                Opacity = 1,
                Radius = Expression<double>.Literal(10),
                Weight = Expression<double>.Literal(1),
                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public new HeatMapLayerOptions DeepClone()
        {
            return new HeatMapLayerOptions
            {
                Color = Color?.DeepClone(),
                Intensity = Intensity,
                Opacity = Opacity,
                Radius = Radius?.DeepClone(),
                SourceLayer = SourceLayer,
                Weight = Weight?.DeepClone(),
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
        public static bool Merge(HeatMapLayerOptions source, HeatMapLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (!Expression.IsNullOrWhiteSpace(source.Color) && source.Color != target.Color)
                {
                    target.Color = source.Color;
                    hasChanges = true;
                }

                if (source.Intensity != null && source.Intensity > 0 && source.Intensity != target.Intensity)
                {
                    target.Intensity = source.Intensity;
                    hasChanges = true;
                }

                if (source.Opacity != null && source.Opacity >= 0 && source.Opacity <= 1 && source.Opacity != target.Opacity)
                {
                    target.Opacity = source.Opacity;
                    hasChanges = true;
                }

                if (Expression.IsGreaterOrEqualTo(source.Radius, 1) && source.Radius != target.Radius)
                {
                    target.Radius = source.Radius;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(source.SourceLayer) && source.SourceLayer != target.SourceLayer)
                {
                    target.SourceLayer = source.SourceLayer;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.Weight) && source.Weight != target.Weight)
                {
                    target.Weight = source.Weight;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}