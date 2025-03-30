using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// How multiple items are laid out in a legend or layer control.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ControlLayout
    {
        [EnumMember(Value = "carousel")]
        Carousel,

        [EnumMember(Value = "list")]
        List,

        [EnumMember(Value = "accordion")]
        Accordion
    }
}
