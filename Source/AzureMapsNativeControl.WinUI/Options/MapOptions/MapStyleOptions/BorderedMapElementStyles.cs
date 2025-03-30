using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The style of the bordered map element.
    /// </summary>
    public class BorderedMapElementStyles
    {
        /// <summary>
        /// Specifies the visibility of the border.
        /// </summary>
        [JsonPropertyName("borderVisible")]
        public bool? BorderVisible { get; set; }
    }
}
