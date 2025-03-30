using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Units of measurement for areas.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AreaUnits
    {
        /// <summary>
        /// Represents areas in square meters (m^2).
        /// </summary>
        [EnumMember(Value = "squareMeters")]
        SquareMeters,

        /// <summary>
        /// Represents areas in acres (ac).
        /// </summary>
        [EnumMember(Value = "acres")]
        Acres,

        /// <summary>
        /// Represents areas in hectares (ha).
        /// </summary>
        [EnumMember(Value = "hectares")]
        Hectares,

        /// <summary>
        /// Represents areas in feet (ft^2).
        /// </summary>
        [EnumMember(Value = "squareFeet")]
        SquareFeet,

        /// <summary>
        /// Represents areas in square kilometers (km^2).
        /// </summary>
        [EnumMember(Value = "squareKilometers")]
        SquareKilometers,

        /// <summary>
        /// Represents areas in miles (mi^2).
        /// </summary>
        [EnumMember(Value = "squareMiles")]
        SquareMiles,

        /// <summary>
        /// Represents areas in yards (yds^2).
        /// </summary>
        [EnumMember(Value = "squareYards")]
        SquareYards
    }

}
