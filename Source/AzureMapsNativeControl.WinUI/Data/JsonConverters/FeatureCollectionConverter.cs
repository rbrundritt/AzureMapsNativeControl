using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Converts a GeoJson FeatureCollection object to and from JSON.
    /// Can write only GeoJson FeatureCollection objects.
    /// Can read the following GeoJson object (returned as a FeatureCollection): 
    /// FeatureCollection, Feature, Point, LineString, Polygon, MultiPoint, MultiLineString, MultiPolygon, GeometryCollection (each geometry will be a feature).
    /// </summary>
    public class FeatureCollectionConverter : JsonConverter<FeatureCollection>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(FeatureCollection).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override FeatureCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return Read(document.RootElement);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, FeatureCollection value, JsonSerializerOptions options)
        {
            Write(writer, value);
        }

        /// <summary>
        /// Writes the GeoJSON FeatureCollection to the JSON writer as a GeometryCollection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="featureCollection"></param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        public static void WriteGeometryCollection(Utf8JsonWriter writer, FeatureCollection featureCollection, int? sigDigits = null)
        {
            WriteAsGeometryCollection(writer, featureCollection, sigDigits);
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// Reads the GeoJSON object from the JSON element.
        /// Robust fallback reading of GeoJson objects. Looks for FeatureCollection, Feature, and Geometry objects, or an array of them.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static FeatureCollection Read(JsonElement element)
        {
            var features = new List<Feature>();
            BoundingBox? boundingBox = null;

            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var childElement in element.EnumerateArray())
                {
                    if (Utils.TryReadStringProperty(childElement, Constants.TypeProperty, out string typeValue))
                    {
                        ReadGeoJsonObjectAsFeature(typeValue, childElement, features);
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                if (Utils.TryReadStringProperty(element, Constants.TypeProperty, out string typeValue))
                {
                    if (typeValue.Equals(Constants.FeatureCollectionType, StringComparison.OrdinalIgnoreCase))
                    {
                        //Try and get the bounding box property.
                        boundingBox = BoundingBox.FromJsonElement(element);
                    }

                    ReadGeoJsonObjectAsFeature(typeValue, element, features);
                }
            }

            return new FeatureCollection(features, boundingBox);
        }

        private static void ReadGeoJsonObjectAsFeature(string typeValue, JsonElement element, List<Feature> features)
        {
            if (typeValue.Equals(Constants.FeatureCollectionType, StringComparison.OrdinalIgnoreCase))
            {
                var featuresElm = Utils.GetJsonElementProperty(element, Constants.FeaturesProperty, true);

                if (featuresElm.HasValue && featuresElm.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement featureElement in featuresElm.Value.EnumerateArray())
                    {
                        var f = FeatureConverter.Read(featureElement);

                        if (f != null)
                        {
                            features.Add(f);
                        }
                    }
                }
            }
            else if (Constants.LowerCaseFeatureGeometryTypes.Contains(typeValue.ToLowerInvariant()))
            {
                var f = FeatureConverter.Read(element);

                if (f != null)
                {
                    features.Add(f);
                }
            }
            else if (typeValue.Equals(Constants.GeometryCollectionType, StringComparison.OrdinalIgnoreCase))
            {
                var geometriesElm = Utils.GetJsonElementProperty(element, Constants.GeometriesProperty, true);

                if (geometriesElm.HasValue && geometriesElm.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var childElement in geometriesElm.Value.EnumerateArray())
                    {
                        var f = FeatureConverter.Read(childElement);

                        if (f != null)
                        {
                            features.Add(f);
                        }
                    }
                }
            }
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes the GeoJSON FeatureCollection to the JSON writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">FeatureCollection to write.</param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        internal static void Write(Utf8JsonWriter writer, FeatureCollection value, int? sigDigits = null)
        {
            writer.WriteStartObject();

            //Write the type
            writer.WritePropertyName(Constants.TypeProperty);
            writer.WriteStringValue(Constants.FeatureCollectionType);

            //Write the Features
            writer.WritePropertyName(Constants.FeaturesProperty);

            writer.WriteStartArray();

            foreach (var feature in value.Features) {
                FeatureConverter.Write(writer, feature, sigDigits);
            }

            writer.WriteEndArray();

            //Write bounding box
            BoundingBoxConverter.Write(writer, value.BoundingBox, true, sigDigits);

            //Finish the feature collection object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes the GeoJSON FeatureCollection to the JSON writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value">FeatureCollection to write.</param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        internal static void WriteAsGeometryCollection(Utf8JsonWriter writer, FeatureCollection value, int? sigDigits = null)
        {
            writer.WriteStartObject();

            //Write the type
            writer.WritePropertyName(Constants.TypeProperty);
            writer.WriteStringValue(Constants.GeometryCollectionType);

            //Write Geometries
            writer.WritePropertyName(Constants.GeometriesProperty);

            writer.WriteStartArray();

            foreach (var feature in value.Features)
            {
                GeometryConverter.Write(writer, feature.Geometry, sigDigits);
            }

            writer.WriteEndArray();

            //Finish the geometry collection object
            writer.WriteEndObject();
        }

        #endregion
    }
}