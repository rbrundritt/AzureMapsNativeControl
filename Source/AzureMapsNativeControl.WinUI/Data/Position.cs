using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// Represents a position on the map.
    /// </summary>
    [JsonConverter(typeof(PositionConverter))]
    public class Position : IEquatable<Position>, IDeepCloneable<Position>
    {
        #region Constructor

        /// <summary>
        /// Creates a new position.
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// Creates a new position from a GeoPosition.
        /// </summary>
        /// <param name="point"></param>
        public Position(GeoPosition point): 
            this(point.Longitude, point.Latitude, point.Altitude)
        {
        }

        /// <summary>
        /// Creates a new position from a GeoPoint.
        /// </summary>
        /// <param name="point"></param>
        public Position(GeoPoint point): 
            this(point.Coordinates.Longitude, point.Coordinates.Latitude, point.Coordinates.Altitude)
        {
        }

        /// <summary>
        /// Creates a new position from a latitude and longitude.
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="altitude"></param>
        public Position(double longitude, double latitude, double? altitude = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The latitude value of the position.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude value of the position.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The altitude value of the position.
        /// </summary>
        public double? Altitude { get; set; } = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new position from an array of coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Position? FromArray(IEnumerable<double>? coordinates)
        {
            if (coordinates == null)
            {
                return null;
            }

            int len = coordinates.Count();
            if (len < 2)
            {
                return null;
            }

            if (len == 2)
            {
                return new Position(coordinates.ElementAt(0), coordinates.ElementAt(1));
            }

            return new Position(coordinates.ElementAt(0), coordinates.ElementAt(1), coordinates.ElementAt(2));
        }

        /// <summary>
        /// Creates a new position from a latitude and longitude.
        /// </summary>
        /// <param name="latitude">Latitude value</param>
        /// <param name="longitude">Longitude value</param>
        /// <param name="altitude">Altitude value</param>
        /// <returns></returns>
        public static Position FromLatLng(double latitude, double longitude, double? altitude = null)
        {
            return new Position(longitude, latitude, altitude);
        }

        /// <summary>
        /// Creates a deep clone of the Position.
        /// </summary>
        /// <returns></returns>
        public Position DeepClone()
        {
            return new Position(Longitude, Latitude, Altitude);
        }

        /// <summary>
        /// Converts the position to a GeoPosition.
        /// </summary>
        /// <returns></returns>
        public GeoPosition ToGeoPosition()
        {
            return new GeoPosition(Longitude, Latitude, Altitude);
        }

        /// <summary>
        /// Returns a string that represents the current object with the format [longitude,latitude,altitude] or [longitude,latitude] if altitude is null.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {            
            return ToString(null);
        }

        /// <summary>
        /// Converts the Position to a JSON representation.
        /// </summary>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        /// <returns>The Position as JSON string.</returns>
        public string ToString(int? sigDigits)
        {
            double lat = Latitude;
            double lon = Longitude;
            double? alt = Altitude;

            if (sigDigits.HasValue && sigDigits >= 0)
            {
                lat = Math.Round(lat, sigDigits.Value);
                lon = Math.Round(lon, sigDigits.Value);

                if (alt.HasValue)
                {
                    alt = Math.Round(alt.Value, sigDigits.Value);
                }
            }
            { }
            if (alt.HasValue)
            {
                return string.Format(CultureInfo.InvariantCulture, "[{0},{1},{2}]", lon, lat, alt);
            }

            return string.Format(CultureInfo.InvariantCulture, "[{0},{1}]", lon, lat);
        }

        /// <summary>
        /// Parses a JSON string to a Position. 
        /// Format should be "longitude, latitude" or "longitude, latitude, elevation" (square brackets supported).
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Position? Parse(string? jsonString)
        {
            //Use PositionConverter
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            var parts = jsonString.Replace("[", "").Replace("]", "").Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2 &&
                double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon) &&
                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat))
            {
                double? alt = null;

                if (parts.Length > 2 && double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double a))
                {
                    alt = a;
                }

                return new Position(lon, lat, alt);
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a JSON string to a Position.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static bool TryParse(string? json, out Position? positions)
        {
            positions = Parse(json);
            return positions != null;
        }

        /// <summary>
        /// Get the value of pixel component using its index.
        /// </summary>
        /// <param name="index"></param>
        public double this[int index] 
        {             
            get
            {
                return index switch
                {
                    0 => Longitude,
                    1 => Latitude,
                    2 => Altitude ?? 0,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        /// <summary>
        /// Determines whether the specified position is equal to the current position.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Position? other)
        {
            return this == other;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current position.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj != null && obj is Position && this == (obj as Position);
        }

        /// <inheritdoc />
        public int GetHashCode([DisallowNull] Position obj)
        {
            // Implement GetHashCode logic here
            return HashCode.Combine(obj.Longitude, obj.Latitude, obj.Altitude);
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        public static bool operator ==(Position? left, Position? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            if (!Utils.TenDecimalCompare(left.Latitude, right.Latitude) ||
                !Utils.TenDecimalCompare(left.Longitude, right.Longitude))
            {
                return false;
            }

            return left.Altitude.HasValue == right.Altitude.HasValue &&
                   (!left.Altitude.HasValue || Utils.TenDecimalCompare(left.Altitude.Value, right.Altitude.Value));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered not equal.
        /// </summary>
        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new position from a JsonElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static Position? FromJsonElement(System.Text.Json.JsonElement element)
        {
            if (element.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                return null;
            }

            var arrayLength = element.GetArrayLength();
            if (arrayLength < 2 || arrayLength > 3)
            {
                throw new JsonException("Only 2 or 3 element coordinates supported");
            }

            var lon = element[0].GetDouble();
            var lat = element[1].GetDouble();
            double? altitude = null;

            if (arrayLength > 2)
            {
                altitude = element[2].GetDouble();
            }

            return new Position(lon, lat, altitude);
        }

        /// <summary>
        /// Converts the position to a GeoPosition.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static Azure.Core.GeoJson.GeoPosition ToGeoPosition(Position position)
        {
            return new GeoPosition(position.Longitude, position.Latitude, position.Altitude);
        }

        #endregion
    }
}
