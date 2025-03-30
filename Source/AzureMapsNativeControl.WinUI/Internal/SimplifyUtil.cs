using AzureMapsNativeControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// A class for simplifing a list of positions or pixels.
    /// </summary>
    internal static class SimplifyUtil
    {
        #region Public Methods

        /// <summary>
        /// Perform a Douglas-Peucker simplification on an array of positions.
        /// </summary>
        /// <param name="positions">The position or pixel points to simplify.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns>A simplified array of positions.</returns>
        public static IList<Position> Run(IList<Position> positions, double tolerance)
        {
            if (positions.Count <= 2)
            {
                return positions;
            }

            double sqTolerance = tolerance * tolerance;

            return SimplifyDouglasPeucker(positions, sqTolerance);
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on an array of pixels.
        /// </summary>
        /// <param name="pixels">The position or pixel points to simplify.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns>A simplified array of pixels.</returns>
        public static IList<Pixel> Run(IList<Pixel> pixels, double tolerance)
        {
            if (pixels.Count <= 2)
            {
                return pixels;
            }

            double sqTolerance = tolerance * tolerance;

            return SimplifyDouglasPeucker(pixels, sqTolerance); 
        }

        #endregion

        #region Private Methods

        private static IList<Position> SimplifyDouglasPeucker(IList<Position> points, double sqTolerance)
        {
            int last = points.Count() - 1;
            List<Position> simplified = [points.First()];

            SimplifyDPStep(points, 0, last, sqTolerance, simplified);

            simplified.Add(points.Last());

            return simplified;
        }

        private static void SimplifyDPStep(IList<Position> points, int first, int last, double sqTolerance, IList<Position> simplified)
        {
            double maxSqDist = sqTolerance;
            int index = 0;

            for (int i = first + 1; i < last; i++)
            {
                var sqDist = GetSquaredSegmentDistance(points[i].Longitude, points[i].Latitude, points[first].Longitude, points[first].Latitude, points[last].Longitude, points[last].Latitude);

                if (sqDist > maxSqDist)
                {
                    index = i;
                    maxSqDist = sqDist;
                }
            }

            if (maxSqDist > sqTolerance)
            {
                if (index - first > 1)
                {
                    SimplifyDPStep(points, first, index, sqTolerance, simplified);
                }

                simplified.Add(points[index]);

                if (last - index > 1)
                {
                    SimplifyDPStep(points, index, last, sqTolerance, simplified);
                }
            }
        }

        private static IList<Pixel> SimplifyDouglasPeucker(IList<Pixel> points, double sqTolerance)
        {
            int last = points.Count() - 1;
            List<Pixel> simplified = [points.First()];

            SimplifyDPStep(points, 0, last, sqTolerance, simplified);

            simplified.Add(points.Last());

            return simplified;
        }

        private static void SimplifyDPStep(IList<Pixel> points, int first, int last, double sqTolerance, IList<Pixel> simplified)
        {
            double maxSqDist = sqTolerance;
            int index = 0;

            for (int i = first + 1; i < last; i++)
            {
                var sqDist = GetSquaredSegmentDistance(points[i][0], points[i][1], points[first][0], points[first][1], points[last][0], points[last][1]);

                if (sqDist > maxSqDist)
                {
                    index = i;
                    maxSqDist = sqDist;
                }
            }

            if (maxSqDist > sqTolerance)
            {
                if (index - first > 1)
                {
                    SimplifyDPStep(points, first, index, sqTolerance, simplified);
                }

                simplified.Add(points[index]);

                if (last - index > 1)
                {
                    SimplifyDPStep(points, index, last, sqTolerance, simplified);
                }
            }
        }

        /// <summary>
        /// Calculate the squared distance between a point and a segment.
        /// </summary>
        /// <param name="p1x"></param>
        /// <param name="p1y"></param>
        /// <param name="p2x"></param>
        /// <param name="p2y"></param>
        /// <param name="p3x"></param>
        /// <param name="p3y"></param>
        /// <returns></returns>
        private static double GetSquaredSegmentDistance(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y)
        {
            double lon = p2x;
            double lat = p2y;
            double dlong = p3x - lon;
            double dlat = p3y - lat;

            if (dlong != 0 || dlat != 0)
            {
                double t = ((p1x - lon) * dlong + (p1y - lat) * dlat) / GetAddedSquares(dlong, dlat);

                if (t > 1)
                {
                    lon = p3x;
                    lat = p3x;
                }
                else if (t > 0)
                {
                    lon += dlong * t;
                    lat += dlat * t;
                }
            }

            return GetAddedSquares(p1x - lon, p1y - lat);
        }

        /// <summary>
        /// Given two values, return the sum of their squares.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double GetAddedSquares(double x, double y)
        {
            return x * x + y * y;
        }

        #endregion
    }
}
