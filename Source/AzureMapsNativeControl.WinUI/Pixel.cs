using System.Text.Json.Serialization;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;

#if WINUI
using Windows.Foundation;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Represent a pixel coordinate or offset. Extends an array of [x, y].
    /// </summary>
    [JsonConverter(typeof(PixelConverter))]
    public class Pixel : IEquatable<Pixel>, IDeepCloneable<Pixel>
    {
        #region Constructor

        /// <summary>
        /// Represent a pixel coordinate or offset. Extends an array of [x, y].
        /// </summary>
        public Pixel()
        {
        }

        /// <summary>
        /// Represent a pixel coordinate or offset. Extends an array of [x, y].
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Pixel(int x, int y)
        {
            X = (double)x;
            Y = (double)y;
        }

        /// <summary>
        /// Represent a pixel coordinate or offset. Extends an array of [x, y].
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Pixel(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Represent a pixel coordinate or offset. Extends an array of [x, y].
        /// </summary>
        /// <param name="point"></param>
        public Pixel(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// X coordinate
        /// </summary>
        public double X { get; set; } = 0;

        /// <summary>
        /// Y coordinates
        /// </summary>
        public double Y { get; set; } = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of this pixel.
        /// </summary>
        /// <returns></returns>
        public Pixel DeepClone()
        {
            return new Pixel(X, Y);
        }

        /// <inheritdoc />
        public bool Equals(Pixel? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Pixel));
        }
        
        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Pixel? left, Pixel? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Determines whether two specified pixels have the same value.
        /// </summary>
        /// <param name="left">The first pixel to compare.</param>
        /// <param name="right">The first pixel to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Pixel? left, Pixel? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (right is null || left is null)
            {
                return false;
            }

            return (left.X.Equals(right.X) && left.Y.Equals(right.Y));
        }

        /// <summary>
        /// Determines whether two specified pixels don't have the same value.
        /// </summary>
        /// <param name="left">The first pixel to compare.</param>
        /// <param name="right">The first pixel to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(Pixel? left, Pixel? right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        /// <summary>
        /// Parses a pixel from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Pixel? Parse(string? value)
        {
            if (value != null)
            {
                if (value.Contains("[") || value.Contains("]"))
                {
                    value = value.Replace("[", "").Replace("]", "");
                }

                var parts = value.Split(',');

                if (parts.Length == 2 && 
                    double.TryParse(parts[0].Trim(), out double x) && 
                    double.TryParse(parts[1].Trim(), out double y))
                {
                    return new Pixel(x, y);
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a pixel from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public static bool TryParse(string? value, out Pixel? pixel)
        {
            pixel = Parse(value);
            return pixel != null;
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
                    0 => X,
                    1 => Y,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        /// <summary>
        /// Calculates the distance to another pixel.
        /// </summary>
        /// <param name="other">The second pixel.</param>
        /// <returns>Returned value is in screen pixel units.</returns>
        public double GetDistance(Pixel other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        /// <summary>
        /// Calculates the heading between to another pixel.​ The heading value is relative to the y-axis (0 = north) with clockwise-rotation.
        /// </summary>
        /// <param name="destination">The pixel the heading will point toward.​</param>
        /// <returns></returns>
        public double GetHeading(Pixel destination)
        {
            return Math.Atan2(destination.Y - Y, destination.X - X) * 180 / Math.PI;
        }

        /// <summary>
        /// Calculates a destination pixel given an origin pixel,​
        /// a heading relative to the y-axis(0 = north) with clockwise-rotation,​
        /// and a distance in pixel units.​
        /// </summary>
        /// <param name="heading">The heading at which to move away from the origin pixel.​</param>
        /// <param name="distance">The distance to move from the origin pixel.​</param>
        /// <returns></returns>
        public Pixel GetDestination(double heading, double distance)
        {
            return new Pixel(
               X + (int)Math.Round(distance * Math.Cos((heading + 270) * Math.PI / 180)),
               Y + (int)Math.Round(distance * Math.Sin((heading + 270) * Math.PI / 180))
           );
        }
        
        #endregion
    }
}
