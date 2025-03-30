using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Specifies how shapes overlap.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ZOrderType
    {
        /// <summary>
        /// The order of shapes is determined by the order in which they are added to the map. 
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto,

        /// <summary>
        /// The order of shapes is determined by the z-index of the layer. 
        /// </summary>
        [EnumMember(Value = "viewport-y")]
        ViewportY,

        /// <summary>
        /// The order of shapes is determined by the z-index of the shape. 
        /// </summary>
        [EnumMember(Value = "source")]
        Source
    }
}
