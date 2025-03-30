using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the maps when a touch event occurs.
    /// </summary>
    public class MapTouchEventArgs: MapMouseEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the maps when a touch event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapTouchEventArgs(Map map, string eventName) : base(map, eventName)
        {
            Pixels = [];
            Positions = [];
        }

        /// <summary>
        /// Event object returned by the maps when a touch event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapTouchEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            Pixels = eventData.Pixels == null ? [] : eventData.Pixels;
            Positions = eventData.Positions == null ? [] : eventData.Positions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The array of pixel coordinates of all touch points on the map.
        /// </summary>
        [JsonPropertyName("pixels")]
        public IList<Pixel> Pixels { get; set; }

        /// <summary>
        /// The geographical location of all touch points on the map.
        /// </summary>
        [JsonPropertyName("positions")]
        public IList<Position> Positions { get; set; }

        #endregion
    }
}
