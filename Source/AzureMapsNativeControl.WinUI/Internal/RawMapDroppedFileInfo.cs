using System.IO;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// Represents the raw data for a dropped file.
    /// </summary>
    internal class RawMapDroppedFileInfo
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The type of the file.
        /// </summary>
        [JsonPropertyName("type")]
        public string? MimeType { get; set; }

        /// <summary>
        /// The last modified date of the file.
        /// </summary>
        [JsonPropertyName("data")]
        public string? Data { get; set; }

        internal MemoryStream? Stream { get; set; }
    }
}
