using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Tiles;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// A tile source that uses a URL or file path to access tiles.
    /// Used by TileLayer and VectorTileLayer.
    /// 
    /// The URL can point to:
    /// - Local files in the Maps LocalWebAssetsFolder.
    /// - Remote files on the internet.
    /// - A data URL.
    /// 
    /// Supported URL parameters:
    /// - `{x}` - X position of tile.Tile URL usually also needs {y} and {z}.
    /// - `{y}` - Y position of tile.Tile URL usually also needs {x} and {z}.
    /// - `{z}` - Zoom level of tile.Tile URL usually also needs {x} and {y}.
    /// - `{quadkey}` - Tile quadkey id based on the Bing Maps tile system naming convention.
    /// - `{bbox-epsg-3857}` - A bounding box string with the format "{west},{south},{east},{north}" with coordinates in the EPSG 3857 Spatial Reference System also commonly known as WGS84 Web Mercator.This is useful when working with WMS imagery services.
    /// - `{subdomain}`: A placeholder where the subdomain values if specified will be added.
    /// </summary>
    public class TileSource: BaseSource
    {
        #region Private Properties

        private ElevationEncoding? _elevationEncoding = null;

        #endregion 

        #region Constructor

        /// <summary>
        /// A data source for raster images tile layers that are loaded from a filePath or URL.
        /// Used by TileLayer and VectorTileLayer.
        /// </summary>
        /// <param name="tileUrl">
        /// Either the direct URL to the image or formatted tile service URL.
        /// 
        /// The URL can point to:
        /// - A local file in the Maps LocalWebAssetsFolder.
        /// - A remote file on the internet.
        /// - A data URL.
        /// 
        /// Supported URL parameters:
        /// - `{x}` - X position of tile.Tile URL usually also needs {y} and {z}.
        /// - `{y}` - Y position of tile.Tile URL usually also needs {x} and {z}.
        /// - `{z}` - Zoom level of tile.Tile URL usually also needs {x} and {y}.
        /// - `{quadkey}` - Tile quadkey id based on the Bing Maps tile system naming convention.
        /// - `{bbox-epsg-3857}` - A bounding box string with the format "{west},{south},{east},{north}" with coordinates in the EPSG 3857 Spatial Reference System also commonly known as WGS84 Web Mercator.This is useful when working with WMS imagery services.
        /// - `{subdomain}`: A placeholder where the subdomain values if specified will be added.
        /// </param>
        /// <param name="useProxy">Specifies if a proxy should be used to access the image(s). This allows accessing resources that are hosted on non-CORs enabled endpoints.</param>
        /// <param name="isVectorTiles">Specifies if the tile source points to vector tiles. If true, the tile source is a VectorTileSource.</param>
        /// <param name="tileSize">The size of the tiles. Ignored by vector tile layer, as they only support tile size of 512.</param>
        /// <param name="subdomains">A list of sub domains.</param>
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
        public TileSource(string tileUrl, bool? useProxy = false, bool isVectorTiles = false, int? tileSize = 512, IList<string>? subdomains = null,
            BoundingBox? bounds = null, int? minSourceZoom = 0, int? maxSourceZoom = 22, bool isTMS = false, ElevationEncoding? elevationEncoding = null) : 
            base(isVectorTiles ? Constants.VectorTileSource : (elevationEncoding != null)? Constants.ElevationTileSource : "TileSource", null)
        {
            //Check to see if a proxy should be used.
            if (useProxy == true && !tileUrl.StartsWith("data:") && !tileUrl.StartsWith("/proxy?"))
            {
                tileUrl = Utils.GetUrlProxy(tileUrl);

                //Need to decode placeholders for the tile requests.
                tileUrl = tileUrl.Replace("%7B", "{").Replace("%7D", "}");
            }

            TileUrl = tileUrl;

            IsVectorTiles = isVectorTiles;
            TileSize = IsVectorTiles ? 512 : tileSize;

            Subdomains = subdomains;

            if (bounds == null)
            {
                bounds = BoundingBox.Global();
            }
            
            MinSourceZoom = minSourceZoom;
            MaxSourceZoom = maxSourceZoom;

            ElevationEncoding = elevationEncoding;

            IsTMS = isTMS;
        }

        /// <summary>
        /// A data source for raster images for tile layers that are loaded from a custom source.
        /// Used by TileLayer and VectorTileLayer.
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
        internal TileSource(bool isVectorTiles = false, int? tileSize = 512,
            BoundingBox? bounds = null, int? minSourceZoom = 0, int? maxSourceZoom = 22, bool isTMS = false, ElevationEncoding? elevationEncoding = null) :
            base(isVectorTiles ? Constants.VectorTileSource : (elevationEncoding != null) ? Constants.ElevationTileSource : "TileSource", null)
        {
            TileUrl = Utils.GetCustomTileSourceProxy(Id);

            IsVectorTiles = isVectorTiles;
            TileSize = IsVectorTiles? 512 : tileSize;

            if (bounds == null)
            {
                bounds = BoundingBox.Global();
            }

            MinSourceZoom = minSourceZoom;
            MaxSourceZoom = maxSourceZoom;

            ElevationEncoding = elevationEncoding;

            IsTMS = isTMS;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Attribution information on the provider of the tile source data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attribution")]
        public string? Attribution { get; private set; }

        /// <summary>
        /// URL used by the Web Map to access the tiles. This may be different than the specified URL.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tileUrl")]
        public string TileUrl { get; internal set; } = "data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw=="; //Defaulting to an empty image. This is a workaround for creating tile layers without an initial tile URL.

        /// <summary>
        /// A bounding box that specifies where tiles are available. When specified, no tiles outside of the bounding box will be requested.
        /// Note: This will not crop tiles to the specific bounding box, it limits the tiles it loads to those that intersect this bounding box.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bounds")]
        public BoundingBox? Bounds { get; private set; } = BoundingBox.Global();

        /// <summary>
        /// Specifies is the tile systems y coordinate uses the OSGeo Tile Map Services which reverses the Y coordinate axis. 
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("isTMS")]
        public bool? IsTMS { get; private set; } = false;

        /// <summary>
        /// An integer value that specifies the width and height dimensions of the map tiles. 
        /// For a seamless experience, the tile size must by a multiplier of 2. (i.e. 256, 512, 1024…).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tileSize")]
        public int? TileSize { get; private set; } = 512;

        /// <summary>
        /// An array of subdomain values to apply to the tile URL.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subdomains")]
        public IList<string>? Subdomains { get; private set; } = null;

        /// <summary>
        /// An integer specifying the maximum zoom level in which tiles are available from the tile source. 
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("maxSourceZoom")]
        public int? MaxSourceZoom { get; private set; } = 0;

        /// <summary>
        /// An integer specifying the minimum zoom level in which tiles are available from the tile source.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("minSourceZoom")]
        public int? MinSourceZoom { get; private set; } = 22;

        /// <summary>
        /// Specifies if the tile source points to vector tiles. If true, the tile source is a VectorTileSource.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("isVectorTiles")]
        public bool IsVectorTiles { get; private set; } = false;

        /// <summary>
        /// If the tile source represents Elevation tiles, this specifies the DEM tiles encoding format. 
        /// If this is null, tile source will be considered as a raster or vector tile source.
        /// Ignored if isVectorTiles is true.
        /// </summary>  
        [JsonInclude]
        [JsonPropertyName("encoding")]
        public ElevationEncoding? ElevationEncoding
        {
            get { 
                return _elevationEncoding; 
            }
            internal set { 
                _elevationEncoding = value; 

                if(value != null)
                {
                    IsVectorTiles = false;

                    if (!JsNamespace.Equals(Constants.ElevationTileSource))
                    {
                        JsNamespace = Constants.ElevationTileSource;
                    }
                } 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Try loading the tile source from a TileJSON file or URL. Will parse details such as min/max source zoom, bounds.
        /// Optionally override the tile URL.
        /// </summary>
        /// <param name="tileJsonFilePathOrUrl">file path or URL to where the TileJson file is located.</param>
        /// <param name="tileSize">The size of the tiles (TileJson schema doesn't currently provide this).</param>
        /// <param name="tileUrlOverride">A url to override the path to the actual tiles.</param>
        /// <returns></returns>
        public static async Task<TileSource?> FromTileJson(string tileJsonFilePathOrUrl, int tileSize = 512, string? tileUrlOverride = null)
        {
            var result = await Utils.TryGetFileStreamAsync(tileJsonFilePathOrUrl);

            if (result != null)
            {
                var tileJson = await JsonSerializer.DeserializeAsync<TileJson>(result.Stream);

                if (tileJson != null)
                {
                    tileJson.Validate();

                    if(tileJson.Tiles != null && tileJson.Tiles.Count > 0)
                    {
                        tileUrlOverride = tileJson.Tiles[0];
                    }

                    if (string.IsNullOrWhiteSpace(tileUrlOverride))
                    {
                        return null;
                    }

                    var tileSource = new TileSource(tileUrlOverride)
                    {
                        MinSourceZoom = tileJson.MinZoom,
                        MaxSourceZoom = tileJson.MaxZoom,
                        Bounds = tileJson.Bounds,
                        IsTMS = tileJson.Scheme == "tms",
                        TileSize = tileSize,
                        Attribution = tileJson.Attribution
                    };

                    return tileSource;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the TileSource to a TileJson object.
        /// </summary>
        /// <returns></returns>
        public TileJson ToTileJson() 
        {
            return new TileJson
            {
                Bounds = Bounds,
                MaxZoom = MaxSourceZoom,
                MinZoom = MinSourceZoom,
                Scheme = IsTMS == true ? "tms" : "xyz",
                Attribution = Attribution,
                Tiles = new string[] { TileUrl },
            };
        }

        /// <summary>
        /// If the source points to vector tiles (thus is a Vector tile source). This method will return all 
        /// GeoJSON features that are loaded into the source which satisfy the specified filter expression.
        /// </summary>
        /// <param name="sourceLayer">Specifies the layer within the VectorTileSource to query.</param>
        /// <param name="filter">A filter that will limit the query.</param>
        /// <returns>If vector tiles, returns list of feature. Otherwise returns an empty list.</returns>
        public async Task<IList<Feature>> GetRenderedShapes(string sourceLayer, Expression<bool>? filter = null)
        {
            if (Map != null && IsVectorTiles)
            {
                return await Map.Sources.GetRenderedShapes(Id, filter, sourceLayer);
            }

            return new List<Feature>(0);
        }

        /// <summary>
        /// Get the base source options.
        /// </summary>
        /// <returns></returns>
        public override BaseSourceOptions GetOptions()
        {
            return new BaseSourceOptions
            {
                MinZoom = MinSourceZoom,
                MaxZoom = MaxSourceZoom
            };
        }

        internal void Validate()
        {
            if (Bounds == null)
            {
                Bounds = BoundingBox.Global();
            }

            if (IsTMS == null)
            {
                IsTMS = false;
            }

            if (MinSourceZoom == null)
            {
                MinSourceZoom = 0;
            }

            if (MaxSourceZoom == null)
            {
                MaxSourceZoom = 22;
            }

            if (MinSourceZoom > MaxSourceZoom)
            {
                var z = MinSourceZoom;
                MinSourceZoom = MaxSourceZoom;
                MaxSourceZoom = z;
            }

            if(TileSize == null || TileSize <= 0)
            {
                TileSize = 512;
            }
        }

        #endregion
    }
}
