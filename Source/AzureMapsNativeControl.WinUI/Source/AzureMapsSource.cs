using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// A source provider that represents a vector tile source that exists with the Azure Maps control and is only accessible by ID.
    /// </summary>
    public class AzureMapsSource: BaseSource
    {
        #region Constructor 

        /// <summary>
        /// A source provider that represents a vector tile source that exists with the Azure Maps control and is only accessible by ID.
        /// </summary>
        /// <param name="info">Details about the source.</param>
        public AzureMapsSource(RawBasemapSourceInfo info) : base("AzureMapsSource", info.Id, true)
        {
            SourceType = info.SourceType;
            Bounds = new BoundingBox(info.Bounds[0], info.Bounds[1], info.Bounds[2], info.Bounds[3]);
            MinZoom = info.MinZoom;
            MaxZoom = info.MaxZoom;
            Attribution = info.Attribution;
        }

        #endregion

        #region Properties

        /// <summary>
        /// An attribution for the source.
        /// </summary>
        public string? Attribution { get; private set; }

        /// <summary>
        /// The bounding box area the source is available for.
        /// </summary>
        public BoundingBox Bounds { get; private set; }

        /// <summary>
        /// The minimum zoom level that the source becomes available.
        /// </summary>
        public int? MinZoom { get; private set; }

        /// <summary>
        /// The maximum zoom level the source is available to.
        /// </summary>
        public int? MaxZoom { get; private set; }

        /// <summary>
        /// The type of source: geojson, vector, raster, raster-dem, image, video.
        /// </summary>
        public string SourceType { get; private set; }

        #endregion

        public override BaseSourceOptions GetOptions()
        {
            return new BaseSourceOptions
            {
                MinZoom = MinZoom,
                MaxZoom = MaxZoom
            };
        }
    }
}
