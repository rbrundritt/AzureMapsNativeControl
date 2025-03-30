using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Specifies the orientation of circle when map is pitched.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum PitchAlignment
    {
        /// <summary>
        /// For rotations alignments, When the `placement` is set to `"point"`, this is equivalent to `"map"`. When the `placement` is set to `"line"` this is equivalent to `"map"`.
        /// For Symbol pitch alignments, Automatically matches the value of `rotationAlignment`.
        /// When used with bubble line, and polygon extrusion layer, will be set to map. 
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto,

        /// <summary>
        /// The entity is aligned to the plane of the map.
        /// </summary>
        [EnumMember(Value = "map")]
        Map,

        /// <summary>
        /// The entity is aligned to the plane of the viewport.
        /// </summary>
        [EnumMember(Value = "viewport")]
        Viewport
    }
}
