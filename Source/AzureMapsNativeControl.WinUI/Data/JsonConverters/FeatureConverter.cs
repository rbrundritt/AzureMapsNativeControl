using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Json converter for GeoJson Feature data.
    /// Can write only GeoJson Feature objects.
    /// Can read the following GeoJson object (returned as a Feature): 
    /// Feature, Point, LineString, Polygon, MultiPoint, MultiLineString, MultiPolygon.
    /// </summary>
    public class FeatureConverter : JsonConverter<Feature>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Feature).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override Feature? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return Read(document.RootElement);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Feature value, JsonSerializerOptions options)
        {
            Write(writer, value);
        }

        /// <summary>
        /// Writes a collection of GeoJSON feature objects to the JSON writer as a GeometryCollection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="features"></param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        public static void WriteGeometryCollection(Utf8JsonWriter writer, IEnumerable<Feature> features, int? sigDigits = null)
        {
            //Write the type
            writer.WritePropertyName(Constants.TypeProperty);
            writer.WriteStringValue(Constants.GeometryCollectionType);

            //Write Geometries
            writer.WritePropertyName(Constants.GeometriesProperty);

            writer.WriteStartArray();

            foreach (var feature in features)
            {
                GeometryConverter.Write(writer, feature.Geometry, sigDigits);
            }

            writer.WriteEndArray();

            //Finish the geometry collection object
            writer.WriteEndObject();
        }

        #endregion

        #region Read Methods

        /// <summary>
        /// Reads the GeoJSON object from the JSON element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static Feature? Read(JsonElement element)
        {
            if (Utils.TryReadStringProperty(element, Constants.TypeProperty, out string type))
            {
                if (type.Equals(Constants.FeatureType, StringComparison.OrdinalIgnoreCase))
                {
                    var geomElm = Utils.GetJsonElementProperty(element, Constants.GeometryProperty, true);

                    if (geomElm.HasValue)
                    {
                        var geom = GeometryConverter.Read(geomElm.Value);

                        if (geom == null)
                        {
                            return null;
                        }

                        BoundingBox? boundingBox = BoundingBox.FromJsonElement(element);

                        if (boundingBox != null && geom.BoundingBox == null)
                        {
                            geom.BoundingBox = boundingBox;
                        }

                        //Try and get the ID property.
                        string? id = Utils.GetJsonElementProperty(element, Constants.IdProperty, true)?.ToString();

                        //Try and get properties. 
                        IDictionary<string, object?>? properties = null;

                        if (element.TryGetProperty(Constants.PropertiesProperty, out JsonElement propertiesElement))
                        {
                            properties = PropertiesTableConverter.ReadProperties(propertiesElement, true);
                        }

                        var f = new Feature(geom, properties, id, boundingBox);

                        //Look for internal data.
                        if (Utils.TryReadStringProperty(element, "source", out string sourceId))
                        {
                            f.SourceId = sourceId;
                        }

                        if (Utils.TryReadStringProperty(element, "sourceLayer", out string sourceLayer))
                        {
                            f.SourceLayer = sourceLayer;
                        }

                        return f;
                    }
                }
                else if (Constants.LowerCaseGeometryTypes.Contains(type.ToLowerInvariant())) //Check to see if it is a GeoJson Geometry object.
                {
                    //Allow geometries JSON objects to be read as Feartures.
                    var geom = GeometryConverter.Read(element);

                    if (geom != null)
                    {
                        return new Feature(geom);
                    }
                }
                else if (!type.Equals("FeatureCollection", StringComparison.OrdinalIgnoreCase))
                {
                    //Allow geometries JSON objects to be read as Features.
                    var geom = GeometryConverter.Read(element);

                    if (geom != null)
                    {
                        return new Feature(geom);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes the GeoJSON Feature to the JSON writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="feature">Feature to write.</param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        internal static void Write(Utf8JsonWriter writer, Feature feature, int? sigDigits = null)
        {
            writer.WriteStartObject();

            //Write the type
            writer.WritePropertyName(Constants.TypeProperty);
            writer.WriteStringValue(Constants.FeatureType);

            //Write ID
            if (!string.IsNullOrWhiteSpace(feature.Id))
            {
                writer.WritePropertyName(Constants.IdProperty);
                writer.WriteStringValue(feature.Id);
            }

            //Write the Geometry
            writer.WritePropertyName(Constants.GeometryProperty);
            GeometryConverter.Write(writer, feature.Geometry, sigDigits);

            //Write bounding box
            BoundingBoxConverter.Write(writer, feature.BoundingBox, true, sigDigits);

            //Write properties
            writer.WritePropertyName(Constants.PropertiesProperty);
            PropertiesTableConverter.WriteProperties(writer, feature.Properties);

            //Finish the feature object
            writer.WriteEndObject();
        }

        #endregion
    }
}