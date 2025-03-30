using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for animating the map along a path.
    /// </summary>
    public class RoutePathAnimationOptions: MapPathAnimationOptions, IDeepCloneable<RoutePathAnimationOptions>
    {
        /// <summary>
        /// Interpolation calculations to perform on property values between points during the animation. Requires `captureMetadata` to be enabled. 
        /// </summary>
        [JsonPropertyName("valueInterpolations")]
        public IEnumerable<PointPairValueInterpolation>? ValueInterpolations { get; set; }

        /// <inheritdoc/>
        public new RoutePathAnimationOptions DeepClone()
        {
            return new RoutePathAnimationOptions
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
                RotationOffset = RotationOffset,
                ValueInterpolations = ValueInterpolations?.Select(x => x.DeepClone())
            };
        }
    }
}
