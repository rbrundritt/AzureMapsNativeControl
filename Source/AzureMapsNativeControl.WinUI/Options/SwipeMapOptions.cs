using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data.JsonConverters;
using System.Text.Json.Serialization;

#if WINUI
using Microsoft.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Options for the SwipeMap control.
    /// </summary>
#if MAUI
    public class SwipeMapOptions : BindableObject
#else
    public class SwipeMapOptions
#endif
    {
        /// <summary>
        /// Specifies if the slider can be moved using mouse, touch or keyboard. Default: true
        /// </summary>
        [JsonPropertyName("interactive")]
        public bool? Interactive { get; set; }

        /// <summary>
        /// The orientation of the swipe map control. Can be vertical or horizontal. Default: vertical
        /// </summary>
        [JsonPropertyName("orientation")]
        public MapOrientation? Orientation { get; set; }

        /// <summary>
        /// The position of the slider in pixels relative to the left or top edge of the viewport, 
        /// depending on orientation. Defaults to half the width or height depending on orientation.
        /// </summary>
        [JsonPropertyName("sliderPosition")]
        public int? SliderPosition { get; set; }

        /// <summary>
        /// The style of the control. Can be; light, dark, auto, or any CSS3 color. Overridden if 
        /// device is in high contrast mode. Default light.
        /// </summary>
        [JsonPropertyName("style")]
        public ControlStyle? Style { get; set; }

        /// <summary>
        /// An alternative to the Style property. Uses a CSS3 color value to set the color of the control.
        /// </summary>
        [JsonPropertyName("styleColor")]
        public string? StyleColor { get; set; }

        /// <summary>
        /// Initial load settings for the primary map.
        /// </summary>
        [JsonIgnore]
        public MapLoadOptions? PrimaryMapSettings { get; set; }

        /// <summary>
        /// Initial load settings for the secondary map.
        /// </summary>
        [JsonIgnore]
        public MapLoadOptions? SecondaryMapSettings { get; set; }
    }
}
