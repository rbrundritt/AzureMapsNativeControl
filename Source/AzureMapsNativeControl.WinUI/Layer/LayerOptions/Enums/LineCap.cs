using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Specifies how the ends of the lines are rendered.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LineCap
    {
        /// <summary>
        /// A cap with a squared-off end which is drawn to the exact endpoint of the line.
        /// </summary>
        [EnumMember(Value = "butt")]
        Butt,

        /// <summary>
        /// A cap with a rounded end which is drawn beyond the endpoint of the line at a radius of one-half of the lines width and centered on the endpoint of the line.
        /// </summary>
        [EnumMember(Value = "round")]
        Round,

        /// <summary>
        /// A cap with a squared-off end which is drawn beyond the endpoint of the line at a distance of one-half of the line width.
        /// </summary>
        [EnumMember(Value = "square")]
        Square
    }
}
