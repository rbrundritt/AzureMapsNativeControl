using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for the PolygonExtrusionLayer.
    /// </summary>
    public class PolygonExtrusionLayerOptions : LayerOptions, IDeepCloneable<PolygonExtrusionLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// The height in meters to extrude the base of this layer. 
        /// This height is relative to the ground. 
        /// Must be greater or equal to 0 and less than or equal to height. 
        /// </summary>
        [JsonPropertyName("base")]
        public Expression<double>? Base { get; set; }

        /// <summary>
        /// The height in meters to extrude this layer. This height is relative to the ground. Must be a number greater or equal to 0.
        /// </summary>
        [JsonPropertyName("height")]
        public Expression<double>? Height { get; set; }

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
        public double? FillOpacity { get; set; }

        /// <summary>
        /// The polygons' pixel offset. Values are [x, y] where negatives indicate left and up, respectively. 
        /// </summary>
        [JsonPropertyName("translate")]
        public Pixel? Translate { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("tanslateAnchor")]
        public PitchAlignment? TranslateAnchor { get; set; }

        /// <summary>
        /// Specifies if the polygon should have a vertical gradient on the sides of the extrusion. 
        /// </summary>
        [JsonPropertyName("verticalGradient")]
        public bool? VerticalGradient { get; set; }

        /// <summary>
        /// Required when the source of the layer is a VectorTileSource. 
        /// A vector source can have multiple layers within it, this identifies which one to render in this layer. 
        /// Prohibited for all other types of sources.
        /// </summary>
        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new PolygonExtrusionLayerOptions Defaults()
        {
            return new PolygonExtrusionLayerOptions()
            {
                Base = Expression<double>.Literal(0),
                FillColor = Expression<string>.Literal("#1E90FF"),
                FillOpacity = 1,
                Height = Expression<double>.Literal(0),
                Translate = new Pixel(0, 0),
                TranslateAnchor = PitchAlignment.Map,
                VerticalGradient = true,

                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public new PolygonExtrusionLayerOptions DeepClone()
        {
            return new PolygonExtrusionLayerOptions
            {
                Base = Base?.DeepClone(),
                Height = Height?.DeepClone(),
                Translate = Translate?.DeepClone(),
                TranslateAnchor = TranslateAnchor,
                VerticalGradient = VerticalGradient,

                SourceLayer = SourceLayer,
                FillColor = FillColor?.DeepClone(),
                FillPattern = FillPattern?.DeepClone(),
                FillOpacity = FillOpacity,

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
        public static bool Merge(PolygonExtrusionLayerOptions source, PolygonExtrusionLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (!Expression.IsNull(source.Base) && source.Base != target.Base)
                {
                    target.Base = source.Base;
                    hasChanges = true;
                }

                if (!Expression.IsNull(source.Height) && source.Height != target.Height)
                {
                    target.Height = source.Height;
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
                    if (target.FillPattern != null)
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

                if (source.FillOpacity != null && source.FillOpacity >= 0 && source.FillOpacity <= 1 && source.FillOpacity != target.FillOpacity)
                {
                    target.FillOpacity = source.FillOpacity;
                    hasChanges = true;
                }

                if (source.Translate != null && source.Translate != target.Translate)
                {
                    target.Translate = source.Translate;
                    hasChanges = true;
                }

                if (source.TranslateAnchor != null && source.TranslateAnchor != target.TranslateAnchor)
                {
                    if (source.TranslateAnchor == PitchAlignment.Auto)
                    {
                        source.TranslateAnchor = PitchAlignment.Map;
                    }

                    target.TranslateAnchor = source.TranslateAnchor;
                    hasChanges = true;
                }

                if (source.VerticalGradient != null && source.VerticalGradient != target.VerticalGradient)
                {
                    target.VerticalGradient = source.VerticalGradient;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
