using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Override for the default styles for the map elements.
    /// https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.styleoverrides?view=azure-maps-typescript-latest
    /// </summary>
    public class StyleOverrides
    {
        /// <summary>
        /// First administrative level within the country/region level, such as a state or a province.
        /// </summary>
        [JsonPropertyName("adminDistrict")]
        public BorderedMapElementStyles? AdminDistrict { get; set; }

        /// <summary>
        /// Second administrative level within the country/region level, such as a county.
        /// </summary>
        [JsonPropertyName("adminDistrict2")]
        public BorderedMapElementStyles? AdminDistrict2 { get; set; }

        /// <summary>
        /// Building footprints along with their address numbers. 
        /// </summary>
        [JsonPropertyName("buildingFootprint")]
        public MapElementStyles? BuildingFootprint { get; set; }

        /// <summary>
        /// Country or regions. 
        /// </summary>
        [JsonPropertyName("countryRegion")]
        public BorderedMapElementStyles? CountryRegion { get; set; }

        /// <summary>
        /// Street blocks in the populated places. 
        /// </summary>
        [JsonPropertyName("roadDetails")]
        public MapElementStyles? RoadDetails { get; set; }
    }
}
