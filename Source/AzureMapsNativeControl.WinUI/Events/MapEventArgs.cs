using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    public interface IMapEventArgs
    {
    }

    /// <summary>
    /// Event object returned by the maps when a basic event occurs.
    /// </summary>
    public class MapEventArgs: IMapEventArgs
    {
        //https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.mapevent?view=azure-maps-typescript-latest
        //Excludes: originalEvent

        #region Constructor

        /// <summary>
        /// Event object returned by the maps when a basic event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapEventArgs(Map map, string eventName) 
        {
            Map = map;
            Type = eventName;
        }

        /// <summary>
        /// Event object returned by the maps when a basic event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapEventArgs(Map map, RawMapMsg eventData): this(map, eventData.Type)
        {
            if(eventData.Camera != null)
            {
                Camera = eventData.Camera;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Map instance in which the event occurred on.
        /// </summary>
        [JsonIgnore]
        public Map Map { get; set; }

        /// <summary>
        /// The event type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The map camera options when the event occurred.
        /// </summary>
        [JsonIgnore]
        public CameraOptions? Camera { get; set; }

        #endregion
    }
}
