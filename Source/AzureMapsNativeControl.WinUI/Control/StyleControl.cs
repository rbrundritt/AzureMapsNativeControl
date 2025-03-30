using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control for changing the style of the map.
    /// </summary>
    public sealed class StyleControl : BaseControl
    {
        #region Contructor

        /// <summary>
        /// A control for changing the style of the map.
        /// </summary>
        public StyleControl() : base("atlas.control.StyleControl") { }

        #endregion

        #region Public Properties

        /// <summary>
        /// The map styles to display as options. 
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("mapStyles")]
        public List<MapStyle> MapStyles { get; set; } = new List<MapStyle>() {
            MapStyle.Road,
            MapStyle.Satellite,
            MapStyle.SatelliteRoadLabels,
            MapStyle.GrayscaleDark,
            MapStyle.Night,
            MapStyle.GrayscaleLight,
            MapStyle.RoadShadedRelief
        };

        /// <summary>
        /// The layout to display the styles in.
        /// Can only be set before adding the control to the map.
        /// </summary>
        [JsonPropertyName("layout")]
        public StyleControlLayout Layout { get; set; } = StyleControlLayout.Icons;

        #endregion
    }
}
