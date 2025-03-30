using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// Base legend type that all other legend types inherit from.
    /// </summary>
    [JsonDerivedType(typeof(CategoryLegend))]
    [JsonDerivedType(typeof(HtmlLegend))]
    [JsonDerivedType(typeof(ImageLegend))]
    [JsonDerivedType(typeof(GradientLegend))]
    [JsonDerivedType(typeof(DynamicLegend))]
    public abstract class BaseLegend
    {
        #region Constructor

        /// <summary>
        /// Base legend type that all other legend types inherit from.
        /// </summary>
        /// <param name="legendType">The type of legend.</param>
        public BaseLegend(LegendType legendType)
        {
            Type = legendType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of legend.
        /// </summary>
        [JsonPropertyName("type")]
        public LegendType Type { get; internal set; }

        /// <summary>
        /// The title for this specific legend.
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        /// <summary>
        /// Text to be added at the bottom of the legend.
        /// </summary>
        [JsonPropertyName("footer")]
        public string? Footer { get; set; }

        /// <summary>
        /// A CSS class to append to the legend type container. 
        /// </summary>
        [JsonPropertyName("cssClass")]
        public string? CssClass { get; set; }

        /// <summary>
        /// Min zoom level that this legend should appear.  Default: `0`
        /// </summary>
        [JsonPropertyName("minZoom")]
        public int MinZoom { get; set; } = 0;

        /// <summary>
        /// Max zoom level that this legend should appear. Default: `24`
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public int MaxZoom { get; set; } = 24;

        /// <summary>
        /// Specifies how a legend card should be treated when the map zoom level falls outside of the items min and max zoom range. Default: `'hide'`
        /// </summary>
        [JsonPropertyName("zoomBehavior")]
        public ZoomBehavior ZoomBehavior { get; set; } = Control.ZoomBehavior.Hide;

        #endregion
    }
}
