using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    [JsonDerivedType(typeof(PointGeometry))]
    [JsonDerivedType(typeof(LineString))]
    [JsonDerivedType(typeof(Polygon))]
    [JsonDerivedType(typeof(MultiPoint))]
    [JsonDerivedType(typeof(MultiLineString))]
    [JsonDerivedType(typeof(MultiPolygon))]
    [JsonConverter(typeof(GeometryConverter))]
    /// <summary>
    /// Abstract class that all GeoJson geometry objects inherit from.
    /// </summary>  
    public abstract class Geometry : IGeoJsonObject, IEquatable<Geometry>, IDeepCloneable<Geometry>, INotifyPropertyChanged
    {
        #region Private Properties

        internal static ReadOnlyDictionary<string, object?> DefaultGeoObjectProperties = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());

        internal BoundingBox? _bbox = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of a Geometry.
        /// </summary>
        /// <param name="geometryType"></param>
        public Geometry(GeoJsonType geometryType, BoundingBox? bbox = null)
        {
            _bbox = bbox;
            Type = geometryType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Event for when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged = null;

        /// <summary>
        /// The type of the geometry.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GeoJsonType Type { get; private set; }

        /// <summary>
        /// The bounding box of the geometry.
        /// </summary>
        [JsonPropertyName("bbox")]
        public BoundingBox? BoundingBox
        {
            get { return _bbox; }
            set
            {
                _bbox = value;
                RaisePropertyChangedEvent("BoundingBox");
            }
        }

        #endregion

        #region Public Methods



        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(Geometry? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Geometry));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Geometry? left, Geometry? right)
        {
            return (left == right);
        }

        /// <summary>
        /// Determines whether two specified geometries are the same.
        /// </summary>
        /// <param name="left">The first geometry to compare.</param>
        /// <param name="right">The first geometry to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Geometry? left, Geometry? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            if(left.Type != right.Type)
            {
                return false;
            }

            switch (left.Type)
            {
                case GeoJsonType.Point:
                    return (left as PointGeometry) == (right as PointGeometry);
                case GeoJsonType.MultiPoint:
                    return (left as MultiPoint) == (right as MultiPoint);
                case GeoJsonType.LineString:
                    return (left as LineString) == (right as LineString);
                case GeoJsonType.MultiLineString:
                    return (left as MultiLineString) == (right as MultiLineString);
                case GeoJsonType.Polygon:
                    return (left as Polygon) == (right as Polygon);
                case GeoJsonType.MultiPolygon:
                    return (left as MultiPolygon) == (right as MultiPolygon);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether two specified geometries don't have the same value.
        /// </summary>
        /// <param name="left">The first geometry to compare.</param>
        /// <param name="right">The first geometry to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(Geometry? left, Geometry? right)
        {
            return !(left == right);
        }

        #endregion

        /// <summary>
        /// Creates a deep clone of the geometry.
        /// </summary>
        /// <returns></returns>
        public abstract Geometry DeepClone();

        /// <summary>
        /// Converts the geometry to a GeoObject.
        /// </summary>
        /// <returns></returns>
        public GeoObject? ToGeoObject(IDictionary<string, object?>? properties = null)
        {
            var props = new ReadOnlyDictionary<string, object?>(properties ?? new Dictionary<string, object?>());
            var bbox = BoundingBox?.ToGeoBoundingBox();

            switch (Type)
            {
                case GeoJsonType.Point:
                    return new GeoPoint(((PointGeometry)this).Coordinates.ToGeoPosition(), bbox, props);
                case GeoJsonType.MultiPoint:
                    return new GeoPointCollection(PositionCollection.ToGeoPoints(((MultiPoint)this).Coordinates), bbox, props);
                case GeoJsonType.LineString:
                    return new GeoLineString(PositionCollection.ToGeoPositions(((LineString)this).Coordinates), bbox, props);
                case GeoJsonType.MultiLineString:
                    return new GeoLineStringCollection(PositionCollection.ToGeoLineStrings(((MultiLineString)this).Coordinates), bbox, props);
                case GeoJsonType.Polygon:
                    return new GeoPolygon(PositionCollection.ToLinearRings(((Polygon)this).Coordinates), bbox, props);
                case GeoJsonType.MultiPolygon:
                    var polygons = new List<GeoPolygon>();
                    var mp = this as MultiPolygon;

                    foreach (var coords in mp.Coordinates)
                    {
                        polygons.Add(new GeoPolygon(PositionCollection.ToLinearRings(coords)));
                    }
                    return new GeoPolygonCollection(polygons, bbox, props);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Calculates the bounding box of the geometry.
        /// If the geometry has a bounding box set, it will return that.
        /// Otherwise it will calculate the bounding box based on the geometry.
        /// </summary>
        /// <returns></returns>
        public BoundingBox? CalculateBounds()
        {
            if (_bbox != null)
            {
                return _bbox.DeepClone();
            }

            _bbox = BoundingBox.FromData(this);

            return _bbox;
        }

        /// <summary>
        /// Converts the Geometry to a JSON representation.
        /// </summary>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Converts the Geometry to a JSON representation.
        /// </summary>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        /// <returns>The Geometry as JSON string.</returns>
        public string ToString(int? sigDigits)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonConverters.GeometryConverter.Write(writer, this, sigDigits);
                    writer.Flush();
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        /// <summary>
        /// Parses an instance of see <see cref="Geometry"/> from provided JSON representation.
        /// </summary>
        /// <param name="json">The GeoJSON representation of an object.</param>
        /// <returns>The resulting <see cref="Geometry"/> object.</returns>
        public static Geometry? Parse(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            return JsonConverters.GeometryConverter.Read(jsonDocument.RootElement);
        }

        /// <summary>
        /// Parses an instance of see <see cref="Geometry"/> from provided JSON representation.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static bool TryParse(string json, out Geometry geometry)
        {
            var g = Parse(json);
            geometry = g ?? new PointGeometry();
            return g != null;
        }

        /// <summary>
        /// Tries to get a single geometry from a list of geometries.
        /// If there is more than one geometry, will check to see if they are of liked types, 
        /// and if they are, will return a Multi-geometry type (MultiPoint, MultiLineString, MultiPolygon).
        /// </summary>
        /// <param name="geometries"></param>
        /// <returns>A single geometry or null.</returns>
        public static Geometry? TryGetSingleGeometry(IList<Geometry> geometries)
        {
            if (geometries.Count == 1)
            {
                return geometries[0];
            }
            else if (geometries.Count > 1)
            {
                //Get the type of the first geometry.
                var type = geometries[0].Type;

                //Check if all geometries are of the same type. (PointGeometry/MultiPoint, LineString/MultiLineString, Polygon/MultiPolygon).
                if (geometries.All(g => g.Type == type))
                {
                    switch (type)
                    {
                        case GeoJsonType.Point:
                        case GeoJsonType.MultiPoint:
                            var mp = new MultiPoint();

                            foreach (var g in geometries)
                            {
                                if (g is PointGeometry p)
                                {
                                    mp.Coordinates.Add(p.Coordinates);
                                }
                                else if (g is MultiPoint mp2)
                                {
                                    mp.Coordinates.AddRange(mp2.Coordinates);
                                }
                            }

                            return mp;
                        case GeoJsonType.LineString:
                        case GeoJsonType.MultiLineString:
                            var mls = new MultiLineString();

                            foreach (var g in geometries)
                            {
                                if (g is LineString ls)
                                {
                                    mls.Coordinates.Add(ls.Coordinates);
                                }
                                else if (g is MultiLineString mls2)
                                {
                                    mls.Coordinates.AddRange(mls2.Coordinates);
                                }
                            }

                            return mls;
                        case GeoJsonType.Polygon:
                        case GeoJsonType.MultiPolygon:
                            var mpoly = new MultiPolygon();

                            foreach (var g in geometries)
                            {
                                if (g is Polygon poly)
                                {
                                    mpoly.Coordinates.Add(poly.Coordinates);
                                }
                                else if (g is MultiPolygon mpoly2)
                                {
                                    mpoly.Coordinates.AddRange(mpoly2.Coordinates);
                                }
                            }

                            return mpoly;
                        default:
                            break;
                    }
                }
            }

            return null;
        }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        #endregion
    }
}