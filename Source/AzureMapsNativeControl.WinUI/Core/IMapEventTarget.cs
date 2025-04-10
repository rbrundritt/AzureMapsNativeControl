using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A map or map entity (layer, marker, popup, DrawingManager, etc) that can be the target of map events.
    /// </summary>
    public interface IMapEventTarget
    {
        /// <summary>
        /// A unique ID for the entity.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        string Id { get; }

        /// <summary>
        /// The Azure Maps namespace of the entity.
        /// </summary>
        string JsNamespace { get; }
    }
}
