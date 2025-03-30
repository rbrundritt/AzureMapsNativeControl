using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Units of measurement for acceleration.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AccelerationUnits
    {
        /// <summary>
        /// Represents an acceleration in miles per second squared (mi/s^2).
        /// Literal value `"milesPerSecondSquared"`
        /// </summary>
        [EnumMember(Value = "milesPerSecondSquared")]
        MilesPerSecondSquared,

        /// <summary>
        /// Represents an acceleration in kilometers per second squared (km/s^2).
        /// Literal value `"kilometersPerSecondSquared"`
        /// </summary>
        [EnumMember(Value = "kilometersPerSecondSquared")]
        KilometersPerSecondSquared,

        /// <summary>
        /// Represents an acceleration in knots per second (knts/s).
        /// Literal value `"knotsPerSecond"`
        /// </summary>
        [EnumMember(Value = "knotsPerSecond")]
        KnotsPerSecond,

        /// <summary>
        /// Represents an acceleration in standard gravity units (g).
        /// Literal value `"standardGravity"`
        /// </summary>
        [EnumMember(Value = "standardGravity")]
        StandardGravity,

        /// <summary>
        /// Represents an acceleration in feet per second squared (ft/s^2).
        /// Literal value `"feetPerSecondSquared"`
        /// </summary>
        [EnumMember(Value = "feetPerSecondSquared")]
        FeetPerSecondSquared,

        /// <summary>
        /// Represents an acceleration in yards per second squared (yds/s^2).
        /// Literal value `"yardsPerSecondSquared"`
        /// </summary>
        [EnumMember(Value = "yardsPerSecondSquared")]
        YardsPerSecondSquared,

        /// <summary>
        /// Represents an acceleration in miles per hour second (mi/h/s).
        /// Literal value `"milesPerHourSecond"`
        /// </summary>
        [EnumMember(Value = "milesPerHourSecond")]
        MilesPerHourSecond,

        /// <summary>
        /// Represents an acceleration in kilometers per hours second (km/h/s).
        /// Literal value `"kilometersPerHourSecond"`
        /// </summary>
        [EnumMember(Value = "kilometersPerHourSecond")]
        KilometersPerHourSecond,

        /// <summary>
        /// Represents an acceleration in meters per second squared (m/s^2).
        /// Literal value `"metersPerSecondSquared"`
        /// </summary>
        [EnumMember(Value = "metersPerSecondSquared")]
        MetersPerSecondSquared
    }
}
