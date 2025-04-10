using System.Text.Json;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// Constants used by this library.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Where in the RAW folder users can add their content for relative URLs.
        /// </summary>
        internal const string MapWebViewHybridAssetRoot = "map_resources";

        /// <summary>
        /// The constant key used to store the Azure Maps Shape ID in the properties of a MapElement.
        /// </summary>
        internal const string AzureMapsShapeID = "_azureMapsShapeId";

        /// <summary>
        /// The default minimum edge length of a bounding box.
        /// </summary>
        internal const double DefaultMinBboxEdgeLength = 0.001;

        /// <summary>
        /// The default json serialization options used by the map.
        /// </summary>
        internal static JsonSerializerOptions MapJsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,

            //Allow support for NaN, Infinity, and -Infinity
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        #region Atlas class namespaces

        internal const string ElevationTileSource = "atlas.source.ElevationTileSource";
        internal const string VectorTileSource = "atlas.source.VectorTileSource";

        #endregion

        internal const string PlainTextMimeType = "text/plain";

        #region Tile MimeTypes

        internal static string PNGMimeType = "image/png";
        internal static string JPEGMimeType = "image/jpeg";
        internal static string WEBPMimeType = "image/webp";
        internal static string PBFMimeType = "application/x-protobuf";
        internal static string MVTMimeType = "application/vnd.mapbox-vector-tile";

        #endregion

        #region Proxy Constants

        internal const string ProxyOperation = "/proxy?operation=";

        internal const string CustomTileSourceOperation = "customTileSource";
        internal const string ProxyWebRequestOperation = "proxyWebRequest";

        #endregion

        #region Mercator Map Constants

        internal const double MinLatitude = -85.05112878;
        internal const double MaxLatitude = 85.05112878;
        internal const double MinLongitude = -180;
        internal const double MaxLongitude = 180;

        #endregion

        #region GeoJson Constants

        internal static string[] LowerCaseFeatureGeometryTypes = { "feature", "point", "linestring", "polygon", "multipoint", "multilinestring", "multipolygon" };
        internal static string[] LowerCaseGeometryTypes = { "point", "linestring", "polygon", "multipoint", "multilinestring", "multipolygon" };

        internal const string TypeProperty = "type";
        internal const string FeaturesProperty = "features";
        internal const string GeometriesProperty = "geometries";

        internal const string FeatureCollectionType = "FeatureCollection";
        internal const string FeatureType = "Feature";
        internal const string PointType = "Point";
        internal const string LineStringType = "LineString";
        internal const string MultiPointType = "MultiPoint";
        internal const string PolygonType = "Polygon";
        internal const string MultiLineStringType = "MultiLineString";
        internal const string MultiPolygonType = "MultiPolygon";
        internal const string GeometryCollectionType = "GeometryCollection";

        internal const string IdProperty = "id";
        internal const string BBoxProperty = "bbox";
        internal const string GeometryProperty = "geometry";
        internal const string CoordinatesProperty = "coordinates";
        internal const string PropertiesProperty = "properties";

        #endregion

        #region JS Caches

        internal const string GenericCache = "itemCache";
        internal const string ControlCache = "controls";
        internal const string PopupCache = "popups";
        internal const string MarkerCache = "markers";

        #endregion
    }
}
