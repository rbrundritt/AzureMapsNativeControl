using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The options for the map's style.
    /// </summary>
    public class StyleOptions
    {
        /***
        * https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.styleoptions?view=azure-maps-typescript-latest
        * 
        * Options that can only be set when initializing the map will only appear in the MapLoadOptions class.
        * 
        * Language and View options are only exposed in MapLoadOptions at this time as they don't seem to be updatable anymore in Azure Maps.
        *  
        * Excluded options (rarely used, expose as needed): 
        * 
        * - autoResize (already set to true, little benefit in setting to false)
        * - progressiveLoadingInitialLayerGroups
        * 
        * Deprecated options (not added): 
        * 
        * - showBuildingModels
        * - userRegion
        */

        /// <summary>
        /// If true, the gl context will be created with MSAA antialiasing, which can be useful for antialiasing WebGL layers.
        /// </summary>
        [JsonPropertyName("antialias")]
        public bool? Antialias { get; set; }

        /// <summary>
        /// Sets the lighting options of the map.
        /// </summary>
        [JsonPropertyName("light")]
        public LightOptions? Light { get; set; }

        /// <summary>
        /// Specifies if multiple copies of the world should be rendered when zoomed out.
        /// </summary>
        [JsonPropertyName("renderWorldCopies")]
        public bool? RenderWorldCopies { get; set; }

        /// <summary>
        /// Specifies if the map should display labels
        /// </summary>
        [JsonPropertyName("showLabels")]
        public bool? ShowLabels { get; set; }

        /// <summary>
        /// Specifies if the Microsoft logo should be hidden or not. If set to true a Microsoft copyright string will be added to the map. 
        /// </summary>
        [JsonPropertyName("showLogo")]
        public bool? ShowLogo { get; set; }

        /// <summary>
        /// Specifies if the feedback link should be displayed on the map or not. 
        /// </summary>
        [JsonPropertyName("showFeedbackLink")]
        public bool? ShowFeedbackLink { get; set; }

        /// <summary>
        /// Specifies if the map should render an outline around each tile and the tile ID. These tile boundaries are useful for debugging. 
        /// The uncompressed file size of the first vector source is drawn in the top left corner of each tile, next to the tile ID. 
        /// </summary>
        [JsonPropertyName("showTileBoundaries")]
        public bool? ShowTileBoundaries { get; set; }

        /// <summary>
        /// The name of the style to use when rendering the map. 
        /// </summary>
        [JsonPropertyName("style")]
        public MapStyle? Style { get; set; }

        /// <summary>
        /// Override the default styles for the map elements.
        /// </summary>
        [JsonPropertyName("styleOverrides")]
        public object? StyleOverrides { get; set; }


        #region Extended Map Options

        /// <summary>
        /// CSS style for the background of the map control.
        /// Examples: 
        ///     CSS color: "#f8f8f8", "red"
        ///     CSS gradients: "linear-gradient(#0B486B 0%, #f56217 50%)"
        ///     Image: "url('https://myImage.png')"
        /// </summary>
        [JsonPropertyName("backgroundStyle")]
        public string? BackgroundStyle { get; set; }

        #endregion
    }
}
