using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// An observable GeoJson Feature object.
    /// </summary>
    [JsonConverter(typeof(FeatureConverter))]
    public class Feature : IGeoJsonObject, IEquatable<Feature>, IDeepCloneable<Feature>, INotifyPropertyChanged
    {
        #region Private Properties

        public PropertiesTable _properties;
        private Geometry _geometry;

        #endregion

        #region Internal Properties

        /// <summary>
        /// The ID of the source the feature is in.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        internal string? SourceId { get; set; }

        /// <summary>
        /// The name of the source layer that feature is contained in.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sourceLayer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        internal string? SourceLayer { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of a GeoJson Feature.
        /// </summary>
        /// <param name="geometry">GeoJson Geometry</param>
        /// <param name="properties">Properties of the feature.</param>
        /// <param name="id">Unique Id of the feature. Will try to maintain it, but if not unique, will overwrite. </param>
        /// <param name="bbox">Bounding box of the feature.</param>
        public Feature(Geometry? geometry = null, IDictionary<string, object?>? properties = null, string? id = null, BoundingBox? bbox = null)
        {
            _properties = properties != null ? new PropertiesTable(properties) : new PropertiesTable();
            _properties.CollectionChanged += Properties_CollectionChanged;

            Id = id ?? UniqueId.Get("Feature", Properties);

            _geometry = geometry ?? new PointGeometry(0,0);

            if (bbox != null)
            {
                _geometry.BoundingBox = bbox;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Event for when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged = null;

        /// <summary>
        /// The id of the feature.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; internal set; }

        /// <summary>
        /// The type of the feature.
        /// </summary>
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GeoJsonType Type => GeoJsonType.Feature;

        /// <summary>
        /// The geometry of the feature.
        /// </summary>
        [JsonPropertyName("geometry")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(GeometryConverter))]
        public Geometry Geometry
        {
            get { return _geometry; }
            set
            {
                if (value != null)
                {
                    _geometry = value;
                    RaisePropertyChangedEvent("Geometry");
                }
            }
        }

        /// <summary>
        /// Properties of the feature.
        /// </summary>

        [JsonPropertyName("properties")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public PropertiesTable Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                if (value != null)
                {
                    if (_properties != null)
                    {
                        _properties.CollectionChanged -= Properties_CollectionChanged;
                    }

                    _properties = value;

                    if (_properties != null)
                    {
                        _properties.CollectionChanged += Properties_CollectionChanged;
                    }

                    RaisePropertyChangedEvent("Properties");
                }
            }
        }

        /// <summary>
        /// The bounding box of the feature.
        /// </summary>
        [JsonPropertyName("bbox")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BoundingBox? BoundingBox
        {
            get { return Geometry?.BoundingBox; }
            set
            {
                if(Geometry != null)
                {
                    Geometry.BoundingBox = value;
                }
            }
        }

        #endregion

        #region Public Methods

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(Feature? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Feature));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Feature? left, Feature? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Id, Geometry, Properties);

        /// <summary>
        /// Determines whether two specified features are the same.
        /// </summary>
        /// <param name="left">The first feature to compare.</param>
        /// <param name="right">The first feature to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Feature? left, Feature? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Id.Equals(right.Id) &&
                left.Geometry.Equals(right.Geometry) &&
                left.Properties.Equals(right.Properties);
        }

        /// <summary>
        /// Determines whether two specified features don't have the same value.
        /// </summary>
        /// <param name="left">The first feature to compare.</param>
        /// <param name="right">The first feature to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(Feature? left, Feature? right)
        {
            return !(left == right);
        }

        #endregion

        /// <summary>
        /// Clones the feature. 
        /// Note, original Id is cloned as well. If you want a new Id, use the other Clone method.
        /// </summary>
        /// <returns></returns>
        public Feature DeepClone()
        {
            return DeepClone();
        }

        /// <summary>
        /// Clones the feature.
        /// </summary>
        /// <param name="regenerateId">If true, the feature Id will be regenerated.</param>
        /// <returns></returns>
        public Feature Clone(bool regenerateId)
        {
            return new Feature(Geometry.DeepClone(), Properties.DeepClone(), regenerateId? UniqueId.Get("Feature", Properties) : Id, BoundingBox?.DeepClone());
        }

        /// <summary>
        /// Calculates the bounding box of the feature.
        /// </summary>
        /// <returns></returns>
        public BoundingBox? CalculateBounds()
        {
            return Geometry?.CalculateBounds();
        }

        /// <summary>
        /// Converts the feature to a GeoObject.
        /// </summary>
        /// <returns></returns>
        public GeoObject? ToGeoObject()
        {
            return Geometry.ToGeoObject(Properties);
        }

        /// <summary>
        /// Converts the feature to a JSON representation.
        /// </summary>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Converts the feature to a JSON representation.
        /// </summary>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        /// <returns>The feature as JSON string.</returns>
        public string ToString(int? sigDigits)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonConverters.FeatureConverter.Write(writer, this, sigDigits);
                    writer.Flush();
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
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

        #region Public Static Methods

        /// <summary>
        /// Creates a Feature from a GeoObject.
        /// </summary>
        /// <param name="geoObject">The GeoObject to create a feature from.</param>
        /// <returns></returns>
        public static Feature? FromGeoObject(GeoObject geoObject, bool ensureUniqueID = true)
        {
            if (geoObject == null)
            {
                return null;
            }

            //Convert GeoObject to Json.
            var json = geoObject.ToString();

            //Convert Json to Feature.
            return JsonSerializer.Deserialize<Feature>(json);
        }

        /// <summary>
        /// Parses an instance of see <see cref="Feature"/> from provided JSON representation.
        /// </summary>
        /// <param name="json">The GeoJSON representation of an object.</param>
        /// <returns>The resulting <see cref="Feature"/> object.</returns>
        public static Feature? Parse(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            return FeatureConverter.Read(jsonDocument.RootElement);
        }

        /// <summary>
        /// Parses an instance of see <see cref="Feature"/> from provided JSON representation.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static bool TryParse(string json, out Feature feature)
        {
            var f = Parse(json);
            feature = f ?? new Feature();
            return f != null;
        }

        #endregion

        #region Azure Maps Shape methods

        /// <summary>
        /// Indicates if the specified feature is a circle, defined by the extended GeoJSON specification supported by Azure Maps.
        /// [Extended Spec]{@link https://docs.microsoft.com/en-us/azure/azure-maps/extend-geojson}
        /// </summary>
        /// <returns></returns>
        public bool IsCircle()
        {
            return _geometry.Type == GeoJsonType.Point &&
                _properties.TryGetValue<string>("subType", out string? subType) &&
                subType != null &&
                subType.ToLower().Equals("circle") &&
                _properties.ContainsKey("radius") &&
                Utils.IsNumber(_properties["radius"]);
        }

        /// <summary>
        /// If the shape is a circle, this gets its coordinates. Otherwise returns null.
        /// </summary>
        /// <returns></returns>
        public IList<Position>? GetCircleCoordinates()
        {
            if (IsCircle() && _geometry is PointGeometry point)
            {
                return AtlasMath.GetRegularPolygonPath(point.Coordinates,
                    _properties.GetDouble("radius", 1),
                    72,
                    DistanceUnits.Meters);
            }

            return null;
        }

        /// <summary>
        /// If the geometry is a Point, this sets the `subType` property to `Circle` and sets the radius property of the circle in meters.
        /// </summary>
        /// <param name="radius">The radius of the circle in the specified distance units.</param>
        /// <param name="units">Distance units the radius is in.</param>
        public void SetCircleRadius(double radius, DistanceUnits units = DistanceUnits.Meters)
        {
            if (_geometry.Type == GeoJsonType.Point)
            {
                _properties["subType"] = "Circle";
                _properties["radius"] = AtlasMath.ConvertDistance(radius, units, DistanceUnits.Meters);
            }
        }

        /// <summary>
        /// If the geometry is a Point, and represents a circle (has `subType` property set to `Circle` and radius property, retrieves the radius in the specified distance units.
        /// </summary>
        /// <param name="radius">The radius of the circle in the specified distance units.</param>
        /// <param name="units">Distance units to convert the units to.</param>
        /// <returns>True if feature represents a circle, otherwise false.</returns>
        public bool TryGetCircleRadius(out double radius, DistanceUnits units = DistanceUnits.Meters)
        {
            if (IsCircle())
            {
                radius = AtlasMath.ConvertDistance(_properties.GetDouble("radius", 1), DistanceUnits.Meters, units);
                return true;
            }

            radius = 0;
            return false;
        }

        /// <summary>
        /// Indicates if the specified feature is a rectangle, defined by the extended GeoJSON specification supported by Azure Maps.
        /// [Extended Spec]{@link https://docs.microsoft.com/en-us/azure/azure-maps/extend-geojson}
        /// </summary>
        /// <returns></returns>
        public bool IsRectangle()
        {
            if (_geometry.Type == GeoJsonType.Polygon &&
               _properties.TryGetValue<string>("subType", out string? subType) &&
               subType != null &&
               subType.ToLower().Equals("rectangle"))
            {
                if(_geometry is Polygon poly)
                {
                    var c = poly.Coordinates;
                    if (c.Count == 1)
                    {
                        var ring = c[0];
                        if (ring.Count == 5)
                        {
                            return ring[0].Equals(ring[4]);
                        }
                        else if (ring.Count == 4)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates if the specified feature is a cluster, defined by the extended GeoJSON specification supported by Azure Maps.
        /// </summary>
        /// <returns></returns>
        public bool IsCluster()
        {
            return Properties.TryGetValue<bool>("cluster", out bool isCluster) &&
                isCluster &&
                Properties.TryGetValue("clusterId", out int clusterId);
        }

        #endregion

        #region Private Methods

        private void Properties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent("Properties");
        }

        #endregion
    }
}
