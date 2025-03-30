using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The options for the map's lighting.
    /// </summary>
    public class LightOptions
    {
        //https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.lightoptions?view=azure-maps-typescript-latest

        private PitchAlignment? anchor;

        /// <summary>
        /// Specifies which part of the icon is placed closest to the icons anchor position on the map.
        /// Auto is not supported and will be converted to Map.
        /// </summary>
        [JsonPropertyName("anchor")]
        public PitchAlignment? Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value == PitchAlignment.Auto ? PitchAlignment.Map : value;
            }
        }

        /// <summary>
        /// Color tint for lighting extruded geometries.
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// Intensity of lighting (on a scale from 0 to 1). Higher numbers will present as more extreme contrast. 
        /// </summary>
        [JsonPropertyName("intensity")]
        public double? Intensity { get; set; }

        /// <summary>
        /// Position of the light source relative to lit (extruded) geometries, in [r radial coordinate, a 
        /// azimuthal angle, p polar angle] where r indicates the distance from the center of the base of an object 
        /// to its light, a indicates the position of the light relative to 0° (0° when anchor is set to viewport 
        /// corresponds to the top of the viewport, or 0° when anchor is set to map corresponds to due north, and degrees proceed clockwise), 
        /// and p indicates the height of the light (from 0°, directly above, to 180°, directly below).
        /// </summary>
        [JsonPropertyName("position")]
        public double[]? Position { get; set; }

    }
}
