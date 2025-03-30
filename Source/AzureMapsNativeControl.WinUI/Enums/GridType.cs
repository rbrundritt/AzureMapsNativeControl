using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Units of measurement for acceleration.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum GridType
    {
        /// <summary>
        /// Renders data within a hexagons grid.
        /// </summary>
        [EnumMember(Value = "hexagon")]
        Hexagon,

        /// <summary>
        /// Renders data within a square grid as circles.
        /// </summary>
        [EnumMember(Value = "circle")]
        Circle,

        /// <summary>
        /// Renders data within a hexagon grid as circles.
        /// </summary>
        [EnumMember(Value = "hexCircle")]
        HexCircle,

        /// <summary>
        /// enders data within a rotate hexagon grid.
        /// </summary>
        [EnumMember(Value = "pointyHexagon")]
        PointyHexagon,

        /// <summary>
        /// Renders data within a square grid.
        /// </summary>
        [EnumMember(Value = "square")]
        Square,

        /// <summary>
        /// Renders data within a triangular grid.
        /// </summary>
        [EnumMember(Value = "triangle")]
        Triangle
    }
}
