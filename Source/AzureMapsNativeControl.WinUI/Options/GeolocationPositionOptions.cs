using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Web options for the Navigator Geolocation API. This is used by the Geolocation control class.
    /// </summary>
    public class GeolocationPositionOptions
    {
        /// <summary>
        /// Indicates the maximum age in milliseconds of a possible cached position that is acceptable to return. 
        /// If set to 0, it means that the device cannot use a cached position and must attempt to retrieve the real current position.
        /// If set to Inifity, it will use the most recently cached position, even if it is very old.
        /// Default: Inifity (no maximum age).
        /// </summary>
        [JsonPropertyName("maximumAge")]
        public double MaximumAge { get; set; } = double.PositiveInfinity;

        /// <summary>
        /// A positive long value representing the maximum length of time (in milliseconds) the device is allowed to take in order to return a position.
        /// Default: 10000 (10 seconds)      
        /// </summary>
        [JsonPropertyName("timeout")]
        public long Timeout { get; set; } = 10000;

        /// <summary>
        /// Indicates if the application would like to receive the best possible results. If true and if the device is able to provide a more accurate position, 
        /// it will do so. Note that this can result in slower response times or increased power consumption (with a GPS chip on a mobile device for example). 
        /// On the other hand, if false, the device can take the liberty to save resources by responding more quickly and/or using less power. Default: true.
        /// </summary>
        [JsonPropertyName("enableHighAccuracy")]
        public bool EnableHighAccuracy { get; set; } = true;
    }
}
