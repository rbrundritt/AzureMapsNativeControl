using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Units of measurement for distances.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum DistanceUnits
    {
        /// <summary>
        /// Represents a distance in meters (m).
        /// Literal value `"meters"`
        /// </summary>
        [EnumMember(Value = "meters")]
        Meters,

        /// <summary>
        /// Represents a distance in kilometers (km).
        /// Literal value `"kilometers"`
        /// </summary>
        [EnumMember(Value = "kilometers")]
        Kilometers,

        /// <summary>
        /// Represents a distance in feet (ft).
        /// Literal value `"feet"`
        /// </summary>
        [EnumMember(Value = "feet")]
        Feet,

        /// <summary>
        /// Represents a distance in miles (mi).
        /// Literal value `"miles"`
        /// </summary>
        [EnumMember(Value = "miles")]
        Miles,

        /// <summary>
        /// Represents a distance in nautical miles.
        /// Literal value `"nauticalMiles"`
        /// </summary>
        [EnumMember(Value = "nauticalMiles")]
        NauticalMiles,

        /// <summary>
        /// Represents a distance in yards (yds).
        /// Literal value `"yards"`
        /// </summary>
        [EnumMember(Value = "yards")]
        Yards
    }
}
