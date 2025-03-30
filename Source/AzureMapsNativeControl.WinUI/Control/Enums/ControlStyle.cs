using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Available styles for a Control.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ControlStyle
    {
        /// <summary>
        /// The control will automatically switch styles based on the style of the map. If a control doesn't support automatic styling the light style will be used by default.
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto,

        /// <summary>
        /// The control will be in the light style.
        /// </summary>
        [EnumMember(Value = "light")]
        Light,

        /// <summary>
        /// The control will be in the dark style. 
        /// </summary>
        [EnumMember(Value = "dark")]
        Dark
    }
}
