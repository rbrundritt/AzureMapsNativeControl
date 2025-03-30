using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A GeoJson LineString geometry that represents a collection of positions that create a line.
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class LineString : Geometry, IEquatable<LineString>, IDeepCloneable<LineString>
    {
        #region Private Properties

        private PositionCollection _coordinates;

        #endregion

        #region Constructor

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        public LineString() : base(GeoJsonType.LineString)
        {
            _coordinates = new PositionCollection();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="path">Positions of the line string</param>
        public LineString(PositionCollection path, BoundingBox? bbox = null) : 
            base(GeoJsonType.LineString, bbox)
        {
            _coordinates = path ?? new PositionCollection();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
            BoundingBox = bbox;
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="positions"></param>
        public LineString(IEnumerable<Position> positions, BoundingBox? bbox = null) : 
            this(new PositionCollection(positions), bbox)
        {
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="positions"></param>
        public LineString(IEnumerable<Azure.Core.GeoJson.GeoPosition> positions, BoundingBox? bbox = null) : 
            this(new PositionCollection(positions), bbox)
        {
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="points"></param>
        public LineString(IEnumerable<Azure.Core.GeoJson.GeoPoint> points, BoundingBox? bbox = null) : 
            this(new PositionCollection(points), bbox)
        {
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="path"></param>
        public LineString(Azure.Core.GeoJson.GeoLineString path) : 
            this(path.Coordinates, path.BoundingBox != null ? new BoundingBox(path.BoundingBox): null)
        {           
        }

        /// <summary>
        /// A GeoJson LineString geometry that represents a collection of positions that create a line.
        /// </summary>
        /// <param name="ring"></param>
        public LineString(Azure.Core.GeoJson.GeoLinearRing ring, BoundingBox? bbox = null) : 
            this(ring.Coordinates, bbox)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the LineString.
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
        /// Creates a deep clone of the LineString.
        /// </summary>
        /// <returns></returns>
        public override LineString DeepClone()
        {
            return new LineString(Coordinates.DeepClone(), BoundingBox?.DeepClone());
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(LineString? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as LineString));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Feature? left, Feature? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <inheritdoc />
        public static bool operator ==(LineString? left, LineString? right)
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
        public static bool operator !=(LineString? left, LineString? right)
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
