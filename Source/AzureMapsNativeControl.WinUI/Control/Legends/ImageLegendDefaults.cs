using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// Default options for image legends within Dynamic legend.
    /// </summary>
    public class ImageLegendDefaults
    {
        #region Public Properties

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
