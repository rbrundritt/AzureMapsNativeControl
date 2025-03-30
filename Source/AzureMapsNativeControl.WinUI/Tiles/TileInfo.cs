namespace AzureMapsNativeControl.Tiles
{
    /// <summary>
     /// Information about a tile.
     /// </summary>
    public class TileInfo
    {
        #region Constructor

        /// <summary>
        /// Information about a tile.
        /// </summary>
        /// <param name="x">X tile position in the tile grid</param>
        /// <param name="y">Y tile position in the tile grid</param>
        /// <param name="zoom">Zoom level of the tile.</param>
        /// <param name="tileSize">The size of the tile.</param>
        /// <param name="quadkey">The Quadkey identifier of the tile. If null, will be calculated.</param>
        /// <param name="bounds3857">The bounding box of the tile in EPSG:3857 coordinates.</param>
        public TileInfo(int x, int y, int zoom, int tileSize = 512, string? quadkey = null, double[]? bounds3857 = null)
        {
            X = x;
            Y = y;
            Zoom = zoom;

            if (quadkey == null)
            {
                Quadkey = TileMath.TileXYToQuadKey(x, y, zoom);
            }
            else
            {
                Quadkey = quadkey;
            }

            TileSize = tileSize;
            Bounds3857 = bounds3857;
        }

        /// <summary>
        /// Information about a tile.Calculates X/Y/Z from quadkey.
        /// </summary>
        /// <param name="quadkey">The Quadkey identifier of the tile.</param>
        /// <param name="tileSize">The size of the tile.</param>
        /// <param name="bounds3857">The bounding box of the tile in EPSG:3857 coordinates.</param>
        public TileInfo(string quadkey, int tileSize = 256, double[]? bounds3857 = null)
        {
            Quadkey = quadkey;

            TileMath.QuadKeyToTileXY(quadkey, out int x, out int y, out int zoom);

            X = x;
            Y = y;
            Zoom = zoom;

            TileSize = tileSize;
            Bounds3857 = bounds3857;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The X position of the tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y position of the tile.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The zoom level for this tile.
        /// </summary>
        public int Zoom { get; private set; }

        /// <summary>
        /// The quadkey string value of the tile.
        /// </summary>
        public string Quadkey { get; private set; }

        /// <summary>
        /// The bounding box of the tile in EPSG:3587 with the format of "[{west},{south},{east},{north}]".
        /// </summary>
        public double[]? Bounds3857 { get; private set; }

        /// <summary>
        /// The size of the tile.
        /// </summary>
        public int TileSize { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given a templated URL, fills in the tile information.
        /// Supported URL parameters:
        /// - `{x}` - X position of tile.Tile URL usually also needs {y} and {z}.
        /// - `{y}` - Y position of tile.Tile URL usually also needs {x} and {z}.
        /// - `{z}` - Zoom level of tile.Tile URL usually also needs {x} and {y}.
        /// - `{quadkey}` - Tile quadkey id based on the Bing Maps tile system naming convention.
        /// - `{bbox-epsg-3857}` - A bounding box string with the format "{west},{south},{east},{north}" with coordinates in the EPSG 3857 Spatial Reference System also commonly known as WGS84 Web Mercator.This is useful when working with WMS imagery services.
        /// - `{subdomain}`: A placeholder where the subdomain values if specified will be added.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tileInfo"></param>
        /// <returns></returns>
        public static string FillTileUrl(string url, TileInfo tileInfo)
        {
            return url
                .Replace("{x}", tileInfo.X.ToString())
                .Replace("{y}", tileInfo.Y.ToString())
                .Replace("{z}", tileInfo.Zoom.ToString())
                .Replace("{quadkey}", tileInfo.Quadkey)
                .Replace("{bbox-epsg-3857}", tileInfo.Bounds3857 != null ? $"{tileInfo.Bounds3857[0]},{tileInfo.Bounds3857[1]},{tileInfo.Bounds3857[2]},{tileInfo.Bounds3857[3]}" : "");
        }

        #endregion
    }
}
