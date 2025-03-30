using AzureMapsNativeControl.Data.JsonConverters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// CSS `flex-direction`
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CssFlexDirection
    {
        [EnumMember(Value = "row")]
        Row,

        [EnumMember(Value = "column")]
        Column,

        [EnumMember(Value = "row-reverse")]
        RowReverse,

        [EnumMember(Value = "column-reverse")]
        ColumnReverse
    }
}
