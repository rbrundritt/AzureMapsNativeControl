using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Orientation of a map control
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MapOrientation
    { 
        /// <summary>
        /// Vertical orientation.
        /// </summary>
        [EnumMember(Value = "vertical")]
        Vertical,

        /// <summary>
        /// Horizontal orientation.
        /// </summary>
        [EnumMember(Value = "horizontal")]
        Horizontal,
    }
}
