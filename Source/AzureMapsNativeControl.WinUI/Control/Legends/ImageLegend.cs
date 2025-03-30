using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// A legend that displays an image.
    /// </summary>
    public class ImageLegend : BaseLegend
    {
        #region Constructor

        /// <summary>
        /// A legend that displays an image.
        /// </summary>
        public ImageLegend() : base(LegendType.Image)
        {
        }

        /// <summary>
        /// A legend that displays an image.
        /// </summary>
        /// <param name="imageUrl">A URL, or inline SVG string for the legend content.</param>
        public ImageLegend(string imageUrl) : base(LegendType.Image)
        {
            Url = imageUrl;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A URL, or inline SVG string for the legend content.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Accessibility description of the legend image. Falls back to `subtitle` if not specified.
        /// </summary>
        [JsonPropertyName("altText")]
        public string? AltText { get; set; }

        /// <summary>
        /// Max height of the image. 
        /// </summary>
        [JsonPropertyName("maxHeight")]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Max width of the image.
        /// </summary>
        [JsonPropertyName("maxWidth")]
        public int? MaxWidth { get; set; }

        #endregion
    }
}
