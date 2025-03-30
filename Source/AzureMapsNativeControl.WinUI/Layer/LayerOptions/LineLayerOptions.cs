using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for the LineLayer.
    /// </summary>
    public class LineLayerOptions : LayerOptions, IDeepCloneable<LineLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// The amount to blur the circles. A value of 1 blurs the circles such that only the center point if at full opacity. 
        /// </summary>
        [JsonPropertyName("blur")]
        public Expression<double>? Blur { get; set; }

        /// <summary>
        /// Specifies how the ends of the lines are rendered.
        /// </summary>
        [JsonPropertyName("lineCap")]
        public LineCap? LineCap { get; set; }

        /// <summary>
        /// Specifies how the joints in the lines are rendered.
        /// </summary>
        [JsonPropertyName("lineJoin")]
        public LineJoin? LineJoin { get; set; }

        /// <summary>
        /// The line's offset. A positive value offsets the line to the right, relative to the direction of the line. A negative value offsets to the left. 
        /// </summary>
        [JsonPropertyName("offset")]
        public Expression<double>? Offset { get; set; }

        /// <summary>
        /// Required when the source of the layer is a VectorTileSource. 
        /// A vector source can have multiple layers within it, this identifies which one to render in this layer. 
        /// Prohibited for all other types of sources.
        /// </summary>
        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        /// <summary>
        /// Specifies the color of the line. 
        /// TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
        /// </summary>
        [JsonPropertyName("strokeColor")]
        public Expression<string>? StrokeColor { get; set; }

        /// <summary>
        /// Specifies the lengths of the alternating dashes and gaps that form the dash pattern. 
        /// Numbers must be equal or greater than 0. 
        /// The lengths are scaled by the strokeWidth. 
        /// To convert a dash length to pixels, multiply the length by the current stroke width.
        /// </summary>
        [JsonPropertyName("strokeDashArray")]
        public IList<int>? StrokeDashArray { get; set; }

        /// <summary>
        /// Defines a gradient with which to color the lines. 
        /// Requires the DataSource lineMetrics option to be set to true. 
        /// Disabled if strokeDashArray is set. 
        /// </summary>
        [JsonPropertyName("strokeGradient")]
        public Expression<string>? StrokeGradient { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the line will be drawn.
        /// </summary>
        [JsonPropertyName("strokeOpacity")]
        public Expression<double>? StrokeOpacity { get; set; }

        /// <summary>
        /// The width of the circles' outlines in pixels. 
        /// </summary>
        [JsonPropertyName("strokeWidth")]
        public Expression<int>? StrokeWidth { get; set; }

        /// <summary>
        /// The amount of offset in pixels to render the line relative to where it would render normally. Negative values indicate left and up.
        /// </summary>
        [JsonPropertyName("translate")]
        public Pixel? Translate { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("translateAnchor")]
        public PitchAlignment? TranslateAnchor { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new LineLayerOptions Defaults()
        {
            return new LineLayerOptions()
            {
                Blur = Expression<double>.Literal(0),
                LineCap = Layer.LineCap.Round,
                LineJoin = Layer.LineJoin.Round,
                Offset = Expression<double>.Literal(0),
                StrokeColor = Expression<string>.Literal("#1E90FF"),
                StrokeOpacity = Expression<double>.Literal(1),
                StrokeWidth = Expression<int>.Literal(2),
                Translate = new Pixel(0, 0),
                TranslateAnchor = PitchAlignment.Map,

                MinZoom = 0,
                MaxZoom = 24,
                Visible = true
            };
        }

        /// <inheritdoc/>
        public new LineLayerOptions DeepClone()
        {
            return new LineLayerOptions
            {
                Blur = Blur?.DeepClone(),
                LineCap = LineCap,
                LineJoin = LineJoin,
                Offset = Offset?.DeepClone(),
                SourceLayer = SourceLayer,
                StrokeColor = StrokeColor?.DeepClone(),
                StrokeDashArray = StrokeDashArray?.ToArray(),
                StrokeGradient = StrokeGradient?.DeepClone(),
                StrokeOpacity = StrokeOpacity?.DeepClone(),
                StrokeWidth = StrokeWidth?.DeepClone(),
                Translate = Translate?.DeepClone(),
                TranslateAnchor = TranslateAnchor,

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
        public static bool Merge(LineLayerOptions source, LineLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (Expression.IsPositive(source.Blur) && source.Blur != target.Blur)
                {
                    target.Blur = source.Blur;
                    hasChanges = true;
                }

                if (source.LineCap != null && source.LineCap != target.LineCap)
                {
                    target.LineCap = source.LineCap;
                    hasChanges = true;
                }

                if (source.LineJoin != null && source.LineJoin != target.LineJoin)
                {
                    target.LineJoin = source.LineJoin;
                    hasChanges = true;
                }

                if (!Expression.IsNull(source.Offset) && source.Offset != target.Offset)
                {
                    target.Offset = source.Offset;
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

                    //Check to see if stroke gradient has been previously set.
                    if (target.StrokeGradient != null)
                    {
                        if (target.StrokeGradient.IsUndefined())
                        {
                            //If it was already set to undefined, we can simply set it to null.
                            target.StrokeGradient = null;
                        }
                        else
                        {
                            //Otherwise we have to set it to undefined as stroke gradient has priority over stroke color.
                            target.StrokeGradient = Expression<string>.Undefined();
                        }
                    }

                    hasChanges = true;
                }

                if (source.StrokeGradient != null && source.StrokeGradient != target.StrokeGradient)
                {
                    target.StrokeGradient = source.StrokeGradient;
                    hasChanges = true;
                }

                if (source.StrokeDashArray != null && source.StrokeDashArray != target.StrokeDashArray)
                {
                    target.StrokeDashArray = source.StrokeDashArray;
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

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
