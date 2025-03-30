using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// Style options for point layers in the drawing manager.
    /// </summary>
    public class DrawingPointLayerOptions: IDeepCloneable<DrawingPointLayerOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies which part of the icon is placed closest to the icons anchor position on the map.
        /// </summary>
        [JsonPropertyName("anchor")]
        public PositionAnchor? Anchor { get; set; }

        /// <summary>
        /// The name of the image in the map's image sprite to use for drawing the icon.
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        /// <summary>
        /// The name of the image in the map's image sprite to use for drawing the icon when in the preview state.
        /// </summary>
        [JsonPropertyName("previewImage")]
        public string? PreviewImage { get; set; }

        /// <summary>
        /// Specifies an offset distance of the icon from its anchor in pixels. 
        /// Positive values indicate right and down, while negative values indicate left and up. 
        /// Each component is multiplied by the value of size to obtain the final offset in pixels. 
        /// When combined with rotation the offset will be as if the rotated direction was up. 
        /// </summary>
        [JsonPropertyName("offset")]
        public Pixel? Offset { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the icon will be drawn. 
        /// </summary>
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("pitchAlignment")]
        public PitchAlignment? PitchAlignment { get; set; }

        /// <summary>
        /// Scales the original size of the icon by the provided factor. Must be greater or equal to 0.
        /// </summary>
        [JsonPropertyName("size")]
        public double? Size { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default options.
        /// </summary>
        /// <returns></returns>
        public static DrawingPointLayerOptions Defaults()
        {
            return new DrawingPointLayerOptions()
            {
                Anchor = PositionAnchor.Bottom,
                Image = "marker-black",
                PreviewImage = "marker-blue",
                Offset = new Pixel(0, 0),
                Opacity = 1,
                PitchAlignment = AzureMapsNativeControl.PitchAlignment.Auto,
                Size = 0.5
            };
        }

        /// <inheritdoc/>
        public DrawingPointLayerOptions DeepClone()
        {
            return new DrawingPointLayerOptions()
            {
                Anchor = Anchor,
                Image = Image,
                PreviewImage = PreviewImage,
                Offset = Offset?.DeepClone(),
                Opacity = Opacity,
                PitchAlignment = PitchAlignment,
                Size = Size
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(DrawingPointLayerOptions? source, DrawingPointLayerOptions? target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.Anchor != null && source.Anchor != target.Anchor)
                {
                    target.Anchor = source.Anchor;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(source.Image) && source.Image != target.Image)
                {
                    target.Image = source.Image;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(source.PreviewImage) && source.PreviewImage != target.PreviewImage)
                {
                    target.PreviewImage = source.PreviewImage;
                    hasChanges = true;
                }

                if (source.Offset != null && source.Offset != target.Offset)
                {
                    target.Offset = source.Offset;
                    hasChanges = true;
                }

                if (source.Opacity >= 0 && source.Opacity <= 1 && source.Opacity != target.Opacity)
                {
                    target.Opacity = source.Opacity;
                    hasChanges = true;
                }

                if (source.PitchAlignment != null && source.PitchAlignment != target.PitchAlignment)
                {
                    target.PitchAlignment = source.PitchAlignment;
                    hasChanges = true;
                }

                if (source.Size >= 0 && source.Size != target.Size)
                {
                    target.Size = source.Size;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
