using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for a playable animation.
    /// </summary>
    public class PlayableAnimationOptions: IDeepCloneable<PlayableAnimationOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies if the animation should start automatically or wait for the play function to be called. Default: false
        /// </summary>
        [JsonPropertyName("autoPlay")]
        public bool AutoPlay { get; set; }

        /// <summary>
        /// The duration of the animation in ms. Default: 1000 ms
        /// </summary>
        [JsonPropertyName("duration")]
        public int Duration { get; set; } = 1000;

        /// <summary>
        /// The easing of the animaiton. Default: 'linear'
        /// </summary>
        [JsonPropertyName("easing")]
        public Easing Easing { get; set; } = Easing.Linear;

        /// <summary>
        /// Specifies if the animation should loop infinitely. Default: false
        /// </summary>
        [JsonPropertyName("loop")]
        public bool Loop { get; set; }

        /// <summary>
        /// Specifies if the animation should play backwards. Default: false
        /// </summary>
        [JsonPropertyName("reverse")]
        public bool Reverse { get; set; }

        /// <summary>
        /// A multiplier of the duration to speed up or down the animation. Default: 1
        /// </summary>
        [JsonPropertyName("speedMultiplier")]
        public double SpeedMultiplier { get; set; } = 1;

        #endregion

        /// <inheritdoc/>
        public PlayableAnimationOptions DeepClone()
        {
            return new PlayableAnimationOptions
            {
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
