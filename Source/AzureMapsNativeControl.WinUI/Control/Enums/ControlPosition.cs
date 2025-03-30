using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Positions where the control can be placed on the map.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ControlPosition
    {
        /// <summary>
        /// Places the control in the bottom left of the map. Literal value "bottom-left"
        /// </summary>
        [EnumMember(Value = "bottom-left")]
        BottomLeft,

        /// <summary>
        /// Places the control in the bottom right of the map. Literal value "bottom-right"
        /// </summary>
        [EnumMember(Value = "bottom-right")]
        BottomRight,

        /// <summary>
        /// The control will place itself in its default location. Literal value "non-fixed"
        /// </summary>
        [EnumMember(Value = "non-fixed")]
        NonFixed,

        /// <summary>
        /// Places the control in the top left of the map. Literal value "top-left"
        /// </summary>
        [EnumMember(Value = "top-left")]
        TopLeft,

        /// <summary>
        /// Places the control in the top right of the map. Literal value "top-right"
        /// </summary>
        [EnumMember(Value = "top-right")]
        TopRight
    }
}
