using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The encoding of the elevation data in the tile source.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ElevationEncoding
    {
        /// <summary>
        /// Mapbox Terrain RGB tiles. See https://www.mapbox.com/help/access-elevation-data/#mapbox-terrain-rgb for more info.
        /// </summary>
        [EnumMember(Value = "mapbox")]
        Mapbox,

        /// <summary>
        /// Terrarium format PNG tiles. See https://aws.amazon.com/es/public-datasets/terrain/ for more info.
        /// </summary>
        [EnumMember(Value = "terrarium")]
        Terrarium
    }
}
