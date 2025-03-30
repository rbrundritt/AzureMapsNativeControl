using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// The justification of the text relative to the point geometry.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum TextJustify
    {
        /// <summary>
        /// The text is centered.
        /// </summary>
        [EnumMember(Value = "center")]
        Center,

        /// <summary>
        /// The text is aligned towards the anchor position.
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto,

        /// <summary>
        /// The text is left justified.
        /// </summary>
        [EnumMember(Value = "left")]
        Left,

        /// <summary>
        /// The text is right justified.
        /// </summary>
        [EnumMember(Value = "right")]
        Right
    }
}
