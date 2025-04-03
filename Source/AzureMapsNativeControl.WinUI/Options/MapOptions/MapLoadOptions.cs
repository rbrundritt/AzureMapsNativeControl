using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Data;
using System.Text.Json.Serialization;


#if WINUI
using Microsoft.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// A set of options to use when loading the map.
    /// </summary>
#if MAUI
    public class MapLoadOptions : BindableObject
#else
    public class MapLoadOptions
#endif
    {
        //A combination of options from CameraOptions, CameraBoundsOptions, ServiceOptions, StyleOptions, and UserInteractionOptions.
        //Some additional options are included that are only available when loading the map.

        #region Camera Options

        /// <summary>
        /// The position to align the center of the map view with.
        /// Format: "longitude, latitude"
        /// </summary>
        [JsonIgnore]
        public string? Center
        {
            get => CenterPosition?.ToString();
            set => CenterPosition = Position.Parse(value);
        }

        /// <summary>
        /// The position to align the center of the map view with.
        /// </summary>
        [JsonPropertyName("center")]
        [JsonConverter(typeof(PositionConverter))]
        public Position? CenterPosition { get; set; }

        /// <summary>
        /// The zoom level of the map view.
        /// </summary>
        [JsonPropertyName("zoom")]
        public double? Zoom { get; set; }

        /// <summary>
        /// The pitch (tilt) of the map in degrees between 0 and 60, where 0 is looking straight down on the map.
        /// Format: "X,Y"
        /// </summary>
        [JsonIgnore]
        public string? CenterOffset
        {
            get => CenterOffsetProperty?.ToString();
            set => CenterOffsetProperty = Pixel.Parse(value);
        }

        /// <summary>
        /// A pixel offset to apply to the center of the map.
        /// This is useful if you want to programmatically pan the map to another location or if you want to center the map over a shape, then offset the maps view to make room for a popup.
        /// </summary>
        [JsonPropertyName("centerOffset")]
        public Pixel? CenterOffsetProperty { get; set; }

        /// <summary>
        /// The minimum zoom level that the map can be zoomed out to during the animation. Must be between 0 and 24, and less than or equal to `maxZoom`.
        /// Setting `minZoom` below 1 may result in an empty map when the zoom level is less than 1.
        /// </summary>
        [JsonPropertyName("minZoom")]
        public double? MinZoom { get; set; }

        /// <summary>
        /// The minimum pitch that the map can be pitched to during the animation. Must be between 0 and 85, and less than or equal to `maxPitch`.
        /// </summary>
        [JsonPropertyName("minPitch")]
        public double? MinPitch { get; set; }

        /// <summary>
        /// The maximum pitch that the map can be pitched to during the animation. Must be between 0 and 85, and greater than or equal to `minPitch`
        /// </summary>
        [JsonPropertyName("maxPitch")]
        public double? MaxPitch { get; set; }

        #endregion

        #region Camera Bounds Options

        /// <summary>
        /// The bounding box of the map view.
        /// Format: "west, south, east, north"
        /// </summary>
        [JsonIgnore]
        public string? Bounds
        {
            get => BoundsProperty?.ToString();
            set => BoundsProperty = BoundingBox.Parse(value);
        }

        /// <summary>
        /// The bounding box of the map view.
        /// </summary>
        [JsonPropertyName("bounds")]
        public BoundingBox? BoundsProperty { get; set; }

        /// <summary>
        /// The amount of padding in pixels to add to the given bounds.
        /// Format: "left, top, right, bottom"
        /// </summary>
        [JsonIgnore]
        public string? Padding
        {
            get => PaddingProperty?.ToString();
            set => PaddingProperty = AzureMapsNativeControl.Padding.Parse(value);
        }

        /// <summary>
        /// The amount of padding in pixels to add to the given bounds.
        /// </summary>
        [JsonPropertyName("padding")]
        public Padding? PaddingProperty { get; set; }

        /// <summary>
        /// A pixel offset to apply to the center of the map.
        /// This is useful if you want to programmatically pan the map to another location or if you want to center the map over a shape, then offset the maps view to make room for a popup.
        /// Format: "X,Y"
        /// </summary>
        [JsonIgnore]
        public string? Offset
        {
            get => OffsetProperty?.ToString();
            set => OffsetProperty = Pixel.Parse(value);
        }

        /// <summary>
        /// A pixel offset to apply to the center of the map.
        /// This is useful if you want to programmatically pan the map to another location or if you want to center the map over a shape, then offset the maps view to make room for a popup.
        /// </summary>
        [JsonPropertyName("offset")]
        public Pixel? OffsetProperty { get; set; }

        #endregion

        #region Common Camera Options

        /// <summary>
        /// A bounding box in which to constrain the viewable map area to.
        /// Users won't be able to pan the center of the map outside of this bounding box.
        /// Set maxBounds to null or undefined to remove maxBounds
        /// Format: "west, south, east, north"
        /// </summary>
        [JsonIgnore]
        public string? MaxBounds
        {
            get => MaxBoundsProperty?.ToString();
            set => MaxBoundsProperty = BoundingBox.Parse(value);
        }

        /// <summary>
        /// A bounding box in which to constrain the viewable map area to.
        /// Users won't be able to pan the center of the map outside of this bounding box.
        /// Set maxBounds to null or undefined to remove maxBounds
        /// Default `undefined`.
        /// </summary>
        [JsonPropertyName("maxBounds")]
        public BoundingBox? MaxBoundsProperty { get; set; }

        /// <summary>
        /// The bearing of the map (rotation) in degrees. When the bearing is 0, 90, 180, or 270 the top of the map container will be north, east, south or west respectively.
        /// </summary>
        [JsonPropertyName("bearing")]
        public double? Bearing { get; set; }

        /// <summary>
        /// The pitch (tilt) of the map in degrees between 0 and 60, where 0 is looking straight down on the map.
        /// </summary>
        [JsonPropertyName("pitch")]
        public double? Pitch { get; set; }

        #endregion

        #region Service Options

        /***
         * https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.serviceoptions?view=azure-maps-typescript-latest
         * 
         * The following options are included in the AzureMapsConfiguration and not here:
         * 
         * - authOptions, 
         * - domain
         * - disableTelementry
         * - enableAccessibility
         * - enableAccessibilityLocationFallback
         * - sessionId
         * 
         * Excluded options (rarely used, expose as needed): 
         * 
         * - staticAssetsDomain
         * - styleDefinitionsPath
         * - styleDefinitionsVersion
         * - styleAPIVersion
         * - mapConfiguration
         * - localIdeographFontFamily
         * - validateStyle
         * 
         * Deprecated options (not added): 
         * - styleSet
         */

        /// <summary>
        /// Controls the duration of the fade-in/fade-out animation for label collisions, in milliseconds. This setting affects all symbol layers. 
        /// This setting does not affect the duration of runtime styling transitions or raster tile cross-fading.
        /// </summary>
        [JsonPropertyName("fadeDuration")]
        public ushort? FadeDuration { get; set; } = 100;

        /// <summary>
        /// Maximum number of images (raster tiles, sprites, icons) to load in parallel, which affects performance in raster-heavy maps.
        /// </summary>
        [JsonPropertyName("maxParallelImageRequests")]
        public byte? MaxParallelImageRequests { get; set; }

        /// <summary>
        /// A boolean that specifies if vector and raster tiles should be reloaded when they expire (based on expires header). 
        /// This is useful for data sets that update frequently. 
        /// When set to false, each tile will be loaded once, when needed, and not reloaded when they expire. 
        /// </summary>
        [JsonPropertyName("refreshExpiredTiles")]
        public bool? RefreshExpiredTiles { get; set; }

        /// <summary>
        /// Number of web workers instantiated on a page. By default, it is set to half the number of CPU cores.
        /// </summary>
        [JsonPropertyName("workerCount")]
        public byte? WorkerCount { get; set; }

        #endregion

        #region Style Options

        #region Options that can only be set when initializing the map

        /// <summary>
        /// The language of the map labels. 
        /// </summary>
        [JsonPropertyName("language")]
        public string? Language { get; set; } = "en-US";

        /// <summary>
        /// If true, the map's canvas can be exported to a PNG using map.getCanvas().toDataURL(). 
        /// </summary>
        [JsonPropertyName("preserveDrawingBuffer")]
        public bool? PreserveDrawingBuffer { get; set; }

        /// <summary>
        /// If true, the map will try to defer non-essential map layers and show essential layers as early as possible. 
        /// </summary>
        [JsonPropertyName("progressiveLoading")]
        public bool? ProgressiveLoading { get; set; }

        /// <summary>
        /// Specifies which set of geopolitically disputed borders and labels are displayed on the map. 
        /// The View parameter (also referred to as “user region parameter”) is a 2-letter ISO-3166 Country Code 
        /// that will show the correct maps for that country/region. Country/Regions that are not on the View list 
        /// or if unspecified will default to the “Unified” View. Please see the supported Views It is your 
        /// responsibility to determine the location of your users, and then set the View parameter correctly for 
        /// that location. The View parameter in Azure Maps must be used in compliance with applicable laws, including 
        /// those regarding mapping, of the country where maps, images and other data and third party content that 
        /// You are authorized to access via Azure Maps is made available. 
        /// </summary>
        [JsonPropertyName("view")]
        public string? View { get; set; } = "Auto";

        #endregion

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

        #endregion

        #region User Interaction Options

        //https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.userinteractionoptions?view=azure-maps-typescript-latest

        /// <summary>
        /// Whether the Shift + left click and drag will draw a zoom box. 
        /// </summary>
        [JsonPropertyName("boxZoomInteraction")]
        public bool? BoxZoomInteraction { get; set; }

        /// <summary>
        /// Whether double left click will zoom the map inwards.
        /// </summary>
        [JsonPropertyName("dblClickZoomInteraction")]
        public bool? DblClickZoomInteraction { get; set; }

        /// <summary>
        /// Whether left click and drag will pan the map.
        /// </summary>
        [JsonPropertyName("dragPanInteraction")]
        public bool? DragPanInteraction { get; set; }

        /// <summary>
        /// Whether right click and drag will rotate and pitch the map. 
        /// </summary>
        [JsonPropertyName("dragRotateInteraction")]
        public bool? DragRotateInteraction { get; set; }

        /// <summary>
        /// Whether the map is interactive or static. If false, all user interaction is disabled. If true, only selected user interactions will enabled. 
        /// </summary>
        [JsonPropertyName("interactive")]
        public bool? Interactive { get; set; }

        /// <summary>
        /// Whether the keyboard interactions are enabled.
        /// </summary>
        [JsonPropertyName("keyboardInteraction")]
        public bool? KeyboardInteraction { get; set; }

        /// <summary>
        /// Whether the map should zoom on scroll input.
        /// </summary>
        [JsonPropertyName("scrollZoomInteraction")]
        public bool? ScrollZoomInteraction { get; set; }

        /// <summary>
        /// Whether touch interactions are enabled for touch devices.
        /// </summary>
        [JsonPropertyName("touchInteraction")]
        public bool? TouchInteraction { get; set; }

        /// <summary>
        /// Whether touch rotation is enabled for touch devices. This option is not applied if touchInteraction is disabled.
        /// </summary>
        [JsonPropertyName("touchRotate")]
        public bool? TouchRotate { get; set; }

        /// <summary>
        /// Sets the zoom rate of the mouse wheel 
        /// </summary>
        [JsonPropertyName("wheelZoomRate")]
        public double? WheelZoomRate { get; set; }

        #endregion

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

        /// <summary>
        /// Specifies if files are allowed to be dropped onto the map.
        /// </summary>
        [JsonPropertyName("allowFileDrop")]
        public bool? AllowFileDrop { get; set; }

        #endregion

        [JsonInclude]
        [JsonPropertyName("platform")]
        internal string? Platform
        {
            get
            {
#if MAUI
                return "MAUI";
#elif WINUI
                return "WINUI";
#else
                return "WPF";
#endif
            }
        }
    }
}
