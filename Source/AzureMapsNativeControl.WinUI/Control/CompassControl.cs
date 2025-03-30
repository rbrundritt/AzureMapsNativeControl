using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control for changing the rotation of the map.
    /// </summary>
    public sealed class CompassControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control for changing the rotation of the map.
        /// </summary>
        public CompassControl() : base("atlas.control.CompassControl") { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inverts the direction of map rotation controls.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("inverted")]
        public bool Inverted { get; set; } = false;

        /// <summary>
        /// The angle that the map will rotate with each click of the control. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("rotationDegreesDelta")]
        public double RotationDegreesDelta { get; set; } = 15;

        #endregion
    }
}
