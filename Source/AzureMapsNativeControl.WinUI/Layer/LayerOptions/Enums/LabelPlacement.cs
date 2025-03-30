using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Specifies the label placement relative to its geometry.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LabelPlacement
    {
        /// <summary>
        /// The label is placed at the point where the geometry is located.
        /// </summary>
        [EnumMember(Value = "point")]
        Point,

        /// <summary>
        /// The label is placed along the line of the geometry. Can only be used on LineString and Polygon geometries.
        /// </summary>
        [EnumMember(Value = "line")]
        Line,

        /// <summary>
        /// The label is placed at the center of the line of the geometry. Can only be used on `LineString` and `Polygon` geometries.
        /// </summary>
        [EnumMember(Value = "line-center")]
        LineCenter,
    }
}
