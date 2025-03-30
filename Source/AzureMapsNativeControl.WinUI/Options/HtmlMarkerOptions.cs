using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Data;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Core;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Options for the HtmlMarker.
    /// </summary>
    public class HtmlMarkerOptions: IDeepCloneable<HtmlMarkerOptions>
    {
        //Popup option not added as it is generally not used and not aligned with how popups are implemented with layers.

        #region Public Properties

        /// <summary>
        /// Indicates the marker's location relative to its position on the map.
        /// </summary>
        [JsonPropertyName("anchor")]
        public PositionAnchor? Anchor { get; set; }

        /// <summary>
        /// A CSS color value that replaces any {color} placeholder property that has been included in a string htmlContent.
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// A boolean indicating whether the marker can be dragged to a new position with the mouse.
        /// </summary>
        [JsonPropertyName("draggable")]
        public bool? Draggable { get; set; }

        /// <summary>
        /// The HTML content of the marker.
        /// </summary>
        [JsonPropertyName("htmlContent")]
        public string? HtmlContent { get; set; }

        /// <summary>
        /// The offset in pixels of the marker from the point on the map specified by the position property.
        /// </summary>
        [JsonPropertyName("pixelOffset")]
        public Pixel? PixelOffset { get; set; }

        /// <summary>
        /// The position of the marker on the map.
        /// </summary>
        [JsonPropertyName("position")]
        [JsonConverter(typeof(PositionConverter))]
        public Position? Position { get; set; }

        /// <summary>
        /// A CSS color value that replaces any {secondaryColor} placeholder property that has been included in a string htmlContent.
        /// </summary>
        [JsonPropertyName("secondaryColor")]
        public string? SecondaryColor { get; set; }

        /// <summary>
        /// The text to display in the marker.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// A boolean indicating whether the marker is visible.
        /// </summary>
        [JsonPropertyName("visible")]
        public bool? Visible { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the defaults options for a HtmlMarker.
        /// </summary>
        /// <returns></returns>
        public static HtmlMarkerOptions Defaults()
        {
            return new HtmlMarkerOptions
            {
                Anchor = PositionAnchor.Bottom,
                Color = "#1A73AA",
                Draggable = false,
                PixelOffset = new Pixel(0, 0),
                Position = new Position(0, 0),
                SecondaryColor = "#FFFFFF",
                Text = null,
                Visible = true
            };
        }

        /// <summary>
        /// Creates a deep copy of the options.
        /// </summary>
        /// <returns></returns>
        public HtmlMarkerOptions DeepClone()
        {
            return new HtmlMarkerOptions
            {
                Anchor = Anchor,
                Color = Color,
                Draggable = Draggable,
                HtmlContent = HtmlContent,
                PixelOffset = PixelOffset,
                Position = Position,
                SecondaryColor = SecondaryColor,
                Text = Text,
                Visible = Visible
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(HtmlMarkerOptions source, HtmlMarkerOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.Anchor != null && source.Anchor != target.Anchor)
                {
                    target.Anchor = source.Anchor;
                    hasChanges = true;
                }

                if (source.Color != null && source.Color != target.Color)
                {
                    target.Color = source.Color;
                    hasChanges = true;
                }

                if (source.Draggable != null && source.Draggable != target.Draggable)
                {
                    target.Draggable = source.Draggable;
                    hasChanges = true;
                }

                if (source.HtmlContent != null && source.HtmlContent != target.HtmlContent)
                {
                    target.HtmlContent = source.HtmlContent;
                    hasChanges = true;
                }

                if (source.PixelOffset != null && source.PixelOffset != target.PixelOffset)
                {
                    target.PixelOffset = source.PixelOffset;
                    hasChanges = true;
                }

                if (source.Position != null && !source.Position.Equals(target.Position))
                {
                    target.Position = source.Position;
                    hasChanges = true;
                }

                if (source.SecondaryColor != null && source.SecondaryColor != target.SecondaryColor)
                {
                    target.SecondaryColor = source.SecondaryColor;
                    hasChanges = true;
                }

                if (source.Text != null && source.Text != target.Text)
                {
                    target.Text = source.Text;
                    hasChanges = true;
                }

                if (source.Visible != null && source.Visible != target.Visible)
                {
                    target.Visible = source.Visible;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
