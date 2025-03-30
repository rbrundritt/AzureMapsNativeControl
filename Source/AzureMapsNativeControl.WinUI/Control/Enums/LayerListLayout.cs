using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// How the layer state items are presented.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum LayerListLayout
    {
        /// <summary>
        /// A list of layers with checkboxes to toggle them on and off.
        /// </summary>
        [EnumMember(Value = "checkbox")]
        Checkbox,

        /// <summary>
        /// A list of layers with a dropdown to switch between them.
        /// </summary>
        [EnumMember(Value = "dropdown")]
        Dropdown,

        /// <summary>
        /// A list of layers with radio buttons to switch between them.
        /// </summary>
        [EnumMember(Value = "radio")]
        Radio,

        /// <summary>
        /// A slider to adjust the a numeric value in a layer.
        /// Not supported by the dynamic layer group.
        /// </summary>
        [EnumMember(Value = "range")]
        Range
    }
}
