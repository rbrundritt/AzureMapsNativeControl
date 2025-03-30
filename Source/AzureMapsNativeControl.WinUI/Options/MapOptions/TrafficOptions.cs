using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The options for setting traffic on the map.
    /// </summary>
    public class TrafficOptions
    {
        /// <summary>
        /// The type of traffic flow to display.
        /// </summary>
        [JsonPropertyName("style")]
        public TrafficFlowType? Style { get; set; }

        /// <summary>
        /// Whether to display incidents on the map. 
        /// </summary>
        [JsonPropertyName("incidents")]
        public bool? Incidents { get; set; }
    }
}
