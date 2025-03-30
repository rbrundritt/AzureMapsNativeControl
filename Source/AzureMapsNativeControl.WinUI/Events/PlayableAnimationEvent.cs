using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Playable animation event argument.
    /// </summary>
    public class PlayableAnimationEvent : MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Playable animation event argument.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public PlayableAnimationEvent(Map map, string eventName) : base(map, eventName)
        {
            Position = new Position(0, 0);
        }

        /// <summary>
        /// Playable animation event argument.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal PlayableAnimationEvent(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            if (!string.IsNullOrEmpty(eventData.AnimationId) && map.Animations.ContainsKey(eventData.AnimationId))
            {
                var animation = map.Animations[eventData.AnimationId];
                if (animation != null)
                {
                    Animation = animation;
                    Progress = eventData.Progress;
                    EasingProgress = eventData.EasingProgress;
                    Heading = eventData.Heading;
                    Position = eventData.Position == null ? new Position(0, 0) : eventData.Position;
                    Speed = eventData.Speed;
                    Timestamp = eventData.Timestamp;
                }
            }

            Position = eventData.Position == null ? new Position(0, 0) : eventData.Position;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The animation the event occurered on.
        /// </summary>
        public IPlayableAnimation? Animation { get; set; }

        /// <summary>
        /// Progress of the animation where 0 is the start and 1 is the end.
        /// </summary>
        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        /// <summary>
        /// The progress of the animation after being passed through an easing function.
        /// </summary>
        [JsonPropertyName("easingProgress")]
        public double EasingProgress { get; set; }

        /// <summary>
        /// The focal heading of an animation frame. Returned by path animations.
        /// </summary>
        [JsonPropertyName("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// The focal position of an animation frame. Returned by path animations. 
        /// </summary>
        [JsonPropertyName("positions")]
        public Position Position { get; set; }

        /// <summary>
        ///  Average speed between points in meters per second.
        /// </summary>
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        /// <summary>
        /// Estimated JSON timestamp in the animation based on the timestamp information provided for each point. 
        /// </summary>
        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }

        #endregion
    }
}
