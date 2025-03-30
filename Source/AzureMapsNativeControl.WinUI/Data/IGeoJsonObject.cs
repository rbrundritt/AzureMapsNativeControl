using AzureMapsNativeControl;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// Interface for GeoJson objects.
    /// </summary>
    public interface IGeoJsonObject
    {
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        GeoJsonType Type { get; }
    }
}
