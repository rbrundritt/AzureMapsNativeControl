using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// Category legend item options.
    /// </summary>
    public class CategoryLegendItem
    {
        public CategoryLegendItem(string? color = null, string? label = null, string? shape = null, int? shapeSize = null, int? strokeWidth = null, string? cssClass = null)
        {
            Color = color;
            Label = label;
            Shape = shape;
            ShapeSize = shapeSize;
            StrokeWidth = strokeWidth;
            CssClass = cssClass;
        }

        #region Public Properties

        /// <summary>
        /// The fill color of SVG items of an individual category item. Overrides `CategoryLegendType` level `color`.
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// The label to display for the item.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// The shape of the color swatch. Overrides the top level shape setting for this individual item. Supports image urls and SVG strings.
        /// </summary>
        [JsonPropertyName("shape")]
        public string? Shape { get; set; }

        /// <summary>
        /// The size of the all shapes in pixels. Used to scale the width of the shape.Overrides `CategoryLegend` level `shapeSize`.
        /// </summary>
        [JsonPropertyName("shapeSize")]
        public int? ShapeSize { get; set; }

        /// <summary>
        /// The thickness of the stroke on SVG shapes in pixels. Overrides `CategoryLegend` level `strokeWidth`.
        /// </summary>
        [JsonPropertyName("strokeWidth")]
        public int? StrokeWidth { get; set; }

        /// <summary>
        /// A CSS class added to an individual item.
        /// </summary>
        [JsonPropertyName("cssClass")]
        public string? CssClass { get; set; }

        #endregion
    }
}
