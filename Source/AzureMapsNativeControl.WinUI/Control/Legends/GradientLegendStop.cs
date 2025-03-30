using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// Color stop used for gradients and steps.
    /// </summary>
    public class GradientLegendStop
    {
        #region Constructor

        /// <summary>
        /// Color stop used for gradients and steps.
        /// </summary>
        public GradientLegendStop(){}

        /// <summary>
        /// Color stop used for gradients and steps.
        /// </summary>
        /// <param name="offset">The offset to add the color to the gradient. 0.0 is the offset at one end of the gradient, 1.0 is the offset at the other end.
        /// </summary></param>
        /// <param name="color">The color to apply at the stop. </param>
        /// <param name="label">Optional label.</param>
        public GradientLegendStop(double offset, string color, string? label = null)
        {
            Offset = offset;
            Color = color;
            Label = label;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The offset to add the color to the gradient. 0.0 is the offset at one end of the gradient, 1.0 is the offset at the other end.
        /// </summary>
        [JsonPropertyName("offset")]
        public double Offset { get; set; }

        /// <summary>
        /// The color to apply at the stop. 
        /// </summary>
        [JsonPropertyName("color")]
        public string Color { get; set; } = "#000000";

        /// <summary>
        /// A label to display at this stop.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        #endregion
    }
}
