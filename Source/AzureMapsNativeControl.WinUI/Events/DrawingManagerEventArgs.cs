using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Drawing;
using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the map when a drawing manager event occurs.
    /// </summary>
    public class DrawingManagerEventArgs: MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the map when a drawing manager event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public DrawingManagerEventArgs(Map map, string eventName) : base(map, eventName)
        {
        }

        /// <summary>
        /// Event object returned by the map when a drawing manager event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal DrawingManagerEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            if (eventData.Features != null && eventData.Features.Count > 0)
            {
                Feature = eventData.Features[0];
            }

            if(eventData.DrawingMode != null)
            {
                Mode = eventData.DrawingMode.Value;
            }
        }

        #endregion

        /// <summary>
        /// The current drawing mode of the drawing manager.
        /// </summary>
        [JsonPropertyName("mode")]
        public DrawingMode Mode { get; set; } = DrawingMode.Idle;

        /// <summary>
        /// The drawing manager that the event occurred on.
        /// </summary>
        [JsonPropertyName("feature")]
        public Feature? Feature { get; set; }
    }
}
