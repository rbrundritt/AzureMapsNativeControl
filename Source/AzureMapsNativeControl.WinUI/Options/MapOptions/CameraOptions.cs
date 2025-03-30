using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// The options for setting the map control's camera.
    /// </summary>
    public class CameraOptions: IDeepCloneable<CameraOptions>
    {
        #region Public Properties

        /// <summary>
        /// The position to align the center of the map view with.
        /// Default `[0, 0]`.
        /// </summary>
        [JsonPropertyName("center")]
        public Position? Center { get; set; }

        /// <summary>
        /// The zoom level of the map view.
        /// </summary>
        [JsonPropertyName("zoom")]
        public double? Zoom { get; set; }

        /// <summary>
        /// A pixel offset to apply to the center of the map.
        /// This is useful if you want to programmatically pan the map to another location or if you want to center the map over a shape, then offset the maps view to make room for a popup.
        /// </summary>
        [JsonPropertyName("centerOffset")]
        public Pixel? CenterOffset { get; set; }

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

        /// <summary>
        /// The minimum zoom level that the map can be zoomed out to during the animation. Must be between 0 and 24, and less than or equal to `maxZoom`.
        /// Setting `minZoom` below 1 may result in an empty map when the zoom level is less than 1.
        /// </summary>
        [JsonPropertyName("minZoom")]
        public double? MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom level that the map can be zoomed into during the animation. Must be between 0 and 24, and greater than or equal to `minZoom`.
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public double? MaxZoom { get; set; }

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

        /// <summary>
        /// A bounding box in which to constrain the viewable map area to.
        /// Users won't be able to pan the center of the map outside of this bounding box.
        /// Set maxBounds to null or undefined to remove maxBounds
        /// Default `undefined`.
        /// </summary>
        [JsonPropertyName("maxBounds")]
        public BoundingBox? MaxBounds { get; set; }

        #endregion

        #region Camera Bounds Options

        /// <summary>
        /// The bounding box of the map view.
        /// </summary>
        [JsonPropertyName("bounds")]
        public BoundingBox? Bounds { get; set; }

        /// <summary>
        /// The amount of padding in pixels to add to the given bounds.
        /// </summary>
        [JsonPropertyName("padding")]
        public Padding? Padding { get; set; }

        /// <summary>
        /// A pixel offset to apply to the center of the map.
        /// This is useful if you want to programmatically pan the map to another location or if you want to center the map over a shape, then offset the maps view to make room for a popup.
        /// </summary>
        [JsonPropertyName("offset")]
        public Pixel? Offset { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deep clones the object.
        /// </summary>
        /// <returns></returns>
        public CameraOptions DeepClone()
        {
            return new CameraOptions()
            {
                Center = Center?.DeepClone(),
                Zoom = Zoom,
                CenterOffset = CenterOffset?.DeepClone(),
                Bearing = Bearing,
                Pitch = Pitch,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                MinPitch = MinPitch,
                MaxPitch = MaxPitch,
                MaxBounds = MaxBounds?.DeepClone(),

                Bounds = Bounds?.DeepClone(),
                Padding = Padding?.DeepClone(),
                Offset = Offset?.DeepClone()
            };
        }

        #endregion
    }
}
