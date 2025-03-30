using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The type of traffic flow to display.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum TrafficFlowType
    {
        /// <summary>
        /// No traffic flow data displayed.
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// The speed of the road relative to free-flow
        /// </summary>
        [EnumMember(Value = "relative")]
        Relative
    }
}
