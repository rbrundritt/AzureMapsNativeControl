using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Internal
{
    public class RawBasemapSourceInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string SourceType { get; set; }

        [JsonPropertyName("attribution")]
        public string? Attribution { get; set; }

        [JsonPropertyName("bounds")]
        public double[] Bounds { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("minzoom")]
        public int? MinZoom { get; set; }

        [JsonPropertyName("maxzoom")]
        public int? MaxZoom { get; set; }
    }
}
