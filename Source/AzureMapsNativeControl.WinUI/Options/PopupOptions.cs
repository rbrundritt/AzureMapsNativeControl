using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Options for the popup.
    /// </summary>
    public class PopupOptions: IDeepCloneable<PopupOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies if the close button should be displayed in the popup or not.
        /// </summary>
        [JsonPropertyName("closeButton")]
        public bool? ShowCloseButton { get; set; }

        /// <summary>
        /// The HTML content of the popup.
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        /// <summary>
        /// A boolean indicating whether the popup can be dragged to a new position with the mouse.
        /// </summary>
        [JsonPropertyName("draggable")]
        public bool? Draggable { get; set; }

        /// <summary>
        /// A CSS color value that fills the background of the popup.
        /// </summary>
        [JsonPropertyName("fillColor")]
        public string? FillColor { get; set; }

        /// <summary>
        /// The offset in pixels of the popup from the point on the map specified by the position property.
        /// </summary>
        [JsonPropertyName("pixelOffset")]
        public Pixel? PixelOffset { get; set; }

        /// <summary>
        /// The position of the popup on the map.
        /// </summary>
        [JsonPropertyName("position")]
        public Position? Position { get; set; }

        /// <summary>
        /// Specifies if the pointer should be displayed in the popup or not. 
        /// </summary>
        [JsonPropertyName("showPointer")]
        public bool? ShowPointer { get; set; }

        /// <summary>
        /// The template for the popup.
        /// </summary>
        [JsonPropertyName("popupTemplate")]
        public PopupTemplate? PopupTemplate { get; set; }

        #endregion 

        #region Public Methods

        /// <summary>
        /// Gets the default options for a Popup.
        /// </summary>
        /// <returns></returns>
        public static PopupOptions Defaults()
        {
            return new PopupOptions
            {
                ShowCloseButton = true,
                FillColor = "#FFFFFF",
                Draggable = false,
                PixelOffset = new Pixel(0, 0),
                Position = new Position(0, 0),
                ShowPointer = true
            };
        }

        /// <summary>
        /// Creates a deep copy of the options.
        /// </summary>
        /// <returns></returns>
        public PopupOptions DeepClone()
        {
            return new PopupOptions
            {
                ShowCloseButton = ShowCloseButton,
                Content = Content,
                Draggable = Draggable,
                FillColor = FillColor,
                PixelOffset = PixelOffset,
                Position = Position?.DeepClone(),
                ShowPointer = ShowPointer,
                PopupTemplate = PopupTemplate?.DeepClone()
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(PopupOptions source, PopupOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.ShowCloseButton != null && source.ShowCloseButton != target.ShowCloseButton)
                {
                    target.ShowCloseButton = source.ShowCloseButton;
                    hasChanges = true;
                }

                if (source.Content != null && source.Content != target.Content)
                {
                    target.Content = source.Content;
                    target.PopupTemplate = null;
                    hasChanges = true;
                }

                if (source.Draggable != null && source.Draggable != target.Draggable)
                {
                    target.Draggable = source.Draggable;
                    hasChanges = true;
                }

                if (source.FillColor != null && source.FillColor != target.FillColor)
                {
                    target.FillColor = source.FillColor;
                    hasChanges = true;
                }

                if (source.PixelOffset != null && source.PixelOffset != target.PixelOffset)
                {
                    target.PixelOffset = source.PixelOffset;
                    hasChanges = true;
                }

                if (source.Position != null && source.Position != target.Position)
                {
                    target.Position = source.Position;
                    hasChanges = true;
                }

                if (source.ShowPointer != null && source.ShowPointer != target.ShowPointer)
                {
                    target.ShowPointer = source.ShowPointer;
                    hasChanges = true;
                }

                if (source.PopupTemplate != null && source.PopupTemplate != target.PopupTemplate)
                {
                    target.PopupTemplate = source.PopupTemplate;
                    target.Content = null;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
