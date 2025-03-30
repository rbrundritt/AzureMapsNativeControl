using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// A custom JSON converter for serializing and deserializing GeoJSON geometries.
    /// </summary>
    public class GeometryConverter : JsonConverter<Geometry>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Geometry).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override Geometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return Read(document.RootElement);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
        {
            Write(writer, value);
        }

        /// <summary>
        /// Writes a collection of GeoJSON geometry objects to the JSON writer as a GeometryCollection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        public static void WriteGeometryCollection(Utf8JsonWriter writer, IEnumerable<Geometry> value, int? sigDigits = null)
        {
            //Write the type
            writer.WritePropertyName(Constants.TypeProperty);
            writer.WriteStringValue(Constants.GeometryCollectionType);

            //Write Geometries
            writer.WritePropertyName(Constants.GeometriesProperty);

            writer.WriteStartArray();

            foreach (var geometry in value)
            {
                Write(writer, geometry);
            }

            writer.WriteEndArray();

            //Finish the geometry collection object
            writer.WriteEndObject();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the GeoJSON object from the JSON element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static Geometry? Read(JsonElement element)
        {
            if (Utils.TryReadStringProperty(element, Constants.TypeProperty, out string type))
            {
                BoundingBox? boundingBox = BoundingBox.FromJsonElement(element);

                var coordinates = Utils.GetJsonElementProperty(element, Constants.CoordinatesProperty, true);

                if (coordinates.HasValue)
                {
                    switch (type)
                    {
                        case Constants.PointType:
                            var p = Position.FromJsonElement(coordinates.Value);

                            if (p != null)
                            {
                                return new PointGeometry(p, boundingBox);
                            }
                            break;
                        case Constants.LineStringType:
                            var linePositions = PositionCollection.FromJsonElement(coordinates.Value);

                            if (linePositions != null)
                            {
                                return new LineString(linePositions, boundingBox);
                            }
                            break;
                        case Constants.MultiPointType:
                            var points = PositionCollection.FromJsonElement(coordinates.Value);

                            if (points != null)
                            {
                                return new MultiPoint(points, boundingBox);
                            }
                            break;
                        case Constants.PolygonType:
                            var rings = new List<PositionCollection>();
                            foreach (JsonElement ringArray in coordinates.Value.EnumerateArray())
                            {
                                var ring = PositionCollection.FromJsonElement(ringArray);
                                if (ring != null)
                                {
                                    rings.Add(ring);
                                }
                            }

                            if (rings.Count > 0)
                            {
                                return new Polygon(rings, boundingBox);
                            }
                            break;
                        case Constants.MultiLineStringType:
                            var lines = new List<PositionCollection>();
                            foreach (JsonElement ringArray in coordinates.Value.EnumerateArray())
                            {
                                var ring = PositionCollection.FromJsonElement(ringArray);
                                if (ring != null)
                                {
                                    lines.Add(ring);
                                }
                            }

                            if (lines.Count > 0)
                            {
                                return new MultiLineString(lines, boundingBox);
                            }
                            break;
                        case Constants.MultiPolygonType:
                            var polygons = new List<List<PositionCollection>>();
                            foreach (JsonElement polygon in coordinates.Value.EnumerateArray())
                            {
                                var polygonRings = new List<PositionCollection>();
                                foreach (JsonElement ringArray in polygon.EnumerateArray())
                                {
                                    var ring = PositionCollection.FromJsonElement(ringArray);
                                    if (ring != null)
                                    {
                                        polygonRings.Add(ring);
                                    }
                                }

                                if (polygonRings.Count > 0)
                                {
                                    polygons.Add(polygonRings);
                                }
                            }

                            if (polygons.Count > 0)
                            {
                                return new MultiPolygon(polygons, boundingBox);
                            }
                            break;
                        default:                           
                            break;
                    }
                }

                //Check to see if it is a GeometryCollection.
                else if (type.Equals("GeometryCollection", StringComparison.InvariantCultureIgnoreCase))
                {
                    var geometriesElm = Utils.GetJsonElementProperty(element, "geometries", true);

                    if (geometriesElm.HasValue)
                    {
                        var geometries = new List<Geometry>();

                        foreach (var geomElm in geometriesElm.Value.EnumerateArray())
                        {
                            var geom = Read(geomElm);

                            if (geom == null)
                            {
                                return null;
                            }

                            geometries.Add(geom);
                        }

                        //Try and convert geometries into a Multi-geometry type if possible.
                        return Geometry.TryGetSingleGeometry(geometries);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the GeoJSON Geometry to the JSON writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry">Geometry to write.</param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        internal static void Write(Utf8JsonWriter writer, Geometry geometry, int? sigDigits = null)
        {
            void WriteType(string type)
            {
                writer.WriteString(Constants.TypeProperty, type);
            }
                        
            void WritePositions(IEnumerable<Position> positions)
            {
                writer.WriteStartArray();
                foreach (var position in positions)
                {
                    PositionConverter.WritePosition(writer, position, sigDigits);
                }

                writer.WriteEndArray();
            }

            //Start writting the geometry.
            writer.WriteStartObject();
            switch (geometry)
            {
                case PointGeometry point:
                    WriteType(Constants.PointType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    PositionConverter.WritePosition(writer, point.Coordinates, sigDigits);
                    break;
                case LineString lineString:
                    WriteType(Constants.LineStringType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    WritePositions(lineString.Coordinates);
                    break;
                case Polygon polygon:
                    WriteType(Constants.PolygonType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    writer.WriteStartArray();
                    foreach (var ring in polygon.Coordinates)
                    {
                        WritePositions(ring);
                    }

                    writer.WriteEndArray();
                    break;
                case MultiPoint multiPoint:
                    WriteType(Constants.MultiPointType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    writer.WriteStartArray();
                    foreach (var point in multiPoint.Coordinates)
                    {
                        PositionConverter.WritePosition(writer, point, sigDigits);
                    }

                    writer.WriteEndArray();
                    break;
                case MultiLineString multiLineString:
                    WriteType(Constants.MultiLineStringType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    writer.WriteStartArray();
                    foreach (var lineString in multiLineString.Coordinates)
                    {
                        WritePositions(lineString);
                    }

                    writer.WriteEndArray();
                    break;
                case MultiPolygon multiPolygon:
                    WriteType(Constants.MultiPolygonType);
                    writer.WritePropertyName(Constants.CoordinatesProperty);
                    writer.WriteStartArray();
                    foreach (var polygon in multiPolygon.Coordinates)
                    {
                        writer.WriteStartArray();
                        foreach (var polygonRing in polygon)
                        {
                            WritePositions(polygonRing);
                        }
                        writer.WriteEndArray();
                    }

                    writer.WriteEndArray();
                    break;
                default:
                    throw new NotSupportedException($"Geometry type '{geometry?.GetType()}' not supported");
            }

            BoundingBoxConverter.Write(writer, geometry.BoundingBox, true, sigDigits);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes the properties value to the GeoJSON object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException"></exception>
        private static void WriteAdditionalPropertyValue(Utf8JsonWriter writer, object? value)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case int i:
                    writer.WriteNumberValue(i);
                    break;
                case double d:
                    writer.WriteNumberValue(d);
                    break;
                case float f:
                    writer.WriteNumberValue(f);
                    break;
                case long l:
                    writer.WriteNumberValue(l);
                    break;
                case string s:
                    writer.WriteStringValue(s);
                    break;
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;
                case IEnumerable<KeyValuePair<string, object?>> enumerable:
                    writer.WriteStartObject();
                    foreach (KeyValuePair<string, object?> pair in enumerable)
                    {
                        writer.WritePropertyName(pair.Key);
                        WriteAdditionalPropertyValue(writer, pair.Value);
                    }
                    writer.WriteEndObject();
                    break;
                case IEnumerable<object?> objectEnumerable:
                    writer.WriteStartArray();
                    foreach (object? item in objectEnumerable)
                    {
                        WriteAdditionalPropertyValue(writer, item);
                    }
                    writer.WriteEndArray();
                    break;

                default:
                    throw new NotSupportedException("Not supported type " + value.GetType());
            }
        }

        #endregion
    }
}