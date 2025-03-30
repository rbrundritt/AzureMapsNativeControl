using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// The options for enabling/disabling user interaction with the map.
    /// </summary>
    public class UserInteractionOptions
    {
        //https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.userinteractionoptions?view=azure-maps-typescript-latest

        /// <summary>
        /// Whether the Shift + left click and drag will draw a zoom box. 
        /// </summary>
        [JsonPropertyName("boxZoomInteraction")]
        public bool? BoxZoomInteraction { get; set; }

        /// <summary>
        /// Whether double left click will zoom the map inwards.
        /// </summary>
        [JsonPropertyName("dblClickZoomInteraction")]
        public bool? DblClickZoomInteraction { get; set; }

        /// <summary>
        /// Whether left click and drag will pan the map.
        /// </summary>
        [JsonPropertyName("dragPanInteraction")]
        public bool? DragPanInteraction { get; set; }

        /// <summary>
        /// Whether right click and drag will rotate and pitch the map. 
        /// </summary>
        [JsonPropertyName("dragRotateInteraction")]
        public bool? DragRotateInteraction { get; set; }

        /// <summary>
        /// Whether the map is interactive or static. If false, all user interaction is disabled. If true, only selected user interactions will enabled. 
        /// </summary>
        [JsonPropertyName("interactive")]
        public bool? Interactive { get; set; }

        /// <summary>
        /// Whether the keyboard interactions are enabled.
        /// </summary>
        [JsonPropertyName("keyboardInteraction")]
        public bool? KeyboardInteraction { get; set; }

        /// <summary>
        /// Whether the map should zoom on scroll input.
        /// </summary>
        [JsonPropertyName("scrollZoomInteraction")]
        public bool? ScrollZoomInteraction { get; set; }

        /// <summary>
        /// Whether touch interactions are enabled for touch devices.
        /// </summary>
        [JsonPropertyName("touchInteraction")]
        public bool? TouchInteraction { get; set; }

        /// <summary>
        /// Whether touch rotation is enabled for touch devices. This option is not applied if touchInteraction is disabled.
        /// </summary>
        [JsonPropertyName("touchRotate")]
        public bool? TouchRotate { get; set; }

        /// <summary>
        /// Sets the zoom rate of the mouse wheel 
        /// </summary>
        [JsonPropertyName("wheelZoomRate")]
        public double? WheelZoomRate { get; set; }
    }
}
