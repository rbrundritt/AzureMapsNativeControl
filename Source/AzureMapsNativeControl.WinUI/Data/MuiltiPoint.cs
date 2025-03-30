using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A MultiPoint Polygon geometry that represents an area on a map.
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class MultiPoint: Geometry, IEquatable<MultiPoint>, IDeepCloneable<MultiPoint>
    {
        private PositionCollection _coordinates;

        #region Constructor

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        public MultiPoint(): base(GeoJsonType.MultiPoint)
        {
            _coordinates = new PositionCollection();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        /// <param name="path">Positions of the line string</param>
        public MultiPoint(PositionCollection path, BoundingBox? bbox = null) : 
            base(GeoJsonType.MultiPoint, bbox)
        {
            _coordinates = path;
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        /// <param name="positions"></param>
        public MultiPoint(IEnumerable<Position> positions, BoundingBox? bbox = null) : 
            this(new PositionCollection(positions), bbox)
        {
        }

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        /// <param name="positions"></param>
        public MultiPoint(IEnumerable<Azure.Core.GeoJson.GeoPosition> positions, BoundingBox? bbox = null) : 
            this(new PositionCollection(positions), bbox)
        {
        }

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        /// <param name="points"></param>
        public MultiPoint(IEnumerable<Azure.Core.GeoJson.GeoPoint> points, BoundingBox? bbox = null) : 
            this(new PositionCollection(points), bbox)
        {
        }

        /// <summary>
        /// A MultiPoint GeoJson Geometry.
        /// </summary>
        /// <param name="path"></param>
        public MultiPoint(Azure.Core.GeoJson.GeoPointCollection path) : 
            this(path.Coordinates, path.BoundingBox != null ? new BoundingBox(path.BoundingBox) : null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the MultiPoint.
        /// </summary>
        [JsonPropertyName("coordinates")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public PositionCollection Coordinates
        {
            get { return _coordinates; }
            set
            {
                if (value != null)
                {
                    if (_coordinates != null)
                    {
                        _coordinates.CollectionChanged -= Coordinates_CollectionChanged;
                    }

                    _coordinates = value;
                    _coordinates.CollectionChanged += Coordinates_CollectionChanged;

                    _bbox = null;
                    RaisePropertyChangedEvent("Coordinates");
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of the MultiPoint.
        /// </summary>
        /// <returns></returns>
        public override MultiPoint DeepClone()
        {
            return new MultiPoint(Coordinates.DeepClone(), BoundingBox?.DeepClone());
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(MultiPoint? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as MultiPoint));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(MultiPoint? left, MultiPoint? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <inheritdoc />
        public static bool operator ==(MultiPoint? left, MultiPoint? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            //Compare the coordinates.
            return left.Coordinates.Equals(right.Coordinates);
        }

        /// <inheritdoc />
        public static bool operator !=(MultiPoint? left, MultiPoint? right)
        {
            return !(left == right);
        }

        #endregion

        #endregion

        #region Private Methods

        private void Coordinates_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _bbox = null;
            RaisePropertyChangedEvent("Coordinates");
        }

        #endregion
    }
}
