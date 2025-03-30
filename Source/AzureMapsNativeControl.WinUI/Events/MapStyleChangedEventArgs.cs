using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the map after loading a new style.
    /// </summary>
    public class MapStyleChangedEventArgs : MapMouseEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the map after loading a new style.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapStyleChangedEventArgs(Map map, string eventName) : base(map, eventName) { }

        /// <summary>
        /// Event object returned by the map after loading a new style.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapStyleChangedEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            Style = eventData.Style;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the style that was loaded.
        /// </summary>
        [JsonPropertyName("style")]
        public MapStyle? Style { get; set; }

        #endregion
    }
}
