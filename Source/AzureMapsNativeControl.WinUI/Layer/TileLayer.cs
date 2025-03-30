using AzureMapsNativeControl.Source;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// A layer for tiled images.
    /// </summary>
    public class TileLayer : BaseLayer
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private MediaLayerOptions _options = MediaLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// A layer for tiled images.
        /// </summary>
        /// <param name="tileSource">The source of the tile.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public TileLayer(TileSource tileSource, MediaLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.TileLayer", null, id)
        {
            SetSource(tileSource);

            if (options != null)
            {
                MediaLayerOptions.Merge(options, this._options);
            }
        }

        #endregion

        #region Public Methods

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
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        /// <summary>
        /// Sets the source of the tile layer.
        /// </summary>
        /// <param name="tileSource"></param>
        public async void SetSource(TileSource tileSource)
        {
            if(tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            } 
            else if (tileSource.IsVectorTiles)
            {
                throw new InvalidDataException("Vector tile source cannot be used with a TileLayer.");
            }

            tileSource.Validate();

            Source = tileSource;

            if (Map != null)
            {
                //Check to see if the tile source is in the source manager. If not, add it.
                if (Map.Sources.GetById(tileSource.Id) == null)
                {
                    Map.Sources.Add(tileSource);
                }

                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, Source);
            }
        }

        #endregion
    }
}