using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options used by tile and image layers.
    /// </summary>
    [JsonDerivedType(typeof(ImageLayerOptions))]
    public class MediaLayerOptions : LayerOptions, IDeepCloneable<MediaLayerOptions>
    {
        #region Public Properties

        /// <summary>
        ///	A number between -1 and 1 that increases or decreases the contrast of the overlay.
        /// </summary>
        [JsonPropertyName("contrast")]
        public double? Contrast { get; set; }

        /// <summary>
        /// The duration in milliseconds of a fade transition when a new tile is added. Must be greater or equal to 0.
        /// </summary>
        [JsonPropertyName("fadeDuration")]
        public int? FadeDuration { get; set; }

        /// <summary>
        ///	Rotates hues around the color wheel. A number in degrees.
        /// </summary>
        [JsonPropertyName("hueRotation")]
        public double? HueRotation { get; set; }

        /// <summary>
        ///	A number between 0 and 1 that increases or decreases the maximum brightness of the overlay.
        /// </summary>
        [JsonPropertyName("maxBrightness")]
        public double? MaxBrightness { get; set; }

        /// <summary>
        ///	A number between 0 and 1 that increases or decreases the minimum brightness of the overlay.
        /// </summary>
        [JsonPropertyName("minBrightness")]
        public double? MinBrightness { get; set; }

        /// <summary>
        ///	A number between 0 and 1 that indicates the opacity at which the overlay will be drawn.
        /// </summary>
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }

        /// <summary>
        ///	A number between -1 and 1 that increases or decreases the saturation of the overlay. 
        /// </summary>
        [JsonPropertyName("saturation")]
        public double? Saturation { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public static new MediaLayerOptions Defaults()
        {
            return new MediaLayerOptions()
            {
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

        /// <inheritdoc/>
        public MediaLayerOptions DeepClone()
        {
            return new MediaLayerOptions()
            {
                Contrast = Contrast,
                FadeDuration = FadeDuration,
                HueRotation = HueRotation,
                MaxBrightness = MaxBrightness,
                MinBrightness = MinBrightness,
                Opacity = Opacity,
                Saturation = Saturation,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Visible = Visible,
                Filter = Filter?.DeepClone()
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(MediaLayerOptions source, MediaLayerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = LayerOptions.Merge(source, target);

                if (source.Contrast != null && source.Contrast >= -1 && source.Contrast <= 1 && source.Contrast != target.Contrast)
                {
                    target.Contrast = source.Contrast;
                    hasChanges = true;
                }

                if (source.FadeDuration != null && source.FadeDuration >= 0 && source.FadeDuration != target.FadeDuration)
                {
                    target.FadeDuration = source.FadeDuration;
                    hasChanges = true;
                }

                if (source.HueRotation != null && source.HueRotation != target.HueRotation)
                {
                    target.HueRotation = source.HueRotation;
                    hasChanges = true;
                }

                if (source.MaxBrightness != null && source.MaxBrightness >= 0 && source.MaxBrightness <= 1 && source.MaxBrightness != target.MaxBrightness)
                {
                    target.MaxBrightness = source.MaxBrightness;
                    hasChanges = true;
                }

                if (source.MinBrightness != null && source.MinBrightness >= 0 && source.MinBrightness <= 1 && source.MinBrightness != target.MinBrightness)
                {
                    target.MinBrightness = source.MinBrightness;
                    hasChanges = true;
                }

                if (source.Opacity != null && source.Opacity >= 0 && source.Opacity <= 1 && source.Opacity != target.Opacity)
                {
                    target.Opacity = source.Opacity;
                    hasChanges = true;
                }

                if (source.Saturation != null && source.Saturation >= -1 && source.Saturation <= 1 && source.Saturation != target.Saturation)
                {
                    target.Saturation = source.Saturation;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}