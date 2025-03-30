using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The options for animating changes to the map control's camera.
    /// </summary>
    public class CameraAnimationOptions
    {
        /// <summary>
        /// The type of animation. Default: Jump
        /// </summary>
        [JsonPropertyName("type")]
        public CameraAnimationType Type { get; set; } = CameraAnimationType.Jump;

        /// <summary>
        /// The duration of the animation in milliseconds. Default: 1000
        /// </summary>
        [JsonPropertyName("duration")]
        public int Duration { get; set; } = 1000;
    }
}
