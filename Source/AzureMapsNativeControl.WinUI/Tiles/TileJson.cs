using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;

namespace AzureMapsNativeControl.Tiles
{
    /// <summary>
    /// A tile json object that describes a tile layer.
    /// </summary>
    public class TileJson: IDeepCloneable<TileJson>
    {
        #region Public Properties

        /// <summary>
        /// REQUIRED. A semver.org style version number. Describes the version of
        /// the TileJSON spec that is implemented by this JSON object.
        /// </summary>
        [JsonPropertyName("tilejson")]
        public string? TileJsonVersion { get; set; } = "3.0.0";

        /// <summary>
        /// OPTIONAL. Default: null. A name describing the tileset. The name can
        /// contain any legal character. Implementations SHOULD NOT interpret the
        /// name as HTML.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// OPTIONAL. Default: null. A text description of the tileset. The
        /// description can contain any legal character. Implementations SHOULD NOT
        /// interpret the description as HTML.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// OPTIONAL. Default: null. Contains an attribution to be displayed
        /// when the map is shown to a user. Implementations MAY decide to treat this
        /// as HTML or literal text. For security reasons, make absolutely sure that
        /// this field can't be abused as a vector for XSS or beacon tracking.
        /// </summary>
        [JsonPropertyName("attribution")]
        public string? Attribution { get; set; }

        /// <summary>
        /// OPTIONAL. Default: null. Contains a legend to be displayed with the map.
        /// Implementations MAY decide to treat this as HTML or literal text.
        /// For security reasons, make absolutely sure that this field can't be
        /// abused as a vector for XSS or beacon tracking.
        /// </summary>
        [JsonPropertyName("legend")]
        public string? Legend { get; set; }

        /// <summary>
        /// OPTIONAL. Default: "1.0.0". A semver.org style version number. When
        /// changes across tiles are introduced, the minor version MUST change.
        /// This may lead to cut off labels. Therefore, implementors can decide to
        /// clean their cache when the minor version changes. Changes to the patch
        /// level MUST only have changes to tiles that are contained within one tile.
        /// When tiles change significantly, the major version MUST be increased.
        /// Implementations MUST NOT use tiles with different major versions.
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; } = "1.0.0";

        /// <summary>
        /// OPTIONAL. Default: "xyz". Either "xyz" or "tms". Influences the y
        /// direction of the tile coordinates.
        /// The global-mercator (aka Spherical Mercator) profile is assumed.
        /// </summary>
        [JsonPropertyName("scheme")]
        public string? Scheme { get; set; } = "xyz";

        /// <summary>
        /// REQUIRED. An array of tile endpoints. {z}, {x} and {y}, if present,
        /// are replaced with the corresponding integers. If multiple endpoints are specified, clients
        /// may use any combination of endpoints. All endpoints MUST return the same
        /// content for the same URL. The array MUST contain at least one endpoint.
        /// </summary>
        [JsonPropertyName("tiles")]
        public IList<string> Tiles = new List<string>();

        /// <summary>
        /// OPTIONAL. Default: 0. >= 0, <= 24.
        /// An integer specifying the minimum zoom level.
        /// </summary>
        [JsonPropertyName("minzoom")]
        public int? MinZoom { get; set; } = 0;

        /// <summary>
        /// OPTIONAL. Default: 22. >= 0, <= 24.
        /// An integer specifying the maximum zoom level. MUST be >= minzoom.
        /// </summary>
        [JsonPropertyName("maxzoom")]
        public int? MaxZoom { get; set; } = 22;

        /// <summary>
        /// OPTIONAL. Array. Default: Global Bounding Box (xyz-compliant tile bounds)
        /// The maximum extent of available map tiles. Bounds MUST define an area covered by all zoom levels.
        /// The bounds are represented in WGS 84 latitude and longitude values, in the order left, bottom, 
        /// right, top. Values may be integers or floating point numbers. The minimum/maximum values for 
        /// longitude and latitude are -180/180 and -90/90 respectively.Bounds MUST NOT "wrap" around the 
        /// ante-meridian. If bounds are not present, the default value MAY assume the set of tiles is 
        /// globally distributed.
        /// </summary>
        [JsonPropertyName("bounds")]
        public BoundingBox? Bounds { get; set; } = BoundingBox.Global();

