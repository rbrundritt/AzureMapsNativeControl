using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// The type of legend to create.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LegendType
    {
        [EnumMember(Value = "category")]
        Category,

        [EnumMember(Value = "gradient")]
        Gradient,

        [EnumMember(Value = "dynamic")]
        Dynamic,

        [EnumMember(Value = "image")]
        Image,

        [EnumMember(Value = "html")]
        HTML
    }
}
