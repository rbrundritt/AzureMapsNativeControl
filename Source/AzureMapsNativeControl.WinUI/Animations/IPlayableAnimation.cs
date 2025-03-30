using AzureMapsNativeControl.Core;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// This interface is used to control a playable animation on the map. It allows you to play, pause, stop, reset, and seek the animation.
    /// </summary>
    public interface IPlayableAnimation: IMapEventTarget
    {
        /// <summary>
        /// Disposes the animation.
        /// </summary>
        public void Dispose();

        /// <summary>
        /// Gets the duration of the animation. Returns Infinity if the animations loops forever.
        /// </summary>
        public Task<int> GetDuration();

        /// <summary>
        /// Checks to see if the animaiton is playing. 
        /// </summary>
        public Task<bool> IsPlaying();

        /// <summary>
        /// Plays the animation.
        /// </summary>
        public void Play(bool reset = false);

        /// <summary>
        /// Reset the animation.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop();
    }
}
