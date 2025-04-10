using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event arg object for the Geolocation control.
    /// </summary>
    public class GeolocationControlEventArgs: MapEventArgs
    {  
        #region Constructor

        /// <summary>
        /// Event object returned by the map when a geolocaiton control event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public GeolocationControlEventArgs(Map map, string eventName) : base(map, eventName)
        {
        }

        /// <summary>
        /// Event object returned by the map when a geolocaiton control event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal GeolocationControlEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            if (eventData.Features != null && eventData.Features.Count > 0)
            {
                Feature = eventData.Features[0];
            }

            CompassHeading = eventData.CompassHeading;
            Error = eventData.GeolocationError;

        }

        #endregion

        /// <summary>
        /// The position of the user. Set on geolocation success. Last known value will be included with the compass heading changed event.
        /// </summary>
        [JsonPropertyName("error")]
        public GeolocationPositionError? Error { get; set; }

        /// <summary>
        /// The position of the user. Set on geolocation success. Last known value will be included with the compass heading changed event.
        /// </summary>
        [JsonPropertyName("feature")]
        public Feature? Feature { get; set; }

        /// <summary>
        /// The compass heading. Set when the compass heading changes or when there is a last known compass heading when there is a geolocation success.
        /// </summary>
        [JsonPropertyName("compassHeading")]
        public double? CompassHeading { get; set; }
    }
}