        /// <summary>
        /// OPTIONAL. Integer. Default: null.
        /// An integer specifying the zoom level from which to generate overzoomed tiles.
        /// Implementations MAY generate overzoomed tiles from parent tiles if the requested 
        /// zoom level does not exist. In most cases, overzoomed tiles are generated from the 
        /// maximum zoom level of the set of tiles. If fillzoom is specified, the overzoomed 
        /// tile MAY be generated from the fillzoom level.
        /// For example, in a set of tiles with maxzoom 10 and no fillzoom specified, a request 
        /// for a z11 tile will use the z10 parent tiles to generate the new, overzoomed z11 tile.
        /// If the same TileJSON object had fillzoom specified at z7, a request for a z11 tile 
        /// would use the z7 tile instead of z10.
        /// While TileJSON may specify rules for overzooming tiles, it is ultimately up to the 
        /// tile serving client or renderer to implement overzooming.
        /// </summary>
        [JsonPropertyName("fillzoom")]
        public int? FillZoom { get; set; }

        /// <summary>
        /// OPTIONAL. Array. Default: null.
        /// The first value is the longitude, the second is latitude(both in WGS:84 values), 
        /// the third value is the zoom level as an integer. Longitude and latitude MUST be 
        /// within the specified bounds. The zoom level MUST be between minzoom and maxzoom.
        /// Implementations MAY use this center value to set the default location. If the 
        /// value is null, implementations MAY use their own algorithm for determining a default location.
        /// </summary>
        [JsonPropertyName("center")]
        public Position? Center { get; set; }

        /// <summary>
        /// An array of objects. Each object describes one layer of vector tile data. A vector_layer object MUST 
        /// contain the id and fields keys, and MAY contain the description, minzoom, or maxzoom keys. An 
        /// implementation MAY include arbitrary keys in the object outside those defined in this specification.
        /// 
        /// Note: When describing a set of raster tiles or other tile format that does not have a "layers" concept
        /// (i.e. "format": "jpeg"), the vector_layers key is not required.
        /// </summary>
        [JsonPropertyName("vector_layers")]
        public IList<TileJsonVectorLayer>? VectorLayers { get; set;} = new List<TileJsonVectorLayer>();

        #endregion

        #region Methods

        /// <summary>
        /// Validates the tile json object and sets default values where needed.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TileJsonVersion))
            {
                TileJsonVersion = "3.0.0";
            }

            if (Bounds == null)
            {
                Bounds = BoundingBox.Global();
            }

            if (Center == null)
            {
                Center = Bounds.GetCenter();
                Center.Altitude = MinZoom;
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                Version = "1.0.0";
            }

            if (string.IsNullOrWhiteSpace(Scheme))
            {
                Scheme = "xyz";
            }

            if(MinZoom == null)
            {
                MinZoom = 0;
            }

            if(MaxZoom == null)
            {
                MaxZoom = 22;
            }

            if(MinZoom > MaxZoom)
            {
                var z = MinZoom;
                MinZoom = MaxZoom;
                MaxZoom = z;
            }

            if(FillZoom == null)
            {
                FillZoom = MaxZoom;
            }

            if(FillZoom != null && FillZoom < MinZoom)
            {
                FillZoom = MinZoom;
            }

            if (Tiles == null || Tiles.Count == 0)
            {
                Tiles = new List<string>();
            }

