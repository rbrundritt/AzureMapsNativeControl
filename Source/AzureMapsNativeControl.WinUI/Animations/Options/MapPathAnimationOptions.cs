using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for animating the map along a path.
    /// </summary>
    public class MapPathAnimationOptions: PathAnimationOptions, IDeepCloneable<MapPathAnimationOptions>
    {
        /// <summary>
        /// A fixed zoom level to snap the map to on each animation frame. By default the maps current zoom level is used.
        /// </summary>
        [JsonPropertyName("zoom")]
        public int? Zoom { get; set; }

        /// <summary>
        /// A pitch value to set on the map. By default this is not set.
        /// </summary>
        [JsonPropertyName("pitch")]
        public double? Pitch { get; set; }

        /// <summary>
        /// Specifies if the map should rotate such that the bearing of the map faces the direction the map is moving. Default: true
        /// </summary>
        [JsonPropertyName("rotate")]
        public bool? Rotate { get; set; }

        /// <summary>
        /// When rotate is set to true, the animation will follow the animation. An offset of 180 will cause the camera to lead the animation and look back. Default: 0
        /// </summary>
        [JsonPropertyName("rotationOffset")]
        public double RotationOffset { get; set; } = 0;

        /// <inheritdoc/>
        public new MapPathAnimationOptions DeepClone()
        {
            return new MapPathAnimationOptions
            {
                AutoPlay = AutoPlay,
                Duration = Duration,
                Easing = Easing,
                Loop = Loop,
                Reverse = Reverse,
                SpeedMultiplier = SpeedMultiplier,
                Zoom = Zoom,
                Pitch = Pitch,
                Rotate = Rotate,
                RotationOffset = RotationOffset
            };
        }
    }
}
