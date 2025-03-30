using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Specifies the type of information to overlay on top of the overview map control.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum OverviewMapOverlay
    {
        /// <summary>
        /// Shows a polygon area of the parent map view port.
        /// </summary>
        [EnumMember(Value = "area")]
        Area,

        /// <summary>
        /// Shows a marker for the center of the parent map.
        /// </summary>
        [EnumMember(Value = "marker")]
        Marker,

        /// <summary>
        /// Does not display any overlay on top of the overview map.
        /// </summary>
        [EnumMember(Value = "none")]
        None
    }
}
