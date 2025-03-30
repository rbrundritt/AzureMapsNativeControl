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
    /// A GeoJson Polygon geometry that represents an area on a map.
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class Polygon : Geometry, IEquatable<Polygon>, IDeepCloneable<Polygon>
    {
        #region Private Properties

        private ObservableRangeCollection<PositionCollection> _coordinates;

        #endregion

        #region Constructor

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        public Polygon(): base(GeoJsonType.Polygon)
        {
            _coordinates = new ObservableRangeCollection<PositionCollection>();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="outerRing">Positions of the outer ring of a polygon</param>
        public Polygon(PositionCollection? outerRing = null, BoundingBox? bbox = null) : base(GeoJsonType.Polygon, bbox)
        {
            var list = new List<PositionCollection>();

            if(outerRing != null)
            {
                outerRing.Close();
                list.Add(outerRing);
            }

            _coordinates = new ObservableRangeCollection<PositionCollection>(list);
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings">Positions of the rings of a polygon</param>
        public Polygon(ObservableRangeCollection<PositionCollection> rings, BoundingBox? bbox = null) : 
            base(GeoJsonType.Polygon, bbox)
        {
            foreach(var r in rings)
            {
                r.Close();
            }

            _coordinates = rings;
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings">Positions of the rings of a polygon</param>
        public Polygon(IEnumerable<PositionCollection> rings, BoundingBox? bbox = null) : this(new ObservableRangeCollection<PositionCollection>(rings), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="outerRing"></param>
        public Polygon(IEnumerable<Azure.Core.GeoJson.GeoPosition> outerRing, BoundingBox? bbox = null) : 
            this(new ObservableRangeCollection<PositionCollection>() { new PositionCollection(outerRing) }, bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygon"></param>
        public Polygon(Azure.Core.GeoJson.GeoLineString polygon) : 
            this(polygon.Coordinates, polygon.BoundingBox != null? new BoundingBox(polygon.BoundingBox): null)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="outerRing"></param>
        public Polygon(Azure.Core.GeoJson.GeoLinearRing outerRing, BoundingBox? bbox = null) : 
            this(outerRing.Coordinates, bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings"></param>
        public Polygon(IEnumerable<IEnumerable<Position>> rings, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(rings, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings"></param>
        public Polygon(IEnumerable<IEnumerable<Azure.Core.GeoJson.GeoPosition>> rings, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(rings, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings"></param>
        public Polygon(Azure.Core.GeoJson.GeoArray<Azure.Core.GeoJson.GeoArray<Azure.Core.GeoJson.GeoPosition>> rings, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(rings, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings"></param>
        public Polygon(IEnumerable<Azure.Core.GeoJson.GeoLineString> rings, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(rings, true), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="rings"></param>
        public Polygon(IEnumerable<Azure.Core.GeoJson.GeoLinearRing> rings, BoundingBox? bbox = null) : 
            this(PositionCollection.FromData(rings), bbox)
        {
        }

        /// <summary>
        /// A GeoJson Polygon geometry that represents an area on a map.
        /// </summary>
        /// <param name="polygon"></param>
        public Polygon(Azure.Core.GeoJson.GeoPolygon polygon) : 
            this(polygon.Coordinates, polygon.BoundingBox != null ? new BoundingBox(polygon.BoundingBox) : null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the Polygon.
        /// </summary>
        [JsonPropertyName("coordinates")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public ObservableRangeCollection<PositionCollection> Coordinates
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
        /// Creates a deep clone of the Polygon.
        /// </summary>
        /// <returns></returns>
        public override Polygon DeepClone()
        {
            //Clone the coordinates.
            var coords = new ObservableRangeCollection<PositionCollection>();

            foreach (var c in Coordinates)
            {
                coords.Add(c.DeepClone());
            }

            return new Polygon(coords, BoundingBox?.DeepClone());
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

            //Ensure each ring is closed.
            foreach (var c in Coordinates)
            {
                int len = c.Count;

                if (len < 4)
                {
                    while (c.Count < 4)
                    {
                        c.Add(c[0]);
                    }
                }
                else
                {
                    c.Close();
                }

                if (c.Count != len)
                {
                    hasChanges = true;
                }
            }

            //Ensure the outer ring is counter clockwise.
            if (Coordinates.Count > 0)
            {
                if (!Coordinates[0].IsCCW())
                {
                    hasChanges = true;
                    Coordinates[0].Reverse();
                }
            }

            //Ensure the inner rings are clockwise.
            for (int i = 1; i < Coordinates.Count; i++)
            {
                if (!Coordinates[i].IsCCW())
                {
                    hasChanges = true;
                    Coordinates[i].Reverse();
                }
            }

            if (hasChanges)
            {
                RaisePropertyChangedEvent("Coordinates");
            }
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(Polygon? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Polygon));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Polygon? left, Polygon? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <inheritdoc />
        public static bool operator ==(Polygon? left, Polygon? right)
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

        /// <inheritdoc />
        public static bool operator !=(Polygon? left, Polygon? right)
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
