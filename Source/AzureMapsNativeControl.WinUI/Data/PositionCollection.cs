using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// An observable collection of Position objects.
    /// </summary>
    [JsonConverter(typeof(JsonConverters.PositionCollectionConverter))]
    public class PositionCollection: Core.ObservableRangeCollection<Position>, IEquatable<PositionCollection>, IDeepCloneable<PositionCollection>
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        public PositionCollection() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="positions"></param>
        public PositionCollection(IEnumerable<Position> positions) : base(positions)
        {
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="positions"></param>
        public PositionCollection(IEnumerable<GeoPosition> positions) : base()
        {
            var items = new List<Position>();

            foreach (var p in positions)
            {
                items.Add(new Position(p));
            }

            AddRangeCore(items);
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="points"></param>
        public PositionCollection(IEnumerable<GeoPoint> points) : base()
        {
            var items = new List<Position>();

            foreach (var p in points)
            {
                items.Add(new Position(p));
            }

            AddRangeCore(items);
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="points"></param>
        public PositionCollection(GeoPointCollection points) : this(points.Coordinates)
        {
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="path"></param>
        public PositionCollection(GeoLineString path) : this(path.Coordinates)
        {
        }

        /// <summary>
        /// Creates a new instance of a PositionCollection.
        /// </summary>
        /// <param name="ring"></param>
        public PositionCollection(GeoLinearRing ring) : this(ring.Coordinates)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the line string is closed (first and last positions are the same) and thus represents a linear ring.
        /// </summary>
        /// <returns>True if the LineString represents a linear ring</returns>
        public bool IsClosed()
        {
            return this.Count > 1 && this[0].Equals(this[this.Count - 1]);
        }

        /// <summary>
        /// Determines if the coordinates in a CoordinateCollection are in a counter clockwise order. 
        /// This is important when performing certain calculates and to ensure compatibility with OGC standards.
        /// </summary>
        /// <returns>A boolean indicating if the coordinates are in a counter clockwise order</returns>
        public bool IsCCW()
        {
            int count = this.Count;

            //Polygons should have closed rings (start and end with the same coordinate). 
            //As such they require a minium of 4 coordinates to be valid.
            if (count < 4)
            {
                return false;
            }

            Position coordinate = this[0];
            int index1 = 0;

            for (int i = 1; i < count; i++)
            {
                Position coordinate2 = this[i];
                if (coordinate2.Latitude > coordinate.Latitude)
                {
                    coordinate = coordinate2;
                    index1 = i;
                }
            }

            int num4 = index1 - 1;

            if (num4 < 0)
            {
                num4 = count - 2;
            }

            int num5 = index1 + 1;

            if (num5 >= count)
            {
                num5 = 1;
            }

            Position coordinate3 = this[num4];
            Position coordinate4 = this[num5];

            double num6 = ((coordinate4.Longitude - coordinate.Longitude) * (coordinate3.Latitude - coordinate.Latitude)) -
                ((coordinate4.Latitude - coordinate.Latitude) * (coordinate3.Longitude - coordinate.Longitude));

            if (num6 == 0.0)
            {
                return (coordinate3.Longitude > coordinate4.Longitude);
            }

            return (num6 > 0.0);
        }

        /// <summary>
        /// Closes the line string to form a linear ring if it isn't already closed.
        /// </summary>
        public void Close()
        {
            if (!IsClosed())
            {
                this.Add(this[0].DeepClone());
            }
        }

        /// <summary>
        /// Creates a deep clone of the PositionCollection.
        /// </summary>
        /// <returns></returns>
        public new PositionCollection DeepClone()
        {
            var list = new List<Position>(this.Count);

            foreach (var p in this)
            {
                list.Add(p.DeepClone());
            }

            return new PositionCollection(list);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < this.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                sb.Append(this[i].ToString());
            }

            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// Parses a PositionCollection from a JSON string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static PositionCollection? Parse(string? jsonString)
        {
            //Use PositionConverter
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<PositionCollection>(jsonString);
        }

        /// <summary>
        /// Tries to parse a PositionCollection from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static bool TryParse(string? json, out PositionCollection? positions)
        {
            positions = Parse(json);
            return positions != null;
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(PositionCollection? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as PositionCollection));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(PositionCollection? left, PositionCollection? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public static bool operator ==(PositionCollection? left, PositionCollection? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            //Compare the coordinates in the collection.
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < left.Count; i++)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public static bool operator !=(PositionCollection? left, PositionCollection? right)
        {
            return !(left == right);
        }

        #endregion

        #endregion

        #region Internal Methods

        #region Create from Data

        internal static ObservableRangeCollection<PositionCollection> FromData(IEnumerable<IEnumerable<Position>> positions, bool close)
        {
            var list = new ObservableRangeCollection<PositionCollection>();

            foreach (var p in positions)
            {
                var pc = new PositionCollection(p);

                if (close)
                {
                    pc.Close();
                }

                list.Add(pc);
            }

            return list;
        }

        internal static ObservableRangeCollection<PositionCollection> FromData(IEnumerable<IEnumerable<GeoPosition>> positions, bool close)
        {
            var list = new ObservableRangeCollection<PositionCollection>();

            foreach (var p in positions)
            {
                var pc = new PositionCollection(p);

                if (close)
                {
                    pc.Close();
                }

                list.Add(pc);
            }

            return list;
        }

        internal static ObservableRangeCollection<PositionCollection> FromData(GeoArray<GeoArray<GeoPosition>> positions, bool close)
        {
            var list = new ObservableRangeCollection<PositionCollection>();

            foreach (var p in positions)
            {
                var pc = new PositionCollection(p);

                if (close)
                {
                    pc.Close();
                }

                list.Add(pc);
            }

            return list;
        }

        internal static ObservableRangeCollection<PositionCollection> FromData(IEnumerable<GeoLineString> lines, bool close)
        {
            var list = new ObservableRangeCollection<PositionCollection>();

            foreach (var p in lines)
            {
                var pc = new PositionCollection(p);

                if (close)
                {
                    pc.Close();
                }

                list.Add(pc);
            }

            return list;
        }

        internal static ObservableRangeCollection<PositionCollection> FromData(IEnumerable<GeoLinearRing> rings)
        {
            var list = new ObservableRangeCollection<PositionCollection>();

            foreach (var p in rings)
            {
                var pc = new PositionCollection(p);
                pc.Close();
                list.Add(pc);
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<IEnumerable<IEnumerable<Position>>> positions, bool close)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var p in positions)
            {
                list.Add(FromData(p, close));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<IEnumerable<IEnumerable<GeoPosition>>> positions, bool close)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var p in positions)
            {
                list.Add(FromData(p, close));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(GeoArray<GeoArray<GeoArray<GeoPosition>>> positions, bool close)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var p in positions)
            {
                list.Add(FromData(p, close));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<IEnumerable<GeoLineString>> lines, bool close)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var p in lines)
            {
                list.Add(FromData(p, true));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<IEnumerable<GeoLinearRing>> rings)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var r in rings)
            {
                list.Add(FromData(r));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<GeoPolygon> polygons, bool close)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var r in polygons)
            {
                list.Add(FromData(r.Coordinates, true));
            }

            return list;
        }

        internal static ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> FromData(IEnumerable<Polygon> polygons)
        {
            var list = new ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>();

            foreach (var r in polygons)
            {
                list.Add(r.Coordinates);
            }

            return list;
        }

        #endregion

        internal static IList<Position> GetPositions(ObservableRangeCollection<PositionCollection> positions)
        {
            return positions.SelectMany(p => p).ToList();
        }

        internal static IList<Position> GetPositions(ObservableRangeCollection<ObservableRangeCollection<PositionCollection>> positions)
        {
            return positions.SelectMany(p => GetPositions(p)).ToList();
        }

        internal static IList<Position> GetPositions(ObservableRangeCollection<ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>> positions)
        {
            return positions.SelectMany(p => GetPositions(p)).ToList();
        }

        internal static IList<GeoPosition> ToGeoPositions(PositionCollection positions)
        {
            return positions.Select(p => Position.ToGeoPosition(p)).ToList();
        }

        internal static IList<GeoLineString> ToGeoLineStrings(ObservableRangeCollection<PositionCollection> positions)
        {
            return positions.Select(p => new GeoLineString(ToGeoPositions(p))).ToList();
        }

        internal static IList<GeoPoint> ToGeoPoints(PositionCollection positions)
        {
            return positions.Select(p => new GeoPoint(Position.ToGeoPosition(p))).ToList();
        }

        internal static GeoLinearRing ToLinearRing(PositionCollection positions)
        {
            positions.Close();
            return new GeoLinearRing(ToGeoPositions(positions));
        }

        internal static IList<GeoLinearRing> ToLinearRings(ObservableRangeCollection<PositionCollection> positions)
        {
            return positions.Select(p => ToLinearRing(p)).ToList();
        }

        /// <summary>
        /// Creates a new PositionCollection from a JsonElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static PositionCollection? FromJsonElement(System.Text.Json.JsonElement element)
        {
            if (element.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                return null;
            }

            var coordinates = new List<Position>(element.GetArrayLength());

            foreach (JsonElement coordinate in element.EnumerateArray())
            {
                var p = Position.FromJsonElement(coordinate);
                if (p != null)
                {
                    coordinates.Add(p);
                }
            }

            return new PositionCollection(coordinates);
        }

        /// <summary>
        /// Compares two collections of position collections to see if they are equal.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="otherPositions"></param>
        /// <returns></returns>
        public static bool AreEqual(IList<PositionCollection> positions, IList<PositionCollection> otherPositions)
        {
            if (positions.Count != otherPositions.Count)
            {
                return false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (!positions[i].Equals(otherPositions[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two collections of collections of position collections to see if they are equal.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="otherPositions"></param>
        /// <returns></returns>
        public static bool AreEqual(IList<IList<PositionCollection>> positions, IList<IList<PositionCollection>> otherPositions)
        {
            if (positions.Count != otherPositions.Count)
            {
                return false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (!AreEqual(positions[i], otherPositions[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two collections of collections of collections of position collections to see if they are equal.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="otherPositions"></param>
        /// <returns></returns>
        public static bool AreEqual(IList<IList<IList<PositionCollection>>> positions, IList<IList<IList<PositionCollection>>> otherPositions)
        {
            if (positions.Count != otherPositions.Count)
            {
                return false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (!AreEqual(positions[i], otherPositions[i]))
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// Compares two collections of collections of position collections to see if they are equal.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="otherPositions"></param>
        /// <returns></returns>
        public static bool AreEqual(IList<ObservableRangeCollection<PositionCollection>> positions, IList<ObservableRangeCollection<PositionCollection>> otherPositions)
        {
            if (positions.Count != otherPositions.Count)
            {
                return false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (!AreEqual(positions[i], otherPositions[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two collections of collections of collections of position collections to see if they are equal.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="otherPositions"></param>
        /// <returns></returns>
        public static bool AreEqual(IList<ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>> positions, IList<ObservableRangeCollection<ObservableRangeCollection<PositionCollection>>> otherPositions)
        {
            if (positions.Count != otherPositions.Count)
            {
                return false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if (!AreEqual(positions[i], otherPositions[i]))
                {
                    return false;
                }
            }

            return true;

        }

        #endregion
    }
}
