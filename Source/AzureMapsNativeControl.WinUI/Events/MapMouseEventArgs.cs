using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the maps when a mouse event occurs.
    /// </summary>
    public class MapMouseEventArgs : MapKeyboardEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the maps when a mouse event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapMouseEventArgs(Map map, string eventName) : base(map, eventName)
        { 
            Shapes = Array.Empty<Feature>();

            Pixel = new Pixel(0, 0);
            Position = new Position(0, 0);
        }

        /// <summary>
        /// Event object returned by the maps when a mouse event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapMouseEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            LayerId = eventData.LayerId;

            Pixel = eventData.Pixel == null ? new Pixel(0,0) : eventData.Pixel;
            Position = eventData.Position == null ? new Position(0, 0) : eventData.Position;
            Shapes = eventData.GetFeatures(map);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The id of the layer the event is attached to.
        /// </summary>
        [JsonPropertyName("layerId")]
        public string? LayerId { get; set; }

        /// <summary>
        /// The pixel coordinates of the mouse pointer, relative to the map.
        /// </summary>
        [JsonPropertyName("pixel")]
        public Pixel Pixel { get; set; }

        /// <summary>
        /// The geographic location on the map of the mouse pointer.
        /// </summary>
        [JsonPropertyName("position")]
        public Position Position { get; set; }

        /// <summary>
        /// Raw GeoJSON features in the event.
        /// Either features in a vector tile or a cluster object.
        /// </summary>
        [JsonPropertyName("shapes")]
        public IList<Feature> Shapes { get; set; }

        #endregion
    }
}
