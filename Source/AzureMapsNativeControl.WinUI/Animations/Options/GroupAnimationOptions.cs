using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Options for group animations.
    /// </summary>
    public class GroupAnimationOptions : IDeepCloneable<GroupAnimationOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies if the animation should start automatically or wait for the play function to be called. Default: false
        /// </summary>
        [JsonPropertyName("autoPlay")]
        public bool AutoPlay { get; set; }

        /// <summary>
        /// If the `playType` is set to `interval`, this option specifies the time interval to start each animation in milliseconds. Default: `100`
        /// </summary>
        [JsonPropertyName("interval")]
        public int Interval { get; set; } = 100;

        /// <summary>
        /// How to play the animations. Default: 'together'
        /// </summary>
        [JsonPropertyName("playType")]
        public GroupAnimationPlayType PlayType { get; set; } = GroupAnimationPlayType.Together;

        #endregion

        /// <inheritdoc/>
        public GroupAnimationOptions DeepClone()
        {
            return new GroupAnimationOptions
            {
                AutoPlay = AutoPlay,
                Interval = Interval,
                PlayType = PlayType
            };
        }
    }
}
