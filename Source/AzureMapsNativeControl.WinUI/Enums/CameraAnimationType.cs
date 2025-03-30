using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The type of animation.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CameraAnimationType
    {
        /// <summary>
        /// Animation is an immediate change.
        /// </summary>
        [EnumMember(Value = "jump")]
        Jump,

        /// <summary>
        /// Animation is a gradual change of the camera's settings.
        /// </summary>
        [EnumMember(Value = "ease")]
        Ease,

        /// <summary>
        /// Animation is a gradual change of the camera's settings following an arc resembling flight.
        /// </summary>
        [EnumMember(Value = "fly")]
        Fly
    }
}
