using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for a symbol layer.
    /// </summary>
    public class IconOptions: IDeepCloneable<IconOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies if the symbol icon can overlay other symbols on the map. If true the icon will be visible even if it collides with other previously drawn symbols. 
        /// Tip: Set this to true if animating an symbol to ensure smooth rendering. 
        /// </summary>
        [JsonPropertyName("allowOverlap")]
        public bool? AllowOverlap { get; set; }

        /// <summary>
        /// Specifies which part of the icon is placed closest to the icons anchor position on the map.
        /// </summary>
        [JsonPropertyName("anchor")]
        public PositionAnchor? Anchor { get; set; }

        /// <summary>
        /// Specifies if other symbols can overlap this symbol. If true, other symbols can be visible even if they collide with the icon.
        /// </summary>
        [JsonPropertyName("ignorePlacement")]
        public bool? IgnorePlacement { get; set; }

        /// <summary>
        /// The name of the image in the map's image sprite to use for drawing the icon.
        /// </summary>
        [JsonPropertyName("image")]
        public Expression<string>? Image { get; set; }

        /// <summary>
        /// Specifies an offset distance of the icon from its anchor in pixels. 
        /// Positive values indicate right and down, while negative values indicate left and up. 
        /// Each component is multiplied by the value of size to obtain the final offset in pixels. 
        /// When combined with rotation the offset will be as if the rotated direction was up. 
        /// </summary>
        [JsonPropertyName("offset")]
        public Expression<Pixel>? Offset { get; set; }

        /// <summary>
        /// A number between 0 and 1 that indicates the opacity at which the icon will be drawn. 
        /// </summary>
        [JsonPropertyName("opacity")]
        public Expression<double>? Opacity { get; set; }

        /// <summary>
        /// Specifies if a symbols icon can be hidden but its text displayed if it is overlapped with another symbol. 
        /// If true, text will display without their corresponding icons when the icon collides with other symbols and the text does not. 
        /// </summary>
        [JsonPropertyName("optional")]
        public bool? Optional { get; set; }

        /// <summary>
        /// Size of the additional area around the icon bounding box used for detecting symbol collisions.
        /// </summary>
        [JsonPropertyName("padding")]
        public Expression<int>? Padding { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("pitchAlignment")]
        public PitchAlignment? PitchAlignment { get; set; }

        /// <summary>
        /// The amount to rotate the icon clockwise in degrees.
        /// </summary>
        [JsonPropertyName("rotation")]
        public Expression<double>? Rotation { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("rotationAlignment")]
        public PitchAlignment? RotationAlignment { get; set; }

        /// <summary>
        /// Scales the original size of the icon by the provided factor. Must be greater or equal to 0.
        /// </summary>
        [JsonPropertyName("size")]
        public Expression<double>? Size { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default options.
        /// </summary>
        /// <returns></returns>
        public static IconOptions Defaults()
        {
            return new IconOptions()
            {
                AllowOverlap = false,
                Anchor = PositionAnchor.Bottom,
                IgnorePlacement = false,
                Image = Expression<string>.Literal("marker-blue"),
                Offset = Expression<Pixel?>.Literal(new Pixel(0, 0)),
                Opacity = Expression<double>.Literal(1),
                Optional = false,
                Padding = Expression<int>.Literal(2),
                PitchAlignment = AzureMapsNativeControl.PitchAlignment.Auto,
                Rotation = Expression<double>.Literal(0),
                RotationAlignment = AzureMapsNativeControl.PitchAlignment.Auto,
                Size = Expression<double>.Literal(1)
            };
        }

        /// <inheritdoc/>
        public IconOptions DeepClone()
        {
            return new IconOptions()
            {
                AllowOverlap = AllowOverlap,
                Anchor = Anchor,
                IgnorePlacement = IgnorePlacement,
                Image = Image?.DeepClone(),
                Offset = Offset?.DeepClone(),
                Opacity = Opacity?.DeepClone(),
                Optional = Optional,
                Padding = Padding?.DeepClone(),
                PitchAlignment = PitchAlignment,
                Rotation = Rotation?.DeepClone(),
                RotationAlignment = RotationAlignment,
                Size = Size?.DeepClone()
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(IconOptions? source, IconOptions? target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.AllowOverlap != null && source.AllowOverlap != target.AllowOverlap)
                {
                    target.AllowOverlap = source.AllowOverlap;
                    hasChanges = true;
                }

                if (source.Anchor != null && source.Anchor != target.Anchor)
                {
                    target.Anchor = source.Anchor;
                    hasChanges = true;
                }

                if (source.IgnorePlacement != null && source.IgnorePlacement != target.IgnorePlacement)
                {
                    target.IgnorePlacement = source.IgnorePlacement;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.Image) && source.Image != target.Image)
                {
                    if (source.Image != null && source.Image.IsLiteralEquals("undefined"))
                    {
                        source.Image = Expression<string>.Literal("marker-blue");
                    }

                    target.Image = source.Image;
                    hasChanges = true;
                }

                if (source.Offset != null && source.Offset != target.Offset)
                {
                    target.Offset = source.Offset;
                    hasChanges = true;
                }

                if (Expression.IsValueInRange(source.Opacity, 0, 1) && source.Opacity != target.Opacity)
                {
                    target.Opacity = source.Opacity;
                    hasChanges = true;
                }

                if (source.Optional != null && source.Optional != target.Optional)
                {
                    target.Optional = source.Optional;
                    hasChanges = true;
                }

                if (!Expression.IsNull(source.Padding) && source.Padding != target.Padding)
                {
                    target.Padding = source.Padding;
                    hasChanges = true;
                }

                if (source.PitchAlignment != null && source.PitchAlignment != target.PitchAlignment)
                {
                    target.PitchAlignment = source.PitchAlignment;
                    hasChanges = true;
                }

                if (!Expression.IsNull(source.Rotation) && source.Rotation != target.Rotation)
                {
                    target.Rotation = source.Rotation;
                    hasChanges = true;
                }

                if (source.RotationAlignment != null && source.RotationAlignment != target.RotationAlignment)
                {
                    target.RotationAlignment = source.RotationAlignment;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.Size) && source.Size != target.Size)
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

