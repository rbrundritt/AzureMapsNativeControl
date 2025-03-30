using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// Represents a GeoJson Point.
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class PointGeometry : Geometry, IEquatable<PointGeometry>, IDeepCloneable<PointGeometry>
    {
        #region Private Properties

        private Position _coordinates;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of a GeoJson Point.
        /// </summary>
        public PointGeometry() : base(GeoJsonType.Point)
        {
            _coordinates = new Position(0, 0);
        }

        /// <summary>
        /// Creates a new instance of a GeoJson Point.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bbox"></param>
        public PointGeometry(Position? position = null, BoundingBox? bbox = null) : base(GeoJsonType.Point, bbox)
        {
            _coordinates = position ?? new Position(0, 0); 
        }

        /// <summary>
        /// Creates a new instance of a GeoJson Point.
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="bbox"></param>
        public PointGeometry(double longitude, double latitude, BoundingBox? bbox = null) : base(GeoJsonType.Point, bbox)
        {
            _coordinates = new Position(longitude, latitude);
        }

        /// <summary>
        /// Creates a new instance of a GeoJson Point.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bbox"></param>
        public PointGeometry(Azure.Core.GeoJson.GeoPosition position, BoundingBox? bbox = null) : base(GeoJsonType.Point, bbox)
        {
            _coordinates = new Position(position);
        }

        /// <summary>
        /// Creates a new instance of a GeoJson Point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="bbox"></param>
        public PointGeometry(Azure.Core.GeoJson.GeoPoint point, BoundingBox? bbox = null) : base(GeoJsonType.Point, bbox)
        {
            _coordinates = new Position(point);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the Point.
        /// </summary>
        [JsonPropertyName("coordinates")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public Position Coordinates
        {
            get { return _coordinates; }
            set
            {
                if (value != null)
                {
                    _coordinates = value;
                    _bbox = null;
                    RaisePropertyChangedEvent("Coordinates");
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of the PointGeometry.
        /// </summary>
        /// <returns></returns>
        public override PointGeometry DeepClone()
        {
            return new PointGeometry(Coordinates.DeepClone(), BoundingBox?.DeepClone());
        }

        /// <summary>
        /// Creates a new position from an array of coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static PointGeometry FromArray(IList<double> coordinates)
        {
            if (coordinates.Count < 2)
            {
                throw new ArgumentException("Coordinates must have at least 2 values");
            }

            if (coordinates.Count == 2)
            {
                return new PointGeometry(coordinates[0], coordinates[1]);
            }
            
            return new PointGeometry(Position.FromArray(coordinates));
        }

        /// <summary>
        /// Creates a new position from a latitude and longitude.
        /// </summary>
        /// <param name="latitude">Latitude value</param>
        /// <param name="longitude">Longitude value</param>
        /// <param name="altitude">Altitude value</param>
        /// <returns></returns>
        public static PointGeometry FromLatLng(double latitude, double longitude, double? altitude = null)
        {
            return new PointGeometry(new Position(longitude, latitude, altitude));
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(PointGeometry? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as PointGeometry));
        }

        /// <inheritdoc />
        public bool Equals(PointGeometry? left, PointGeometry? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <inheritdoc />
        public static bool operator ==(PointGeometry? left, PointGeometry? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Coordinates.Equals(right.Coordinates);
        }

        /// <inheritdoc />
        public static bool operator !=(PointGeometry? left, PointGeometry? right)
        {
            return !(left == right);
        }

        #endregion


        #endregion
    }
}
