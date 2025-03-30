using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for animations that involve coordiates following a path.
    /// </summary>
    public class PathAnimationOptions: PlayableAnimationOptions, IDeepCloneable<PathAnimationOptions>
    {
        /// <summary>
        /// Specifies if a curved geodesic path should be used between points rather than a straight pixel path. Default: false
        /// </summary>
        [JsonPropertyName("geodesic")]
        public bool? Geodesic { get; set; }

        /// <summary>
        /// Specifies if metadata should be captured as properties of the shape. Potential metadata properties that may be captured: heading
        /// </summary>
        [JsonPropertyName("captureMetadata")]
        public bool? CaptureMetadata { get; set; }

        /// <inheritdoc/>
        public new PathAnimationOptions DeepClone()
        {
            return new PathAnimationOptions
            {
                AutoPlay = AutoPlay,
                Duration = Duration,
                Easing = Easing,
                Loop = Loop,
                Reverse = Reverse,
                SpeedMultiplier = SpeedMultiplier,
                Geodesic = Geodesic,
                CaptureMetadata = CaptureMetadata
            };
        }
    }
}
