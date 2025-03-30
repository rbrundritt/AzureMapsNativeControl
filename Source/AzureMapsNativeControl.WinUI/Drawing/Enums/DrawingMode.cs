using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// An enumeration of the available drawing modes.
    /// https://learn.microsoft.com/en-us/javascript/api/azure-maps-drawing-tools/atlas.drawing.drawingmode
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum DrawingMode
    {
        /// <summary>
        /// Draw circles on the map. Literal value "draw-circle"
        /// </summary>
        [EnumMember(Value = "draw-circle")]
        DrawCircle,

        /// <summary>
        /// Draw lines on the map. Literal value "draw-line"
        /// </summary>
        [EnumMember(Value = "draw-line")]
        DrawLine,

        /// <summary>
        /// Draw individual points on the map. Literal value "draw-point"
        /// </summary>
        [EnumMember(Value = "draw-point")]
        DrawPoint,

        /// <summary>
        /// Draw polygons on the map. Literal value "draw-polygon"
        /// </summary>
        [EnumMember(Value = "draw-polygon")]
        DrawPolygon,

        /// <summary>
        /// Draw rectangles on the map. Literal value "draw-rectangle"
        /// </summary>
        [EnumMember(Value = "draw-rectangle")]
        DrawRectangle,

        /// <summary>
        /// When in this mode the user can add/remove/move points/coordinates of a shape, rotate shapes, drag shapes.
        /// </summary>
        [EnumMember(Value = "edit-geometry")]
        EditGeometry,

        /// <summary>
        /// When in this mode the user can erase (delete) shapes tracked by the DrawingManager.
        /// </summary>
        [EnumMember(Value = "erase-geometry")]
        EraseGeometry,

        /// <summary>
        /// Sets the drawing manager into an idle state. Completes any drawing/edit that are in progress. Literal value "idle"
        /// </summary>
        [EnumMember(Value = "idle")]
        Idle
    }
}
