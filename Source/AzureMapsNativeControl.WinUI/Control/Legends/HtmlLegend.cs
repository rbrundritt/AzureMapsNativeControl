using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// A legend that displays a custom HTML elements.
    /// </summary>
    public class HtmlLegend : BaseLegend
    {
        #region Constructor

        /// <summary>
        /// A legend that displays a custom HTML elements.
        /// </summary>
        public HtmlLegend() : base(LegendType.HTML)
        {
        }

        /// <summary>
        /// A legend that displays a custom HTML elements.
        /// </summary>
        /// <param name="html">HTML content for the legend.</param>
        public HtmlLegend(string html) : base(LegendType.HTML)
        {
            Html = html;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// HTML content for the legend.
        /// </summary>
        [JsonPropertyName("html")]
        public string? Html { get; set; }

        #endregion
    }
}
