using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the maps when an error event occurs.
    /// </summary>
    public class MapErrorEventArgs: MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the maps when an error event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapErrorEventArgs(Map map, string eventName) : base(map, eventName) { }

        /// <summary>
        /// Event object returned by the maps when an error event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapErrorEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            Error = eventData.Error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Error object that triggered the event.
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        #endregion
    }
}
