using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// A legend that displays a color gradient with optional labelled points.
    /// </summary>
    public class GradientLegend: BaseLegend
    {
        #region Constructor

        /// <summary>
        /// A legend that displays a color gradient with optional labelled points.
        /// </summary>
        public GradientLegend(): base(LegendType.Gradient)
        {
            Stops = new List<GradientLegendStop>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The color stops that form the gradient.
        /// </summary>
        [JsonPropertyName("stops")]
        public IList<GradientLegendStop> Stops { get; set; }

        /// <summary>
        /// The orientation of the legend. Default: `'horizontal'`
        /// </summary>
        [JsonPropertyName("orientation")]
        public MapOrientation Orientation { get; set; } = MapOrientation.Horizontal;

        /// <summary>
        /// The length of line ticks for each label. Default: `5`
        /// </summary>
        [JsonPropertyName("tickSize")]
        public int TickSize { get; set; } = 5;

        /// <summary>
        /// The length of the gradient bar in pixels. Default: `200`
        /// </summary>
        [JsonPropertyName("barLength")]
        public int BarLength { get; set; } = 200;

        /// <summary>
        /// How thick the gradient bar should be in pixels. Default: `20`
        /// </summary>
        [JsonPropertyName("barThickness")]
        public int BarThickness { get; set; } = 20;

        /// <summary>
        /// The font size used for labels. Default: `12`
        /// </summary>
        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// The font family used for labels. Default: `"'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif"`
        /// </summary>
        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; } = "'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif";

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

        #endregion
    }
}
