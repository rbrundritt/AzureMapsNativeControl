using System.Threading.Tasks;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// A class for frame based animations.
    /// 
    /// Supported events: onprogress, oncomplete, onframe
    /// </summary>
    public class FrameBasedAnimation: PlayableAnimation
    {
        #region Private Properties

        private PlayableAnimationOptions _options = new PlayableAnimationOptions();

        #endregion

        #region Constructor

        /// <summary>
        /// A class for frame based animations.
        /// </summary>
        /// <param name="map">The Map instance the animation is attached to.</param>
        internal FrameBasedAnimation(Map map) : base(map)
        {
        }

        /// <summary>
        /// A class for frame based animations.
        /// </summary>
        /// <param name="map">The Map instance the animation is attached to.</param>
        internal FrameBasedAnimation(Map map, string id) : base(map, id)
        {
        }

        #endregion

        public int NumberOfFrames { get; set; }

        #region Public Methods

        /// <summary>
        /// Gets the current frame index of the animation. Returns -1 if animation hasn't started, or if there is 0 frames.
        /// </summary>
        public async Task<int> GetCurrentFrameIdxAsync() {
            return await Map.JsInterlop.InvokeJsMethodAsync<int>(Map, "animationCommand", Id, "getCurrentFrameIdx");
        }

        /// <summary>
        /// Sets the frame index of the animation.
        /// </summary>
        /// <param name="frameIdx">The frame index to advance to.</param>
        public async void SetFrameIdxAsync(int frameIdx) {
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "setFrameIdx", frameIdx);
        }

        /// <summary>
        /// Sets the number of frames in the animation.
        /// </summary>
        /// <param name="numberOfFrames">The number of frames in the animation.</param>
        public async void SetNumberOfFramesAsync(int numberOfFrames) {
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "setNumberOfFrames", numberOfFrames);
        }

        #endregion
    }
}
