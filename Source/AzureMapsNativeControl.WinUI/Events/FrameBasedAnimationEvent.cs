using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event arguments for a frame based animation.
    /// </summary>
    public class FrameBasedAnimationEvent : MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event arguments for a frame based animation.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public FrameBasedAnimationEvent(Map map, string eventName) : base(map, eventName)
        {
        }

        /// <summary>
        /// Event arguments for a frame based animation.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal FrameBasedAnimationEvent(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            if (!string.IsNullOrEmpty(eventData.AnimationId) && map.Animations.ContainsKey(eventData.AnimationId))
            {
                var animation = map.Animations[eventData.AnimationId];
                if (animation != null && animation is FrameBasedAnimation)
                {
                    Animation = animation as FrameBasedAnimation;
                }
            }

            FrameIdx = eventData.FrameIdx;
            NumFrames = eventData.NumFrames;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The animation the event occurered on.
        /// </summary>
        public FrameBasedAnimation? Animation { get; set; }

        /// <summary>
        /// The index of the frame if using the frame based animation timer.
        /// </summary>
        [JsonPropertyName("frameIdx")]
        public int FrameIdx { get; set; }

        /// <summary>
        /// The number of frames in the animation. 
        /// </summary>
        [JsonPropertyName("numFrames")]
        public int NumFrames { get; set; }

        #endregion
    }
}
