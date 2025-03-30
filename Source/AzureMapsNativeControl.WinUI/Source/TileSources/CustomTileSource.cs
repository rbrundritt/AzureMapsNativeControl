using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Tiles;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// An abstract class that allows for the creation of custom tile source.
    /// </summary>
    public abstract class CustomTileSource : TileSource
    {
        /// <summary>
        /// The URL of the custom tile source.
        /// </summary>
        /// <param name="isVectorTiles">Specifies if the tile source points to vector tiles. If true, the tile source is a VectorTileSource.</param>
        /// <param name="tileSize">The size of the tiles. Ignored by vector tile layer as they only support tile size of 512.</param>
        /// <param name="bounds">
        /// A bounding box that specifies where tiles are available. When specified, no tiles outside of the bounding box will be requested.
        /// Note: This will not crop tiles to the specific bounding box, it limits the tiles it loads to those that intersect this bounding box.
        /// </param>
        /// <param name="minSourceZoom">An integer specifying the minimum zoom level in which tiles are available from the tile source.</param>
        /// <param name="maxSourceZoom">An integer specifying the maximum zoom level in which tiles are available from the tile source.</param>
        /// <param name="isTMS">Specifies is the tile systems y coordinate uses the OSGeo Tile Map Services which reverses the Y coordinate axis. </param>
        /// <param name="elevationEncoding">
        /// If the tile source represents Elevation tiles, this specifies the DEM tiles encoding format. 
        /// If this is null, tile source will be considered as a raster or vector tile source.
        /// Ignored if isVectorTiles is true.
        /// </param>
        public CustomTileSource(
            bool isVectorTiles = false,
            int? tileSize = 512, 
            BoundingBox? bounds = null, 
            int? minSourceZoom = 0, 
            int? maxSourceZoom = 22, 
            bool isTMS = false, 
            ElevationEncoding? elevationEncoding = null) : 
        base(isVectorTiles, tileSize, bounds, minSourceZoom, maxSourceZoom, isTMS, elevationEncoding)
        {
            TileUrl = Utils.GetCustomTileSourceProxy(Id, isVectorTiles? 512: (tileSize ?? 512));
        }

        /// <summary>
        /// Abstract method that gets the tile stream for the given tile info.
        /// Any class that inherits from this class must implement this method.
        /// </summary>
        /// <param name="tileInfo"></param>
        /// <returns></returns>
        public abstract Task<MapFileStream?> GetTileStream(TileInfo tileInfo);
    }
}
