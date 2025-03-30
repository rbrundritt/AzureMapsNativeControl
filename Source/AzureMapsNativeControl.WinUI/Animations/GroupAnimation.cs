using AzureMapsNativeControl.Core;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Group animation handler.
    /// 
    /// Supported events: oncomplete
    /// </summary>
    public class GroupAnimation: MapEntity<GroupAnimationOptions>, IPlayableAnimation
    {
        #region Private Properties

        private GroupAnimationOptions _options = new GroupAnimationOptions();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the GroupAnimation class. This is used to create a group animation.
        /// </summary>
        /// <param name="map">The map instance the animation is attached to.</param>
        internal GroupAnimation(Map map): base("atlas.animations.GroupAnimation", null)
        {
            Map = map;
            map.Animations.Add(Id, this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disposes the animation. This will remove the animation from the map and free up any resources used by it.
        /// </summary>
        public async void Dispose()
        {
            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "dispose");

            Map.Animations.Remove(Id);
        }

        /// <summary>
        /// Gets the duration of the animation. Returns Infinity if the animations loops forever.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetDuration()
        {
            //Get the duration of the animation.
            return await Map.JsInterlop.InvokeJsMethodAsync<int>(Map, "animationCommand", Id, "getDuration");
        }

        /// <summary>
        /// Gets the animation options.
        /// </summary>
        /// <returns></returns>
        public override GroupAnimationOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Checks to see if the animaiton is playing.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPlaying()
        {
            //Get the duration of the animation.
            return await Map.JsInterlop.InvokeJsMethodAsync<bool>(Map, "animationCommand", Id, "isPlaying");
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="reset">Specifies if the animation should reset before playing.</param>
        public async void Play(bool reset = false)
        {
            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "play", reset);
        }

        /// <summary>
        /// Stops the animation and jumps back to the beginning of the animation.
        /// </summary>
        public async void Reset()
        {
            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "reset");
        }

        /// <summary>
        /// Advances the animation to specific step. 
        /// </summary>
        /// <param name="progress">The progress of the animation to advance to. A value between 0 and 1.</param>
        public async void Seek(double progress)
        {
            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "seek", progress);
        }

        /// <summary>
        /// Sets the options of the animation.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(GroupAnimationOptions options)
        {
            //Merge the options.
            _options = options.DeepClone();

            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "setOptions", options);
        }

        /// <summary>
        /// Stops the animation and jumps back to the end of the animation.
        /// </summary>
        public async void Stop()
        {
            //Play the animation.
            await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "stop");
        }

        #endregion
    }
}
