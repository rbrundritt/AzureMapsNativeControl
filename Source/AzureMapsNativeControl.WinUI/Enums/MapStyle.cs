using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Map Style
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MapStyle
    {
        /// <summary>
        /// Road map
        /// </summary>
        [EnumMember(Value = "road")]
        Road,

        /// <summary>
        /// Road map with hillshading.
        /// </summary>
        [EnumMember(Value = "road_shaded_relief")]
        RoadShadedRelief,

        /// <summary>
        /// Satellite imagery
        /// </summary>
        [EnumMember(Value = "satellite")]
        Satellite,

        /// <summary>
        /// A hybrid of satellite imagery with road labels
        /// </summary>
        [EnumMember(Value = "satellite_road_labels")]
        SatelliteRoadLabels,

        /// <summary>
        /// Empty map style
        /// </summary>
        [EnumMember(Value = "blank")]
        Blank,

        /// <summary>
        /// Empty map style, but with accessibility features (road map vector tiles are loaded behind the scenes).
        /// </summary>
        [EnumMember(Value = "blank_accessible")]
        BlankAccessible,

        /// <summary>
        /// A light grayscale version of the road map style.
        /// </summary>
        [EnumMember(Value = "grayscale_light")]
        GrayscaleLight,

        /// <summary>
        /// A dark grayscale version of the road map style.
        /// </summary>
        [EnumMember(Value = "grayscale_dark")]
        GrayscaleDark,

        /// <summary>
        /// A version of the road map style designed for low light conditions.
        /// </summary>
        [EnumMember(Value = "night")]
        Night,

        /// <summary>
        /// A dark high contrast version of the road map style.
        /// </summary>
        [EnumMember(Value = "high_contrast_dark")]
        HighContrastDark,

        /// <summary>
        /// A light high contrast version of the road map style.
        /// </summary>
        [EnumMember(Value = "high_contrast_light")]
        HighContrastLight
    }
}
