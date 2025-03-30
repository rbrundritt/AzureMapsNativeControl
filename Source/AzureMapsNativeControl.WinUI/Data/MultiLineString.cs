using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A GeoJson MultiLineString geometry that represents lines on a map.
    /// </summary>
    [JsonConverter(typeof(GeometryConverter))]
    public class MultiLineString : Geometry, IEquatable<MultiLineString>, IDeepCloneable<MultiLineString>
    {
        #region Private Properties

        private ObservableRangeCollection<PositionCollection> _coordinates;

        #endregion

        #region Constructor

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        public MultiLineString(): base(GeoJsonType.MultiLineString)
        {
            _coordinates = new ObservableRangeCollection<PositionCollection>();
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="line">Positions of the single line</param>
        public MultiLineString(PositionCollection? line = null, BoundingBox? bbox = null) : base(GeoJsonType.MultiLineString, bbox)
        {
            var list = new List<PositionCollection>();

            if (line != null)
            {
                list.Add(line);
            }

            _coordinates = new ObservableRangeCollection<PositionCollection>(list);
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines">Positions of the MultiLineString</param>
        public MultiLineString(ObservableRangeCollection<PositionCollection> lines, BoundingBox? bbox = null) : 
            base(GeoJsonType.MultiLineString, bbox)
        {
            _coordinates = lines;
            _coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines">Positions of the MultiLineString</param>
        public MultiLineString(IEnumerable<PositionCollection> lines, BoundingBox? bbox = null) : 
            this(new ObservableRangeCollection<PositionCollection>(lines), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="rings"></param>
        /// <param name="bbox"></param>
        public MultiLineString(IEnumerable<IEnumerable<Position>> rings, BoundingBox? bbox = null) :
           this(PositionCollection.FromData(rings, false), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="line"></param>
        public MultiLineString(IEnumerable<Azure.Core.GeoJson.GeoPosition> line, BoundingBox? bbox = null) : 
            this(new PositionCollection(line), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="line"></param>
        public MultiLineString(Azure.Core.GeoJson.GeoLineString line) : 
            this(line.Coordinates, line.BoundingBox != null ? new BoundingBox(line.BoundingBox) : null)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="line"></param>
        public MultiLineString(Azure.Core.GeoJson.GeoLinearRing line, BoundingBox? bbox = null) : 
            this(line.Coordinates, bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines"></param>
        public MultiLineString(IEnumerable<IEnumerable<Azure.Core.GeoJson.GeoPosition>> lines, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(lines, false), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines"></param>
        public MultiLineString(Azure.Core.GeoJson.GeoArray<Azure.Core.GeoJson.GeoArray<Azure.Core.GeoJson.GeoPosition>> lines, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(lines, false), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines"></param>
        public MultiLineString(IEnumerable<Azure.Core.GeoJson.GeoLineString> lines, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(lines, false), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines"></param>
        public MultiLineString(IEnumerable<Azure.Core.GeoJson.GeoLinearRing> lines, BoundingBox? bbox = null) :
            this(PositionCollection.FromData(lines), bbox)
        {
        }

        /// <summary>
        /// A GeoJson MultiLineString geometry that represents lines on a map.
        /// </summary>
        /// <param name="lines"></param>
        public MultiLineString(Azure.Core.GeoJson.GeoLineStringCollection lines) : 
            this(lines.Coordinates, lines.BoundingBox != null ? new BoundingBox(lines.BoundingBox) : null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The coordinates of the MultiLineString.
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
        /// Creates a deep clone of the MultiLineString.
        /// </summary>
        /// <returns></returns>
        public override MultiLineString DeepClone()
        {
            //Clone the coordinates.
            var coords = new ObservableRangeCollection<PositionCollection>();

            foreach (var c in Coordinates)
            {
                coords.Add(c.DeepClone());
            }

            return new MultiLineString(coords, BoundingBox?.DeepClone());
        }


        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(MultiLineString? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as MultiLineString));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(MultiLineString? left, MultiLineString? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Type, Coordinates);

        /// <inheritdoc />
        public static bool operator ==(MultiLineString? left, MultiLineString? right)
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
        public static bool operator !=(MultiLineString? left, MultiLineString? right)
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