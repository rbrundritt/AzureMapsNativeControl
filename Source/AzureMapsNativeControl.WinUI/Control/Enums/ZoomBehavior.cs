using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Specifies how all legend cards should be treated when the map zoom level falls outside of the items min and max zoom range. 
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ZoomBehavior
    {
        /// <summary>
        /// Hides the legend card.
        /// </summary>
        [EnumMember(Value = "hide")]
        Hide,

        /// <summary>
        /// Still visible, but disabled.
        /// </summary>
        [EnumMember(Value = "disable")]
        Disable
    }
}
