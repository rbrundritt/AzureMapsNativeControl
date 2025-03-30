using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Units of the scale control.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ScaleControlUnits
    {
        /// <summary>
        /// Metric units.
        /// </summary>
        [EnumMember(Value = "metric")]
        Metric,

        /// <summary>
        /// Imperial units.
        /// </summary>
        [EnumMember(Value = "imperial")]
        Imperial,

        /// <summary>
        /// Nautical units.
        /// </summary>
        [EnumMember(Value = "nautical")]
        Nautical
    }
}
