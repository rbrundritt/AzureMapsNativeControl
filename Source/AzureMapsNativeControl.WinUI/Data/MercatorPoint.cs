using System;
using System.Collections.Generic;
using System.Linq;
using static AzureMapsNativeControl.AtlasMath;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A `MercatorPoint` object represents a projected three dimensional position.
    ///
    /// `MercatorPoint` uses the web mercator projection ([EPSG:3857](https://epsg.io/3857)) with slightly different units:
    /// - the size of 1 unit is the width of the projected world instead of the "mercator meter"
    /// - the origin of the coordinate space is at the north-west corner instead of the middle.
    ///
    /// For example, `MercatorPoint(0, 0, 0)` is the north-west corner of the mercator world and
    /// `MercatorPoint(1, 1, 0)` is the south-east corner. If you are familiar with
    /// [vector tiles](https://github.com/mapbox/vector-tile-spec) it may be helpful to think
    /// of the coordinate space as the `0/0/0` tile with an extent of `1`.
    ///
    /// The `z` dimension of `MercatorPoint` is conformal. A cube in the mercator coordinate space would be rendered as a cube.
    /// </summary>
    public class MercatorPoint
    {
        #region Constructor

        /// <summary>
        /// A `MercatorPoint` object represents a projected three dimensional position.
        ///
        /// `MercatorPoint` uses the web mercator projection ([EPSG:3857](https://epsg.io/3857)) with slightly different units:
        /// - the size of 1 unit is the width of the projected world instead of the "mercator meter"
        /// - the origin of the coordinate space is at the north-west corner instead of the middle.
        ///
        /// For example, `MercatorPoint(0, 0, 0)` is the north-west corner of the mercator world and
        /// `MercatorPoint(1, 1, 0)` is the south-east corner. If you are familiar with
        /// [vector tiles](https://github.com/mapbox/vector-tile-spec) it may be helpful to think
        /// of the coordinate space as the `0/0/0` tile with an extent of `1`.
        ///
        /// The `z` dimension of `MercatorPoint` is conformal. A cube in the mercator coordinate space would be rendered as a cube.
        /// </summary>
        /// <param name="x">A points x position in mercator units.</param>
        /// <param name="y">A points y position in mercator units.</param>
        /// <param name="z">A points z position in mercator units.</param>
        public MercatorPoint(double x, double y, double? z = null)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The x coordinate of the point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The y coordinate of the point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The z coordinate of the point.
        /// </summary>
        public double? Z { get; set; }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Converts a position into a mercator point.
        /// </summary>
        /// <param name="position">Position to convert.</param>
        /// <returns>A mercator point.</returns>
        public static MercatorPoint FromPosition(Position position)
        {
            return new MercatorPoint(
                (180 + position[0]) / 360,
                MercatorPoint._latToMercatorY(position[1]),
                position.Altitude != null ? MercatorPoint.AltitudeToMercatorZ(position[2], position[1]) : null
            );
        }

        /// <summary>
        /// Converts a list of positions into a list of mercator points.
        /// </summary>
        /// <param name="positions">Positions to convert.</param>
        /// <returns>A list of mercator points.</returns>
        public static IList<MercatorPoint> FromPositions(IEnumerable<Position> positions)
        {
            return positions.Select(MercatorPoint.FromPosition).ToList();
        }

        /// <summary>
        /// Converts a mercator point into a position.
        /// </summary>
        /// <param name="mercator">Mercator point to convert.</param>
        /// <returns>A position</returns>
        public static Position ToPosition(MercatorPoint mercator)
        {
            var lat = (360 / Math.PI) * Math.Atan(Math.Exp(((180 - mercator.Y * 360) * Math.PI) / 180)) - 90;

            return new Position(
                mercator.X * 360 - 180,
                lat,
                mercator.Z != null ? MercatorPoint.MercatorZToAltitude(mercator.Z ?? 0, mercator.Y) : null
            );
        }

        /// <summary>
        /// Converts a list of mercator points into a list of positions.
        /// </summary>
        /// <param name="mercators">Mercator points to convert.</param>
        /// <returns>A list of positions</returns>
        public static IList<Position> ToPositions(IEnumerable<MercatorPoint> mercators)
        {
            return mercators.Select(MercatorPoint.ToPosition).ToList();
        }

        /// <summary>
        /// Determine the Mercator scale factor for a given latitude, see https://en.wikipedia.org/wiki/Mercator_projection#Scale_factor
        /// At the equator the scale factor will be 1, which increases at higher latitudes.
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static double MercatorScale(double latitude)
        {
            return 1 / Math.Cos((latitude * Math.PI) / 180);
        }

        /// <summary>
        /// Returns the distance of 1 meter in `MercatorPoint` units at this latitude.
        /// For coordinates in real world units using meters, this naturally provides the scale to transform into `MercatorPoint`s.
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns>Distance of 1 meter in `MercatorPoint` units.</returns>
        public static double MeterInMercatorUnits(double latitude)
        {
            // 1 meter / circumference at equator in meters * Mercator projection scale factor at this latitude
            return (1 / EarthCircumferenceMeters) * MercatorScale(_latToMercatorY(latitude));
        }

        #endregion

        #region Private Static Methods

        private const double EarthCircumferenceMeters = 2 * Math.PI * EarthRadius.Meters;

        private static double _latToMercatorY(double latitude)
        {
            return (180 - (180 / Math.PI) * Math.Log(Math.Tan(Math.PI / 4 + (latitude * Math.PI) / 360))) / 360;
        }

        /// <summary>
        /// The circumference at a line of latitude in meters.
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        private static double CircumferenceAtLatitude(double latitude)
        {
            return EarthCircumferenceMeters * Math.Cos((latitude * Math.PI) / 180);
        }

        private static double AltitudeToMercatorZ(double altitude, double latitude)
        {
            return altitude / CircumferenceAtLatitude(latitude);
        }

        private static double MercatorZToAltitude(double zoom, double y)
        {
            return zoom * CircumferenceAtLatitude(MercatorPoint._latToMercatorY(y));
        }

        #endregion
    }
}
