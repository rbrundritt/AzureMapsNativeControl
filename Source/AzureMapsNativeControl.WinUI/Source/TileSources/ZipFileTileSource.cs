using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Tiles;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// A tile source that retrieves tiles from a zip file.
    /// </summary>
    public class ZipFileTileSource : CustomTileSource
    {
        #region Private Properties

        private ZipArchive _zipFile;
        private bool _disposeZip;
        private string _formattedFilePath;
        private string _mimeType;

        #endregion

        #region Constructor

        /// <summary>
        /// A tile source that retrieves tiles from a zip file.
        /// </summary>
        /// <param name="zipFile">The zip file to read from.</param>
        /// <param name="formattedFilePath">
        /// A formatted tile path to the files in the zip.
        /// 
        /// Supported placeholders:
        /// - `{x}` - X position of tile.Tile URL usually also needs {y} and {z}.
        /// - `{y}` - Y position of tile.Tile URL usually also needs {x} and {z}.
        /// - `{z}` - Zoom level of tile.Tile URL usually also needs {x} and {y}.
        /// - `{quadkey}` - Tile quadkey id based on the Bing Maps tile system naming convention.
        /// - `{bbox-epsg-3857}` - A bounding box string with the format "{west},{south},{east},{north}" with coordinates in the EPSG 3857 Spatial Reference System also commonly known as WGS84 Web Mercator.This is useful when working with WMS imagery services.
        /// - `{subdomain}`: A placeholder where the subdomain values if specified will be added.
        /// </param>
        /// <param name="contentType">The mimeType of the tiles. Defaults to "application/x-protobuf" if isVectorTiles is true, otherwise "image/png" </param>
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
        public ZipFileTileSource(
            ZipArchive zipFile,
            string formattedFilePath,
            string? contentType = null,
            bool isVectorTiles = false,
            int? tileSize = 512,
            BoundingBox? bounds = null,
            int? minSourceZoom = 0,
            int? maxSourceZoom = 22,
            bool isTMS = false, 
            ElevationEncoding? elevationEncoding = null) : 
        base(isVectorTiles, tileSize, bounds, minSourceZoom, maxSourceZoom, isTMS, elevationEncoding)
        {
            _zipFile = zipFile;
            _formattedFilePath = formattedFilePath;
            _mimeType = isVectorTiles? Constants.PBFMimeType : Constants.PNGMimeType;

            if (contentType != null)
            {
                _mimeType = Utils.GetMimeType(contentType, _mimeType);
            }
            else
            {
                //Try to get the mime type from the formatted file path.
                var mt = Utils.GetMimeType(formattedFilePath, _mimeType);

                if(mt.Equals(Constants.PlainTextMimeType))
                {
                    mt = _mimeType;
                }

                _mimeType = mt;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Looks at the input and tries to load a zip file from a URL (full path), a file path (full, relative to asset folder, or app data directory).
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <returns></returns>
        public static async Task<ZipArchive?> TryLoadAsync(string filePathOrUrl)
        {
            var result = await Utils.TryGetFileStreamAsync(filePathOrUrl);

            if(result != null)
            {
               return new ZipArchive(result.Stream, ZipArchiveMode.Read);
            }

            return null;
        }

        /// <inheritdoc/>
        public override async Task<MapFileStream?> GetTileStream(TileInfo tileInfo)
        {
            //Get the tile path
            string tilePath = TileInfo.FillTileUrl(_formattedFilePath, tileInfo);

            //Get the entry from the zip file
            var entry = _zipFile.GetEntry(tilePath);

            var file = _zipFile.Entries.Where(x => x.FullName == tilePath).FirstOrDefault();

            if (file != null)
            {
                //Copy the file stream to a memory stream.
                var ms = new MemoryStream();
                using (var fs = file.Open())
                {
                    await fs.CopyToAsync(ms);
                }
                ms.Position = 0;

                return new MapFileStream(ms, _mimeType);
            }

            return null;
        }

        #endregion
    }
}
