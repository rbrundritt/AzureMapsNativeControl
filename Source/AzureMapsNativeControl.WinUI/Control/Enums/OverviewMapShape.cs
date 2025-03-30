using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Specifies the shape of the overview map. 
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum OverviewMapShape
    {
        /// <summary>
        /// A square or rectangular map based on the width/height aspect ratio of the parent map.
        /// </summary>
        [EnumMember(Value = "square")]
        Square,

        /// <summary>
        /// A square or elliptical map based on the width/height aspect ratio of the parent map.
        /// </summary>
        [EnumMember(Value = "round")]
        Round
    }
}
