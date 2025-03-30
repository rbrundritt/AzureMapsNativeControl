using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Tiles;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// A set of spatial math functions.
    /// </summary>
    public static class AtlasMath
    {
        #region Earth Related Constants

        /// <summary>
        /// The approximate spherical radius of the Earth
        /// NOTE: In reality the Earth is an ellipsoid.
        /// </summary>
        internal static class EarthRadius
        {
            /// <summary>
            /// Earth Radius in Kilometers
            /// </summary>
            public const double KM = 6378.137;

            /// <summary>
            /// Earth Radius in Meters
            /// </summary>
            public const double Meters = 6378137;

            /// <summary>
            /// Earth Radius in Miles
            /// </summary>
            public const double Miles = 3963.19;

            /// <summary>
            /// Earth Radius in Feet
            /// </summary>
            public const double Feet = 20925646.357;
        }

        #endregion

        #region Earth Radius

        /// <summary>
        /// Retrieves the radius of the earth in a specific distance unit for WGS84. Defaults unit is in Meters.
        /// </summary>
        /// <param name="units">Unit of distance measurement</param>
        /// <returns>A double that represents the radius of the earth in a specific distance unit. Defaults unit is in KM's.</returns>
        public static double GetEarthRadius(DistanceUnits units)
        {
            switch (units)
            {
                case DistanceUnits.Feet:
                    return EarthRadius.Feet;
                case DistanceUnits.Miles:
                    return EarthRadius.Miles;
                case DistanceUnits.Yards:
                    return ConvertDistance(EarthRadius.KM, DistanceUnits.Kilometers, DistanceUnits.Yards);
                case DistanceUnits.Kilometers:
                    return EarthRadius.KM;
                case DistanceUnits.Meters:
                default:
                    return EarthRadius.Meters;
            }
        }

        #endregion

        #region Degree and Radian Conversions

        /// <summary>
        /// Converts an angle that is in degrees to radians. Angle * (PI / 180)
        /// </summary>
        /// <param name="angle">An angle in degrees</param>
        /// <returns>An angle in radians</returns>
        public static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }

        /// <summary>
        /// Converts an angle that is in radians to degress. Angle * (180 / PI)
        /// </summary>
        /// <param name="angle">An angle in radians</param>
        /// <returns>An angle in degrees</returns>
        public static double ToDegrees(double angle)
        {
            return angle * (180 / Math.PI);
        }

        #endregion

        #region Decimal Degree and Degree Minute Second Converstions

        /// <summary>
        /// Converts a decimal degree into a string in the format of days minutes seconds
        /// </summary>
        /// <param name="degree">Decimal degree</param>
        /// <param name="isLatitude">Boolean specifying if the degree is a latitude coordinate</param>
        /// <returns>A string version of an angle in days, minutes, seconds format.</returns>
        public static string DecimalDegreeToDMS(double degree, bool isLatitude)
        {
            var orientation = "";

            if (isLatitude)
            {
                if (degree < 0)
                {
                    orientation = "S";
                    degree *= -1;
                }
                else
                {
                    orientation = "N";
                }
            }
            else
            {
                if (degree < 0)
                {
                    orientation = "W";
                    degree *= -1;
                }
                else
                {
                    orientation = "E";
                }
            }

            int day = (int)degree;
            int min = (int)((degree - day) * 60);
            double sec = (degree - (double)day - (double)min / 60) * 3600;

            return string.Format("{0} {1}° {2}' {3}\"", orientation, day, min, sec);
        }

        /// <summary>
        /// Converts a days minutes seconds coordinate into a decimal degree's coordinate
        /// </summary>
        /// <param name="degree">Degree coordinate</param>
        /// <param name="minute">Minute coordinate</param>
        /// <param name="second">Second coordinate</param>
        /// <returns>A decimal degree coordinate</returns>
        public static double DMSToDecimalDegree(double degree, double minute, double second)
        {
            return degree + (minute / 60) + (second / 3600);
        }

        #endregion

        #region Distance Calculations

        /// <summary>
        /// Converts string distance unit name to DistanceUnits enum. Defaults to meters if not recognized.
        /// Supports multiple fall variations; 
        /// - "kilometers", "kilometer", "kilometres", "kilometre", "km", "kms" all return DistanceUnits.Kilometers.
        /// - "meters", "metres", "m" all return DistanceUnits.Meters.
        /// - "miles", "mile", "mi" all return DistanceUnits.Miles.
        /// - "nauticalmiles", "nauticalmile", "nms", "nm" all return DistanceUnits.NauticalMiles.
        /// - "yards", "yard", "yds", "yrd", "yrds" all return DistanceUnits.Yards.
        /// - "feet", "foot", "ft" all return DistanceUnits.Feet.
        /// </summary>
        /// <param name="units"></param>
        /// <returns>Either a matching distance unit type or Meters</returns>
        public static DistanceUnits GetDistanceUnit(string? units)
        {
            if (units != null)
            {
                switch (units.ToLower())
                {
                    case "feet":
                    case "foot":
                    case "ft":
                        return DistanceUnits.Feet;
                    case "kilometers":
                    case "kilometer":
                    case "kilometres":
                    case "kilometre":
                    case "km":
                    case "kms":
                        return DistanceUnits.Kilometers;
                    case "miles":
                    case "mile":
                    case "mi":
                        return DistanceUnits.Miles;
                    case "nauticalmiles":
                    case "nauticalmile":
                    case "nms":
                    case "nm":
                        return DistanceUnits.NauticalMiles;
                    case "yards":
                    case "yard":
                    case "yds":
                    case "yrd":
                    case "yrds":
                        return DistanceUnits.Yards;
                    case "meters":
                    case "metres":
                    case "m":
                    default:
                        return DistanceUnits.Meters;
                }
            }

            return DistanceUnits.Meters;
        }

        /// <summary>
        /// Converts a distance from distance unit to another.
        /// </summary>
        /// <param name="distance">a double that represents the distance.</param>
        /// <param name="fromUnits">The distance unit the original distance is in.</param>
        /// <param name="toUnits">The disired distance unit to convert to.</param>
        /// <param name="decimals">The number of decimal places to round the result to. Default is 10.</param>
        /// <returns>A distance in the new units.</returns>
        public static double ConvertDistance(double distance, DistanceUnits fromUnits, DistanceUnits toUnits, int decimals = 10)
        {
            //Convert the distance to kilometers
            switch (fromUnits)
            {
                case DistanceUnits.Meters:
                    distance /= 1000;
                    break;
                case DistanceUnits.Feet:
                    distance /= 3288.839895;
                    break;
                case DistanceUnits.Miles:
                    distance *= 1.609344;
                    break;
                case DistanceUnits.Yards:
                    distance *= 0.0009144;
                    break;
                case DistanceUnits.NauticalMiles:
                    distance /= 0.5399568;
                    break;
                case DistanceUnits.Kilometers:
                    break;
            }

            //Convert from kilometers to output distance unit
            switch (toUnits)
            {
                case DistanceUnits.Meters:
                    distance *= 1000;
                    break;
                case DistanceUnits.Feet:
                    distance *= 5280;
                    break;
                case DistanceUnits.Miles:
                    distance /= 1.609344;
                    break;
                case DistanceUnits.Yards:
                    distance *= 1093.6133;
                    break;
                case DistanceUnits.NauticalMiles:
                    distance *= 0.5399568;
                    break;
                case DistanceUnits.Kilometers:
                    break;
            }

            return Math.Round(distance, decimals);
        }

        /// <summary>
        /// Calculate the distance between two coordinates on the surface of a sphere (Earth).
        /// </summary>
        /// <param name="origin">First coordinate to calculate distance between</param>
        /// <param name="destination">Second coordinate to calculate distance between</param>
        /// <param name="units">Unit of distance measurement. Default: Meters</param>
        /// <returns>The shortest distance in the specifed units</returns>
        public static double GetDistanceTo(Position origin, Position destination, DistanceUnits units = DistanceUnits.Meters)
        {
            double radius = GetEarthRadius(units);

            double dLat = ToRadians(destination.Latitude - origin.Latitude);
            double dLon = ToRadians(destination.Longitude - origin.Longitude);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(ToRadians(origin.Latitude)) * Math.Cos(ToRadians(destination.Latitude)) * Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return radius * c;
        }

        /// <summary>
        /// Calculates the distance between all position objects in an array.
        /// </summary>
        /// <param name="path">The array of position objects that make up the path to calculate the length of.</param>
        /// <param name="units">Unit of distance measurement. Default: Meters</param>
        /// <returns>The distance between all positions in between all position objects in an array on the surface of a earth in the specified units.</returns>
        public static double GetLengthOfPath(IList<Position> path, DistanceUnits units = DistanceUnits.Meters) {
            double totalLength = 0;

            if (path.Count() >= 2)
            {
                for(int i = 1; i < path.Count(); i++)
                {
                    totalLength += GetDistanceTo(path[i - 1], path[i], units);
                }
            }

            return totalLength;
        }

        #endregion

        #region Speed, Time & Acceleration Calculations

        /// <summary>
        /// Converts a string speed unit value to a SpeedUnit enum. Supports multiple common variations for each speed unit type. 
        /// Defaults to MetersPerSecond if not recognized.
        /// </summary>
        /// <param name="units">String name of the units</param>
        /// <returns></returns>
        public static SpeedUnits GetSpeedUnit(string units)
        {
            if (units != null)
            {
                switch (units.ToLowerInvariant().Replace(" ", ""))
                {
                    case "feetpersecond":
                    case "footsecond":
                    case "ftps":
                    case "ft/s":
                        return SpeedUnits.FeetPerSecond;
                    case "milesperhour":
                    case "mileperhour":
                    case "mph":
                    case "mi/hr":
                    case "mi/h":
                        return SpeedUnits.MilesPerHour;
                    case "knots":
                    case "knot":
                    case "knts":
                    case "knt":
                    case "kn":
                    case "kt":
                        return SpeedUnits.Knots;
                    case "mach":
                    case "m":
                        return SpeedUnits.Mach;
                    case "kilometersperhour":
                    case "kilometresperhour":
                    case "kmperhour":
                    case "kmph":
                    case "km/hr":
                    case "km/h":
                        return SpeedUnits.KilometersPerHour;
                    case "meterspersecond":
                    case "metrespersecond":
                    case "mps":
                    case "ms":
                    case "m/s":
                    default:
                        return SpeedUnits.MetersPerSecond;
                }
            }

            return SpeedUnits.MetersPerSecond;
        }

        /// <summary>
        /// Converts a string acceleration unit value to a AccelerationUnit enum. Supports multiple common variations for each acceleration unit type.
        /// </summary>
        /// <param name="units">String name of the units</param>
        /// <returns></returns>
        public static AccelerationUnits GetAccelerationUnits(string units)
        {
            // Convert to metersPerSecondSquared
            if (!string.IsNullOrEmpty(units))
            {
                switch (units.ToLowerInvariant().Replace(" ", ""))
                {
                    case "milespersecondsquared":       // mi/s^2
                    case "milepersecondsquared":
                    case "mi/s^2":
                    case "mi/s2":
                        return AccelerationUnits.MilesPerSecondSquared;
                    case "kilometerspersecondsquared":  // km/s^2
                    case "kilometrespersecondsquared":
                    case "kilometerpersecondsquared":
                    case "kilometrepersecondsquared":
                    case "km/s^2":
                    case "km/s2":
                        return AccelerationUnits.KilometersPerSecondSquared;
                    case "knotspersecond":              // knts/s
                    case "knotpersecond":
                    case "knts/s":
                    case "kn/s":
                    case "kt/s":
                        return AccelerationUnits.KnotsPerSecond;
                    case "standardgravity":             // g
                    case "g":
                        return AccelerationUnits.StandardGravity;
                    case "feetpersecondsquared":        // ft/s^2
                    case "footpersecondsquared":
                    case "ft/s^2":
                    case "ft/s2":
                        return AccelerationUnits.FeetPerSecondSquared;
                    case "yardspersecondsquared":       // yds/s^2
                    case "yardpersecondsquared":
                    case "yds/s^2":
                    case "yds/s2":
                    case "yd/s^2":
                    case "yd/s2":
                        return AccelerationUnits.YardsPerSecondSquared;
                    case "milesperhoursecond":          // mi/h/s
                    case "mileperhoursecond":
                    case "milesperhourseconds":
                    case "mileperhourseconds":
                    case "mi/h/s":
                        return AccelerationUnits.MilesPerHourSecond;
                    case "kilometersperhoursecond":    // km/h/s
                    case "kilometrespersoursecond":
                    case "kilometerperhoursecond":
                    case "kilometrepersoursecond":
                    case "kilometersperhourssecond":
                    case "kilometrespersourssecond":
                    case "kilometerperhourssecond":
                    case "kilometrepersourssecond":
                    case "kmhs":
                    case "km/h/s":
                        return AccelerationUnits.KilometersPerHourSecond;
                    case "meterspersecondsquared":      // m/s^2
                    case "metrespersecondsquared":
                    case "meterpersecondsquared":
                    case "metrepersecondsquared":
                    case "m/s^2":
                    case "m/s2":
                    default:
                        return AccelerationUnits.MetersPerSecondSquared;
                }
            }

            return AccelerationUnits.MetersPerSecondSquared;
        }

        /// <summary>
        /// Converts a speed value from one unit to another.
        /// </summary>
        /// <param name="speed">The speed value to convert.</param>
        /// <param name="fromUnits">The speed unit the original distance is in.</param>
        /// <param name="toUnits">The disired speed unit to convert to.</param>
        /// <param name="decimals">The number of decimal places to round the result to. Default is 10.</param>
        /// <returns>A speed in the new units.</returns>
        public static double ConvertSpeed(double speed, SpeedUnits fromUnits, SpeedUnits toUnits = SpeedUnits.MetersPerSecond, int decimals = 10)
        {
            //Convert the speed to meters per second
            switch (fromUnits)
            {
                case SpeedUnits.MetersPerSecond:
                    break;
                case SpeedUnits.FeetPerSecond:
                    speed *= 0.3048;
                    break;
                case SpeedUnits.MilesPerHour:
                    speed *= 0.44704;
                    break;
                case SpeedUnits.KilometersPerHour:
                    speed *= 0.277778;
                    break;
                case SpeedUnits.Knots:
                    speed *= 0.514444;
                    break;
                case SpeedUnits.Mach:
                    speed *= 343.2; //Speed of sound at sea level
                    break;
            }

            //Convert from meters per second to output distance unit
            switch (toUnits)
            {
                case SpeedUnits.MetersPerSecond:
                    break;
                case SpeedUnits.FeetPerSecond:
                    speed /= 0.3048;
                    break;
                case SpeedUnits.MilesPerHour:
                    speed /= 0.44704;
                    break;
                case SpeedUnits.KilometersPerHour:
                    speed /= 0.277778;
                    break;
                case SpeedUnits.Knots:
                    speed /= 0.514444;
                    break;
                case SpeedUnits.Mach:
                    speed /= 343.2; //Speed of sound at sea level
                    break;
            }

            return Math.Round(speed, decimals);
        }

        /// <summary>
        /// Converts a acceleration value from one unit to another.
        /// </summary>
        /// <param name="acceleration">The acceleration value to convert.</param>
        /// <param name="fromUnits">The acceleration units the value is in.</param>
        /// <param name="toUnits">The acceleration units to convert to.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>An acceleration value convertered from one unit to another.</returns>
        public static double ConvertAcceleration(double acceleration, AccelerationUnits fromUnits, AccelerationUnits toUnits = AccelerationUnits.MetersPerSecondSquared, int decimals = 10)
        {
            //Convert the acceleration to meters per second squared
            switch (fromUnits)
            {
                case AccelerationUnits.KilometersPerHourSecond:
                    acceleration /= 3.6;
                    break;
                case AccelerationUnits.MilesPerHourSecond:
                    acceleration /= 2.236936292054;
                    break;
                case AccelerationUnits.KnotsPerSecond:
                    acceleration /= 1.943844492441;
                    break;
                case AccelerationUnits.StandardGravity:
                    acceleration /= 0.1019716212978;
                    break;
                case AccelerationUnits.KilometersPerSecondSquared:
                    acceleration /= 0.001;
                    break;
                case AccelerationUnits.MilesPerSecondSquared:
                    acceleration /= 0.000621371192;
                    break;
                case AccelerationUnits.FeetPerSecondSquared:
                    acceleration /= 3.280839895012;
                    break;
                case AccelerationUnits.YardsPerSecondSquared:
                    acceleration /= 1.093613298338;
                    break;
                case AccelerationUnits.MetersPerSecondSquared:
                default:
                    break;
            }

            //Convert from meters per second squared to output distance unit
            switch (toUnits)
            {
                case AccelerationUnits.KilometersPerHourSecond:
                    acceleration *= 3.6;
                    break;
                case AccelerationUnits.MilesPerHourSecond:
                    acceleration *= 2.236936292054;
                    break;
                case AccelerationUnits.KnotsPerSecond:
                    acceleration *= 1.943844492441;
                    break;
                case AccelerationUnits.StandardGravity:
                    acceleration *= 0.1019716212978;
                    break;
                case AccelerationUnits.KilometersPerSecondSquared:
                    acceleration *= 0.001;
                    break;
                case AccelerationUnits.MilesPerSecondSquared:
                    acceleration *= 0.000621371192;
                    break;
                case AccelerationUnits.FeetPerSecondSquared:
                    acceleration *= 3.280839895012;
                    break;
                case AccelerationUnits.YardsPerSecondSquared:
                    acceleration *= 1.093613298338;
                    break;
                case AccelerationUnits.MetersPerSecondSquared:
                default:
                    break;
            }

            return Math.Round(acceleration, decimals);
        }

        /// <summary>
        /// Converts a timespan value from one unit to another.
        /// </summary>
        /// <param name="timespan">Time span value.</param>
        /// <param name="timeUnit">Timespan unit name</param>
        /// <returns>A .NET TimeSpan</returns>
        public static TimeSpan ConvertTimeSpan(double timespan, string timeUnit)
        {
            if (!string.IsNullOrEmpty(timeUnit))
            {
                switch (timeUnit.ToLowerInvariant())
                {
                    case "milliseconds":
                    case "ms":
                        return TimeSpan.FromMilliseconds(timespan);
                    case "minutes":
                    case "minute":
                    case "mins":
                    case "min":
                        return TimeSpan.FromMinutes(timespan);
                    case "hours":
                    case "hour":
                    case "hr":
                    case "h":
                        return TimeSpan.FromHours(timespan);
                    case "days":
                    case "day":
                    case "d":
                        return TimeSpan.FromDays(timespan);
                    case "seconds":
                    case "second":
                    case "secs":
                    case "sec":
                    case "s":
                    default:
                        break;
                }
            }

            return TimeSpan.FromSeconds(timespan);
        }

        /// <summary>
        /// Calculates the average speed of travel between two points based on the provided amount of time.
        /// </summary>
        /// <param name="origin">The initial point in which the speed is calculated from.</param>
        /// <param name="destination">The final point in which the speed is calculated from.</param>
        /// <param name="timespan">The time take to travel between the start and end points.</param>
        /// <param name="speedUnits">The units to return the speed value in. If not specified m/s are used.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>The average speed of travel between two points based on the provided amount of time.</returns>
        public static double GetSpeed(
            Position origin, 
            Position destination, 
            TimeSpan timespan, 
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond, 
            int decimals = 10)
        {
            double d = GetDistanceTo(origin, destination, DistanceUnits.Meters);
            double t = timespan.TotalSeconds;
            return ConvertSpeed(d / t, SpeedUnits.MetersPerSecond, speedUnits, decimals);
        }

        /// <summary>
        /// Calculates the average speed of travel between two point features that have a timestamp property.
        /// </summary>
        /// <param name="origin">The initial point in which the speed is calculated from.</param>
        /// <param name="destination">The final point in which the speed is calculated from.</param>
        /// <param name="timestampProperty">The name of the property on the features which has the timestamp information.</param>
        /// <param name="speedUnits">The units to return the speed value in. If not specified m/s are used.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>The speed in the specified units or NaN if valid timestamps are not found.</returns>
        public static double GetSpeedFromFeatures(
            Feature origin,
            Feature destination,
            string timestampProperty,
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond,
            int decimals = 10)
        {
            if(origin.Geometry.Type != GeoJsonType.Point)
            {
                throw new ArgumentException("Origin feature must be a point feature.");
            }

            if (destination.Geometry.Type != GeoJsonType.Point)
            {
                throw new ArgumentException("Destination feature must be a point feature.");
            }

            if (origin.Properties == null || !origin.Properties.ContainsKey(timestampProperty))
            {
                throw new ArgumentException("Origin feature must have the timestamp property.");
            }

            if (destination.Properties == null || !destination.Properties.ContainsKey(timestampProperty))
            {
                throw new ArgumentException("Destination feature must have the timestamp property.");
            }

            if (origin.Properties.ContainsKey(timestampProperty) && destination.Properties.ContainsKey(timestampProperty))
            {
                var startTime = FromJsonDateTime(origin.Properties[timestampProperty]);
                var endTime = FromJsonDateTime(destination.Properties[timestampProperty]);

                if (startTime == null)
                {
                    throw new ArgumentException("Unable to parse timestamp from Origin");
                }

                if (endTime == null)
                {
                    throw new ArgumentException("Unable to parse timestamp from Destination");
                }

                TimeSpan dt = endTime.Value - startTime.Value;
                double d = GetDistanceTo(((PointGeometry)origin.Geometry).Coordinates, ((PointGeometry)destination.Geometry).Coordinates, DistanceUnits.Meters);
                double t = dt.TotalSeconds;

                return ConvertSpeed(d / t, SpeedUnits.MetersPerSecond, speedUnits, decimals);
            }

            return double.NaN;
        }

        /// <summary>
        /// Calculates an acceleration based on an initial speed, travel distance and timespan. Formula: a = 2*(d - v*t)/t^2
        /// </summary>
        /// <param name="initialSpeed">The initial speed.</param>
        /// <param name="distance">The distance that has been travelled.</param>
        /// <param name="timespan">The timespan that was travelled.</param>
        /// <param name="distanceUnits">The units of the distance information. If not specified meters are used.</param>
        /// <param name="speedUnits">The units of the speed information. If not specified m/s are used.</param>
        /// <param name="accelerationUnits">The units to return the acceleration value in. If not specified m/s^2 are used.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>An acceleration based on an initial speed, travel distance and timespan.</returns>
        public static double GetAcceleration(
            double initialSpeed, 
            double distance, 
            TimeSpan timespan,
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond, 
            DistanceUnits distanceUnits = DistanceUnits.Meters,
            AccelerationUnits accelerationUnits = AccelerationUnits.MetersPerSecondSquared,
            int decimals = 10) 
        {
            double d = ConvertDistance(distance, distanceUnits, DistanceUnits.Meters);            
            double v = ConvertSpeed(initialSpeed, speedUnits, SpeedUnits.MetersPerSecond);
            double t = timespan.TotalSeconds;

            return ConvertAcceleration(2 * (d - v * t) / (t * t),
                AccelerationUnits.MetersPerSecondSquared, accelerationUnits, decimals);
        }

        /// <summary>
        /// Calculates the distance traveled for a specified timespan, speed and optionally an acceleration.
        /// Formula: d = v* t + 0.5*a* t^2
        /// </summary>
        /// <param name="speed">The initial or constant speed.</param>
        /// <param name="timespan">The timespan to calculate the distance for.</param>
        /// <param name="acceleration">An acceleration which increases the speed over time.</param>
        /// <param name="speedUnits">The units of the speed value. If not specified m/s are used.</param>
        /// <param name="accelerationUnits">The units of the acceleration value. If not specified m/s^2 are used.</param>
        /// <param name="distanceUnits"> The distance units in which to return the distance in.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>The distance traveled for a specified timespan, speed and optionally an acceleration.</returns>
        public static double GetTravelDistance(
            double speed,
            TimeSpan timespan,
            double acceleration = 0,
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond,
            AccelerationUnits accelerationUnits = AccelerationUnits.MetersPerSecondSquared,
            DistanceUnits distanceUnits = DistanceUnits.Meters,
            int decimals = 10)
        {
            double v = ConvertSpeed(speed, speedUnits, SpeedUnits.MetersPerSecond);
            double t = timespan.TotalSeconds;
            double a = ConvertAcceleration(acceleration, accelerationUnits, AccelerationUnits.MetersPerSecondSquared);
            return ConvertDistance(v * t + 0.5 * a * t * t,
                DistanceUnits.Meters, distanceUnits, decimals);
        }

        /// <summary>
        /// Calculates an acceleration based on an initial speed, final speed and timespan. Formula: a = 2* (v2 - v1)/t
        /// </summary>
        /// <param name="initialSpeed">The initial speed.</param>
        /// <param name="finalSpeed">The final speed.</param>
        /// <param name="timespan">The timespan that was travelled.</param>
        /// <param name="speedUnits">The units of the speed information. If not specified meters are used.</param>
        /// <param name="accelerationUnits">The units to return the acceleration value in. If not specified m/s^2 are used.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>An acceleration based on an initial speed, final speed and timespan.</returns>
        public static double GetAccelerationFromSpeeds(
            double initialSpeed, 
            double finalSpeed, 
            TimeSpan timespan,
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond,
            AccelerationUnits accelerationUnits = AccelerationUnits.MetersPerSecondSquared, 
            int decimals = 10)
        {
            double v1 = ConvertSpeed(initialSpeed, speedUnits, SpeedUnits.MetersPerSecond);
            double v2 = ConvertSpeed(finalSpeed, speedUnits, SpeedUnits.MetersPerSecond);
            double t = timespan.TotalSeconds;

            return ConvertAcceleration((v2 - v1) / t,
                AccelerationUnits.MetersPerSecondSquared, accelerationUnits, decimals);
        }

        /// <summary>
        /// Calculates an acceleration between two point features that have a timestamp property and optionally a speed property.
        /// if speeds are provided, ignore distance between points as the path may not have been straight and calculate: a = (v2 - v1)/(t2 - t1)
        /// if speeds not provided or only provided on first point, calculate straight line distance between points and calculate: a = 2*(d - v*t)/t^2
        /// </summary>
        /// <param name="origin">The initial point in which the acceleration is calculated from.</param>
        /// <param name="destination">The final point in which the acceleration is calculated from.</param>
        /// <param name="timestampProperty">The name of the property on the features that contains the timestamp information.</param>
        /// <param name="speedProperty">The name of the property on the features that contains a speed information.</param>
        /// <param name="speedUnits">The units of the speed information. If not specified m/s is used.</param>
        /// <param name="accelerationUnits">The units to return the acceleration value in. If not specified m/s^2 are used.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>An acceleration between two point features that have a timestamp property and optionally a speed property. Returns NaN if unable to parse timestamp.</returns>
        public static double GetAccelerationFromFeatures(
            Feature origin, 
            Feature destination,
            string timestampProperty, 
            string speedProperty,
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond,
            AccelerationUnits accelerationUnits = AccelerationUnits.MetersPerSecondSquared,
            int decimals = 10)
        {
            if(origin.Geometry.Type != GeoJsonType.Point)
            {
                throw new ArgumentException("Origin feature must be a point feature.");
            }

            if (destination.Geometry.Type != GeoJsonType.Point)
            {
                throw new ArgumentException("Destination feature must be a point feature.");
            }

            if(origin.Properties == null || !origin.Properties.ContainsKey(timestampProperty))
            {
                throw new ArgumentException("Origin feature must have the timestamp property.");
            }

            if (destination.Properties == null || !destination.Properties.ContainsKey(timestampProperty))
            {
                throw new ArgumentException("Origin feature must have the timestamp property.");
            }

            if (origin.Properties.ContainsKey(timestampProperty) && destination.Properties.ContainsKey(timestampProperty))
            {
                var startTime = FromJsonDateTime(origin.Properties[timestampProperty]);
                var endTime = FromJsonDateTime(destination.Properties[timestampProperty]);

                if (startTime == null)
                {
                    throw new ArgumentException("Unable to parse timestamp from Origin");
                }

                if (endTime == null)
                {
                    throw new ArgumentException("Unable to parse timestamp from Destination");
                }

                TimeSpan dt = endTime.Value - startTime.Value;

                if (origin.Properties.ContainsKey(speedProperty) && destination.Properties.ContainsKey(speedProperty))
                {
                    var v1 = origin.Properties[speedProperty];
                    var v2 = destination.Properties[speedProperty];
                    if (v1 != null && v2 != null && double.TryParse(v1.ToString(), out double v11) && double.TryParse(v2.ToString(), out double v22))
                    {
                        return GetAccelerationFromSpeeds(v11, v22, dt, speedUnits, accelerationUnits, decimals);
                    }
                }

                //No speed info, calculate straight line distance.
                double d = GetDistanceTo(((PointGeometry)origin.Geometry).Coordinates, ((PointGeometry)destination.Geometry).Coordinates, DistanceUnits.Meters);

                //Assume acceleration from 0 speed.
                return GetAcceleration(0, d, dt, speedUnits, DistanceUnits.Meters, accelerationUnits, decimals);
            }

            return double.NaN;
        }

        #endregion

        #region Calaculate Heading

        /// <summary>
        /// Calculates the heading from one Coordinate to another.
        /// </summary>
        /// <param name="origin">Point of origin.</param>
        /// <param name="destination">Destination point to calculate relative heading to.</param>
        /// <returns>A heading degrees between 0 and 360. 0 degrees points due North.</returns>
        public static double GetHeading(Position origin, Position destination)
        {
            double radianLat1 = ToRadians(origin.Latitude);
            double radianLat2 = ToRadians(destination.Latitude);

            double dLon = ToRadians(destination.Longitude - origin.Longitude);

            double dy = Math.Sin(dLon) * Math.Cos(radianLat2);
            double dx = Math.Cos(radianLat1) * Math.Sin(radianLat2) - Math.Sin(radianLat1) * Math.Cos(radianLat2) * Math.Cos(dLon);

            return (ToDegrees(Math.Atan2(dy, dx)) + 360) % 360;
        }

        #endregion

        #region Calculate Destination Coordinate

        /// <summary>
        /// Calculates a destination coordinate based on a starting coordinate, a bearing, a distance, and a distance unit type.
        /// </summary>
        /// <param name="origin">Coordinate that the destination is relative to</param>
        /// <param name="bearing">A bearing (heading) angle between 0 - 360 degrees. 0 - North, 90 - East, 180 - South, 270 - West</param>
        /// <param name="distance">Distance that destination is away</param>
        /// <param name="units">Unit of distance measurement</param>
        /// <returns>A coordinate that is the specified distance away from the origin</returns>
        public static Position GetDestination(Position origin, double bearing, double distance, DistanceUnits units = DistanceUnits.Meters)
        {
            var radius = GetEarthRadius(units);

            //convert latitude, longitude and heading into radians
            double latitudeRad = ToRadians(origin.Latitude);
            double longitudeRad = ToRadians(origin.Longitude);
            double bearingRad = ToRadians(bearing);

            double centralAngle = distance / radius;
            double destinationLatitudeRad = Math.Asin(Math.Sin(latitudeRad) * Math.Cos(centralAngle) + Math.Cos(latitudeRad) * Math.Sin(centralAngle) * Math.Cos(bearingRad));
            double destinationLongitudeRad = longitudeRad + Math.Atan2(Math.Sin(bearingRad) * Math.Sin(centralAngle) * Math.Cos(latitudeRad), Math.Cos(centralAngle) - Math.Sin(latitudeRad) * Math.Sin(destinationLatitudeRad));

            return new Position(ToDegrees(destinationLongitudeRad), ToDegrees(destinationLatitudeRad));
        }

        #endregion

        #region Interpolations

        /// <summary>
        /// Calculates the position object on a path that is a specified distance away from the start of the path. 
        /// If the specified distance is longer than the length of the path, the last position of the path will be returned.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="distance"></param>
        /// <param name="units"></param>
        /// <returns>A position object that is the specified distance away from the start of the path when following the path. Null is np coordinates in path.</returns>
        public static Position? GetPositionAlongPath(IList<Position> path, double distance, DistanceUnits units = DistanceUnits.Meters) 
        {
            int len = path.Count();

            if (len == 1)
            {
                return path[0];
            }
            else if (len >= 2)
            {
                double travelled = 0;
                double dx;

                for (int i = 1; i < len; i++)
                {
                    dx = GetDistanceTo(path[i - 1], path[i], units);

                    if (travelled + dx >= distance)
                    {
                        // Overshot
                        double heading = GetHeading(path[i - 1], path[i]);
                        return GetDestination(path[i - 1], heading, distance - travelled, units);
                    }

                    travelled += dx;
                }

                if (distance >= travelled)
                {
                    return path[len - 1];
                }

                return path[0];
            }

            return null;
        }

        /// <summary>
        /// Calculates the position object on a path that is a specified distance away from the start of the path. 
        /// If the specified distance is longer than the length of the path, the last position of the path will be returned.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="distance"></param>
        /// <param name="units"></param>
        /// <returns>A position object that is the specified distance away from the start of the path when following the path. Null is np coordinates in path.</returns>
        public static Position? GetPositionAlongPath(LineString path, double distance, DistanceUnits units = DistanceUnits.Meters)
        {
            return GetPositionAlongPath(path.Coordinates, distance, units);
        }

        /// <summary>
        /// Gets a point with heading a specified distance along a path.
        /// The PointGeometry will have a custom property called "heading" of type double.
        /// </summary>
        /// <param name="path">The path to get the point from.</param>
        /// <param name="distance">The distance along the path to get the point at.</param>
        /// <param name="units">The distance units.</param>
        /// <returns>A point with heading a specified distance along a path. 
        /// "heading" property is added to custom properties of the PointGeometry.</returns>
        public static Feature? GetPointWithHeadingAlongPath(IList<Position> path, double distance, DistanceUnits units = DistanceUnits.Meters)
        {
            double travelled = 0;
            double dx;
            int len = path.Count;

            if (len >= 2)
            {
                Position? position = null;
                double heading;

                for (int i = 1; i < len; i++)
                {
                    dx = GetDistanceTo(path[i - 1], path[i], units);

                    if (travelled + dx >= distance)
                    {
                        // Overshot
                        heading = GetHeading(path[i - 1], path[i]);
                        position = GetDestination(path[i - 1], heading, distance - travelled, units);
                        break;
                    }

                    travelled += dx;
                }

                if (distance >= travelled)
                {
                    position = path[len - 1];
                    heading = GetHeading(path[len - 2], path[len - 1]);
                }
                else
                {
                    position = path[0];
                    heading = GetHeading(path[0], path[1]);
                }

                if (position != null)
                {
                    return new Feature(new PointGeometry(position), new Dictionary<string, object?>() {
                        { "heading", heading }
                    });
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a point with heading a specified distance along a path.
        /// The PointGeometry will have a custom property called "heading" of type double.
        /// </summary>
        /// <param name="path">The path to get the point from.</param>
        /// <param name="distance">The distance along the path to get the point at.</param>
        /// <param name="units">The distance units.</param>
        /// <returns>A point with heading a specified distance along a path. 
        /// "heading" property is added to custom properties of the PointGeometry.</returns>
        public static Feature? GetPointWithHeadingAlongPath(LineString path, double distance, DistanceUnits units = DistanceUnits.Meters)
        {
            return GetPointWithHeadingAlongPath(path.Coordinates, distance, units);
        }

        /// <summary>
        /// Gets an array of evenly spaced points with headings along a path.
        /// Each PointGeometry will have a custom property called "heading" of type double.
        /// </summary>
        /// <param name="path">The path to get the positions from.</param>
        /// <param name="numPoints">The number of points to get.</param>
        /// <returns>An array of evenly spaced points with headings along a path.</returns>
        public static IList<Feature> GetPointsWithHeadingsAlongPath(IList<Position> path, int numPoints)
        {
            var points = new List<Feature>();

            if (numPoints <= 0)
            {
                return points;
            }

            var len = GetLengthOfPath(path, DistanceUnits.Meters);

            // Calculate the gaps between the points. There is one less gap than there are points.
            var dx = len / (numPoints - 1);

            for (int i = 0; i < numPoints; i++)
            {
                var p = GetPointWithHeadingAlongPath(path, dx * i, DistanceUnits.Meters);

                if (p != null)
                {
                    points.Add(p);
                }
            }

            return points;
        }

        /// <summary>
        /// Gets an array of evenly spaced points with headings along a path.
        /// Each PointGeometry will have a custom property called "heading" of type double.
        /// </summary>
        /// <param name="path">The path to get the positions from.</param>
        /// <param name="numPoints">The number of points to get.</param>
        /// <returns>An array of evenly spaced points with headings along a path.</returns>
        public static IList<Feature> GetPointsWithHeadingsAlongPath(LineString path, int numPoints)
        {
            return GetPointsWithHeadingsAlongPath(path.Coordinates, numPoints);
        }

        /// <summary>
        /// Calculates a position object that is a fractional distance between two position objects.
        /// </summary>
        /// <param name="origin">First position to calculate mid-point between.</param>
        /// <param name="destination">Second position to calculate mid-point between.</param>
        /// <param name="fraction">The fractional parameter to calculate a mid-point for. Default 0.5.</param>
        /// <returns>A position that lies a fraction of the distance between two position objects, relative to the first position object.</returns>
        public static Position Interpolate(Position origin, Position destination, double fraction = 0.5) {
            double arcLength = GetDistanceTo(origin, destination, DistanceUnits.Meters);
            double brng = GetHeading(origin, destination);

            return GetDestination(origin, brng, arcLength * fraction, DistanceUnits.Meters);
        }

        /// <summary>
        /// Gets an array of evenly spaced positions along a path.
        /// </summary>
        /// <param name="path">The path to get the positions from.</param>
        /// <param name="numPositions">The number of positions to get.</param>
        /// <returns>An array of evenly spaced positions along a path.</returns>
        public static IList<Position> GetPositionsAlongPath(IList<Position> path, int numPositions)
        {
            var positions = new List<Position>(numPositions);

            if (numPositions < 2)
            {
                return positions;
            }

            double len = GetLengthOfPath(path, DistanceUnits.Meters);

            // Calculate the gaps between the positions. There is one less gap than there are positions.
            double dx = len / ((double)numPositions - 1);

            for (int i = 0; i < numPositions; i++)
            {
                var p = GetPositionAlongPath(path, dx * i, DistanceUnits.Meters);

                if (p != null)
                {
                    positions.Add(p);
                }
            }

            return positions;
        }

        /// <summary>
        /// Gets an array of evenly spaced positions along a path.
        /// </summary>
        /// <param name="path">The path to get the positions from.</param>
        /// <param name="numPositions">The number of positions to get.</param>
        /// <returns>An array of evenly spaced positions along a path.</returns>
        public static IList<Position> GetPositionsAlongPath(LineString path, int numPositions)
        {
            return GetPositionsAlongPath(path.Coordinates, numPositions);
        }

        #endregion

        #region Regular Polygon Coordinate Generator

        /// <summary>
        /// Calculates a list of coordinates that are an equal distance away from a central point to create a regular polygon.
        /// </summary>
        /// <param name="center">Center of the polygon.</param>
        /// <param name="radius">Radius of the polygon.</param>
        /// <param name="units">Distance units of radius. Default: Meters</param>
        /// <param name="numberOfPoints">Number of points the polygon should have. Default: 36</param>
        /// <param name="offset">The offset to rotate the polygon. When 0 the first coordinate will align with North. Default: 0</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A list of coordinates that form a regular polygon</returns>
        public static IList<Position> GetRegularPolygonPath(Position center, double radius, int numberOfPoints = 36, DistanceUnits units = DistanceUnits.Meters, double offset = 0, bool crossAntimerdian = false)
        {
            var positions = new List<Position>();
            double centralAngle = 360 / numberOfPoints;

            for (var i = 0; i <= numberOfPoints; i++)
            {
                positions.Add(GetDestination(center, (i * centralAngle + offset) % 360, radius, units));
            }

            if (crossAntimerdian)
            {
                return GetPathDenormalizedAtAntimerian(positions);
            }

            return positions;
        }

        #endregion

        #region Calculate Geodesic Coordinates

        /// <summary>
        /// Takes a list of coordinates and fills in the space between them with accurately 
        /// positioned pints to form a Geodesic path.        
        /// Source: http://alastaira.wordpress.com/?s=geodesic
        /// </summary>
        /// <param name="positions">List of coordinates to work with.</param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A set of coordinates that for geodesic paths.</returns>
        public static IList<Position> CalculateGeodesic(IList<Position> positions, int nodeSize = 15, bool crossAntimerdian = false)
        {
            if (nodeSize <= 0)
            {
                nodeSize = 32;
            }

            var locs = new List<Position>();

            var last = positions[0];

            foreach (var p in positions.Skip(1))
            {
                // Convert coordinates from degrees to Radians    
                var lat1 = ToRadians(last.Latitude);
                var lon1 = ToRadians(last.Longitude);
                var lat2 = ToRadians(p.Latitude);
                var lon2 = ToRadians(p.Longitude);

                // Calculate the total extent of the route           
                var d = 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((lat1 - lat2) / 2)), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow((Math.Sin((lon1 - lon2) / 2)), 2)));

                // Calculate positions at fixed intervals along the route
                for (var k = 0; k <= nodeSize; k++)
                {
                    var f = (k / (double)nodeSize);
                    var A = Math.Sin((1 - f) * d) / Math.Sin(d);
                    var B = Math.Sin(f * d) / Math.Sin(d);

                    // Obtain 3D Cartesian coordinates of each point             
                    var x = A * Math.Cos(lat1) * Math.Cos(lon1) + B * Math.Cos(lat2) * Math.Cos(lon2);
                    var y = A * Math.Cos(lat1) * Math.Sin(lon1) + B * Math.Cos(lat2) * Math.Sin(lon2);
                    var z = A * Math.Sin(lat1) + B * Math.Sin(lat2);

                    // Convert these to latitude/longitude             
                    var lat = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                    var lon = Math.Atan2(y, x);

                    // Add this to the array             
                    locs.Add(new Position(ToDegrees(lon), ToDegrees(lat)));
                }
            }

            if (crossAntimerdian)
            {
                return GetPathDenormalizedAtAntimerian(locs);
            }

            return locs;
        }

        /// <summary>
        /// Create a geodesic version of a geometry object. Point and MultiPoint objects are returned as-is.
        /// </summary>
        /// <param name="geometry">The geometry to make the edges geodesic.</param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A geometry with smoothed geodesic edges. Point and MultiPoint objects are returned as-is.</returns>
        public static Geometry CalculateGeodesic(Geometry geometry, int nodeSize = 15, bool crossAntimerdian = false)
        {
            switch (geometry.Type)
            {
                case GeoJsonType.LineString:
                    return CalculateGeodesic(geometry as LineString, nodeSize, crossAntimerdian);
                case GeoJsonType.MultiLineString:
                    return CalculateGeodesic(geometry as MultiLineString, nodeSize, crossAntimerdian);
                case GeoJsonType.Polygon:
                    return CalculateGeodesic(geometry as Polygon, nodeSize, crossAntimerdian);
                case GeoJsonType.MultiPolygon:
                    return CalculateGeodesic(geometry as MultiPolygon, nodeSize, crossAntimerdian);
                case GeoJsonType.Point:
                case GeoJsonType.MultiPoint:
                default:
                    return geometry;
            }
        }

        /// <summary>
        /// Create a geodesic version of a line.
        /// </summary>
        /// <param name="line">The line to create a geodesic version of.</param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A geodesic version of the line</returns>
        public static LineString CalculateGeodesic(LineString line, int nodeSize = 15, bool crossAntimerdian = false)
        {
            var pos = CalculateGeodesic(line.Coordinates, nodeSize, crossAntimerdian);
            return new LineString(pos);
        }

        /// <summary>
        /// Create a geodesic version of a Polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A geodesic version of the Polygon</returns>
        public static Polygon CalculateGeodesic(Polygon polygon, int nodeSize = 15, bool crossAntimerdian = false)
        {
            var rings = new List<PositionCollection>();

            foreach (var ring in polygon.Coordinates)
            {
                rings.Add(new PositionCollection(CalculateGeodesic(ring, nodeSize, crossAntimerdian)));
            }

            return new Polygon(rings);
        }

        /// <summary>
        /// Create a geodesic version of a MultiLineString.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString</param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A geodesic version of the MultiLineString</returns>
        public static MultiLineString CalculateGeodesic(MultiLineString multiLineString, int nodeSize = 15, bool crossAntimerdian = false)
        {
            var geodesicLines = new List<IList<Position>>();

            foreach (var line in multiLineString.Coordinates)
            {
                geodesicLines.Add(CalculateGeodesic(line, nodeSize, crossAntimerdian));
            }

            return new MultiLineString(geodesicLines);
        }

        /// <summary>
        /// Create a geodesic version of a MultiPolygon.
        /// </summary>
        /// <param name="multiPolygon">The MultiPolygon</param>
        /// <param name="nodeSize">Number of nodes to insert between each coordinate</param>
        /// <param name="crossAntimerdian">Specifies if paths crossing antimeridian should contain longitude outside of -180 to 180 range for proper rendering.</param>
        /// <returns>A geodesic version of the MultiPolygon</returns>
        public static MultiPolygon CalculateGeodesic(MultiPolygon multiPolygon, int nodeSize = 15, bool crossAntimerdian = false)
        {
            var geodesicPolygons = new List<IList<PositionCollection>> ();

            foreach (var polygon in multiPolygon.Coordinates)
            {
                if (polygon != null)
                {
                    var polygonRings = new List<PositionCollection>();

                    foreach (var ring in polygon)
                    {
                        if (ring != null)
                        {
                            polygonRings.Add(new PositionCollection(CalculateGeodesic(ring, nodeSize, crossAntimerdian)));
                        }
                    }

                    geodesicPolygons.Add(polygonRings);
                }
            }

            return new MultiPolygon(geodesicPolygons);
        }

        #endregion

        #region Cardinal Spline

        /// <summary>
        /// Calculates an array of positions that form a cardinal spline between the specified array of positions.
        /// </summary>
        /// <param name="positions">The array of positions to calculate the spline through.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <param name="close">A boolean indicating if the spline should be a closed ring or not. Default: false</param>
        /// <returns>An array of positions that form a cardinal spline between the specified array of positions.</returns>
        public static IList<Position> GetCardinalSpline(IList<Position> positions, double tension = 0.5, int nodeSize = 15, bool close = false) {
            // Resources:
            // http://www.cubic.org/docs/hermite.htm
            // http://codeplea.com/introduction-to-splines
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms536358(v=vs.85).aspx
            var locs = new List<Position>();

            // Get the number of locations the spline passs through.
            int len = positions.Count();

            if (len <= 2) {
                return locs;
            }

            if (nodeSize <= 0)
            {
                nodeSize = 15;
            }

            // Create a copy of the array of locations so that we don't alter the original array.
            locs = new List<Position>(positions);

            // Add additional locations to array so that tangents can be calculated for end points.
            if (close)
            {
                // If the location array forms a closed ring, remove the last location.
                if (locs[0].Equals(locs[len - 1]))
                {
                    locs.RemoveAt(len - 1);
                    len--;
                }

                // Insert the last coordinate as the first point.
                locs.Insert(0, positions.Last());

                // Add the first two points to the end of the array.
                locs.Add(positions[0]);
                locs.Add(positions[1]);

                // Increase index so that spline wraps back around to starting location.
                len++;
            }
            else
            {
                // In this case the spline is not closed, so tanget of end points will be 0.
                // Buffer the end-points so that tanget calculations can be performed.
                locs.Insert(0, positions[0]);
                locs.Add(positions.Last());
            }

            // Precalculate the hermite basis function steps along the spline.
            var hermiteSteps = new List<int[]>
            {
                // Force the first step between two locations to be the first location.
                ([1, 0, 0, 0])
            };

            int step;
            int step2;
            int step3;

            // Calculate the steps along the spline between two locations.
            for (int i = 1; i < nodeSize - 1; i++)
            {
                step = i / nodeSize;            // Scale step to go from 0 to 1.

                step2 = step * step;            // s^2
                step3 = step * step2;           // s^3

                hermiteSteps.Add([
                    2 * step3 - 3 * step2 + 1,  // Calculate hermite basis function 1.
                    -2 * step3 + 3 * step2,     // Calculate hermite basis function 2.
                    step3 - 2 * step2 + step,   // Calculate hermite basis function 3.
                    step3 - step2               // Calculate hermite basis function 4.
                ]);
            }

            // Force the last step between two locations to be the last location.
            hermiteSteps.Add([0, 1, 0, 0]);

            var splineLocs = new List<Position>();

            // Tangents
            double t1x;
            double t1y;
            double t2x;
            double t2y;

            double lat;
            double lon;

            // Loop through and calculate the spline path between each location pair.
            for (int i = 1; i < len; i++)
            {
                t1x = tension * (locs[i + 1].Longitude - locs[i - 1].Longitude);
                t1y = tension * (locs[i + 1].Latitude - locs[i - 1].Latitude);
                t2x = tension * (locs[i + 2].Longitude - locs[i].Longitude);
                t2y = tension * (locs[i + 2].Latitude - locs[i].Latitude);

                for (step = 0; step < nodeSize; step++)
                {
                    var hermiteStep = hermiteSteps[step];

                    lon = hermiteStep[0] * locs[i].Longitude + hermiteStep[1] * locs[i + 1].Longitude + hermiteStep[2] * t1x + hermiteStep[3] * t2x;
                    lat = hermiteStep[0] * locs[i].Latitude + hermiteStep[1] * locs[i + 1].Latitude + hermiteStep[2] * t1y + hermiteStep[3] * t2y;

                    lat = TileMath.ClipLatitude(lat);

                    splineLocs.Add(new Position(lon, lat));
                }
            }

            return splineLocs;
        }

        /// <summary>
        /// Calculates a geometry object that has been smoothed using a cardinal spline. Point and MultiPoint objects are returned as-is.
        /// </summary>
        /// <param name="geometry">The geometry object to smooth.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <returns>A smoothed version of the geometry. Point and MultiPoint objects are returned as-is.</returns>
        public static Geometry GetCardinalSpline(Geometry geometry, double tension = 0.5, int nodeSize = 15)
        {
            switch (geometry.Type)
            {
                case GeoJsonType.LineString:
                    return GetCardinalSpline(geometry as LineString, tension, nodeSize);
                case GeoJsonType.MultiLineString:
                    return GetCardinalSpline(geometry as MultiLineString, tension, nodeSize);
                case GeoJsonType.Polygon:
                    return GetCardinalSpline(geometry as Polygon, tension, nodeSize);
                case GeoJsonType.MultiPolygon:
                    return GetCardinalSpline(geometry as MultiPolygon, tension, nodeSize);
                case GeoJsonType.Point:
                case GeoJsonType.MultiPoint:
                default:
                    return geometry;
            }
        }

        /// <summary>
        /// Calculates a LineString that has been smoothed using a cardinal spline.
        /// </summary>
        /// <param name="line">The LineString.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <returns>A LineString that has been smoothed using a cardinal spline.</returns>
        public static LineString GetCardinalSpline(LineString line, double tension = 0.5, int nodeSize = 15)
        {
            var pos = GetCardinalSpline(line.Coordinates, tension, nodeSize, false);
            return new LineString(pos);
        }

        /// <summary>
        /// Calculates a Polygon that has been smoothed using a cardinal spline.
        /// </summary>
        /// <param name="polygon">The Polygon.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <returns>A Polygon that has been smoothed using a cardinal spline.</returns>
        public static Polygon GetCardinalSpline(Polygon polygon, double tension = 0.5, int nodeSize = 15)
        {
            var rings = new List<PositionCollection>();

            foreach (var ring in polygon.Coordinates)
            {
                rings.Add(new PositionCollection(GetCardinalSpline(ring, tension, nodeSize)));
            }

            return new Polygon(rings);
        }

        /// <summary>
        /// Calculates a MultiLineString that has been smoothed using a cardinal spline.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <returns>A MultiLineString that has been smoothed using a cardinal spline.</returns>
        public static MultiLineString GetCardinalSpline(MultiLineString multiLineString, double tension = 0.5, int nodeSize = 15)
        {
            var splineLines = new List<PositionCollection>();

            foreach (var line in multiLineString.Coordinates)
            {
                splineLines.Add(new PositionCollection(GetCardinalSpline(line, tension, nodeSize)));
            }

            return new MultiLineString(splineLines);
        }

        /// <summary>
        /// Calculates a MultiPolygon that has been smoothed using a cardinal spline.
        /// </summary>
        /// <param name="multiPolygon">TheMultiPolygon.</param>
        /// <param name="tension">A number that indicates the tightness of the curve. Can be any number, although a value between 0 and 1 is usually used. Default: 0.5</param>
        /// <param name="nodeSize">Number of nodes to insert between each position. Default: 15</param>
        /// <returns>A MultiPolygon that has been smoothed using a cardinal spline.</returns>
        public static MultiPolygon GetCardinalSpline(MultiPolygon multiPolygon, double tension = 0.5, int nodeSize = 15)
        {
            var splinePolygons = new List<Polygon>();

            foreach (var polygon in multiPolygon.Coordinates)
            {
                splinePolygons.Add(GetCardinalSpline(new Polygon(polygon), tension, nodeSize));
            }

            return new MultiPolygon(splinePolygons);
        }

        #endregion

        #region Simplify

        /// <summary>
        /// Perform a Douglas-Peucker simplification on an array of positions.
        /// </summary>
        /// <param name="positions">The position or pixel points to simplify.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns>A simplified array of positions.</returns>
        public static IList<Position> Simplify(IList<Position> positions, double tolerance)
        {
            return SimplifyUtil.Run(positions, tolerance);
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on an array of pixels.
        /// </summary>
        /// <param name="pixels">The position or pixel points to simplify.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns>A simplified array of pixels.</returns>
        public static IList<Pixel> Simplify(IList<Pixel> pixels, double tolerance)
        {
            return SimplifyUtil.Run(pixels.ToList(), tolerance);
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on a Geometry object. Point and MultiPoint objects are returned as-is.
        /// </summary>
        /// <param name="geometry">The geometry to simplify</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns>A simplified version of the geometry. Point and MultiPoint objects are returned as-is.</returns>
        public static Geometry Simplify(Geometry geometry, double tolerance)
        {
            switch (geometry.Type)
            {
                case GeoJsonType.LineString:
                    return Simplify(geometry as LineString, tolerance);
                case GeoJsonType.MultiLineString:
                    return Simplify(geometry as MultiLineString, tolerance);
                case GeoJsonType.Polygon:
                    return Simplify(geometry as Polygon, tolerance);
                case GeoJsonType.MultiPolygon:
                    return Simplify(geometry as MultiPolygon, tolerance);
                case GeoJsonType.Point:
                case GeoJsonType.MultiPoint:
                default:
                    return geometry;
            }
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on a LineString.
        /// </summary>
        /// <param name="line">The LineString to simplify.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns></returns>
        public static LineString Simplify(LineString line, double tolerance)
        {
            return new LineString(Simplify(line.Coordinates, tolerance));
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on a Polygon.
        /// </summary>
        /// <param name="polygon">The line to Polygon.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns></returns>
        public static Polygon Simplify(Polygon polygon, double tolerance)
        {
            var rings = new List<IList<Position>>();

            foreach (var ring in polygon.Coordinates)
            {
                rings.Add(Simplify(ring, tolerance));
            }

            return new Polygon(rings);
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on a MultiLineString.
        /// </summary>
        /// <param name="multiLineString">The line to MultiLineString.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns></returns>
        public static MultiLineString Simplify(MultiLineString multiLineString, double tolerance)
        {
            var simplifiedLines = new List<IList<Position>>();

            foreach (var line in multiLineString.Coordinates)
            {
                simplifiedLines.Add(Simplify(line, tolerance));
            }

            return new MultiLineString(simplifiedLines);
        }

        /// <summary>
        /// Perform a Douglas-Peucker simplification on a MultiPolygon.
        /// </summary>
        /// <param name="multiPolygon">The line to MultiPolygon.</param>
        /// <param name="tolerance">A tolerance to use in the simplification.</param>
        /// <returns></returns>
        public static MultiPolygon Simplify(MultiPolygon multiPolygon, double tolerance)
        {
            var simplifiedPolygons = new List<IList<IList<Position>>>();

            foreach (var polygon in multiPolygon.Coordinates)
            {
                var rings = new List<IList<Position>>();

                foreach (var ring in polygon)
                {
                    rings.Add(Simplify(ring, tolerance));
                }

                simplifiedPolygons.Add(rings);
            }

            return new MultiPolygon(simplifiedPolygons);
        }

        #endregion

        #region Area calculations

        /// <summary>
        /// Converts an area value from one unit to another.
        /// Supported units: squareMeters, acres, hectares, squareFeet, squareYards, squareMiles, squareKilometers
        /// </summary>
        /// <param name="area">The area value to convert.</param>
        /// <param name="fromUnits">The area units the value is in.</param>
        /// <param name="toUnits">The area units to convert to.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>An area value convertered from one unit to another.</returns>
        public static double ConvertArea(double area, AreaUnits fromUnits, AreaUnits toUnits, int decimals = 10)
        {
            switch (fromUnits)
            {
                case AreaUnits.Acres:
                    area *= 4046.8564224;
                    break;
                case AreaUnits.Hectares:
                    area *= 10000;
                    break;
                case AreaUnits.SquareFeet:
                    area *= 0.09290304;
                    break;
                case AreaUnits.SquareKilometers:
                    area *= 1000000;
                    break;
                case AreaUnits.SquareMiles:
                    area *= 2590000;
                    break;
                case AreaUnits.SquareYards:
                    area *= 0.83612736;
                    break;
                case AreaUnits.SquareMeters:
                default:
                    break;
            }

            switch (toUnits)
            {
                case AreaUnits.Acres:
                    area /= 4046.8564224;
                    break;
                case AreaUnits.Hectares:
                    area /= 10000;
                    break;
                case AreaUnits.SquareFeet:
                    area /= 0.09290304;
                    break;
                case AreaUnits.SquareKilometers:
                    area /= 1000000;
                    break;
                case AreaUnits.SquareMiles:
                    area /= 2590000;
                    break;
                case AreaUnits.SquareYards:
                    area /= 0.83612736;
                    break;
                case AreaUnits.SquareMeters:
                default:
                    break;
            }

            return Math.Round(area, decimals);
        }

        /// <summary>
        /// Calculates the approximate area of a geometry in the specified units
        /// </summary>
        /// <param name="geometry">he coordinates of the polyon ring.
        /// For a polygon, the first ring is the outer/exterior ring and all other rings are the interior ring.</param>
        /// <param name="areaUnits">Unit of area measurement. Default is squareMeters.</param>
        /// <param name="decimals">The number of decimal places to round the result to.</param>
        /// <returns>The area of a geometry in the specified units.</returns>
        public static double GetArea(Geometry geometry, AreaUnits areaUnits = AreaUnits.SquareMeters, int decimals = 10) {
            double area = 0;

            switch (geometry.Type)
            {
                case GeoJsonType.Polygon:
                    var p = geometry as Polygon;
                    area = CalculatePolygonArea(p);
                    break;
                case GeoJsonType.MultiPolygon:
                    var mp = (geometry as MultiPolygon).Coordinates;

                    for (int i = 0, len = mp.Count; i < len; i++)
                    {
                        area += CalculatePolygonArea(new Polygon(mp[i]));
                    }
                    break;
                //Points, lines, and multi-geometries of these types do not have area.
                default:
                    return 0;
            }

            return ConvertArea(area, AreaUnits.SquareMeters, areaUnits, decimals);
        }

        /// <summary>
        /// Calculates the area of a polygon in square meters.
        /// </summary>
        /// <param name="polygon">The coordinates of the polygon ring.
        /// The first ring is the outer/exterior ring and all other rings are the interior ring.</param>
        /// <returns>The area of a polygon in square meters.</returns>
        private static double CalculatePolygonArea(Polygon polygon)
        {
            // Based on https://trs-new.jpl.nasa.gov/handle/2014/40409
            double area = 0;
            var rings = polygon.Coordinates;
            if (rings.Count > 0)
            {
                // Calculate the area of the outer/exterior ring of the polygon.
                area = Math.Abs(CalculatePolygonRingArea(new LineString(rings[0])));

                // Subtract the area of the holes of the polygon.
                for (int i = 1, len = rings.Count; i < len; i++)
                {
                    area -= Math.Abs(CalculatePolygonRingArea(new LineString(rings[i])));
                }
            }

            return area;
        }

        /// <summary>
        /// Calculates the area of a polygon ring  in square meters.
        /// The area value will be positive if the coordinates in the ring are ordered clockwise,
        /// and negative if ordered counter-clockwise.
        /// </summary>
        /// <param name="ring">The coordinates of the polygon ring.</param>
        /// <returns>The area of the ring in square meters.</returns>
        private static double CalculatePolygonRingArea(LineString ring)
        {
            // Based on https://trs-new.jpl.nasa.gov/handle/2014/40409
            double area = 0;
            var coordinates = ring.Coordinates;
            if (coordinates.Count >= 3)
            {
                int p1;
                int p2;
                int p3;

                for (int i = 0, len = coordinates.Count; i < len; i++)
                {
                    // Create triangles from the coordinates.
                    if (i == len - 2)
                    { // i = N-2
                        p1 = len - 2;
                        p2 = len - 1;
                        p3 = 0;
                    }
                    else if (i == len - 1)
                    { // i = N-1
                        p1 = len - 1;
                        p2 = 0;
                        p3 = 1;
                    }
                    else
                    { // i = 0 to N-3
                        p1 = i;
                        p2 = i + 1;
                        p3 = i + 2;
                    }

                    area += (ToRadians(coordinates[p3].Longitude) - ToRadians(coordinates[p1].Longitude)) * Math.Sin(ToRadians(coordinates[p2].Latitude));
                }

                area = area * EarthRadius.Meters * EarthRadius.Meters / 2;
            }

            return area;
        }

        #endregion

        #region Normalize coordinates

        /// <summary>
        /// Normalizes a latitude value between -90 and 90 degrees.
        /// </summary>
        /// <param name="lat">The latitude value to normalize.</param>
        /// <returns>A latitude value between -90 and 90.</returns>
        public static double NormalizeLatitude(double lat) {
            if (lat > 90) {
                lat = (lat + 90) % 360;
                return lat > 180 ? 90 - (lat - 180) : lat - 90;
            } else if (lat < -90) {
                lat = (lat - 90) % 360;
                return lat < -180 ? -90 - (lat + 180) : lat + 90;
            } else
            {
                return lat;
            }
        }

        /// <summary>
        /// Normalizes a longitude value between -180 and 180 degrees.
        /// </summary>
        /// <param name="lng">The longitude value to normalize.</param>
        /// <returns>A longitude value between -180 and 180.</returns>
        public static double NormalizeLongitude(double lng) {
            if (lng > 180) {
                return ((lng + 180) % 360) - 180;
            } else if (lng < -180)
            {
                return ((lng - 180) % 360) + 180;
            }
            else
            {
                return lng;
            }
        }

        /// <summary>
        /// Normalizes a position to ensure that the latitude and longitude values are within the correct range.
        /// </summary>
        /// <param name="position">The position to normalize.</param>
        /// <returns></returns>
        public static Position Normalize(Position position)
        {
            return new Position(NormalizeLongitude(position.Longitude), NormalizeLatitude(position.Latitude));
        }

        /// <summary>
        /// Normalizes a collection of positions to ensure that the latitude and longitude values are within the correct range.
        /// </summary>
        /// <param name="positions">A collection of positions.</param>
        /// <returns>Collection of normalized positions.</returns>
        public static IList<Position> Normalize(IList<Position> positions)
        {
            var normalized = new List<Position>();

            foreach (var pos in positions)
            {
                normalized.Add(new Position(NormalizeLongitude(pos.Longitude), NormalizeLatitude(pos.Latitude)));
            }

            return normalized;
        }

        #endregion

        #region Rotate positions

        /// <summary>
        /// Takes an array of positions and rotates them around a given position for the specified angle of rotation.
        /// </summary>
        /// <param name="positions">An array of positions to be rotated.</param>
        /// <param name="origin">The position to rotate the positions around.</param>
        /// <param name="angle">The amount to rotate the array of positions in degrees clockwise.</param>
        /// <returns></returns>
        public static IList<Position> RotatePositions(IList<Position> positions, Position origin, double angle) {           
            if (angle == 0) {
                // When the rotation is 0 the results can sometimes be odd if the coordinates are on the meridians.
                return positions.ToList();
            }

            var rotatedPositions = new List<Position>(positions.Count());

            foreach (var pos in positions)
            {
                var distance = GetDistanceTo(origin, pos);
                var heading = GetHeading(origin, pos);

                rotatedPositions.Add(GetDestination(origin, heading + angle, distance));
            }

            return rotatedPositions;
        }

        #endregion

        #region Pixel based heading

        /// <summary>
        /// Calculates the pixel accurate heading from one position to another based on the Mercator map projection. This heading is visually accurate.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static double GetPixelHeading(Position origin, Position destination) 
        {
            var p1 = MercatorPositionToPixel(origin);
            var p2 = MercatorPositionToPixel(destination);

            var dx = (double)(p2[0] - p1[0]);
            var dy = (double)(p1[1] - p2[1]);

            return ((5 / 2 * Math.PI) - Math.Atan2(dy, dx)) * (180 / Math.PI) % 360;
        }

        #endregion

        #region Extract positions

        /// <summary>
        /// Retrieves an array of all positions in the provided geometry.
        /// </summary>
        /// <param name="geometry">The geometry to retrieve the positions from.</param>
        /// <returns>An array of all positions in the provided geometry.</returns>
        public static PositionCollection GetPositions(Geometry geometry)
        {
            var positions = new PositionCollection();

            switch (geometry.Type)
            {
                case GeoJsonType.Point:
                    positions.Add((geometry as PointGeometry).Coordinates);
                    break;
                case GeoJsonType.LineString:                 
                    positions.AddRange((geometry as LineString).Coordinates);
                    break;
                case GeoJsonType.Polygon:
                    positions.AddRange(PositionCollection.GetPositions((geometry as Polygon).Coordinates));
                    break;
                case GeoJsonType.MultiPoint:
                    positions.AddRange((geometry as MultiPoint).Coordinates);
                    break;
                case GeoJsonType.MultiLineString:
                    positions.AddRange(PositionCollection.GetPositions((geometry as MultiLineString).Coordinates));
                    break;
                case GeoJsonType.MultiPolygon:
                    positions.AddRange(PositionCollection.GetPositions((geometry as MultiPolygon).Coordinates));
                    break;
                default:
                    break;
            }

            return positions;
        }

        /// <summary>
        /// Retrieves an array of all positions in the provided geometries.
        /// </summary>
        /// <param name="geometries">The geometries to retrieve the positions from.</param>
        /// <returns>An array of all positions in the provided geometries.</returns>
        public static PositionCollection GetPositions(IList<Geometry> geometries)
        {
            var positions = new PositionCollection();

            foreach (var geom in geometries)
            {
                positions.AddRange(GetPositions(geom));
            }

            return positions;
        }

        #endregion

        #region Antimerdian Handlers

        /// <summary>
        /// Denormalizes path on antimeridian, this makes lines with coordinates on the opposite side of the antimeridian to always cross it. Note that the path crossing antimeridian will contain longitude outside of -180 to 180 range.
        /// See GetPathSplitByAntimeridian when this is not desired.
        /// </summary>
        /// <param name="path">Array of position objects or linestring to denormalize</param>
        /// <returns>A denormalized array of position objects, path crossing antimeridian will contain longitude outside of -180 to 180 range.</returns>
        public static IList<Position> GetPathDenormalizedAtAntimerian(IList<Position> path)
        {
            var targetPath = new List<Position>();

            foreach (var coord in path)
            {
                if (targetPath.Count > 0 && Math.Abs(coord.Longitude - targetPath[targetPath.Count - 1].Longitude) > 180.0)
                {
                    double denormLon = targetPath[targetPath.Count - 1].Longitude < 0 ? coord.Longitude + 360.0 : coord.Longitude - 360.0;
                    targetPath.Add(new Position(denormLon, coord.Latitude));
                }
                else
                {
                    targetPath.Add(coord);
                }
            }

            return targetPath;
        }

        /// <summary>
        /// Split path on antimeridian into multiple paths.
        /// See getPathDenormalizedAtAntimerian when this is not desired.
        /// </summary>
        /// <param name="path"> Array of position objects or linestring to split</param>
        /// <returns>A path split into multiple paths by antimeridian.</returns>
        public static IList<List<Position>> SplitPathByAntimeridian(IList<Position> path)
        {
            var currentNonCrossing = new List<Position>();
            var outputPaths = new List<List<Position>>();

            var pathList = path.ToList();

            for (int k = 0; k < pathList.Count; k++)
            {
                currentNonCrossing.Add(pathList[k]);
                if (k + 1 >= pathList.Count)
                {
                    continue;
                }

                double lon1 = pathList[k].Longitude;
                double lat1 = pathList[k].Latitude;
                double lon2 = pathList[k + 1].Longitude;
                double lat2 = pathList[k + 1].Latitude;

                // split the line by antimeridian
                // and break geodesic into two line segments
                if (Math.Abs(lon2 - lon1) > 180.0)
                {
                    double denormLon2 = lon1 > 0 ? lon2 + 360.0 : lon2 - 360.0;
                    double antiLon = lon1 > 0 ? 180.0 : -180.0;
                    double abs = Math.Abs(denormLon2 - lon1);
                    double antiAbs = Math.Abs(antiLon - lon1);
                    double f = antiAbs / abs;
                    double dLat = (lat2 - lat1) * f;
                    double antiLat = lat1 + dLat;

                    currentNonCrossing.Add(new Position(antiLon, antiLat));
                    outputPaths.Add(currentNonCrossing);
                    currentNonCrossing = new List<Position>() { new Position(-antiLon, antiLat) };
                }
            }

            outputPaths.Add(currentNonCrossing);
            return outputPaths;
        }

        #endregion

        #region Mercator Projection Math

        /// <summary>
        ///  Converts an array of global Mercator pixel coordinates into an array of geospatial positions at a specified zoom level.
        ///  Global pixel coordinates are relative to the top left corner of the map[-180, 90].
        /// </summary>
        /// <param name="pixels"> Array of pixel coordinates to convert.</param>
        /// <param name="zoom">The mercator zoom level.</param>
        /// <returns> An array of positions.</returns>
        public static IList<Position> MercatorPixelsToPositions(IList<Pixel> pixels, int zoom = 22) {
            // 512 is our tile size in pixels.
            double mapSize = 512 * Math.Pow(2, zoom);

            var positions = new List<Position>(pixels.Count());

            foreach (var pixel in pixels) {
                var x = ((double)pixel.X / mapSize) - 0.5;
                var y = 0.5 - ((double)pixel.Y / mapSize);

                positions.Add(new Position(
                    360 * x,
                    90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI
                ));
            }

            return positions;
        }

        /// <summary>
        /// Converts a global Mercator pixel coordinate into a geospatial position at a specified zoom level.
        /// </summary>
        /// <param name="pixel">The pixel to convert.</param>
        /// <param name="zoom">The mercator zoom level.</param>
        /// <returns>A geospatial position.</returns>
        public static Position MercatorPixelToPosition(Pixel pixel, int zoom = 22)
        {
            // 512 is our tile size in pixels.
            double mapSize = 512 * Math.Pow(2, zoom);

            var x = ((double)pixel[0] / mapSize) - 0.5;
            var y = 0.5 - ((double)pixel[1] / mapSize);

            return new Position(
                 360 * x,
                 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI
             );
        }

        /// <summary>
        /// Converts an array of positions into an array of global Mercator pixel coordinates at a specified zoom level.
        /// </summary>
        /// <param name="positions">Array of positions to convert.</param>
        /// <param name="zoom">The mercator zoom level.</param>
        /// <returns>Array of global Mercator pixels.</returns>
        public static IList<Pixel> MercatorPositionsToPixels(IList<Position> positions, int zoom = 22) {
            // 512 is our tile size in pixels.
            double mapSize = 512 * Math.Pow(2, zoom);

            var pixels = new List<Pixel>(positions.Count());

            foreach (var position in positions) {
                var sinLatitude = Math.Sin(position.Latitude * Math.PI / 180);

                var x = (position.Longitude + 180) / 360;
                var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

                pixels.Add(new Pixel(
                    (int)Math.Round(x * mapSize),
                    (int)Math.Round(y * mapSize)
                ));
            }

            return pixels;
        }

        /// <summary>
        /// Converts a geospatial position into a global Mercator pixel coordinate at a specified zoom level.
        /// </summary>
        /// <param name="position">The position to convert.</param>
        /// <param name="zoom">The mercator zoom level.</param>
        /// <returns>A global Mercator pixel coordinate at a specified zoom level.</returns>
        public static Pixel MercatorPositionToPixel(Position position, int zoom = 22)
        {
            // 512 is our tile size in pixels.
            double mapSize = 512 * Math.Pow(2, zoom);

            var sinLatitude = Math.Sin(position.Latitude * Math.PI / 180);

            var x = (position.Longitude + 180) / 360;
            var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            return new Pixel(
                (int)Math.Round(x * mapSize),
                (int)Math.Round(y * mapSize)
            );
        }

        #endregion

        #region Convex Hull 

        /// <summary>
        /// Calculates a Convex Hull from an array of positions.
        /// </summary>
        /// <param name="positions"> The array of positions to calculate a convex hull for.</param>
        /// <returns>A Convex Hull from an array of positions.</returns>
        public static Polygon GetConvexHull(IList<Position> positions)
        {
            // Uses a Monotone chain algorithm for convex hulls:
            // https://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain

            //Make a ccopy of the list so we don't mix up the original  Normalize coordinates while doing this.
            var pos = new List<Position>();

            foreach (var item in positions)
            {
                pos.Add(new Position(
                    NormalizeLatitude(item.Longitude),
                    NormalizeLatitude(item.Latitude)
                ));
            }

            pos.Sort((a, b) => {
                return a.Longitude == b.Longitude ? (a.Latitude < b.Latitude ? -1 : 1) : (a.Longitude < b.Longitude ? -1 : 1);
            });

            var lower = new List<Position>();
            foreach (var position in pos) {
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], position) <= 0) {
                    lower.RemoveAt(lower.Count - 1); //Remove the last item.
                }
                lower.Add(position);
            }

            var upper = new List<Position>();
            for (int i = pos.Count - 1; i >= 0; i--)
            {
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], pos[i]) <= 0)
                {
                    upper.RemoveAt(upper.Count - 1); //Remove the last item.
                }
                upper.Add(pos[i]);
            }

            lower.AddRange(upper);

            return new Polygon([new PositionCollection(lower)]);
        }

        /// <summary>
        /// Calculates a Convex Hull for a geometry.
        /// </summary>
        /// <param name="geometry">Geometry to calculate convex hull for.</param>
        /// <returns>A Convex Hull from a geometry.</returns>
        public static Polygon GetConvexHull(Geometry geometry)
        {
            var positions = GetPositions(geometry);
            return GetConvexHull(positions);
        }

        /// <summary>
        /// Calculates a Convex Hull for geometries.
        /// </summary>
        /// <param name="geometries">Geometries to calculate convex hull for.</param>
        /// <returns>A Convex Hull from geometries.</returns>
        public static Polygon GetConvexHull(IList<Geometry> geometries)
        {
            var positions = GetPositions(geometries);
            return GetConvexHull(positions);
        }

        /// <summary>
        /// 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
        /// Returns a positive value, if OAB makes a counter-clockwise turn,
        /// negative for clockwise turn, and zero if the points are collinear.
        /// </summary>
        /// <param name="a">Position A</param>
        /// <param name="b">Position B</param>
        /// <param name="o">Position O</param>
        /// <returns>The 2D cross product of OA and OB vectors.</returns>
        private static double Cross(Position a, Position b, Position o)
        {
            return (a.Longitude - o.Longitude) * (b.Latitude - o.Latitude) - (a.Latitude - o.Latitude) * (b.Longitude - o.Longitude);
        }

        #endregion

        #region Closest Point on Geometry
 
        /// <summary>
        /// Calculates the closest point on the edge of a geometry to a specified point or position.
        /// The returned point feature will have a `distance` property that specifies the distance between the two points in the specified units.
        /// If the geometry is a Point, that points position will be used for the result.
        /// If the geometry is a MultiPoint, the distances to the individual positions will be used.
        /// If the geometry is a Polygon or MultiPolygon, the point closest to any edge will be returned regardless of if the point intersects the geometry or not.
        /// </summary>
        /// <param name="pos">The point or position to find the closest point on the edge of the geometry.</param>
        /// <param name="geom">The geometry to find the closest point on.</param>
        /// <param name="units">Unit of distance measurement. Default is meters.</param>
        /// <param name="decimals">The number of decimal places to round the result to. Default: 10</param>
        /// <returns></returns>
        public static Feature? GetClosestPointOnGeometry(Position pos, Geometry geom, DistanceUnits units = DistanceUnits.Meters, int decimals = 10)
        {
            // Does not support geometries that cross the antimerdian.
            var px = MercatorPositionToPixel(pos);
            double minDis = double.PositiveInfinity;
            double d;
            Feature? closest = null;

            switch (geom.Type)
            {
                case GeoJsonType.Point:
                    var point = (geom as PointGeometry).Coordinates;
                    closest = new Feature(new PointGeometry(point), new Dictionary<string, object?>(){{ "distance", GetDistanceTo(pos, point) } });
                    break;
                case GeoJsonType.MultiPoint:
                    var mp = (geom as MultiPoint).Coordinates;
                    for (int i = 0, len = mp.Count; i < len; i++)
                    {
                        d = GetDistanceTo(pos, mp[i]);

                        if (d < minDis)
                        {
                            minDis = d;
                            closest = new Feature(new PointGeometry(mp[i]), new Dictionary<string, object?>() { { "distance", minDis } });
                        }
                    }
                    break;
                case GeoJsonType.LineString:
                    closest = ClosestPointOnPath(pos, px, (geom as LineString).Coordinates);
                    break;
                case GeoJsonType.MultiLineString:
                    closest = ClosestPointOnPath(pos, px, (geom as MultiLineString).Coordinates);
                    break;
                case GeoJsonType.Polygon:
                    closest = ClosestPointOnPath(pos, px, (geom as Polygon).Coordinates, true);
                    break;
                case GeoJsonType.MultiPolygon:
                    var paths = (geom as MultiPolygon).Coordinates;
                    for (int i = 0, len = paths.Count(); i < len; i++)
                    {
                        foreach (var r in paths[i])
                        {
                            var temp = ClosestPointOnPath(pos, px, r, true);

                            if (temp != null)
                            {
                                d = temp.Properties.GetDouble("distance", double.PositiveInfinity);

                                if (d < minDis)
                                {
                                    minDis = d;
                                    closest = temp;
                                }
                            }
                        }
                    }
                    break;
            }

            if (closest != null)
            {
                closest.Properties["distance"] = ConvertDistance(minDis, DistanceUnits.Meters, units, decimals);
                return closest;
            }

            return null;
        }

        private static Feature? ClosestPointOnPath(Position pos, Pixel px, IList<IList<IList<PositionCollection>>> paths, bool closedPath = false)
        {
            Feature? result = null;
            double? minDis = double.PositiveInfinity;
            double? d;

            for (int i = 0, len = paths.Count(); i < len; i++)
            {
                foreach (var r in paths[i])
                {
                    var temp = ClosestPointOnPath(pos, px, r, closedPath);

                    if (temp != null)
                    {
                        d = temp.Properties.GetDouble("distance", double.PositiveInfinity);

                        if (d < minDis)
                        {
                            minDis = d;
                            result = temp;
                        }
                    }
                }
            }

            return result;
        }

        private static Feature? ClosestPointOnPath(Position pos, Pixel px, IList<IList<PositionCollection>> paths, bool closedPath = false)
        {
            Feature? result = null;
            double? minDis = double.PositiveInfinity;
            double? d;

            for (int i = 0, len = paths.Count(); i < len; i++)
            {
                foreach (var r in paths[i])
                {
                    var temp = ClosestPointOnPath(pos, px, r, closedPath);

                    if (temp != null)
                    {
                        d = temp.Properties.GetDouble("distance", double.PositiveInfinity);

                        if (d < minDis)
                        {
                            minDis = d;
                            result = temp;
                        }
                    }
                }
            }

            return result;
        }

        private static Feature? ClosestPointOnPath(Position pos, Pixel px, IList<PositionCollection> paths, bool closedPath = false)
        {
            Feature? result = null;
            double? minDis = double.PositiveInfinity;
            double? d;

            for (int i = 0, len = paths.Count(); i < len; i++)
            {
                var temp = ClosestPointOnPath(pos, px, paths[i], closedPath);

                if (temp != null)
                {
                    d = temp.Properties.GetDouble("distance", double.PositiveInfinity);

                    if (d < minDis)
                    {
                        minDis = d;
                        result = temp;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Takes an array of positions that form a path and calculates the closest point on the path to a specified position.
        /// </summary>
        /// <param name="pos">The position to find the closest point to.</param>
        /// <param name="px">The pixel value of the position at zoom level 22.</param>
        /// <param name="path">An array of positions that form a path.</param>
        /// <param name="closedPath">The closest point on the path to the specified position.</param>
        /// <returns></returns>
        private static Feature? ClosestPointOnPath(Position pos, Pixel px, IList<Position> path, bool closedPath = false)
        {
            // Need atleast two points
            if (path.Count() >= 2)
            {
                double minDis = double.PositiveInfinity;
                double d;
                Pixel? closest = null;

                //Convert path to pixels.
                var pixels = MercatorPositionsToPixels(path.ToList());
                Pixel cPx;

                for (int i = 0, len = pixels.Count - 1; i < len; i++)
                {
                    cPx = ClosestPixelOnLineSegment(px, pixels[i], pixels[i + 1]);
                    d = px.GetDistance(cPx);

                    if (d < minDis)
                    {
                        minDis = d;
                        closest = cPx;
                    }
                }

                if (closest != null)
                {
                    var cPos = MercatorPixelToPosition(closest);
                    return new Feature(new PointGeometry(cPos), new Dictionary<string, object?>() {
                        { "distance", GetDistanceTo(pos, cPos) }
                    });
                }
            }
            else if (path.Count() == 1)
            {
                return new Feature(new PointGeometry(path[0]), new Dictionary<string, object?>() {
                    { "distance", GetDistanceTo(pos, path[0]) }
                });
            }

            return null;
        }

        /// <summary>
        /// Calculates the closest pixel on a line segment from a given point in 2D space.
        /// </summary>
        /// <param name="px">The pixel near the line that we are working with.</param>
        /// <param name="sPx">Start pixel of the line segment.</param>
        /// <param name="ePx">End pixel of the line segment.</param>
        /// <returns>The closest pixel on the line segement to the specified pixel.</returns>
        private static Pixel ClosestPixelOnLineSegment(Pixel px, Pixel sPx, Pixel ePx)
        {
            // If start and end points of line are equal, then that is the closest point.
            if (sPx.Equals(ePx))
            {
                return sPx;
            }

            double APx = px[0] - sPx[0];
            double APy = px[1] - sPx[1];
            double ABx = ePx[0] - sPx[0];
            double ABy = ePx[1] - sPx[1];
            double magAB2 = ABx * ABx + ABy * ABy;
            double ABdotAP = ABx * APx + ABy * APy;
            double t = ABdotAP / magAB2;

            if (t < 0)
            {
                return sPx;
            }
            else if (t > 1)
            {
                return ePx;
            }
            else
            {
                return new Pixel(sPx[0] + ABx * t, sPx[1] + ABy * t);
            }
        }

        #endregion

        #region AffineTransform

        /// <summary>
        /// An Affine Transform class generated from a set of reference points.
        /// </summary>
        public class AffineTransform
        {
            #region Private Properties

            private double[][] M;
            private double[][] inverseM;

            #endregion

            #region Constructor

            /// <summary>
            /// An Affine Transform class generated from a set of reference points.
            /// </summary>
            /// <param name="source">A set of reference points from the source reference system to transform from.</param>
            /// <param name="target">A set of reference points from the target reference system to transform to.</param>
            public AffineTransform(double[][] source, double[][] target)
            {
                this.M = CalculateAffineTransform(source, target);
                this.inverseM = CalculateAffineTransform(target, source);
            }

            /// <summary>
            /// An Affine Transform class generated from a set of reference points.
            /// </summary>
            /// <param name="source">A set of reference points from the source reference system to transform from.</param>
            /// <param name="target">A set of reference points from the target reference system to transform to.</param>
            public AffineTransform(IList<Position> source, IList<Pixel> target):
                this(PositionsToDoubleArray(source), PixelsToDoubleArray(target))
            {                
            }

            /// <summary>
            /// An Affine Transform class generated from a set of reference points.
            /// </summary>
            /// <param name="source">A set of reference points from the source reference system to transform from.</param>
            /// <param name="target">A set of reference points from the target reference system to transform to.</param>
            public AffineTransform(IList<Pixel> source, IList<Position> target):
                this(PixelsToDoubleArray(source), PositionsToDoubleArray(target))
            {
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///  Converts an array of points from the source reference system to the target reference system.
            /// </summary>
            /// <param name="sourcePoints">An array of points from the source reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the target reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public double[][] ToTarget(double[][] sourcePoints, int decimals = 6)
            {
                if (sourcePoints != null)
                {
                    return TransformArray(sourcePoints, M, decimals);
                }

                throw new ArgumentException("Invalid sourcePoints specified.");
            }

            /// <summary>
            /// Converts an array of points from the target reference system to the source reference system.
            /// </summary>
            /// <param name="targetPoints">An array of points from the target reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the source reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public double[][] ToSource(double[][] targetPoints, int decimals = 6)
            {
                if (targetPoints != null)
                {
                    return TransformArray(targetPoints, inverseM, decimals);
                }

                throw new ArgumentException("Invalid targetPoints specified.");
            }

            /// <summary>
            ///  Converts an array of points from the source reference system to the target reference system.
            /// </summary>
            /// <param name="sourcePoints">An array of points from the source reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the target reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public IList<Position> ToTargetPositions(IList<Pixel> sourcePoints, int decimals = 6)
            {
                return DoubleArrayToPositions(ToTarget(PixelsToDoubleArray(sourcePoints), decimals));
            }

            /// <summary>
            /// Converts an array of points from the source reference system to the target reference system.
            /// </summary>
            /// <param name="sourcePoints">An array of points from the source reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the target reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public IList<Pixel> ToTargetPixels(IList<Position> sourcePoints, int decimals = 6)
            {
                return DoubleArrayToPixel(ToTarget(PositionsToDoubleArray(sourcePoints), decimals));
            }

            /// <summary>
            /// Converts an array of points from the target reference system to the source reference system.
            /// </summary>
            /// <param name="targetPoints">An array of points from the target reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the source reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public IList<Position> ToSourcePositions(IList<Pixel> targetPoints, int decimals = 6)
            {
                return DoubleArrayToPositions(ToSource(PixelsToDoubleArray(targetPoints), decimals));
            }

            /// <summary>
            /// Converts an array of points from the target reference system to the source reference system.
            /// </summary>
            /// <param name="targetPoints">An array of points from the target reference system to transform.</param>
            /// <param name="decimals">Number of decimal places to round the results off to.</param>
            /// <returns>An array of points that have been transformed to the source reference system.</returns>
            /// <exception cref="ArgumentException"></exception>
            public IList<Pixel> ToSourcePixels(IList<Position> targetPoints, int decimals = 6)
            {
                return DoubleArrayToPixel(ToSource(PositionsToDoubleArray(targetPoints), decimals));
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Applies a transform matrix over a set of points and optionally rounds off the values to a specified number of decimals.
            /// </summary>
            /// <param name="points">The array of points to transform.</param>
            /// <param name="transformMatrix">The transform matrix to apply.</param>
            /// <param name="decimals">The number of decimals to round each calculated point value off to.</param>
            /// <returns>An array of points that have been transformed.</returns>
            private static double[][] TransformArray(double[][] points, double[][] transformMatrix, int decimals = 6)
            {
                int rows = points.Length;
                int cols = transformMatrix.Length - 1; // -1 because the transform matrix has one extra column

                double[][] transformedPoints = new double[rows][];

                for (int i = 0; i < rows; i++)
                {
                    transformedPoints[i] = Transform(points[i], transformMatrix, decimals);
                }

                return transformedPoints;
            }

            /// <summary>
            /// Applies a transform matrix on a point and optionally rounds off the values to a specified number of decimals.
            /// </summary>
            /// <param name="point">The point to transform.</param>
            /// <param name="transformMatrix">The transform matrix to apply.</param>
            /// <param name="decimals">The number of decimals to round each calculated point value off to.</param>
            /// <returns>An array of points that have been transformed.</returns>
            private static double[] Transform(double[] point, double[][] transformMatrix, int decimals = 6)
            {
                double x = point[0] * transformMatrix[0][3] + point[1] * transformMatrix[1][3] + transformMatrix[2][3];
                double y = point[0] * transformMatrix[0][4] + point[1] * transformMatrix[1][4] + transformMatrix[2][4];

                return [Math.Round(x, decimals), Math.Round(y, decimals)];
            }

            /// <summary>
            /// Takes in a set of source and target points and calculates an approximate Affine Transform matrix that best fits the 
            /// </summary>
            /// <param name="sourcePoints">A set of source points to transform from.</param>
            /// <param name="targetPoints">A set of target points to transform to.</param>
            /// <returns>An Affine Tranform matrix.</returns>
            /// <exception cref="ArgumentException"></exception>
            private static double[][] CalculateAffineTransform(double[][] sourcePoints, double[][] targetPoints)
            {
                int count = sourcePoints.Length;
                int dim = Math.Min(sourcePoints[0].Length, targetPoints[0].Length);
                int dimPlusOne = dim + 1;

                if (sourcePoints.Length != targetPoints.Length || sourcePoints.Length < 1)
                {
                    throw new ArgumentException("Error: source and target arrays must have the same length.");
                }

                if (sourcePoints.Length < dim)
                {
                    throw new ArgumentException("Error: At least " + dim + " reference points required.");
                }

                double[] c = new double[dim];
                double[][] transformMatrix = new double[dimPlusOne][];

                for (int i = 0; i < dimPlusOne; i++)
                {
                    transformMatrix[i] = new double[dimPlusOne];

                    for (int j = 0; j < dimPlusOne; j++)
                    {
                        if (j < dim)
                        {
                            c[j] = 0;
                        }

                        transformMatrix[i][j] = 0;

                        for (int k = 0; k < count; k++)
                        {
                            if (j < dim)
                            {
                                if (i < dim)
                                {
                                    c[j] += sourcePoints[k][i] * targetPoints[k][j];
                                }
                                else
                                {
                                    c[j] += targetPoints[k][j];
                                }
                            }

                            if (i >= dim && j >= dim)
                            {
                                transformMatrix[i][j] += 1;
                            }
                            else if (i >= dim)
                            {
                                transformMatrix[i][j] += sourcePoints[k][j];
                            }
                            else if (j >= dim)
                            {
                                transformMatrix[i][j] += sourcePoints[k][i];
                            }
                            else
                            {
                                transformMatrix[i][j] += sourcePoints[k][i] * sourcePoints[k][j];
                            }
                        }
                    }

                    transformMatrix[i] = transformMatrix[i].Concat(c).ToArray();
                }

                if (!GaussJordanElimination(transformMatrix))
                {
                    throw new ArgumentException("Error: Singular matrix. Points are likely coplanar.");
                }

                return transformMatrix;
            }

            /// <summary>
            /// Puts a given matrix (2D array) into the Reduced Row Echelon Form.
            /// Returns True if successful, False if the transformMatrix is singular.
            /// Code from: https://github.com/commenthol/affinefit
            /// </summary>
            /// <param name="transformMatrix"></param>
            /// <returns></returns>
            private static bool GaussJordanElimination(double[][] transformMatrix)
            {
                double eps = 1e-12; // 1.0 / Math.Pow(10, 12)

                int dimPlusOne = transformMatrix.Length;
                int w = 2 * dimPlusOne - 1;
                double tempNum;
                double[] tempArray;

                for (int j = 0; j < dimPlusOne; j++)
                {
                    int maxrow = j;
                    for (int i = j + 1; i < dimPlusOne; i++)
                    {
                        // Find max pivot.
                        if (Math.Abs(transformMatrix[i][j]) > Math.Abs(transformMatrix[maxrow][j]))
                        {
                            maxrow = i;
                        }
                    }

                    tempArray = transformMatrix[maxrow];
                    transformMatrix[maxrow] = transformMatrix[j];
                    transformMatrix[j] = tempArray;

                    if (Math.Abs(transformMatrix[j][j]) <= eps)
                    {
                        // Is Singular?
                        return false;
                    }

                    for (int _j = j + 1; _j < dimPlusOne; _j++)
                    {
                        // Eliminate column y.
                        tempNum = transformMatrix[_j][j] / transformMatrix[j][j];
                        for (int _i = j; _i < w; _i++)
                        {
                            transformMatrix[_j][_i] -= transformMatrix[j][_i] * tempNum;
                        }
                    }
                }

                for (int j = dimPlusOne - 1; j > -1; j--)
                {
                    // Backsubstitute.
                    tempNum = transformMatrix[j][j];

                    for (int i = 0; i < j; i++)
                    {
                        for (int _x = w - 1; _x > j - 1; _x--)
                        {
                            transformMatrix[i][_x] -= transformMatrix[j][_x] * transformMatrix[i][j] / tempNum;
                        }
                    }

                    transformMatrix[j][j] /= tempNum;

                    for (int _x2 = dimPlusOne; _x2 < w; _x2++)
                    {
                        // Normalize row y.
                        transformMatrix[j][_x2] /= tempNum;
                    }
                }

                return true;
            }

            private static double[][] PositionsToDoubleArray(IList<Position> positions)
            {
                return positions.Select(x => new double[] { x.Longitude, x.Latitude }).ToArray();
            }

            private static double[][] PixelsToDoubleArray(IList<Pixel> pixels)
            {
                return pixels.Select(x => new double[] { x.X, x.Y }).ToArray();
            }

            private static IList<Position> DoubleArrayToPositions(double[][] positions)
            {
                var points = new List<Position>();

                for (int i = 0, len = positions.Length; i < len; i++)
                {
                    points.Add(new Position(positions[i][0], positions[i][1]));
                }

                return points ;
            }

            private static IList<Pixel> DoubleArrayToPixel(double[][] pixels)
            {
                return pixels.Select(x => new Pixel((int)x[0], (int)x[1])).ToList();
            }

            #endregion
        }

        #endregion

        #region Point in Polygon

        /// <summary>
        /// Deterimines if a point is in a Polygon.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="polygon">Polygon to check against.</param>
        /// <returns>True is point is in polygon.</returns>
        public static bool IsPointInPolygon(PointGeometry point, Polygon polygon)
        {
            return IsPointInPolygon(point.Coordinates, polygon);
        }

        /// <summary>
        /// Deterimines if a point is in a Polygon.
        /// </summary>
        /// <param name="position">Point to check.</param>
        /// <param name="polygon">Polygon to check against.</param>
        /// <returns>True is point is in polygon.</returns>
        public static bool IsPointInPolygon(Position position, Polygon polygon)
        {
            //The first ring is the exterior ring, check to see if the point is within it.
            bool inPoly = IsPointInPolygon(position, polygon.Coordinates[0]);

            if(inPoly)
            {
                //If the point is  in the exterior ring, ensure it is not in any of the interior rings.
                for (int i = 1, len = polygon.Coordinates.Count; i < len; i++)
                {
                    if (IsPointInPolygon(position, polygon.Coordinates[i]))
                    {
                        //As soon as it is inside any ring, we know the position is not in the polygon.
                        inPoly = false;
                        break;
                    }
                }
            }

            return inPoly;
        }

        /// <summary>
        /// Deterimines if a point is in a MultiPolygon.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="multiPolygon">MultiPolygon to check against.</param>
        /// <returns>True is point is in polygon.</returns>
        public static bool IsPointInPolygon(PointGeometry point, MultiPolygon multiPolygon)
        {
            return IsPointInPolygon(point.Coordinates, multiPolygon);
        }

        /// <summary>
        /// Deterimines if a point is in a MultiPolygon.
        /// </summary>
        /// <param name="position">Point to check.</param>
        /// <param name="multiPolygon">MultiPolygon to check against.</param>
        /// <returns>True is point is in polygon.</returns>
        public static bool IsPointInPolygon(Position position, MultiPolygon multiPolygon)
        {
            foreach (var polygon in multiPolygon.Coordinates)
            {
                if (IsPointInPolygon(position, new Polygon(polygon)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a point is in a polygon. Rings must be closed.
        /// </summary>
        /// <param name="point">Point with format [lat, lon]</param>
        /// <param name="polygonRing">Array of polygon coordinates with the format: [[lat0, lon0],  [lat1, lon1], ... [latN, lonN]]</param>
        /// <returns></returns>
        private static bool IsPointInPolygon(Position point, PositionCollection polygonRing)
        {
            var ring = polygonRing;

            //Ensure the polygon ring is closed.
            polygonRing.Close();

            //Ensure the polygon ring has at least 4 points. 3 to create a triangle, plus 4th to close the polyogn.
            if (ring.Count < 4)
            {
                return false;
            }

            double lat = point.Latitude;
            double lon = point.Longitude;
            bool inPoly = false;
            
            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
            {
                if (ring[i].Latitude > lat != ring[j].Latitude > lat &&
                    lon < ((ring[j].Longitude - ring[i].Longitude) * (lat - ring[i].Latitude)) /
                        (ring[j].Latitude - ring[i].Latitude) + ring[i].Longitude)
                {
                    inPoly = !inPoly;
                }
            }

            return inPoly;
        }

        /// <summary>
        /// Filters a set of points to those that are within a MultiPolygon.
        /// </summary>
        /// <param name="points">Points to filter.</param>
        /// <param name="multiPolygon">MultiPolygon to check against.</param>
        /// <returns>A filtered set of points.</returns>
        public static IList<PointGeometry> FilterIsPointInPolygon(IList<PointGeometry> points, MultiPolygon multiPolygon)
        {
            return points.Where(x => IsPointInPolygon(x, multiPolygon)).ToList();
        }

        /// <summary>
        /// Filters a set of points to those that are within a Polygon.
        /// </summary>
        /// <param name="points">Points to filter.</param>
        /// <param name="polygon">Polygon to check against.</param>
        /// <returns>A filtered set of points.</returns>
        public static IList<PointGeometry> FilterIsPointInPolygon(IList<PointGeometry> points, Polygon polygon)
        {
            return points.Where(x => IsPointInPolygon(x.Coordinates, polygon)).ToList();
        }

        #endregion

        #region Point in Circle

        /// <summary>
        /// Determines if a point is within a circle.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="origin">Origin of a circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="units">Distance units of the radius.</param>
        /// <returns>True if point is in circle.</returns>
        public static bool IsPointInCircle(PointGeometry point, Position origin, double radius, DistanceUnits units = DistanceUnits.Meters)
        {
            return IsPointInCircle(point.Coordinates, origin, radius, units);
        }

        /// <summary>
        /// Determines if a point is within a circle.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="origin">Origin of a circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="units">Distance units of the radius.</param>
        /// <returns>True if point is in circle.</returns>
        public static bool IsPointInCircle(Position point, Position origin, double radius, DistanceUnits units = DistanceUnits.Meters)
        {
            return GetDistanceTo(point, origin, units) <= radius;
        }

        /// <summary>
        /// Filters a set of points to those that are within a circle.
        /// </summary>
        /// <param name="points">Points to filter.</param>
        /// <param name="origin">Origin of a circle.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="units">Distance units of the radius.</param>
        /// <returns>A filtered list of points that are in the circle.</returns>
        public static IList<PointGeometry> FilterIsPointsInCircle(IList<PointGeometry> points, Position origin, double radius, DistanceUnits units = DistanceUnits.Meters)
        {
            return points.Where(x => IsPointInCircle(x.Coordinates, origin, radius, units)).ToList();
        }

        #endregion

        /// <summary>
        /// Returns a set of positions that outline the globe. Adds positions at 0 longitude to prevent polygon from collapsing on the anti-merdian.
        /// </summary>
        /// <returns></returns>
        public static PositionCollection GetGlobePolygonRing()
        {
            return new PositionCollection
            {
                new Position(-180, 90),
                new Position(-180, -90),
                new Position(0, -90),
                new Position(180, -90),
                new Position(180, 90),
                new Position(0, 90),
                new Position(-180, 90)
            };
        }

        #region DateTime Helpers

        /// <summary>
        /// Converts a DateTime object to a JSON date time.
        /// </summary>
        /// <param name="dateTime">The date to convert.</param>
        /// <returns>JSON date in milliseconds since Jan 1st, 1970</returns>
        public static double ToJsonDateTime(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// Converts a JSON date time to a DateTime object.
        /// </summary>
        /// <param name="dateTime">JSON date in milliseconds since Jan 1st, 1970</param>
        /// <returns>.NET DateTime representation of the JSON date.</returns>
        public static DateTime FromJsonDateTime(double dateTime)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(dateTime);
        }

        /// <summary>
        /// Converts a JSON date time to a DateTime object.
        /// </summary>
        /// <param name="dateTime">The date time object. Can be a string or JSON date number.</param>
        /// <returns>.NET DateTime representation of the JSON date.</returns>
        public static DateTime? FromJsonDateTime(object? dateTime)
        {
            if (dateTime != null)
            {
                //Try to parse the string as a double.
                if (double.TryParse(dateTime.ToString(), out double result))
                {
                    return FromJsonDateTime(result);
                }
                else if (DateTime.TryParse(dateTime.ToString(), out DateTime result2))
                {
                    return result2;
                }
            }

            return null;
        }

        #endregion
    }
}
