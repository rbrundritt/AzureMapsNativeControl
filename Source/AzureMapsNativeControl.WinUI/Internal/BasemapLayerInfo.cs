using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// Information about non-user added layers in the map. 
    /// Information based on MapLibre Style Specification: https://maplibre.org/maplibre-style-spec/layers/
    /// </summary>
    internal class BasemapLayerInfo: IDeepCloneable<BasemapLayerInfo>
    {
        #region Properties

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Possible values: background, fill, line, symbol, raster, circle, fill-extrusion, heatmap, hillshade
        /// </summary>
        [JsonPropertyName("type")]
        public string LayerType { get; set; } = string.Empty;

        [JsonPropertyName("minzoom")]
        public int? MinZoom { get; set; }

        [JsonPropertyName("maxzoom")]
        public int? MaxZoom { get; set; }

        /// <summary>
        /// The Azure Maps source Id. 
        /// </summary>
        [JsonPropertyName("sourceId")]
        public string? SourceId { get; set; }

        [JsonPropertyName("sourceLayer")]
        public string? SourceLayer { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public BasemapLayerInfo DeepClone()
        {
            return new BasemapLayerInfo()
            {
                Id = Id,
                LayerType = LayerType,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                SourceId = SourceId,
                SourceLayer = SourceLayer,
                Visible = Visible
            };
        }

        #endregion
    }
}
