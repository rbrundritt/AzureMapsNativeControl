using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The style of the map element.​
    /// </summary>
    public class MapElementStyles
    {
        /// <summary>
        /// Specifies the visibility of the element.
        /// </summary>
        [JsonPropertyName("visible")]
        public bool? Visble { get; set; }
    }
}
