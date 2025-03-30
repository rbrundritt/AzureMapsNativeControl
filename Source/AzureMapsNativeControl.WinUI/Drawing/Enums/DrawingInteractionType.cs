using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// An enumeration of the available drawing interaction types. The drawing interaction type specifies how certain drawing modes behave.
    /// https://learn.microsoft.com/en-us/javascript/api/azure-maps-drawing-tools/atlas.drawing.drawinginteractiontype
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum DrawingInteractionType
    {
        /// <summary>
        /// Coordinates are added when the mouse or touch is clicked.
        /// </summary>
        [EnumMember(Value = "click")]
        Click,

        /// <summary>
        /// Coordinates are added when the mouse or touch is dragged on the map. 
        /// </summary>
        [EnumMember(Value = "freehand")]
        Freehand,

        /// <summary>
        /// Coordinates are added when the mouse or touch is clicked or dragged. 
        /// </summary>
        [EnumMember(Value = "hybrid")]
        Hybrid
    }
}
