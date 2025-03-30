using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Legends
{
    /// <summary>
    /// A legend that dynamically generated from a layers style.
    /// </summary>
    public class DynamicLegend: BaseLegend
    {
        #region Constructor

        /// <summary>
        /// A legend that dynamically generated from a layers style.
        /// </summary>
        public DynamicLegend() : base(LegendType.Dynamic)
        {
        }

        /// <summary>
        /// A legend that dynamically generated from a layers style.
        /// </summary>
        /// <param name="layer"> The layer to generate the legend(s) for.</param>
        public DynamicLegend(BaseLayer layer) : base(LegendType.Dynamic)
        {
            LayerId = layer.Id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The id of the layer to generate the legend(s) for.
        /// </summary>
        [JsonPropertyName("layer")]
        public string? LayerId { get; set; }

        /// <summary>
        /// Default options to apply to category legends.
        /// </summary>

        [JsonPropertyName("defaultCategory")]
        public CategoryLegendDefaults? DefaultCategory { get; set; }

        /// <summary>
        /// Default options to apply to image legends.
        /// </summary>

        [JsonPropertyName("defaultImage")]
        public ImageLegendDefaults? DefaultImage { get; set; }

        /// <summary>
        /// Default options to apply to gradient legends.
        /// </summary>

        [JsonPropertyName("defaultGradient")]
        public GradientLegendDefaults? DefaultGradient { get; set; }

        /// <summary>
        /// Specifies how subtitles should be set if not explicitly set in the legend type. 
        /// - `'auto'` - Looks at the layers metadata for the following properties, in this order `'title'`,  `'subtitle'`. Falls back to the layers ID.
        /// - `'expression'` - If a style expression has a simple `get` expression such as `['get', 'revenue']` the property name will be extracted and set as the subtitle of the legend card. Falls back to the layers ID.
        /// - `'none'` - No subtitle value is added to the legend.
        /// - `string` - The name of a property in the layers metadata to use as the subtitle.
        /// Falls back to the layers ID unless set to `'none'`.
        /// Default: `'auto'`
        /// </summary>
        [JsonPropertyName("subtitleFallback")]
        public string? SubtitleFallback { get; set; } = "auto";

        /// <summary>
        /// Specifies how footer should be set if not explicitly set in the legend type. 
        /// - `'auto'` - Looks at the layers metadata for the following properties, in this order `'footer'`,  `'description'`, `'abstract'`.
        /// - `'none'` - No footer value is added to the legend.
        /// - `string` - The name of a property in the layers metadata to use as the footer.
        /// Default: `'auto'`
        /// </summary>
        [JsonPropertyName("footerFallback")]
        public string? FooterFallback { get; set; } = "auto";

        #endregion
    }
}
