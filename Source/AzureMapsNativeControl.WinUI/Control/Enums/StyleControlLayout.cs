using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// The layout to display the style control in.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum StyleControlLayout
    {
        /// <summary>
        /// A row of clickable icons for each style.
        /// </summary>
        [EnumMember(Value = "icons")]
        Icons,

        /// <summary>
        /// A scrollable list with the icons and names for each style.
        /// </summary>
        [EnumMember(Value = "list")]
        List
    }
}
