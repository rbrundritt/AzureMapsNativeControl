using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Interface for all controls.
    /// </summary>
    public interface IBaseControl
    {
        Map? _map { get; internal set; }

        /// <summary>
        /// A unique ID for the entity.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        string Id { get; }
    }
}
