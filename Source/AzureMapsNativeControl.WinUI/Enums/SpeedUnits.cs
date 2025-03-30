using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Units of measurement for speed.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum SpeedUnits
    {
        /// <summary>
        /// Represents a speed in meters per second (m/s).
        /// </summary>
        [EnumMember(Value = "metersPerSecond")]
        MetersPerSecond,

        /// <summary>
        /// Represents a speed in kilometers per hour (km/h).
        /// </summary>
        [EnumMember(Value = "kilometersPerHour")]
        KilometersPerHour,

        /// <summary>
        /// Represents a speed in feet per second (ft/s).
        /// </summary>
        [EnumMember(Value = "feetPerSecond")]
        FeetPerSecond,

        /// <summary>
        /// Represents a speed in miles per hour (mph).
        /// </summary>
        [EnumMember(Value = "milesPerHour\"")]
        MilesPerHour,

        /// <summary>
        /// Represents a speed in knots (knts).
        /// </summary>
        [EnumMember(Value = "knots")]
        Knots,

        /// <summary>
        /// Represents a speed in mach.
        /// </summary>
        [EnumMember(Value = "mach")]
        Mach
    }
}
