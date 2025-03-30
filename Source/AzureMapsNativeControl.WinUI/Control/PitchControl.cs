using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control for changing the pitch of the map.
    /// </summary>
    public sealed class PitchControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control for changing the pitch of the map.
        /// </summary>
        public PitchControl() : base("atlas.control.PitchControl") { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Inverts the direction of map pitch controls.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("inverted")]
        public bool Inverted { get; set; } = false;

        /// <summary>
        /// The angle that the map will tilt with each click of the control. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("pitchDegreesDelta")]
        public double PitchDegreesDelta { get; set; } = 10;

        #endregion
    }
}
