using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Options for text in a symbol layer.
    /// </summary>
    public class TextOptions: IDeepCloneable<TextOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies if the text will be visible if it collides with other symbols. 
        /// If true, the text will be visible even if it collides with other previously drawn symbols. 
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
        /// Specifies an offset distance of the text from its anchor in pixels. 
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
        public Expression<double>? Opacity { get; set; }

        /// <summary>
        /// Specifies if a symbols icon can be hidden but its text displayed if it is overlapped with another symbol. 
        /// If true, text will display without their corresponding icons when the icon collides with other symbols and the text does not. 
        /// </summary>
        [JsonPropertyName("optional")]
        public bool? Optional { get; set; }

        /// <summary>
        /// Size of the additional area around the text bounding box used for detecting symbol collisions.
        /// </summary>
        [JsonPropertyName("padding")]
        public int? Padding { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("pitchAlignment")]
        public PitchAlignment? PitchAlignment { get; set; }

        /// <summary>
        /// Radial offset of text, in the direction of the symbol's anchor. 
        /// Useful in combination with variableAnchor, which defaults to using the two-dimensional offset if present.  
        /// </summary>
        [JsonPropertyName("radialOffset")]
        public Expression<double>? RadialOffset { get; set; }

        /// <summary>
        /// The amount to rotate the text  clockwise in degrees.
        /// </summary>
        [JsonPropertyName("rotation")]
        public Expression<double>? Rotation { get; set; }

        /// <summary>
        /// Specifies the frame of reference for translate.
        /// </summary>
        [JsonPropertyName("rotationAlignment")]
        public PitchAlignment? RotationAlignment { get; set; }

        /// <summary>
        /// The size of the font in pixels. Must be a number greater or equal to 0.
        /// </summary>
        [JsonPropertyName("size")]
        public Expression<int>? Size { get; set; }

        /// <summary>
        /// The color of the text. 
        /// TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
        /// </summary>
        [JsonPropertyName("color")]
        public Expression<string>? Color { get; set; }

        /// <summary>
        /// The font stack to use for displaying text. 
        /// </summary>
        [JsonPropertyName("font")]
        public IList<string>? Font { get; set; }

        /// <summary>
        /// The halo's fadeout distance towards the outside in pixels. Must be a number greater or equal to 0. 
        /// </summary>
        [JsonPropertyName("haloBlur")]
        public Expression<double>? HaloBlur { get; set; }

        /// <summary>
        /// The color of the text's halo, which helps it stand out from backgrounds. 
        /// </summary>
        [JsonPropertyName("haloColor")]
        public Expression<string>? HaloColor { get; set; }

        /// <summary>
        /// The distance of the halo to the font outline in pixels. 
        /// Must be a number greater or equal to 0. 
        /// The maximum text halo width is 1/4 of the font size. 
        /// </summary>
        [JsonPropertyName("haloWidth")]
        public Expression<double>? HaloWidth { get; set; }

        /// <summary>
        /// Specifies the name of a property on the features to use for a text label.
        /// </summary>
        [JsonPropertyName("textField")]
        public Expression<string>? TextField { get; set; }

        /// <summary>
        /// List of potential anchor locations, to increase the chance of placing high-priority labels on the map. 
        /// The renderer will attempt to place the label at each location, in order, before moving onto the next label. 
        /// Use justify: "auto" to choose text justification based on anchor position. 
        /// To apply an offset use the radialOffset or two-dimensional offset options.
        /// </summary>
        [JsonPropertyName("variableAnchor")]
        public PitchAlignment? VariableAnchor { get; set; }

        /// <summary>
        /// Text justification options.
        /// </summary>
        [JsonPropertyName("justify")]
        public TextJustify? Justify { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default options.
        /// </summary>
        /// <returns></returns>
        public static TextOptions Defaults()
        {
            return new TextOptions()
            {
                AllowOverlap = false,
                Anchor = PositionAnchor.Center,
                Color = Expression<string>.Literal("#000000"),
                Font = new string[] { "StandardFont-Regular" },
                HaloBlur = Expression<double>.Literal(0),
                HaloColor = Expression<string>.Literal("transparent"),
                HaloWidth = Expression<double>.Literal(0),
                IgnorePlacement = false,
                Justify = TextJustify.Center,
                Offset = new Pixel(0, 0),
                Opacity = Expression<double>.Literal(1),
                Optional = false,
                Padding = 2,
                PitchAlignment = AzureMapsNativeControl.PitchAlignment.Auto,
                RadialOffset = Expression<double>.Literal(0),
                Rotation = Expression<double>.Literal(0),
                RotationAlignment = AzureMapsNativeControl.PitchAlignment.Auto,
                Size = Expression<int>.Literal(16)
            };
        }

        /// <summary>
        /// Creates a deep clone of the options.
        /// </summary>
        /// <returns></returns>
        public TextOptions DeepClone()
        {
            return new TextOptions()
            {
                AllowOverlap = AllowOverlap,
                Anchor = Anchor,
                IgnorePlacement = IgnorePlacement,
                Offset = Offset?.DeepClone(),
                Opacity = Opacity?.DeepClone(),
                Optional = Optional,
                Padding = Padding,
                PitchAlignment = PitchAlignment,
                Rotation = Rotation?.DeepClone(),
                RotationAlignment = RotationAlignment,
                Size = Size?.DeepClone(),
                Color = Color?.DeepClone(),
                RadialOffset = RadialOffset?.DeepClone(),
                Font = Font?.ToArray(),
                HaloBlur = HaloBlur?.DeepClone(),
                HaloColor = HaloColor?.DeepClone(),
                HaloWidth = HaloWidth?.DeepClone(),
                TextField = TextField?.DeepClone(),
                VariableAnchor = VariableAnchor,
                Justify = Justify
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(TextOptions? source, TextOptions? target)
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

                if (source.Padding != null && source.Padding != target.Padding)
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

                if (!Expression.IsNullOrWhiteSpace(source.Color) && source.Color != target.Color)
                {
                    if (source.Color != null && source.Color.IsLiteralEquals("undefined"))
                    {
                        source.Color = Expression<string>.Literal("#000000");
                    }

                    target.Color = source.Color;
                    hasChanges = true;
                }

                if (source.Font != null && source.Font != target.Font)
                {
                    target.Font = source.Font;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.HaloBlur) && source.HaloBlur != target.HaloBlur)
                {
                    target.HaloBlur = source.HaloBlur;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.HaloColor) && source.HaloColor != target.HaloColor)
                {
                    if (source.HaloColor != null && source.HaloColor.IsLiteralEquals("undefined"))
                    {
                        source.HaloColor = Expression<string>.Literal("transparent");
                    }

                    target.HaloColor = source.HaloColor;
                    hasChanges = true;
                }

                if (Expression.IsPositive(source.HaloWidth) && source.HaloWidth != target.HaloWidth)
                {
                    target.HaloWidth = source.HaloWidth;
                    hasChanges = true;
                }

                if (!Expression.IsNullOrWhiteSpace(source.TextField) && source.TextField != target.TextField)
                {
                    target.TextField = source.TextField;
                    hasChanges = true;
                }

                if (source.VariableAnchor != null && source.VariableAnchor != target.VariableAnchor)
                {
                    target.VariableAnchor = source.VariableAnchor;
                    hasChanges = true;
                }

                if (source.Justify != null && source.Justify != target.Justify)
                {
                    target.Justify = source.Justify;
                    hasChanges = true;
                }

                if (!Expression.IsNull(source.RadialOffset) && source.RadialOffset != target.RadialOffset)
                {
                    target.RadialOffset = source.RadialOffset;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}

