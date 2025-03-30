using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureMapsNativeControl.Tiles
{
    /// <summary>
    /// Tile System math for the Spherical Mercator projection coordinate system (EPSG:3857)
    /// Based on:https://learn.microsoft.com/en-us/azure/azure-maps/zoom-levels-and-tile-grid?tabs=csharp
    /// </summary>
    public static class TileMath
    {
        #region Public Methods

        /// <summary>
        /// Calculates width and height of the map in pixels at a specific zoom level from -180 degrees to 180 degrees.
        /// </summary>
        /// <param name="zoom">Zoom Level to calculate width at</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>Width and height of the map in pixels</returns>
        public static double MapSize(double zoom, int tileSize = 512)
        {
            return Math.Ceiling(tileSize * Math.Pow(2, zoom));
        }

        /// <summary>
        /// Calculates the Ground resolution at a specific degree of latitude in meters per pixel.
        /// </summary>
        /// <param name="latitude">Degree of latitude to calculate resolution at</param>
        /// <param name="zoom">Zoom level to calculate resolution at</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>Ground resolution in meters per pixels</returns>
        public static double GroundResolution(double latitude, double zoom, int tileSize = 512)
        {
            latitude = ClipLatitude(latitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * AtlasMath.EarthRadius.Meters / MapSize(zoom, tileSize);
        }

        /// <summary>
        /// Determines the map scale at a specified latitude, level of detail, and screen resolution.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the map scale.</param>
        /// <param name="zoom">Level of detail, from 0 (lowest detail) to 24 (highest detail).</param>
        /// <param name="screenDpi">Resolution of the screen, in dots per inch.</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>The map scale, expressed as the denominator N of the ratio 1 : N.</returns>
        public static double MapScale(double latitude, double zoom, int screenDpi, int tileSize = 512)
        {
            return GroundResolution(latitude, zoom, tileSize) * screenDpi / 0.0254;
        }

        /// <summary>
        /// Global Converts a Pixel coordinate into a geospatial coordinate at a specified zoom level. 
        /// Global Pixel coordinates are relative to the top left corner of the map (90, -180)
        /// </summary>
        /// <param name="pixel">Pixel coordinates in the format of [x, y].</param>  
        /// <param name="zoom">Zoom level</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>A position value in the format [longitude, latitude].</returns>
        public static Position GlobalPixelToPosition(Pixel pixel, double zoom, int tileSize = 512)
        {
            var mapSize = MapSize(zoom, tileSize);

            var x = Clip(pixel[0], 0, mapSize - 1) / mapSize - 0.5;
            var y = 0.5 - Clip(pixel[1], 0, mapSize - 1) / mapSize;

            return new Position(
                360 * x,    //Longitude
                90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI  //Latitude
            );
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees) into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="position">Position coordinate in the format [longitude, latitude]</param>
        /// <param name="zoom">Zoom level.</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param> 
        /// <returns>A global pixel coordinate.</returns>
        public static Pixel PositionToGlobalPixel(Position position, int zoom, int tileSize = 512)
        {
            var latitude = ClipLatitude(position.Latitude);
            var longitude = ClipLongitude(position.Longitude);

            var x = (longitude + 180) / 360;
            var sinLatitude = Math.Sin(latitude * Math.PI / 180);
            var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            var mapSize = MapSize(zoom, tileSize);

            return new Pixel(
                 Clip(x * mapSize + 0.5, 0, mapSize - 1),
                 Clip(y * mapSize + 0.5, 0, mapSize - 1)
            );
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing the specified pixel.
        /// Tile size defaulted to 512.
        /// </summary>
        /// <param name="pixel">Pixel coordinates in the format of [x, y].</param>  
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        public static void GlobalPixelToTileXY(Pixel pixel, out int tileX, out int tileY)
        {
            tileX = (int)pixel.X / 512;
            tileY = (int)pixel.Y / 512;
        }

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing the specified pixel.
        /// </summary>
        /// <param name="pixel">Pixel coordinates in the format of [x, y].</param>  
        /// <param name="tileSize">The size of the tiles in the tile pyramid.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        public static void GlobalPixelToTileXY(Pixel pixel, int tileSize, out int tileX, out int tileY)
        {
            tileX = (int)(pixel.X / tileSize);
            tileY = (int)(pixel.Y / tileSize);
        }

        /// <summary>
        /// Performs a scale transform on a global pixel value from one zoom level to another.
        /// </summary>
        /// <param name="pixel">Pixel coordinates in the format of [x, y].</param>  
        /// <param name="oldZoom">The zoom level in which the input global pixel value is from.</param>  
        /// <returns>A scale pixel coordinate.</returns>
        public static Pixel ScaleGlobalPixel(Pixel pixel, double oldZoom, double newZoom)
        {
            var scale = Math.Pow(2, oldZoom - newZoom);

            return new Pixel(pixel[0] * scale, pixel[1] * scale);
        }

        /// <summary>
        /// Performs a scale transform on a set of global pixel values from one zoom level to another.
        /// </summary>
        /// <param name="pixels">A set of global pixel value from the old zoom level. Points are in the format [x,y].</param>
        /// <param name="oldZoom">The zoom level in which the input global pixel values is from.</param>
        /// <param name="newZoom">The new zoom level in which the output global pixel values should be aligned with.</param>
        /// <returns>A set of global pixel values that has been scaled for the new zoom level.</returns>
        public static IList<Pixel> ScaleGlobalPixels(IEnumerable<Pixel> pixels, double oldZoom, double newZoom)
        {
            var scale = Math.Pow(2, oldZoom - newZoom);

            var output = new List<Pixel>();
            foreach (var p in pixels)
            {
                output.Add(new Pixel(p[0] * scale, p[1] * scale));
            }

            return output;
        }

        /// <summary>
        /// Converts tile XY coordinates into a global pixel XY coordinates of the upper-left pixel of the specified tile.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        public static Pixel TileXYToGlobalPixel(int tileX, int tileY, int tileSize = 512)
        {
            return new Pixel(tileX * tileSize, tileY * tileSize);
        }

        /// <summary>
        /// Converts tile XY coordinates into a quadkey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="zoom">Zoom level</param>
        /// <returns>A string containing the quadkey.</returns>
        public static string TileXYToQuadKey(int tileX, int tileY, int zoom)
        {
            var quadKey = new StringBuilder();
            for (int i = zoom; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << i - 1;
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        /// <summary>
        /// Converts a quadkey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">Quadkey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="zoom">Output parameter receiving the zoom level.</param>
        public static void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int zoom)
        {
            tileX = tileY = 0;
            zoom = quadKey.Length;
            for (int i = zoom; i > 0; i--)
            {
                int mask = 1 << i - 1;
                switch (quadKey[zoom - i])
                {
                    case '0':
                        break;

                    case '1':
                        tileX |= mask;
                        break;

                    case '2':
                        tileY |= mask;
                        break;

                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;

                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
        }

        /// <summary>
        /// Calculates the XY tile coordinates that a coordinate falls into for a specific zoom level.
        /// Tile size defaulted to 512.
        /// </summary>
        /// <param name="position">Position coordinate in the format [longitude, latitude]</param>
        /// <param name="zoom">Zoom level</param>
        /// <param name="tileX">Output parameter receiving the tile X position.</param>
        /// <param name="tileY">Output parameter receiving the tile Y position.</param>
        public static void PositionToTileXY(Position position, int zoom, out int tileX, out int tileY)
        {
            PositionToTileXY(position, zoom, 512, out tileX, out tileY);
        }

        /// <summary>
        /// Calculates the XY tile coordinates that a coordinate falls into for a specific zoom level.
        /// </summary>
        /// <param name="position">Position coordinate in the format [longitude, latitude]</param>
        /// <param name="zoom">Zoom level</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid.</param>
        /// <param name="tileX">Output parameter receiving the tile X position.</param>
        /// <param name="tileY">Output parameter receiving the tile Y position.</param>
        public static void PositionToTileXY(Position position, int zoom, int tileSize, out int tileX, out int tileY)
        {
            var latitude = Clip(position.Latitude, Constants.MinLatitude, Constants.MaxLatitude);
            var longitude = Clip(position.Longitude, Constants.MinLongitude, Constants.MaxLongitude);

            var x = (longitude + 180) / 360;
            var sinLatitude = Math.Sin(latitude * Math.PI / 180);
            var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            //tileSize needed in calculations as in rare cases the multiplying/rounding/dividing can make the difference of a pixel which can result in a completely different tile. 
            var mapSize = MapSize(zoom, tileSize);
            tileX = (int)Math.Floor(Clip(x * mapSize + 0.5, 0, mapSize - 1) / tileSize);
            tileY = (int)Math.Floor(Clip(y * mapSize + 0.5, 0, mapSize - 1) / tileSize);
        }

        /// <summary>
        /// Calculates the tiles that are within a specified viewport.
        /// </summary>
        /// <param name="position">Position coordinate in the format [longitude, latitude]</param>
        /// <param name="zoom">Zoom level</param>
        /// <param name="width">The width of the map viewport in pixels.</param>
        /// <param name="height">The height of the map viewport in pixels.</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>A list of quadkey strings that are within the specified viewport.</returns>
        public static IList<TileInfo> GetTilesInView(Position position, int zoom, int width, int height, int tileSize = 512)
        {
            var p = PositionToGlobalPixel(position, zoom, tileSize);

            var top = p[1] - height * 0.5;
            var left = p[0] - width * 0.5;

            var bottom = p[1] + height * 0.5;
            var right = p[0] + width * 0.5;

            var tl = GlobalPixelToPosition(new Pixel(left, top), zoom, tileSize);
            var br = GlobalPixelToPosition(new Pixel(right, bottom), zoom, tileSize);

            //Boudning box in the format: [west, south, east, north];
            var bounds = new BoundingBox(tl.Longitude, br.Latitude, br.Longitude, tl.Latitude);

            return GetTilesInBoundingBox(bounds, zoom, tileSize);
        }

        /// <summary>
        /// Calculates the tile quadkey strings that are within a bounding box at a specific zoom level.
        /// </summary>
        /// <param name="bounds">A bounding box defined as an array of numbers in the format of [west, south, east, north].</param>
        /// <param name="zoom">Zoom level to calculate tiles for.</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <returns>A list of quadkey strings.</returns>
        public static IList<TileInfo> GetTilesInBoundingBox(BoundingBox bounds, int zoom, int tileSize = 512)
        {
            var keys = new List<TileInfo>();

            if (bounds != null)
            {
                PositionToTileXY(new Position(bounds.West, bounds.North), zoom, tileSize, out int tlX, out int tlY);
                PositionToTileXY(new Position(bounds.South, bounds.West), zoom, tileSize, out int brX, out int brY);

                for (int x = tlX; x <= brX; x++)
                {
                    for (int y = tlY; y <= brY; y++)
                    {                        
                        keys.Add(new TileInfo(x, y, zoom, tileSize));
                    }
                }
            }

            return keys;
        }

        /// <summary>
        /// Calculates the best map view (center, zoom) for a bounding box on a map.
        /// </summary>
        /// <param name="bounds">A bounding box defined as an array of numbers in the format of [west, south, east, north].</param>
        /// <param name="mapWidth">Map width in pixels.</param>
        /// <param name="mapHeight">Map height in pixels.</param>
        /// <param name="padding">Width in pixels to use to create a buffer around the map. This is to keep markers from being cut off on the edge. Default: 0</param>
        /// <param name="tileSize">The size of the tiles in the tile pyramid. Default: 512</param>
        /// <param name="maxZoom">Optional maximum zoom level to return. Useful when the bounding box represents a very small area. Default: 24</param>
        /// <param name="allowFloatZoom">Specifies if the returned zoom level should be a float or rounded down to an whole integer zoom level. Default: true</param>
        public static CameraOptions BestMapView(BoundingBox bounds, double mapWidth, double mapHeight, int padding = 0, int tileSize = 512, double maxZoom = 24, bool allowFloatZoom = true)
        {
            Position center = new Position(0, 0);
            double zoom = 0;

            if (bounds != null && mapWidth > 0 && mapHeight > 0)
            {
                //Ensure padding is valid.
                padding = Math.Abs(padding);

                //Ensure max zoom is within valid range.
                maxZoom = Clip(maxZoom, 0, 24);

                //Do pixel calculations at zoom level 24 as that will provide a high level of visual accuracy.
                int pixelZoom = 24;

                //Calculate mercator pixel coordinate at zoom level 24.
                var wnPixel = PositionToGlobalPixel(bounds.GetNorthWest(), pixelZoom, tileSize);
                var esPixel = PositionToGlobalPixel(bounds.GetSouthEast(), pixelZoom, tileSize);

                //Calculate the pixel distance between pixels for each axis.
                double dx = esPixel[0] - wnPixel[0];
                double dy = esPixel[1] - wnPixel[1];

                //Calculate the average pixel positions to get the visual center.
                double xAvg = (esPixel[0] + wnPixel[0]) / 2;
                double yAvg = (esPixel[1] + wnPixel[1]) / 2;

                //Determine if the bounding box crosses the antimeridian. (West pixel will be greater than East pixel).
                if (wnPixel[0] > esPixel[0])
                {
                    double mapSize = MapSize(24, tileSize);

                    //We are interested in the opposite area of the map. Calculate the opposite area and visual center.
                    dx = mapSize - Math.Abs(dx);

                    //Offset the visual center by half the global map width at zoom 24 on the x axis.
                    xAvg += mapSize / 2;
                }

                //Convert visual center pixel from zoom 24 to lngLat.
                center = GlobalPixelToPosition(new Pixel(xAvg, yAvg), pixelZoom, tileSize);

                //Calculate scale of screen pixels per unit on the Web Mercator plane.
                double scaleX = (mapWidth - padding * 2) / Math.Abs(dx) * Math.Pow(2, pixelZoom);
                double scaleY = (mapHeight - padding * 2) / Math.Abs(dy) * Math.Pow(2, pixelZoom);

                //Calculate zoom levels based on the x/y scales. Choose the most zoomed out value.
                zoom = Math.Max(0, Math.Min(maxZoom, Math.Log2(Math.Abs(Math.Min(scaleX, scaleY)))));

                //Round down zoom level if float values are not desired.
                if (!allowFloatZoom)
                {
                    zoom = Math.Floor(zoom);
                }
            }

            return new CameraOptions
            {
                Center = center,
                Zoom = zoom
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        internal static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Clips a latitude value to acceptable range.
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        internal static double ClipLatitude(double lat)
        {
            return Math.Min(Math.Max(lat, Constants.MinLatitude), Constants.MaxLatitude);
        }

        /// <summary>
        /// Clips a latitude value to acceptable range.
        /// </summary>
        /// <param name="lon"></param>
        /// <returns></returns>
        internal static double ClipLongitude(double lon)
        {
            return Math.Min(Math.Max(lon, Constants.MinLongitude), Constants.MaxLongitude);
        }

        #endregion
    }
}
