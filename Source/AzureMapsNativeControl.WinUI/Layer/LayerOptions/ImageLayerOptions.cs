using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for an Image layer.
    /// </summary>
    public class ImageLayerOptions : MediaLayerOptions, IDeepCloneable<ImageLayerOptions>
    {
        #region Constructor

        /// <summary>
        /// Options for an Image layer.
        /// </summary>
        public ImageLayerOptions() : base()
        {
        }

        /// <summary>
        /// Options for an Image layer.
        /// </summary>
        /// <param name="filePathOrUrl">
        /// URL to an image to overlay.Images hosted on other domains must have CORs enabled.
        /// Can be a data URI.
        /// </param>
        /// <param name="coordinates">An array of positions for the corners of the image listed in clockwise order: [top left, top right, bottom right, bottom left].</param>
        public ImageLayerOptions(string filePathOrUrl, IList<Position>? coordinates = null) : base()
        {
            //Check to see if a proxy should be used.
            //if (useProxy == true && !filePathOrUrl.StartsWith("data:") && !filePathOrUrl.StartsWith("/proxy?"))
            //{
            //    filePathOrUrl = Utils.GetUrlProxy(filePathOrUrl);

            //    //Need to decode placeholders for the tile requests.
            //    filePathOrUrl = filePathOrUrl.Replace("%7B", "{").Replace("%7D", "}");
            //}
            Coordinates = coordinates;
            Url = filePathOrUrl;
        }

        /// <summary>
        /// Options for an Image layer.
        /// </summary>
        /// <param name="stream">Image stream</param>
        /// <param name="coordinates">An array of positions for the corners of the image listed in clockwise order: [top left, top right, bottom right, bottom left].</param>
        /// <param name="contentType">The mime type of the stream content. Can be the mimeType (e.g. "image/png") or common file extension (e.g. "png") value</param>
        public ImageLayerOptions(Stream stream, IList<Position>? coordinates = null, string contentType = "image/png") : base()
        {
            Coordinates = coordinates;
            Url = Internal.Utils.GetDataUriFromStream(stream, contentType);
        }

        #endregion

        #region Public Properties

        /// <inheritdoc/>
        public static new ImageLayerOptions Defaults()
        {
            return new ImageLayerOptions()
            {
                Url = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==",

                //Base layer options
                MaxZoom = 24,
                MinZoom = 0,
                Visible = true,

                //Media options
                Contrast = 0,
                FadeDuration = 100,
                HueRotation = 0,
                MaxBrightness = 1,
                MinBrightness = 0,
                Opacity = 1,
                Saturation = 0
            };
        }

        /// <summary>
        /// URL to an image to overlay.Images hosted on other domains must have CORs enabled.
        /// Can be a data URI.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; internal set; }

        /// <summary>
        /// An array of positions for the corners of the image listed in clockwise order: [top left, top right, bottom right, bottom left].
        /// </summary>
        [JsonPropertyName("coordinates")]
        [JsonConverter(typeof(PositionEnumerableConverter))]
        public IList<Position>? Coordinates { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public new ImageLayerOptions DeepClone()
        {
            return new ImageLayerOptions(Url ?? "")
            {
                Coordinates = Coordinates,
                Url = Url,
                Contrast = Contrast,
                FadeDuration = FadeDuration,
                HueRotation = HueRotation,
                MaxBrightness = MaxBrightness,
                MinBrightness = MinBrightness,
                Opacity = Opacity,
                Saturation = Saturation,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Visible = Visible
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(ImageLayerOptions source, ImageLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = MediaLayerOptions.Merge(source, target);

                if (!string.IsNullOrWhiteSpace(source.Url) && source.Url != target.Url)
                {
                    target.Url = source.Url;
                    hasChanges = true;
                }

                if (source.Coordinates != null && source.Coordinates != target.Coordinates)
                {
                    if(source.Coordinates.Count != 4)
                    {
                        throw new ArgumentException("Coordinates must have 4 positions.");
                    }

                    target.Coordinates = source.Coordinates;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}