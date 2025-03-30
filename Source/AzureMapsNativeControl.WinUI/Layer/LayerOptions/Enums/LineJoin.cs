using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Specifies how the joints in the lines are rendered.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LineJoin
    {
        /// <summary>
        /// A join with a squared-off end which is drawn beyond the endpoint of the line at a distance of one-half of the lines width.
        /// </summary>
        [EnumMember(Value = "bevel")]
        Bevel,

        /// <summary>
        /// A join with a rounded end which is drawn beyond the endpoint of the line at a radius of one-half of the lines width and centered on the endpoint of the line.
        /// </summary>
        [EnumMember(Value = "round")]
        Round,

        /// <summary>
        /// A join with a sharp, angled corner which is drawn with the outer sides beyond the endpoint of the path until they meet.
        /// </summary>
        [EnumMember(Value = "miter")]
        Miter
    }
}
