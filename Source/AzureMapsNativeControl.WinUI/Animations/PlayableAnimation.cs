using AzureMapsNativeControl.Core;
using System;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// This class is used to control a playable animation on the Map. It allows you to play, pause, stop, reset, and seek the animation.
    /// 
    /// Supported events: onprogress, oncomplete, onframe
    /// </summary>
    public class PlayableAnimation: MapEntity<PlayableAnimationOptions>, IPlayableAnimation
    {
        #region Private Properties

        private PlayableAnimationOptions _options = new PlayableAnimationOptions();
        private bool isDisposed = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the PlayableAnimation class. This is used to interact with an animation on the Map. 
        /// </summary>
        /// <param name="map">The Map instance the animation is attached to.</param>
        internal PlayableAnimation(Map map) : base("atlas.animations.PlayableAnimation", null)
        {
            Map = map;
            map.Animations.Add(Id, this);
        }
        
        /// <summary>
        /// Constructor for the PlayableAnimation class. This is used to interact with an animation on the Map. 
        /// </summary>
        /// <param name="map">The Map instance the animation is attached to.</param>
        internal PlayableAnimation(Map map, string id) : base("atlas.animations.PlayableAnimation", id, true)
        {
            Map = map;
            map.Animations.Add(Id, this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the animation options.
        /// </summary>
        /// <returns></returns>
        public override PlayableAnimationOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Disposes the animation. This will remove the animation from the Map and free up any resources used by it.
        /// </summary>
        public async void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "dispose");

                Map.Animations.Remove(Id);
            }
        }

        /// <summary>
        /// Gets the duration of the animation. Returns Infinity if the animations loops forever.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetDuration()
        {
            if (!isDisposed)
            {
                //Get the duration of the animation.
                return await Map.JsInterlop.InvokeJsMethodAsync<int>(Map, "animationCommand", Id, "getDuration");
            }

            return _options.Duration;
        }

        /// <summary>
        /// Checks to see if the animaiton is playing.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPlaying()
        {
            if (!isDisposed)
            {
                //Get the duration of the animation.
                return await Map.JsInterlop.InvokeJsMethodAsync<bool>(Map, "animationCommand", Id, "isPlaying");
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the animation is disposed. Some animations can only be ran once and are automatically disposed.
        /// </summary>
        /// <returns></returns>
        public bool IsDisposed()
        {
            return isDisposed;
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="reset">Specifies if the animation should reset before playing.</param>
        public async void Play(bool reset = false)
        {
            if (!isDisposed)
            {
                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "play", reset);
            }
        }

        /// <summary>
        /// Pauses the animation. 
        /// </summary>
        public async void Pause()
        {
            if (!isDisposed)
            {
                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "pause");
            }
        }

        /// <summary>
        /// Stops the animation and jumps back to the beginning of the animation.
        /// </summary>
        public async void Reset()
        {
            if (!isDisposed)
            {
                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "reset");
            }
        }

        /// <summary>
        /// Advances the animation to specific step. 
        /// </summary>
        /// <param name="progress">The progress of the animation to advance to. A value between 0 and 1.</param>
        public async void Seek(double progress)
        {
            if (!isDisposed)
            {
                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "seek", progress);
            }
        }

        /// <summary>
        /// Sets the options of the animation.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(PlayableAnimationOptions options)
        {
            if (!isDisposed)
            {
                //Merge the options.
                _options = options.DeepClone();

                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "setOptions", options);
            }
        }

        /// <summary>
        /// Stops the animation and jumps back to the end of the animation.
        /// </summary>
        public async void Stop()
        {
            if (!isDisposed)
            {
                //Play the animation.
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "animationCommand", Id, "stop");
            }
        }

        #endregion
    }
}
