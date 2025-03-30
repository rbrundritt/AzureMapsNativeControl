using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Animation easing types.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Easing
    {
        /// <summary>
        /// A linear easing function.
        /// </summary>
        [EnumMember(Value = "linear")]
        Linear,

        /// <summary>
        /// Slight acceleration from zero to full speed.  
        /// </summary>
        [EnumMember(Value = "easeInSine")]
        EaseInSine,

        /// <summary>
        /// Slight deceleration at the end. 
        /// </summary>
        [EnumMember(Value = "easeOutSine")]
        EaseOutSine,

        /// <summary>
        /// Slight acceleration at beginning and slight deceleration at end. 
        /// </summary>
        [EnumMember(Value = "easeInOutSine")]
        EaseInOutSine,

        /// <summary>
        /// Accelerating from zero velocity.
        /// </summary>
        [EnumMember(Value = "easeInQuad")]
        EaseInQuad,

        /// <summary>
        /// Acceleration until halfway, then deceleration. 
        /// </summary>
        [EnumMember(Value = "easeOutQuad")]
        EaseOutQuad,

        /// <summary>
        /// Accelerating from zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeInCubic")]
        EaseInCubic,

        /// <summary>
        /// Decelerating to zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeOutCubic")]
        EaseOutCubic,

        /// <summary>
        /// Acceleration until halfway, then deceleration. 
        /// </summary>
        [EnumMember(Value = "easeInOutCubic")]
        EaseInOutCubic,

        /// <summary>
        /// Accelerating from zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeInQuart")]
        EaseInQuart,

        /// <summary>
        /// Decelerating to zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeOutQuart")]
        EaseOutQuart,

        /// <summary>
        /// Acceleration until halfway, then deceleration. 
        /// </summary>
        [EnumMember(Value = "easeInOutQuart")]
        EaseInOutQuart,

        /// <summary>
        /// Accelerating from zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeInQuint")]
        EaseInQuint,

        /// <summary>
        /// Decelerating to zero velocity. 
        /// </summary>
        [EnumMember(Value = "easeOutQuint")]
        EaseOutQuint,

        /// <summary>
        /// Acceleration until halfway, then deceleration. 
        /// </summary>
        [EnumMember(Value = "easeInOutQuint")]
        EaseInOutQuint,

        /// <summary>
        /// Accelerate exponentially until finish. 
        /// </summary>
        [EnumMember(Value = "easeInExpo")]
        EaseInExpo,

        /// <summary>
        /// Initial exponential acceleration slowing to stop. 
        /// </summary>
        [EnumMember(Value = "easeOutExpo")]
        EaseOutExpo,

        /// <summary>
        /// Exponential acceleration and deceleration. 
        /// </summary>
        [EnumMember(Value = "easeInOutExpo")]
        EaseInOutExpo,

        /// <summary>
        /// Increasing velocity until stop. 
        /// </summary>
        [EnumMember(Value = "easeInCirc")]
        EaseInCirc,

        /// <summary>
        /// Start fast, decreasing velocity until stop. 
        /// </summary>
        [EnumMember(Value = "easeOutCirc")]
        EaseOutCirc,

        /// <summary>
        /// Fast increase in velocity, fast decrease in velocity. 
        /// </summary>
        [EnumMember(Value = "easeInOutCirc")]
        EaseInOutCirc,

        /// <summary>
        /// Slow movement backwards then fast snap to finish. 
        /// </summary>
        [EnumMember(Value = "easeInBack")]
        EaseInBack,

        /// <summary>
        /// Fast snap to backwards point then slow resolve to finish. 
        /// </summary>
        [EnumMember(Value = "easeOutBack")]
        EaseOutBack,

        /// <summary>
        /// Slow movement backwards, fast snap to past finish, slow resolve to finish. 
        /// </summary>
        [EnumMember(Value = "easeInOutBack")]
        EaseInOutBack,

        /// <summary>
        /// Bounces slowly then quickly to finish. 
        /// </summary>
        [EnumMember(Value = "easeInElastic")]
        EaseInElastic,

        /// <summary>
        /// Fast acceleration, bounces to zero. 
        /// </summary>
        [EnumMember(Value = "easeOutElastic")]
        EaseOutElastic,

        /// <summary>
        /// Slow start and end, two bounces sandwich a fast motion.  
        /// </summary>
        [EnumMember(Value = "easeInOutElastic")]
        EaseInOutElastic,

        /// <summary>
        /// Bounce to completion. 
        /// </summary>
        [EnumMember(Value = "easeOutBounce")]
        EaseOutBounce,

        /// <summary>
        /// Bounce increasing in velocity until completion. 
        /// </summary>
        [EnumMember(Value = "easeInBounce")]
        EaseInBounce,

        /// <summary>
        /// Bounce in and bounce out. 
        /// </summary>
        [EnumMember(Value = "easeInOutBounce")]
        EaseInOutBounce
    }
}
