using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Defines the play type of a group animation. This determines how the animations in the group are played.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum GroupAnimationPlayType
    {
        /// <summary>
        /// Play animations together at the same time.
        /// </summary>
        [EnumMember(Value = "together")]
        Together,

        /// <summary>
        /// Play animations one after the other.
        /// </summary>
        [EnumMember(Value = "sequential")]
        Sequential,

        /// <summary>
        /// Start each animation at a set interval.
        /// </summary>
        [EnumMember(Value = "interval")]
        Interval
    }
}
