using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control for changing the zoom of the map.
    /// </summary>
    public sealed class ZoomControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control for changing the zoom of the map.
        /// </summary>
        public ZoomControl() : base("atlas.control.ZoomControl") { }

        #endregion

        #region Public Properties

        /// <summary>
        /// The extent to which the map will zoom with each click of the control. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("zoomDelta")]
        public int? ZoomDelta { get; set; } = 1;

        #endregion
    }
}
