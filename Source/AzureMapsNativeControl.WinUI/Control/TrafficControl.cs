using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control to display the traffic information of the map.
    /// </summary>
    public sealed class TrafficControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control to display the traffic information of the map.
        /// </summary>
        public TrafficControl() : base("atlas.control.TrafficControl") { }

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of traffic flow to display.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("flow")]
        public TrafficFlowType Flow { get; set; } = TrafficFlowType.Relative;

        /// <summary>
        /// Whether to display incidents on the map. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("incidents")]
        public bool Incidents { get; set; } = true;

        /// <summary>
        /// Whether to display incidents on the map. This value does not update, use IsActiveAsync to get the current state.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// An alternative to the Style property. Uses a CSS3 color value to set the color of the control.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("styleColor")]
        public string? StyleColor { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current control state (is traffic information displayed?) 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsActiveAsync()
        {
            if (_map != null)
            {
                return await _map.JsInterlop.InvokeJsMethodAsync<bool>(_map, "callGenericItemFunction", Id, Internal.Constants.ControlCache, "isActive");
            }

            return false;
        }

        #endregion
    }
}