            if(VectorLayers != null && VectorLayers.Count > 0)
            {
                foreach(var layer in VectorLayers)
                {
                    if (string.IsNullOrWhiteSpace(layer.Id))
                    {
                        layer.Id = "layer" + VectorLayers.IndexOf(layer);
                    }

                    if (layer.Fields == null || layer.Fields.Count == 0)
                    {
                        layer.Fields = new Dictionary<string, string>();
                    }

                    if(layer.MinZoom == null || layer.MinZoom < MinZoom)
                    {
                        layer.MinZoom = MinZoom;
                    }

                    if (layer.MaxZoom == null || layer.MaxZoom > MaxZoom)
                    {
                        layer.MaxZoom = MaxZoom;
                    }
                }
            }
        }

        /// <summary>
        /// Merges the properties of another tile json object with this one.
        /// </summary>
        /// <param name="otherTileJson"></param>
        public void Merge(TileJson otherTileJson)
        {
            if(string.IsNullOrWhiteSpace(Name)) { Name = otherTileJson.Name; }
            if(string.IsNullOrWhiteSpace(Description)) { Description = otherTileJson.Description; }
            if(string.IsNullOrWhiteSpace(Attribution)) { Attribution = otherTileJson.Attribution; }
            if(string.IsNullOrWhiteSpace(Legend)) { Legend = otherTileJson.Legend; }
            if(string.IsNullOrWhiteSpace(Version)) { Version = otherTileJson.Version; }
            if(string.IsNullOrWhiteSpace(Scheme)) { Scheme = otherTileJson.Scheme; }

            if (Tiles == null || Tiles.Count == 0)
            {
                Tiles = otherTileJson.Tiles;
            }
            else if(otherTileJson.Tiles != null && otherTileJson.Tiles.Count > 0)
            {
                foreach (var tile in otherTileJson.Tiles)
                {
                    if (!Tiles.Contains(tile))
                    {
                        Tiles.Add(tile);
                    }
                }
            }

            if (Bounds == null)
            {
                Bounds = otherTileJson.Bounds;
            } 
            else if(otherTileJson.Bounds != null)
            {
                Bounds.Merge(otherTileJson.Bounds);
            }

            if (otherTileJson.MinZoom < MinZoom)
            {
                MinZoom = otherTileJson.MinZoom;
            }

            if (otherTileJson.MaxZoom > MaxZoom)
            {
                MaxZoom = otherTileJson.MaxZoom;
            }

            if (otherTileJson.VectorLayers != null && otherTileJson.VectorLayers.Count > 0)
            {
                if (VectorLayers == null || VectorLayers.Count > 0)
                {
                    VectorLayers = otherTileJson.VectorLayers;
                }
                else
                {
                    var newLayers = new List<TileJsonVectorLayer>();

                    foreach (var vl in otherTileJson.VectorLayers)
                    {
                        bool hasLayer = false;

                        foreach (var vl2 in VectorLayers)
                        {
                            //Check to see if layer already exists. If it does, update zoom info.
                            if (vl.Id == vl2.Id)
                            {
                                if (vl.MinZoom != null)
                                {
                                    if (vl2.MinZoom == null)
                                    {
                                        vl2.MinZoom = vl.MinZoom;
                                    }
                                    else
                                    {
                                        vl2.MinZoom = Math.Min(vl.MinZoom.Value, vl2.MinZoom.Value);
                                    }
                                }

                                if (vl.MaxZoom != null)
                                {
                                    if (vl2.MaxZoom == null)
                                    {
                                        vl2.MaxZoom = vl.MaxZoom;
                                    }
                                    else
                                    {
                                        vl2.MaxZoom = Math.Max(vl.MaxZoom.Value, vl2.MaxZoom.Value);
                                    }
                                }

                                if (vl.Fields != null)
                                {
                                    if (vl2.Fields == null)
                                    {
                                        vl2.Fields = vl.Fields;
                                    }
                                    else
                                    {
                                        foreach (var field in vl.Fields)
                                        {
                                            if (!vl2.Fields.ContainsKey(field.Key))
                                            {
                                                vl2.Fields.Add(field.Key, field.Value);
                                            }
                                        }
                                    }
                                }
                                hasLayer = true;
                                break;
                            }
                        }

                        if (!hasLayer)
                        {
                            VectorLayers.Add(vl);
                        }
                    }
                }
            }

            Validate();
        }

        /// <summary>
        /// Gets a MBTiles insert command for the tileset.
        /// https://github.com/mapbox/mbtiles-spec/blob/master/1.3/spec.md
        /// </summary>
        /// <param name="format">The file format of the tile data: pbf, jpg, png, webp, or an IETF media type for other formats.</param>
        /// <returns></returns>
        internal string GetMBTileInsertCmd(string format = "pbf")
        {
            Validate();

            var cmd = new StringBuilder();
            cmd.AppendFormat("BEGIN TRANSACTION;INSERT INTO metadata (name, value) VALUES ('name', '{0}');", Name ?? "tiles");
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('format', '{0}');", format);
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('version', {0});", Version ?? "1.0.0");
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('minzoom', '{0}');", MinZoom ?? 0);
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('maxzoom', '{0}');", MaxZoom ?? 22);

            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('bounds', '{0}');", Bounds?.ToString().Replace("[", "").Replace("]", ""));
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('center', '{0},{1},{2}');", Center?.Longitude, Center?.Latitude, Center?.Altitude);

            if (VectorLayers != null && VectorLayers.Count > 0)
            {
                cmd.Append("INSERT INTO metadata (name, value) VALUES ('json', '{\"vector_layers\":[");
                cmd.Append(JsonSerializer.Serialize(VectorLayers));
                cmd.Append("]}');");
            }
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('attribution', '{0}');", Attribution ?? "");
            cmd.AppendFormat("INSERT INTO metadata (name, value) VALUES ('description', '{0}');", Description ?? "");

            cmd.Append("COMMIT;");

            return cmd.ToString();
        }

        /// <inheritdoc/>
        public TileJson DeepClone()
        {
            var clone = new TileJson
            {
                TileJsonVersion = TileJsonVersion,
                Name = Name,
                Description = Description,
                Attribution = Attribution,
                Legend = Legend,
                Version = Version,
                Scheme = Scheme,
                Tiles = new List<string>(Tiles),
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Bounds = Bounds?.DeepClone(),
                FillZoom = FillZoom,
                Center = Center?.DeepClone(),
                VectorLayers = VectorLayers?.Select(vl => vl.DeepClone()).ToList()
            };

            return clone;
        }

        #endregion
    }

    /// <summary>
    /// A tile json object that describes a vector layer.
    /// </summary>
    public class TileJsonVectorLayer: IDeepCloneable<TileJsonVectorLayer>
    {
        #region Public Properties

        /// <summary>
        /// REQUIRED. String.
        /// A string value representing the layer id. For added context, this is referred to as the name 
        /// of the layer in the Mapbox Vector Tile spec.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// OPTIONAL. Default: null. A text description of the data layer. The
        /// description can contain any legal character. Implementations SHOULD NOT
        /// interpret the description as HTML.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// OPTIONAL. Default: 0. >= 0, <= 24.
        /// An integer specifying the minimum zoom level this data layer is available for.
        /// </summary>
        [JsonPropertyName("minzoom")]
        public int? MinZoom { get; set; } = 0;

        /// <summary>
        /// OPTIONAL. Default: 22. >= 0, <= 24.
        /// An integer specifying the maximum zoom level this data layer is available for. MUST be >= minzoom.
        /// </summary>
        [JsonPropertyName("maxzoom")]
        public int? MaxZoom { get; set; } = 22;

        /// <summary>
        /// REQUIRED. Object.
        /// An object whose keys and values are the names and descriptions of attributes available in this layer.
        /// Each value(description) MUST be a string that describes the underlying data. If no fields are present, 
        /// the fields key MUST be an empty object.
        /// </summary>
        [JsonPropertyName("fields")]
        public IDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public TileJsonVectorLayer DeepClone()
        {
            return new TileJsonVectorLayer
            {
                Id = Id,
                Description = Description,
                MinZoom = MinZoom,
                MaxZoom = MaxZoom,
                Fields = new Dictionary<string, string>(Fields)
            };
        }

        #endregion
    }
}
