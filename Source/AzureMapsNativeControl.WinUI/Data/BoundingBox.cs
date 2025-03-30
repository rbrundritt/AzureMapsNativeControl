using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Source;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A bounding box that defines a rectangular area on the map.
    /// </summary>
    [JsonConverter(typeof(JsonConverters.BoundingBoxConverter))]
    public class BoundingBox: IEquatable<BoundingBox>, IDeepCloneable<BoundingBox>
    {
        #region Constructor

        /// <summary>
        /// A bounding box that defines a rectangular area on the map.
        /// </summary>
        /// <param name="west">The western longitude of the bounding box.</param>
        /// <param name="south">The southern latitude of the bounding box.</param>
        /// <param name="east">The eastern longitude of the bounding box.</param>
        /// <param name="north">The northern latitude of the bounding box.</param>
        public BoundingBox(double west, double south, double east, double north)
        {
            West = west;
            South = south;
            East = east;
            North = north;
        }

        /// <summary>
        /// A bounding box that defines a rectangular area on the map.
        /// </summary>
        /// <param name="west">The western longitude of the bounding box.</param>
        /// <param name="south">The southern latitude of the bounding box.</param>
        /// <param name="minAltitude">The minimum altitude of the bounding box.</param>
        /// <param name="east">The eastern longitude of the bounding box.</param>
        /// <param name="north">The northern latitude of the bounding box.</param>
        /// <param name="maxAltitude">The maximum altitude of the bounding box.</param>
        public BoundingBox(double west, double south, double minAltitude, double east, double north, double maxAltitude)
        {
            West = west;
            South = south;
            East = east;
            North = north;
        }

        /// <summary>
        /// A bounding box that defines a rectangular area on the map.
        /// </summary>
        /// <param name="NorthWest"></param>
        /// <param name="SouthEast"></param>
        public BoundingBox(Position NorthWest, Position SouthEast)
        {
            North = NorthWest.Latitude;
            West = NorthWest.Longitude;
            South = SouthEast.Latitude;
            East = SouthEast.Longitude;
        }

        /// <summary>
        /// A bounding box that defines a rectangular area on the map.
        /// </summary>
        /// <param name="center">The center position of the bounding box.</param>
        /// <param name="width">The width of the bounding box in degrees longitude.</param>
        /// <param name="height">The height of the bounding box in degrees latitude.</param>
        public BoundingBox(Position center, double width, double height)
        {
            North = Math.Min(Math.Max(center.Latitude + height / 2, -85.5), 85.5);
            South = Math.Min(Math.Max(center.Latitude - height / 2, -85.5), 85.5);
            East = AtlasMath.NormalizeLongitude(center.Longitude + width / 2);
            West = AtlasMath.NormalizeLongitude(center.Longitude - width / 2);
        }

        /// <summary>
        /// A bounding box that defines a rectangular area on the map.
        /// </summary>
        /// <param name="bbox"></param>
        public BoundingBox(GeoBoundingBox bbox)
        {
            North = bbox.North;
            South = bbox.South;
            East = bbox.East;
            West = bbox.West;
        }

        #endregion

        #region Properties

        private double north = 85.5;

        /// <summary>
        /// The northern latitude of the bounding box.
        /// </summary>
        public double North
        {
            get
            {
                return north;
            }
            set
            {
                north = Math.Round(value, 7);
            }
        }

        private double south = -85.5;

        /// <summary>
        /// The southern latitude of the bounding box.
        /// </summary>
        public double South
        {
            get
            {
                return south;
            }
            set
            {
                south = Math.Round(value, 7);
            }
        }

        private double west = -180;

        /// <summary>
        /// The western longitude of the bounding box.
        /// </summary>
        public double West
        {
            get
            {
                return west;
            }
            set
            {
                west = Math.Round(value, 7);
            }
        }

        private double east = 180;

        /// <summary>
        /// The eastern longitude of the bounding box.
        /// </summary>
        public double East
        {
            get
            {
                return east;
            }
            set
            {
                east = Math.Round(value, 7);
            }
        }

        /// <summary>
        /// The minimum altitude of the bounding box.
        /// </summary>
        public double? MinAltitude { get; set; }

        /// <summary>
        /// The maximum altitude of the bounding box.
        /// </summary>
        public double? MaxAltitude { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a BoundingBox from an array of numbers.
        /// Format should be [west, south, east, north] or [west, south, minElevation, east, north, maxElevation].
        /// </summary>
        /// <param name="bbox">Array of numbers that make up the bounding box.</param>
        /// <returns></returns>
        public static BoundingBox? FromArray(IEnumerable<double> bbox)
        {
            if (bbox == null)
            {
                return null;
            }

            int len = bbox.Count();

            if (len >= 4)
            {
                if (len == 4)
                {
                    // [west, south, east, north]
                    return new BoundingBox(bbox.ElementAt(0), bbox.ElementAt(1), bbox.ElementAt(2), bbox.ElementAt(3));
                }
                //Check to see if bbox contains altitude information.
                else if (len == 6)
                {
                    // [west, south, minElevation, east, north, maxElevation]
                    return new BoundingBox(bbox.ElementAt(0), bbox.ElementAt(1), bbox.ElementAt(2), bbox.ElementAt(3), bbox.ElementAt(4), bbox.ElementAt(5));
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a BoundingBox that contains all provided Position objects.
        /// </summary>
        /// <param name="positions">An array of locations to use to generate the bounding box.</param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns>A bounding box that contains all given positions.</returns>
        public static BoundingBox? FromPositions(IEnumerable<Position> positions, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            int len = positions.Count();

            if (len == 0)
            {
                return null;
            }

            double north = double.NaN;
            double south = double.NaN;
            double west = double.NaN;
            double east = double.NaN;

            var longitudes = new double[positions.Count()];
            int longCount = 0;

            foreach (var pos in positions)
            {
                var normLat = AtlasMath.NormalizeLatitude(pos.Latitude);
                north = double.IsNaN(north) ? normLat : Math.Max(north, normLat);
                south = double.IsNaN(south) ? normLat : Math.Min(south, normLat);

                longitudes[longCount++] = AtlasMath.NormalizeLongitude(pos.Longitude);
            }

            if (longCount > 0)
            {
                // Find largest gap between longitudes
                Array.Sort(longitudes, (a, b) => a.CompareTo(b));

                double maxGap = (longitudes[0] + 360) - longitudes[longCount - 1];
                int maxGapIndex = 0;

                for (int i = 1; i < longCount; i++)
                {
                    double gap = longitudes[i] - longitudes[i - 1];

                    if (gap > maxGap)
                    {
                        maxGap = gap;
                        maxGapIndex = i;
                    }
                }

                west = longitudes[maxGapIndex];

                if (maxGapIndex == 0)
                {
                    maxGapIndex = longCount;
                }

                east = longitudes[maxGapIndex - 1];
            }

            if (double.IsNaN(west) || double.IsNaN(south) || double.IsNaN(east) || double.IsNaN(north))
            {
                return null;
            }

            var bbox = new BoundingBox(west, south, east, north);
            bbox.BufferToMinEdgeLength(minEdgeLength);
            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a GeoJson object.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static BoundingBox? FromData(Feature feature, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            //Try to get circle coordinates. Will return null if feature does not represent a circle.
            var p = feature.GetCircleCoordinates();

            if (p != null)
            {
                var bbox = FromPositions(p, minEdgeLength);
                feature.Geometry._bbox = bbox;
                return bbox;
            }

            return FromData(feature.Geometry, minEdgeLength);
        }

        /// <summary>
        /// Calculates the bounding box for a DataSource.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static BoundingBox? FromData(DataSource dataSource, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            BoundingBox? bbox = null;

            foreach (var item in dataSource)
            {
                //Set min edge length to 0. Will calculate this after all features have been processed.
                var temp = FromData(item, 0);

                if (temp != null)
                {
                    bbox = bbox == null ? temp : bbox.Merge(temp);
                }
            }

            if (bbox != null)
            {
                bbox.BufferToMinEdgeLength(minEdgeLength);
            }

            //Cache the result with the data source.
            dataSource._bbox = bbox;

            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a DataSource.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static async Task<BoundingBox?> FromDataAsync(DataSourceLite dataSource, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            BoundingBox? bbox = null;

            if (dataSource._bbox != null)
            {
                bbox = dataSource._bbox;
            }
            else
            {
                bbox = await dataSource.GetBoundsAsync();
            }

            if (bbox != null)
            {
                bbox.BufferToMinEdgeLength(minEdgeLength);
            }

            //Cache the result with the data source.
            dataSource._bbox = bbox;

            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a collection of GeoJson objects.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static BoundingBox? FromData(IEnumerable<Feature> features, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            BoundingBox? bbox = null;

            foreach (var item in features)
            {
                //Set min edge length to 0. Will calculate this after all features have been processed.
                var temp = FromData(item, 0);

                if (temp != null)
                {
                    bbox = bbox == null ? temp : bbox.Merge(temp);
                }
            }

            if (bbox != null)
            {
                bbox.BufferToMinEdgeLength(minEdgeLength);
            }

            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a GeoJson object.
        /// </summary>
        /// <param name="features"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static BoundingBox? FromData(FeatureCollection features, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            var bbox = FromData(features.Features, minEdgeLength);
            features.BoundingBox = bbox;
            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box of a GeoJson object.
        /// </summary>
        /// <param name="geometry">The GeoJson object to calculate the bounds for.</param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns>The bounding box of a GeoJson object.</returns>
        public static BoundingBox? FromData(Geometry geometry, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            BoundingBox? bbox = null;

            if (geometry._bbox != null)
            {
                bbox = geometry._bbox;
            }
            else
            {
                switch (geometry)
                {
                    case PointGeometry pos:
                        bbox = new BoundingBox(pos.Coordinates.Longitude, pos.Coordinates.Latitude, pos.Coordinates.Longitude, pos.Coordinates.Latitude);
                        break;
                    case MultiPoint mpos:
                        bbox = FromPositions(AtlasMath.GetPositions(mpos), 0);
                        break;
                    case LineString line:
                        bbox = FromPositions(line.Coordinates, 0);
                        break;
                    case MultiLineString mlpos:
                        foreach (var item in mlpos.Coordinates)
                        {
                            var temp = FromPositions(item, 0);

                            if (temp != null)
                            {
                                bbox = bbox == null ? temp : bbox.Merge(temp);
                            }
                        }
                        break;
                    case Polygon poly:
                        if (poly.Coordinates.Count > 0)
                        {
                            //Only need to calculate bounding box of outer ring.
                            bbox = FromPositions(poly.Coordinates[0], 0);
                        }
                        break;
                    case MultiPolygon gc:
                        foreach (var item in gc.Coordinates)
                        {
                            //Only need to calculate bounding box of outer ring of each polygon.
                            if (item.Count > 0)
                            {
                                var temp = FromPositions(item[0], 0);

                                if (temp != null)
                                {
                                    bbox = bbox == null ? temp : bbox.Merge(temp);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (bbox != null)
            {
                bbox.BufferToMinEdgeLength(minEdgeLength);
            }

            if(minEdgeLength == 0)
            {
                geometry._bbox = bbox;
            }

            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a collection of GeoJson objects.
        /// </summary>
        /// <param name="geometries">The GeoJson objects to calculate the bounds for.</param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns>The bounding box for a collection of GeoJson objects.</returns>
        public static BoundingBox? FromData(IEnumerable<Geometry> geometries, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            BoundingBox? bbox = null;

            foreach (var item in geometries)
            {
                var temp = FromData(item, 0);

                if (temp != null)
                {
                    bbox = bbox == null ? temp : bbox.Merge(temp);
                }
            }

            if (bbox != null)
            {
                bbox.BufferToMinEdgeLength(minEdgeLength);
            }

            return bbox;
        }

        /// <summary>
        /// Calculates the bounding box for a collection of markers.
        /// </summary>
        /// <param name="markers"></param>
        /// <param name="minEdgeLength">
        /// The minimum edge length of the bounding box. 
        /// This helps to prevent bounding boxes that represent infinately small areas from being generated.
        /// </param>
        /// <returns></returns>
        public static BoundingBox? FromData(IEnumerable<HtmlMarker> markers, double minEdgeLength = Constants.DefaultMinBboxEdgeLength)
        {
            var positions = new List<Position>();

            foreach (var item in markers)
            {
                var p = item._options.Position;

                if (p != null)
                {
                    positions.Add(p);
                }
            }

            return FromPositions(positions, minEdgeLength);
        }

        /// <summary>
        /// Gets a global bounding box that covers the entire world.
        /// </summary>
        /// <returns></returns>
        public static BoundingBox Global()
        {
            return new BoundingBox(Constants.MinLongitude, Constants.MinLatitude, Constants.MaxLongitude, Constants.MaxLatitude);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the southwest corner of the bounding box.
        /// </summary>
        /// <returns></returns>
        public Position GetNorthWest()
        {
            return new Position(West, North);
        }

        /// <summary>
        /// Gets the southeast corner of the bounding box.
        /// </summary>
        /// <returns></returns>
        public Position GetNorthEast()
        {
            return new Position(East, North);
        }

        /// <summary>
        /// Gets the southwest corner of the bounding box.
        /// </summary>
        /// <returns></returns>
        public Position GetSouthWest()
        {
            return new Position(West, South);
        }

        /// <summary>
        /// Gets the southeast corner of the bounding box.
        /// </summary>
        /// <returns></returns>
        public Position GetSouthEast()
        {
            return new Position(East, South);
        }

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(BoundingBox? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as BoundingBox));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(BoundingBox? left, BoundingBox? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(west, south, east, north);

        /// <summary>
        /// Determines whether two specified bounding boxes are the same.
        /// </summary>
        /// <param name="left">The first bounding box to compare.</param>
        /// <param name="right">The first bounding box to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BoundingBox? left, BoundingBox? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            if (!Utils.TenDecimalCompare(left.North, right.North) ||
                !Utils.TenDecimalCompare(left.South, right.South) ||
                !Utils.TenDecimalCompare(left.East, right.East) ||
                !Utils.TenDecimalCompare(left.West, right.West))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether two specified bounding boxes don't have the same value.
        /// </summary>
        /// <param name="left">The first bounding box to compare.</param>
        /// <param name="right">The first bounding box to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(BoundingBox? left, BoundingBox? right)
        {
            return !(left == right);
        }

        #endregion


        /// <summary>
        /// Creates a deep clone of the bounding box.
        /// </summary>
        /// <returns></returns>
        public BoundingBox DeepClone()
        {
            return new BoundingBox(West, South, East, North);
        }

        /// <summary>
        /// Gets a polygon representation of a BoundingBox.
        /// </summary>
        /// <returns>A polygon representation of the BoundingBox.</returns>
        public Polygon ToPolygon()
        {
            double w = AtlasMath.NormalizeLongitude(West);
            double e = AtlasMath.NormalizeLongitude(East);
            double n = AtlasMath.NormalizeLatitude(North);
            double s = AtlasMath.NormalizeLatitude(South);
            var center = GetCenter();

            return new Polygon([new PositionCollection([
                    new Position(w, n),
                    new Position(w, s),
                    // We add an extra point along the longitude stretch in the center to ensure long bounding boxes stretches across the map rather than collapsing on itself at the antimeridian.
                    new Position(center.Longitude, s),
                    new Position(e, s),
                    new Position(e, n),
                    new Position(center.Longitude, n),
                    new Position(w, n)
                ])
            ]);
        }

        /// <summary>
        /// Converts a bounding box to a GeoBoundingBox.
        /// </summary>
        /// <returns></returns>
        public GeoBoundingBox? ToGeoBoundingBox()
        {
            return new GeoBoundingBox(West, South, East, North);
        }

        /// <summary>
        /// Returns a string representation of the bounding box.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Converts the BoundingBox to a JSON representation.
        /// </summary>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        /// <returns>The BoundingBox as JSON string.</returns>
        public string ToString(int? sigDigits)
        {
            double w = West;
            double s = South;
            double e = East;
            double n = North;
            double? minAlt = MinAltitude;
            double? maxAlt = MaxAltitude;

            if (sigDigits.HasValue && sigDigits >= 0)
            {
                w = Math.Round(w, sigDigits.Value);
                s = Math.Round(s, sigDigits.Value);
                e = Math.Round(e, sigDigits.Value);
                n = Math.Round(n, sigDigits.Value);

                if (minAlt.HasValue && maxAlt.HasValue)
                {
                    minAlt = Math.Round(minAlt.Value, sigDigits.Value);
                    maxAlt = Math.Round(maxAlt.Value, sigDigits.Value);
                }
            }

            if (minAlt.HasValue && maxAlt.HasValue)
            {
                return string.Format(CultureInfo.InvariantCulture, "[{0},{1},{2},{3},{4},{5}]", w, s, minAlt.Value, e, n, maxAlt.Value);
            }

            return string.Format(CultureInfo.InvariantCulture, "[{0},{1},{2},{3}]", w, s, e, n);
        }

        /// <summary>
        /// Parses a JSON string to a BoundingBox.
        /// Format should be "west, south, east, north" or "west, south, minElevation, east, north, maxElevation" (square brackets supported).
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static BoundingBox? Parse(string? jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            var parts = jsonString.Replace("[", "").Replace("]", "").Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                return null;
            }

            if (parts.Length == 4 &&
               double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double west) &&
               double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double south) &&
               double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double east) &&
               double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double north))
            {
                return new BoundingBox(west, south, east, north);
            } 
            else if (parts.Length == 6 &&
                double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double west2) &&
                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double south2) &&
                double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double minAlt) &&
                double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double east2) &&
                double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double north2) &&
                double.TryParse(parts[5], NumberStyles.Any, CultureInfo.InvariantCulture, out double maxAlt))
            {
                return new BoundingBox(west2, south2, minAlt, east2, north2, maxAlt);
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a JSON string to a BoundingBox.
        /// Format should be "west, south, east, north" or "west, south, minElevation, east, north, maxElevation" (square brackets supported).
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static bool TryParse(string? jsonString, out BoundingBox? bbox)
        {
            bbox = Parse(jsonString);
            return bbox != null;
        }

        /// <summary>
        /// Gets the center of the bounding box. Accounts for Antimeridian.
        /// </summary>
        /// <returns></returns>
        public Position GetCenter()
        {
            // [west, south, east, north]
            double east = AtlasMath.NormalizeLongitude(East);
            double west = AtlasMath.NormalizeLongitude(West);

            if (west > east)
            {
                east += 360.0;
            }

            double centerLongitude = AtlasMath.NormalizeLongitude((west + east) / 2.0);

            double south = AtlasMath.NormalizeLatitude(South);
            double north = AtlasMath.NormalizeLatitude(North);

            double centerLatitude = AtlasMath.NormalizeLatitude((south + north) / 2.0);

            return new Position(centerLongitude, centerLatitude);
        }

        /// <summary>
        /// Gets the height of a bounding box in degrees.
        /// </summary>
        /// <returns></returns>
        public double GetHeight()
        {
            double north = AtlasMath.NormalizeLatitude(North);
            double south = AtlasMath.NormalizeLatitude(South);
            double height = north - south;
            return double.IsNaN(height) ? 0 : height;
        }

        /// <summary>
        /// Gets the width of a bounding box in degrees.
        /// </summary>
        /// <returns></returns>
        public double GetWidth()
        {
            double east = AtlasMath.NormalizeLongitude(East);
            double west = AtlasMath.NormalizeLongitude(West);
            double width = east - west;
            // Check to see if bounds crosses antimeridian
            return double.IsNaN(width) ? 0 : width < 0 ? width += 360 : width;
        }

        /// <summary>
        /// Merges two bounding boxes together.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BoundingBox Merge(BoundingBox other)
        {
            double north = Math.Max(
                AtlasMath.NormalizeLatitude(North),
                AtlasMath.NormalizeLatitude(other.North)
            );

            double south = Math.Min(
                AtlasMath.NormalizeLatitude(South),
                AtlasMath.NormalizeLatitude(other.South)
            );

            // Find out the longitude spans of the two bounding boxes.
            var boundingBoxes = new BoundingBox[] { this, other };
            var longIntervals = new List<double[]>();
            int i = 0;

            for (i = 0; i < boundingBoxes.Length; i++)
            {
                var b = boundingBoxes[i];
                double w = AtlasMath.NormalizeLongitude(b.West);
                double e = AtlasMath.NormalizeLongitude(b.East);

                if (b.CrossesAntimeridian())
                {
                    longIntervals.Add([w, 180]);
                    longIntervals.Add([-180, e]);
                }
                else
                {
                    longIntervals.Add([w, e]);
                }
            }

            // sort and merge (overlaped intervals) these intervals
            // after the sort and merge we should have a list of intervals that don't overlap with each other.
            longIntervals.Sort((a, b) =>
            {
                return a[0] == b[0] ? (a[1] < b[1] ? -1 : 1) : (a[0] < b[0] ? -1 : 1);
            });

            var mergedIntervals = new List<double[]>();
            var currentInterval = longIntervals[0];

            i = 1;
            while (i < longIntervals.Count)
            {
                var nextInterval = longIntervals[i];
                if (currentInterval[1] >= nextInterval[0])
                {
                    currentInterval[1] = Math.Max(currentInterval[1], nextInterval[1]);
                }
                else
                {
                    mergedIntervals.Add(currentInterval);
                    currentInterval = nextInterval;
                }
                i++;
            }
            mergedIntervals.Add(currentInterval);

            // find out the biggest gap between these intervals. suppose this gap is [A, B], then A will be the east bound and B will be the west bound of the merged rectangle.
            int length = mergedIntervals.Count;
            double maxGapSpan = mergedIntervals[0][0] + 360 - mergedIntervals[length - 1][1];
            double[] maxGap = [mergedIntervals[length - 1][1], mergedIntervals[0][0]];

            for (i = 1; i < length; i++)
            {
                double thisGapSpan = mergedIntervals[i][0] - mergedIntervals[i - 1][1];
                if (thisGapSpan > maxGapSpan)
                {
                    maxGapSpan = thisGapSpan;
                    maxGap[0] = mergedIntervals[i - 1][1];
                    maxGap[1] = mergedIntervals[i][0];
                }
            }

            // now we have all the edges of the rectangle
            return new BoundingBox(maxGap[1], south, maxGap[0], north);
        }

        /// <summary>
        /// Determines is two bounding boxes intersect.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the provided bounding boxes intersect.</returns>
        public bool Intersect(BoundingBox other)
        {
            var center1 = GetCenter();
            var center2 = other.GetCenter();

            // Use the distance between the 2 centers to determine if they intersect.
            double diffLatitude = Math.Abs(center1.Latitude - center2.Latitude);
            double diffLongitude = Math.Abs(center1.Longitude - center2.Longitude);

            if (diffLongitude > 180.0)
            {
                // We want the shortest distance between the 2 points.
                diffLongitude = 360 - diffLongitude;
            }

            double height1 = GetHeight();
            double width1 = GetWidth();
            double height2 = other.GetHeight();
            double width2 = other.GetWidth();

            return (diffLatitude <= (height1 / 2 + height2 / 2) && diffLongitude <= (width1 / 2 + width2 / 2));
        }

        #region Contains

        /// <summary>
        /// Determines if a position is within a bounding box.
        /// </summary>
        /// <param name="position">The position to see if it is in the bounding box.</param>
        /// <returns>True if the position is within the bounding box.</returns>
        public bool Contains(GeoPosition position)
        {
            // Allow a small difference to account for arithmetic accuracy errors.
            // This is important for points on the edge of the bounding box which is the case when using BoundingBox.fromLocations
            double accuracyAllowance = 0.00000001;

            // Use the distance between the position and center of the bounding box to determine if they intersect.
            var center = GetCenter();

            var diffLatitude = Math.Abs(center.Latitude - position.Latitude);
            var diffLongitude = Math.Abs(center.Longitude - position.Longitude);

            if (diffLongitude > 180.0)
            {
                // We want the shortest distance between the 2 points.
                diffLongitude = 360 - diffLongitude;
            }

            var height = GetHeight();
            var width = GetWidth();

            return (diffLatitude <= (height / 2) + accuracyAllowance) && (diffLongitude <= (width / 2) + accuracyAllowance);
        }

        /// <summary>
        /// Determines if a point is within a bounding box.
        /// </summary>
        /// <param name="point">The point to see if it is in the bounding box.</param>
        /// <returns>True if the point is within the bounding box.</returns>
        public bool Contains(GeoPoint point)
        {
            return Contains(point.Coordinates);
        }

        /// <summary>
        /// Determines if a bounding box is within another bounding box
        /// </summary>
        /// <param name="inner">The inner bounding box (the one that should be contained within outer)</param>
        /// <returns>True if the inner bounding box is fully container in outer</returns>
        public bool ContainsBoundingBox(BoundingBox inner)
        {
            return this.West < inner.West &&
                inner.West < this.East &&
                this.West < inner.East &&
                inner.East < this.East &&
                this.South < inner.South &&
                inner.South < this.North &&
                this.South < inner.North &&
                inner.North < this.North;
        }

        #endregion

        #region Antimeridian 

        /// <summary>
        /// Returns a boolean indicating if the bounding box is larger than the globe or not.
        /// </summary>
        /// <returns></returns>
        private bool IsBoundingBoxLargerThanGlobe()
        {
            return East - West > 360;
        }

        /// <summary>
        /// Returns a boolean indicating if the bounding box crosses the antimeridian or not.
        /// </summary>
        /// <returns>A boolean indicating if the bounding box crosses the antimeridian or not.</returns>
        public bool CrossesAntimeridian()
        {
            double east = AtlasMath.NormalizeLongitude(East);
            double west = AtlasMath.NormalizeLongitude(West);
            return (east - west) < 0 || IsBoundingBoxLargerThanGlobe();
        }

        /// <summary>
        /// Splits a BoundingBox that crosses the Antimeridian into two BoundingBox's. One entirely west of the Antimerdian and another entirely east of the Antimerdian.
        /// </summary>
        /// <returns>One or two bounding boxes depending on if the original bounding box crosses the antimerdian</returns>
        public IList<BoundingBox> SplitOnAntimeridian()
        {
            //Test the case when the bounding box is wider than a single globe.
            if (IsBoundingBoxLargerThanGlobe())
            {
                return [new BoundingBox(-180, South, 180, North)];
            }
            else if (CrossesAntimeridian())
            {
                //Case when map less than full globe width, but crosses anti-meridian.
                double west = AtlasMath.NormalizeLongitude(West);
                double east = AtlasMath.NormalizeLongitude(East);
                double north = AtlasMath.NormalizeLatitude(North);
                double south = AtlasMath.NormalizeLatitude(South);

                return [
                    new BoundingBox(west, south, 180, north),
                    new BoundingBox(-180, south, east, north)
                ];
            }

            return [DeepClone()];
        }

        /// <summary>
        /// Inspects the width and height of the bounding box and buffers these if they are less than the specified edge length in degrees.
        /// Longitude values are normalized, and Latitude values are clipped at their max allowed values.
        /// </summary>
        /// <param name="minEdgeLength"></param>
        public void BufferToMinEdgeLength(double minEdgeLength = 0.0001)
        {
            if (minEdgeLength > 0)
            {
                var center = GetCenter();

                if (GetWidth() < minEdgeLength)
                {
                    double centerLongitude = center.Longitude;
                    West = AtlasMath.NormalizeLongitude(centerLongitude - minEdgeLength / 2);
                    East = AtlasMath.NormalizeLongitude(centerLongitude + minEdgeLength / 2);
                }

                if (GetHeight() < minEdgeLength)
                {
                    double centerLatitude = center.Latitude;
                    South = Math.Max(centerLatitude - minEdgeLength / 2, -85.5);
                    North = Math.Min(centerLatitude + minEdgeLength / 2, 85.5);
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the bounding box from the GeoJSON element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        internal static BoundingBox? FromJsonElement(in JsonElement element)
        {
            BoundingBox? bbox = null;

            var bboxElement = Utils.GetJsonElementProperty(element, "bbox", true);

            if (bboxElement.HasValue)
            {
                // According to RFC 7946, the bbox member is optional. If one is provided, it MUST
                // be an array and cannot be null.
                // The code below is intentionally lenient and allows a null value to be treated
                // as if the bbox member was omitted. The Geometry.BoundingBox property is already
                // set to null when there is no bbox, so setting it to null when the GeoJSON data has
                // a null bbox does not impact the behavior of the Geometry class.
                // This was done to be compatible with third-party GeoJSON serializers. There are some
                // GeoJSON serializer packages in the broader community that either don't follow this
                // part of the spec, or interpret optional as being equal to nullable.
                // Note: The Azure.Core serializer follows the spec and never writes "bbox": null
                if (bboxElement.Value.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }

                var arrayLength = bboxElement.Value.GetArrayLength();

                switch (arrayLength)
                {
                    case 4:
                        bbox = new BoundingBox(
                            bboxElement.Value[0].GetDouble(),
                            bboxElement.Value[1].GetDouble(),
                            bboxElement.Value[2].GetDouble(),
                            bboxElement.Value[3].GetDouble()
                        );
                        break;
                    case 6:
                        bbox = new BoundingBox(
                            bboxElement.Value[0].GetDouble(),
                            bboxElement.Value[1].GetDouble(),
                            bboxElement.Value[3].GetDouble(),
                            bboxElement.Value[4].GetDouble(),
                            bboxElement.Value[2].GetDouble(),
                            bboxElement.Value[5].GetDouble()
                        );
                        break;
                    default:
                        throw new JsonException("Only 4 or 6 elements supported");
                }
            }

            return bbox;
        }

        #endregion
    }
}
