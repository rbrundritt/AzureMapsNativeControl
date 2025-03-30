using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// A legend that displays a list of items that are categorized.
    /// </summary>
    public class CategoryLegend : BaseLegend
    {
        #region Constructor

        /// <summary>
        /// A legend that displays a list of items that are categorized.
        /// </summary>
        /// <param name="items">Initial set of legend items.</param>
        public CategoryLegend(IList<CategoryLegendItem>? items = null) : base(LegendType.Category)
        {
            Items = items != null ? items : new List<CategoryLegendItem>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The category items.
        /// </summary>
        [JsonPropertyName("items")]
        public IList<CategoryLegendItem>? Items { get; set; }

        /// <summary>
        /// How all items are laid out. Overrides the CSS `flex-direction` style. Default: 'column'
        /// </summary>
        [JsonPropertyName("layout")]
        public CssFlexDirection? Layout { get; set; } = CssFlexDirection.Column;

        /// <summary>
        /// How the color swatch and label of each item are laid out. Overrides the CSS `flex-direction` style. Default: 'row' 
        /// </summary>
        [JsonPropertyName("itemLayout")]
        public CssFlexDirection? ItemLayout { get; set; } = CssFlexDirection.Row;

        /// <summary>
        /// The fill color of SVG items in all category items.
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// The shape of the color swatches of all items. 
        /// Built-in types: 'circle' | 'triangle' | 'square' | 'line'
        /// Or can be an image url and SVG string. 
        /// Default: 'circle'
        /// </summary>
        [JsonPropertyName("shape")]
        public string Shape { get; set; } = "circle";

        /// <summary>
        /// The size of the all shapes in pixels. Used to scale the width of the shape. Default: 20
        /// </summary>
        [JsonPropertyName("shapeSize")]
        public int ShapeSize { get; set; } = 20;

        /// <summary>
        /// Specifies if all items should be fit into the largest container created by an item. Default: false
        /// </summary>
        [JsonPropertyName("fitItems")]
        public bool FitItems { get; set; } = false;

        /// <summary>
        /// The thickness of the stroke on SVG shapes in pixels. Default: `1` 
        /// </summary>
        [JsonPropertyName("strokeWidth")]
        public int StrokeWidth { get; set; } = 1;

        /// <summary>
        /// Specifies if the text label should be centered overtop the shapes. Default: `false`
        /// </summary>
        [JsonPropertyName("labelsOverlapShapes")]
        public bool? LabelsOverlapShapes { get; set; } = false;

        /// <summary>
        /// The number format options to use when converting a number label to a string.
        /// </summary>
        [JsonPropertyName("numberFormat")]
        public JSNumberFormatOptions? NumberFormat { get; set; }

        /// <summary>
        /// The number format locales to use when converting a number label to a string.
        /// </summary>
        [JsonPropertyName("numberFormatLocales")]
        public IList<string>? NumberFormatLocales { get; set; }

        /// <summary>
        /// Specifies if space around the shapes should be collapsed so that items are close together. Default: `false`
        /// </summary>
        [JsonPropertyName("collapse")]
        public bool Collapse { get; set; } = false;

        #endregion
    }
}
