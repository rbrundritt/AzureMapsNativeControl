using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control to display a scale bar on the map.
    /// </summary>
    public sealed class ScaleControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control to display a scale bar on the map.
        /// </summary>
        public ScaleControl() : base("atlas.control.ScaleControl") { }

        #endregion

        #region Public Properties

        /// <summary>
        /// The maximum length of the scale control in pixels. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("maxWidth")]
        public int MaxWidth { get; set; } = 100;

        /// <summary>
        /// Unit of the distance ('imperial', 'metric' or 'nautical').
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("unit")]
        public ScaleControlUnits Unit { get; set; } = ScaleControlUnits.Metric;

        #endregion
    }
}

