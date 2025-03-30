using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A GeoJson FeatureCollection object.
    /// </summary>
    [JsonConverter(typeof(FeatureCollectionConverter))]
    public class FeatureCollection : IGeoJsonObject, IDeepCloneable<FeatureCollection>
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of a GeoJson FeatureCollection.
        /// </summary>
        /// <param name="features">Collection of GeoJson features</param>
        /// <param name="bbox">Bounding box of the feature.</param>
        public FeatureCollection(IEnumerable<Feature>? features = null, BoundingBox? bbox = null)
        {
            Features = new List<Feature>(features ?? new List<Feature>());
            BoundingBox = bbox;
        }

        /// <summary>
        /// Creates a new instance of a GeoJson FeatureCollection.
        /// </summary>
        /// <param name="geoCollection"></param>
        public FeatureCollection(Azure.Core.GeoJson.GeoCollection geoCollection)
        {
            Features = new List<Feature>();
            foreach (var geoObject in geoCollection)
            {
                var f = Feature.FromGeoObject(geoObject);
                if (f != null)
                {
                    Features.Add(f);
                }
            }

            if (geoCollection.BoundingBox != null)
            {
                BoundingBox = new BoundingBox(geoCollection.BoundingBox);
            }
        }

        /// <summary>
        /// Creates a new instance of a GeoJson FeatureCollection.
        /// </summary>
        /// <param name="geoObjects"></param>
        /// <param name="bbox"></param>
        public FeatureCollection(IEnumerable<Azure.Core.GeoJson.GeoObject> geoObjects, BoundingBox? bbox = null)
        {
            Features = new List<Feature>();
            foreach (var geoObject in geoObjects)
            {
                var f = Feature.FromGeoObject(geoObject);
                if (f != null)
                {
                    Features.Add(f);
                }
            }

            BoundingBox = bbox;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of the feature collection.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GeoJsonType Type => GeoJsonType.FeatureCollection;

        /// <summary>
        /// Collection of features.
        /// </summary>
        [JsonPropertyName("features")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(FeatureEnumerableConverter))]
        public IList<Feature> Features { get; set; }

        /// <summary>
        /// The bounding box of the feature collection.
        /// </summary>
        [JsonPropertyName("bbox")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BoundingBox? BoundingBox { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of the feature collection.
        /// Note, original feature Ids are cloned as well. If you want a new Ids, use the other Clone method.
        /// </summary>
        /// <returns></returns>
        public FeatureCollection DeepClone()
        {
            return Clone(true);
        }

        /// <summary>
        /// Creates a deep clone of the feature collection.
        /// </summary>
        /// <param name="regenerateIds">If true, the feature Ids will be regenerated.</param>
        /// <returns></returns>
        public FeatureCollection Clone(bool regenerateIds)
        {
            var features = new List<Feature>();
            foreach (var feature in Features)
            {
                features.Add(feature.Clone(regenerateIds));
            }

            return new FeatureCollection(features, BoundingBox);
        }

        /// <summary>
        /// Calculates the bounding box of the feature collection.
        /// </summary>
        /// <returns></returns>
        public BoundingBox? CalculateBounds()
        {
            if (BoundingBox != null)
            {
                return BoundingBox.DeepClone();
            }

            BoundingBox = BoundingBox.FromData(this);

            return BoundingBox;
        }

        /// <summary>
        /// Converts the feature collection to a list of GeoObjects.
        /// </summary>
        /// <returns></returns>
        public Azure.Core.GeoJson.GeoCollection ToGeoObject()
        {
            var geoObjects = new List<Azure.Core.GeoJson.GeoObject>();
            foreach (var feature in Features)
            {
                var obj = feature.ToGeoObject();

                if (obj != null)
                {
                    geoObjects.Add(obj);
                }
            }

            return new Azure.Core.GeoJson.GeoCollection(geoObjects, BoundingBox?.ToGeoBoundingBox(), new Dictionary<string,object?>());
        }

        /// <summary>
        /// Converts the feature collection to a JSON representation.
        /// </summary>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Converts the feature collection to a JSON representation.
        /// </summary>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        /// <returns>The feature collection as JSON string.</returns>
        public string ToString(int? sigDigits)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonConverters.FeatureCollectionConverter.Write(writer, this, sigDigits);
                    writer.Flush();
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        /// <summary>
        /// Converts the feature collection to a JSON representation of a GeometryCollection.
        /// </summary>
        /// <param name="sigDigits"></param>
        /// <returns></returns>
        public string ToGeometryCollectionString(int? sigDigits = null)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonConverters.FeatureCollectionConverter.Write(writer, this, sigDigits);
                    writer.Flush();
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        /// <summary>
        /// Parses an instance of see <see cref="FeatureCollection"/> from provided JSON representation.
        /// </summary>
        /// <param name="json">The GeoJSON representation of an object.</param>
        /// <returns>The resulting <see cref="FeatureCollection"/> object.</returns>
        public static FeatureCollection Parse(string json)
        {
            if(string.IsNullOrWhiteSpace(json))
            {
                return new FeatureCollection();
            }

            using var jsonDocument = JsonDocument.Parse(json);
            return JsonConverters.FeatureCollectionConverter.Read(jsonDocument.RootElement);
        }

        /// <summary>
        /// Parses an instance of see <see cref="FeatureCollection"/> from provided JSON stream representation.
        /// </summary>
        /// <param name="stream">Stream containing UTF8 GeoJson data.</param>
        /// <returns></returns>
        public static FeatureCollection Parse(Stream? stream)
        {
            if (stream != null)
            {
                using var jsonDocument = JsonDocument.Parse(stream);
                return JsonConverters.FeatureCollectionConverter.Read(jsonDocument.RootElement);
            }

            return new FeatureCollection();
        }

        #endregion
    }
}
