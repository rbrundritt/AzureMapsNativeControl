using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Represents the error object returned by the geolocation control.
    /// https://developer.mozilla.org/en-US/docs/Web/API/GeolocationPositionError
    /// </summary>
    public class GeolocationPositionError
    {
        /// <summary>
        /// Code indicating the error type. One of the following values:
        /// 1 - PERMISSION_DENIED: The user denied the request for Geolocation information.
        /// 2 - POSITION_UNAVAILABLE: The device was unable to provide a position.
        /// 3 - TIMEOUT: The request to get the user's location timed out.
        /// </summary>
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        /// <summary>
        /// A string containing a human-readable error message. The message may be empty if no error message is available.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
