using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Specifies the position of the marker image relative to the marker's location.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum PositionAnchor
    {
        /// <summary>
        /// The top left corner of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "top-left")]
        TopLeft,

        /// <summary>
        /// The top center of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "top")]
        Top,

        /// <summary>
        /// The top right corner of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "top-right")]
        TopRight,

        /// <summary>
        /// The center left of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "left")]
        Left,

        /// <summary>
        /// The center of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "center")]
        Center,

        /// <summary>
        /// The center right of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "right")]
        Right,

        /// <summary>
        /// The bottom left corner of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "bottom-left")]
        BottomLeft,

        /// <summary>
        /// The bottom center of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "bottom")]
        Bottom,

        /// <summary>
        /// The bottom right corner of the marker image is anchored to the marker's location.
        /// </summary>
        [EnumMember(Value = "bottom-right")]
        BottomRight
    }
}
