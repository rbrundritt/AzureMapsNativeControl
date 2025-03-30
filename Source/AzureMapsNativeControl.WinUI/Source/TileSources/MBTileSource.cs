using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Tiles;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// A tile source that reads tiles from an MBTiles file.
    /// MBTile files can contain raster or vector tiles.
    /// Supported raster tile formats: png, jpg, webp.
    /// Supported vector tile format: pbf.
    /// </summary>
    public class MBTileSource : CustomTileSource, IDisposable
    {
        //Maui Sqlite documentation: https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/database-sqlite?view=net-maui-8.0

        #region Private Properties

        private SQLiteAsyncConnection _conn;
        private MBTileMetadata _metadata;

        /// <summary>
        /// Sqlite open flags for the connection. Optimize for parallel reading.
        /// </summary>
        private const SQLiteOpenFlags SqliteOpenFlags =
            //Open the database in read only mode
            SQLiteOpenFlags.ReadOnly |
            //Enable multi-threaded database access
            SQLiteOpenFlags.SharedCache |
            //Open in multi-threading mode.
            SQLiteOpenFlags.NoMutex;

        /// <summary>
        /// SQL query statement for retrieving a tile from the MBTiles file.
        /// Input: Zoom, Y, X
        /// </summary>
        private const string TileSqlQuery = "SELECT tile_data FROM \"tiles\" WHERE zoom_level=? AND tile_row=? AND tile_column=?;";

        /// <summary>
        /// SQL query statement for retrieving metadata value from the MBTiles file.
        /// Input: property name.
        /// </summary>
        private const string MetadataSqlQuery = "SELECT \"value\" FROM metadata WHERE \"name\"=?;";

        #endregion

        #region Constructor

        /// <summary>
        /// A tile source that reads tiles from an MBTiles file.
        /// MBTile files can contain raster or vector tiles.
        /// Supported raster tile formats: png, jpg, webp.
        /// Supported vector tile format: pbf.
        /// </summary>
        /// <param name="conn">Sqlite connection.</param>
        /// <param name="metadata">Metadata about the MBTile file.</param>
        /// <param name="elevationEncoding"></param>
        internal MBTileSource(
            SQLiteAsyncConnection conn,
            MBTileMetadata metadata,
            ElevationEncoding? elevationEncoding = null) :
            base(metadata.IsVectorTiles, 
                metadata.TileSize, 
                metadata.TileJson.Bounds, 
                metadata.TileJson.MinZoom,
                metadata.TileJson.MaxZoom, 
                true, 
                !metadata.IsVectorTiles ? elevationEncoding : null)
        {
            _conn = conn;
            _metadata = metadata;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tries to load a MBTile file and setup to be used as a tile source. If the file path points to the app package, the file will be copied to local app data storage.
        /// </summary>
        /// <param name="mbTileFilePath">File path to the MBTile file</param>
        /// <param name="tileSize">Size of tiles. Will try and extract information from MBTile file. Vector tiles default to 512, raster to 256.</param>
        /// <param name="elevationEncoding">If the tiles are RGB elevation tiles, this is the encoding.</param>
        /// <returns>A MBTileSource instance or null if unable to find/read MBTile file.</returns>
        public static async Task<MBTileSource?> TryLoadAsync(string mbTileFilePath, int? tileSize = null, ElevationEncoding? elevationEncoding = null)
        {
            try
            {
                SQLiteAsyncConnection? conn = null;

                if (conn == null)
                {
                    var localFilePath = await Utils.TryFindLocalFilePathAsync(mbTileFilePath);

                    if (!string.IsNullOrEmpty(localFilePath))
                    {
#if MAUI
                        //Check to see if the file exists in the app package.
                        if (await FileSystem.AppPackageFileExistsAsync(mbTileFilePath))
                        {                
                            //Need to copy the file to local app data storage as Sqlite can't access Raw folder.
                            var localPath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(mbTileFilePath));

                            //Need to copy the file to local app data storage as Sqlite can't access Raw folder.
                            using (var asset = await FileSystem.OpenAppPackageFileAsync(mbTileFilePath))
                            {
                                using (var file = File.Create(localPath))
                                {
                                    asset.CopyTo(file);
                                }
                            }

                            conn = new SQLiteAsyncConnection(localPath, SqliteOpenFlags);
                        }
#else

                        conn = new SQLiteAsyncConnection(localFilePath, SqliteOpenFlags);
#endif
                    }
                }

                //Try and get metadata from the MBTiles file.
                if (conn != null)
                {
                    var metadata = await ReadMBTileMetadata(conn);

                    if (metadata != null)
                    {
                        return new MBTileSource(conn, metadata);
                    }
                }
            } 
            catch(Exception ex)
            {
                Debug.WriteLine("Error loading MBTileSource: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Tries to load a MBTile file and setup to be used as a tile source.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tileSize">Size of tiles. Will try and extract information from MBTile file. Vector tiles default to 512, raster to 256.</param>
        /// <param name="elevationEncoding">If the tiles are RGB elevation tiles, this is the encoding.</param>
        /// <returns>A MBTileSource instance or null if unable to find/read MBTile file.</returns>
        public static async Task<MBTileSource?> TryLoadAsync(SQLiteConnectionString connectionString, int? tileSize = null, ElevationEncoding? elevationEncoding = null)
        {
            try
            { 
                var conn = new SQLiteAsyncConnection(connectionString);

                if (conn != null)
                {
                    var metadata = await ReadMBTileMetadata(conn, tileSize);

                    if (metadata != null)
                    {
                        return new MBTileSource(conn, metadata);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading MBTileSource: " + ex.Message);
            }

            return null;
        }

        /// <inheritdoc/>
        public new TileJson ToTileJson()
        {
           return _metadata.TileJson.DeepClone();
        }

        /// <inheritdoc/>
        public override async Task<MapFileStream?> GetTileStream(TileInfo tileInfo)
        {
            var result = await _conn.ExecuteScalarAsync<byte[]>(TileSqlQuery, tileInfo.Zoom, tileInfo.Y, tileInfo.X).ConfigureAwait(false);

            if(result != null && result.Length > 0)
            {
                return new MapFileStream(new MemoryStream(result), _metadata.MimeType);
            }

            return null;
        }

        /// <inheritdoc/>
        public async void Dispose()
        {
            if (_conn != null)
            {
                try
                {
                    //Close the connection when disposed.
                    await _conn.CloseAsync();
                }
                catch { }
            }
        }

#endregion

        #region Private Methods

        private static async Task<MBTileMetadata?> ReadMBTileMetadata(SQLiteAsyncConnection conn, int? ts = null)
        {
            var tileJson = new TileJson()
            {
                //MBTiles files are always TMS.
                Scheme = "tms"
            };

            //Need to try and read individual properties from the metadata table.
            //This will allow us to handle multiple MBTiles file schema versions.

            //Determine the mimeType of the tiles. Default to PNG.
            var format = await ReadString(conn, "format");

            if(string.IsNullOrWhiteSpace(format))
            {
                //The format is a required property. If it is null, then most likely invalid MBTile file or unable to query it.
                return null;
            }

            string mimeType = Constants.PNGMimeType;

            if (!Utils.TryGetMimeType(format, out mimeType))
            {
                mimeType = Constants.PNGMimeType;
            }

            //Determine if the tiles are vector tiles.
            bool isVectorTiles = false;

            if (mimeType.Equals("application/x-protobuf", StringComparison.InvariantCultureIgnoreCase) ||
                mimeType.Equals("application/vnd.mapbox-vector-tile", StringComparison.InvariantCultureIgnoreCase))
            {
                isVectorTiles = true;
            }

            //Determine the min/max zoom range of the tiles.
            var zoomMin = await ReadInt(conn, "minzoom");

            //If min zoom property is not set, look at the tiles table and find the min zoom level.
            if (zoomMin == null)
            {
                zoomMin = await conn.ExecuteScalarAsync<int>("SELECT MIN(zoom_level) FROM tiles;");
            }

            var zoomMax = await ReadInt(conn, "maxzoom");
            
            //If max zoom property is not set, look at the tiles table and find the max zoom level.
            if (zoomMax == null)
            {
                zoomMax = await conn.ExecuteScalarAsync<int>("SELECT MAX(zoom_level) FROM tiles;");
            }

            tileJson.MinZoom = Math.Max(zoomMin ?? 0, 0);
            tileJson.MaxZoom = Math.Min(zoomMax ?? 22, 24);

            //Determine the bounding box of the tiles.
            tileJson.Bounds = await ReadBounds(conn);

            //Try get the default view information of the tiles.
            var centerString = await ReadString(conn, "center");

            if (!string.IsNullOrWhiteSpace(centerString))
            {
                var components = centerString.Split(',');

                if(components.Length >= 3 &&
                    double.TryParse(components[0], out double lon) &&
                    double.TryParse(components[1], out double lat) &&
                    double.TryParse(components[2], out double zoom))
                {
                    tileJson.Center = new Position(lon, lat, zoom);
                }
            }

            tileJson.Attribution = await ReadString(conn, "attribution");
            tileJson.Description = await ReadString(conn, "description");
            tileJson.Name = await ReadString(conn, "name");
            tileJson.Version = await ReadString(conn, "version");
            
            //Try get vector tile information.
            var jsonString = await ReadString(conn, "json");

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    var vt = JsonSerializer.Deserialize<MBVectorTileJson>(jsonString);

                    if(vt != null)
                    {
                        tileJson.VectorLayers = vt.VectorLayers;
                    }
                } 
                catch (Exception ex)
                {
                    Debug.WriteLine("MBTileSource - Error parsing vector tile json: " + ex.Message);
                }
            }

            //Try to determine tile size. Might be included in metadata as an extensions.
            int? tileSize = isVectorTiles ? 512 : await ReadInt(conn, "tilesize");
            
            if (tileSize == null) {
                tileSize = await ReadInt(conn, "tileSize");

                if (tileSize == null)
                {
                    tileSize = await ReadInt(conn, "tile_size");

                    if(tileSize == null)
                    {
                       tileSize = ts ?? 256;
                    }
                }
            }

            return new MBTileMetadata(tileJson, mimeType, isVectorTiles, tileSize);
        }

        private static async Task<string?> ReadString(SQLiteAsyncConnection conn, string name)
        {
            try
            {
                return await conn.ExecuteScalarAsync<string>(MetadataSqlQuery, name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<int?> ReadInt(SQLiteAsyncConnection conn, string name)
        {
            try
            {
                return await conn.ExecuteScalarAsync<int?>(MetadataSqlQuery, name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<BoundingBox> ReadBounds(SQLiteAsyncConnection conn)
        {
            try
            {

                var extentString = await conn.ExecuteScalarAsync<string>(MetadataSqlQuery, "bounds");
                var components = extentString.Split(',');
                return new BoundingBox(
                    double.Parse(components[0], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[1], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[2], NumberFormatInfo.InvariantInfo),
                    double.Parse(components[3], NumberFormatInfo.InvariantInfo)
                );

            }
            catch (Exception)
            {
                return BoundingBox.Global();
            }
        }

        #endregion
    }

    /// <summary>
    /// Metadata about an MBTiles file.
    /// </summary>
    internal class MBTileMetadata
    {
        public MBTileMetadata(TileJson tileJson, string mimeType, bool isVectorTiles, int? tileSize)
        {
            TileJson = tileJson;
            MimeType = mimeType;
            IsVectorTiles = isVectorTiles;
            TileSize = tileSize;
        }

        public TileJson TileJson { get; set; }

        public string MimeType { get; set; }

        public bool IsVectorTiles { get; set; } = false;

        public int? TileSize { get; set; }
    }

    /// <summary>
    /// Helper class for deserialization.
    /// </summary>
    internal class MBVectorTileJson
    {
        [JsonPropertyName("vector_layers")]
        public List<TileJsonVectorLayer>? VectorLayers { get; set; }
    }
}
