using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for the flowing dashed line animation.
    /// </summary>
    public class FlowingDashLineOptions: PlayableAnimationOptions, IDeepCloneable<FlowingDashLineOptions>
    {
        /// <summary>
        /// The length of the dashed part of the line. Default: 4
        /// </summary>
        [JsonPropertyName("dashLength")]
        public int DashLength { get; set; } = 4;

        /// <summary>
        /// The length of the gap part of the line. Default: 4 
        /// </summary>
        [JsonPropertyName("gapLength")]
        public int GapLength { get; set; } = 4;

        /// <inheritdoc/>
        public new FlowingDashLineOptions DeepClone()
        {
            return new FlowingDashLineOptions
            {
                DashLength = DashLength,
                GapLength = GapLength,
                AutoPlay = AutoPlay,
                Duration = Duration,
                Easing = Easing,
                Loop = Loop,
                Reverse = Reverse,
                SpeedMultiplier = SpeedMultiplier
            };
        }
    }
}
