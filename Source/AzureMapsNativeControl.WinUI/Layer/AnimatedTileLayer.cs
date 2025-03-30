using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// A layer that can smoothly animate through an array of tile layers.
    /// </summary>
    public class AnimatedTileLayer: BaseLayer
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private MediaLayerOptions _options = MediaLayerOptions.Defaults();

        internal FrameBasedAnimation? _animation = null;

        [JsonInclude]
        [JsonPropertyName("sources")]
        internal IEnumerable<TileSource> _tileSources = new List<TileSource>();

        [JsonInclude]
        [JsonPropertyName("animationOptions")]
        private PlayableAnimationOptions _animationOptions = new PlayableAnimationOptions();

        #endregion

        #region Constructor 

        /// <summary>
        /// A layer for tiled images.
        /// </summary>
        /// <param name="tileSources">The tile sources for each frame of the animation.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public AnimatedTileLayer(IEnumerable<TileSource> tileSources, MediaLayerOptions? options = null, PlayableAnimationOptions? animationOptions = null, string? id = null) :
            base("atlas.layer.AnimatedTileLayer", null, id)
        {
            SetSources(tileSources);

            if (options != null)
            {
                MediaLayerOptions.Merge(options, this._options);
            }

            if (animationOptions != null)
            {
                _animationOptions = animationOptions;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the underlying frame based animation instance.
        /// </summary>
        /// <returns></returns>
        public async Task<FrameBasedAnimation?> GetPlayableAnimation() 
        {
            return _animation;
        }

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override MediaLayerOptions GetOptions()
        {
            return (_options as MediaLayerOptions).DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(MediaLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (MediaLayerOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setAnimatedTileLayerOptions", Id, null, _options);
            }
        }

        /// <summary>
        /// Sets the tile sources to animate through.
        /// </summary>
        /// <param name="tileSources"></param>
        public async void SetSources(IEnumerable<TileSource> tileSources)
        {
            if (tileSources == null)
            {
                throw new ArgumentNullException(nameof(tileSources));
            }

            foreach(var tileSource in tileSources)
            {
                if (tileSource == null)
                {
                    throw new ArgumentNullException(nameof(tileSource));
                }
                else if (tileSource.IsVectorTiles)
                {
                    throw new InvalidDataException("Vector tile source cannot be used with a TileLayer.");
                }

                tileSource.Validate();
            }

            _tileSources = tileSources;

            if (Map != null)
            {
                if(_animation != null)
                {
                    _animation.NumberOfFrames = _tileSources.Count();
                }
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setAnimatedTileLayerOptions", Id, Source, null);
            }
        }

        #endregion
    }
}
