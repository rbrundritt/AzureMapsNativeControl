using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// Represents a collection of Polygons
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class MultiPolygon : Geometry, IEquatable<MultiPolygon>, IDeepCloneable<MultiPolygon>
    {
        #region Private Properties

        private ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> _coordinates;

        #endregion

        #region Constructor

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        public MultiPolygon(): base(GeoJsonType.MultiPolygon)
        {
            _coordinates = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons">Positions of the MultiPolygon</param>
        public MultiPolygon(ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> polygons, BoundingBox? bbox = null) :
            base(GeoJsonType.MultiPolygon, bbox)
        {
            //Ensure all rings are closed.
            foreach (var polygon in polygons)
            {
                foreach (var ring in polygon)
                {
                    ring.Close();
                }
            }

            _coordinates = polygons;
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(IEnumerable<IEnumerable<IEnumerable<GeoPosition>>> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(IEnumerable<IEnumerable<IEnumerable<Position>>> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(GeoArray<GeoArray<GeoArray<GeoPosition>>> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(IEnumerable<IEnumerable<GeoLineString>> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(IEnumerable<GeoPolygon> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(IEnumerable<IEnumerable<GeoLinearRing>> polygons, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(polygons), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        public MultiPolygon(GeoPolygonCollection polygons) :
            this(polygons.Coordinates, polygons.BoundingBox != null ? new BoundingBox(polygons.BoundingBox) : null)
        {
        }

        /// <summary>
        /// A GeoJson MultiPolygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="bbox"></param>
        public MultiPolygon(IEnumerable<Polygon> polygons, BoundingBox? bbox = null) : 
            this(PositionCollection.FromData(polygons))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the MultiPolygon.
        /// </summary>
        [JsonPropertyName("coordinates")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> Coordinates
        {
            get { return _coordinates; }
            set
            {
                if (value != null)
                {
                    if(_coordinates != null)
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
        /// Creates a deep clone of the MultiPolygon.
        /// </summary>
        /// <returns></returns>
        public override MultiPolygon DeepClone()
        {
            //Clone the coordinates.
            var coordinates = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var coords in Coordinates)
            {
                var positions = new ObservableRangeCollection<PositionCollection>();

                foreach (var pos in coords)
                {
                    positions.Add(pos.DeepClone());
                }

                coordinates.Add(positions);
            }

            return new MultiPolygon(coordinates, BoundingBox?.DeepClone());
        }

        /// <summary>
        /// Ensures the polygon is valid.
        /// - Outer rings are counter clockwise. Inner rings are clockwise.
        /// - Rings are closed.
        /// - Rings do not have less than 4 points.
        /// 
        /// The following checks are not currently implemented.
        /// - Rings do not intersect.
        /// - Rings do not have self intersections.
        /// </summary>
        public void MakeValid()
        {
            bool hasChanges = false;

            foreach (var polygon in Coordinates)
            {
                //Ensure each ring is closed.
                foreach (var ring in polygon)
                {
                    int len = ring.Count;

                    if (len < 4)
                    {
                        while (ring.Count < 4)
                        {
                            ring.Add(ring[0]);
                        }
                    }
                    else
                    {
                        ring.Close();
                    }

                    if (ring.Count != len)
                    {
                        hasChanges = true;
                    }
                }

                //Ensure the outer ring is counter clockwise.
                if (polygon.Count > 0)
                {
                    if (!polygon[0].IsCCW())
                    {
                        hasChanges = true;
                        polygon[0].Reverse();
                    }
                }

                //Ensure the inner rings are clockwise.
                for (int i = 1; i < polygon.Count; i++)
                {
                    if (!polygon[i].IsCCW())
                    {
                        hasChanges = true;
                        polygon[i].Reverse();
                    }
                }
            }

            if (hasChanges)
            {
                RaisePropertyChangedEvent("Coordinates");
            }
        }


        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(MultiPolygon? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as MultiPolygon));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(MultiPolygon? left, MultiPolygon? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <summary>
        /// Determines whether two specified features are the same.
        /// </summary>
        /// <param name="left">The first feature to compare.</param>
        /// <param name="right">The first feature to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MultiPolygon? left, MultiPolygon? right)
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
            return PositionCollection.AreEqual(left.Coordinates, right.Coordinates);
        }

        /// <summary>
        /// Determines whether two specified features don't have the same value.
        /// </summary>
        /// <param name="left">The first feature to compare.</param>
        /// <param name="right">The first feature to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(MultiPolygon? left, MultiPolygon? right)
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