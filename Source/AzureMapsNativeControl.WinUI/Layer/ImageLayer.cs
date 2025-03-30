using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Overlays an image on the map with each corner anchored to a coordinate on the map. Also known as a ground or image overlay.
    /// </summary>
    public class ImageLayer : BaseLayer
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        private ImageLayerOptions _options = ImageLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// A layer for tiled images.
        /// </summary>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public ImageLayer(ImageLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.ImageLayer", null, id)
        {
            if (options != null)
            {
                ImageLayerOptions.Merge(options, _options);
            }

            if(options == null || options.Coordinates == null)
            {
                _options.Coordinates = ImageLayer.GetCoordinatesFromBoundingBox(BoundingBox.Global());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates coordinates for a rotated image layer when provided with the bounding box edges and rotation value. 
        /// Note: If your rotation value is from a KML Ground Overlay it will need to be converted to a clockwise rotation using the following formula: rotation = 360 – KmlRotation
        /// </summary>
        /// <param name="north">The north edge of the bounding box.</param>
        /// <param name="south">The south edge of the bounding box.</param>
        /// <param name="east">The east edge of the bounding box.</param>
        /// <param name="west">The west edge of the bounding box.</param>
        /// <param name="rotation">Clockwise rotation in degrees</param>
        /// <returns>Coordinates for a rotated image layer when provided with the bounding box edges and rotation value.</returns>
        public static Position[] GetCoordinatesFromEdges(double north, double south, double east, double west, double rotation = 0)
        {
            return GetCoordinatesFromBoundingBox(new BoundingBox(west, south, east, north), rotation);
        }

        /// <summary>
        /// Calculates coordinates for a rotated image layer when provided with a bounding box for the edges and rotation value. 
        /// Note: If your rotation value is from a KML Ground Overlay it will need to be converted to a clockwise rotation using the following formula: rotation = 360 – KmlRotation
        /// </summary>
        /// <param name="bounds">The bounding box.</param>
        /// <param name="rotation">Clockwise rotation in degrees</param>
        /// <returns>Coordinates for a rotated image layer when provided with the bounding box edges and rotation value.</returns>
        public static Position[] GetCoordinatesFromBoundingBox(BoundingBox bounds, double rotation = 0)
        {
            // Calculate the center of the bounding box and use that as the rotation origin.
            var origin = bounds.GetCenter();

            // Calculate the corner coordinates of the bounding box.
            var topLeft = new Position(bounds.West, bounds.North);
            var topRight = new Position(bounds.East, bounds.North);
            var bottomRight = new Position(bounds.East, bounds.South);
            var bottomLeft = new Position(bounds.West, bounds.South);
            
            // Calcuate to rotated corners of the bounding box.
            return AtlasMath.RotatePositions([
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            ], origin, rotation).ToArray();
        }

        /// <summary>
        /// Calculates the approximate corner coordinates for an image based on the image width, height and by calculating an affine transform from a set of source pixels in the image and a set of target positions that are related.
        /// The same number of source and target values must be provided as reference points.It is recommended to provide atleast 3 reference points.
        /// </summary>
        /// <param name="imgWidth">Image width</param>
        /// <param name="imgHeight">Image height</param>
        /// <param name="source">A set of source pixels</param>
        /// <param name="target">Target positions</param>
        /// <returns>Corner positions for the </returns>
        public static Position[] GetCoordinatesFromRefPoints(double imgWidth, double imgHeight, Pixel[] source, Position[] target)
        {
            var transform = new AtlasMath.AffineTransform(source, target);

            double[][] sourcePoints = new double[4][];
            sourcePoints[0] = new double[] { 0, 0 };
            sourcePoints[0] = new double[] { imgWidth, 0 };
            sourcePoints[0] = new double[] { imgWidth, imgHeight };
            sourcePoints[0] = new double[] { 0, imgHeight };

            var positionPoints = transform.ToTarget(sourcePoints);

            return positionPoints.Select(p => new Position(p[0], p[1])).ToArray();
        }

        /// <summary>
        /// Calculates the approximate pixels on the source image that align with the provided positions.
        /// </summary>
        /// <param name="positions">Positions from the source image used to calculate the pixels.</param>
        /// <returns>The approximate pixels on the source image that align with the provided positions.</returns>
        public async Task<Pixel[]?> GetPixels(IEnumerable<Position> positions)
        {
            if(Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<Pixel[]?>(Map, "getImageLayerPixels", Id, positions);
            }

            return null;
        }

        /// <summary>
        /// Calculates the approximate positions that align with the provided pixels from the source image.
        /// </summary>
        /// <param name="pixels">Pixels from the source image used to calculate the positions.</param>
        /// <returns>The approximate positions that align with the provided pixels from the source image.</returns>
        public async Task<Position[]?> GetPositions(IEnumerable<Pixel> pixels)
        {
            if (Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<Position[]?>(Map, "getImageLayerPositions", Id, pixels);
            }

            return null;
        }

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override ImageLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(ImageLayerOptions options)
        {
            //Convert the original url to a data URI if not already done.
            if (!string.IsNullOrWhiteSpace(_options.Url) && !_options.Url.StartsWith("data:"))
            {
                _options.Url = await Utils.TryGetDataUri(_options.Url);
            }

            //Convert the url to a data URI if not already done.
            if (!string.IsNullOrWhiteSpace(options.Url) && !options.Url.StartsWith("data:"))
            {
                options.Url = await Utils.TryGetDataUri(options.Url);
            }

            var originalUrl = _options.Url;

            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (ImageLayerOptions.Merge(options, _options) && Map != null)
            {
                //Check to see if the URL is the same as the original URL.
                if (originalUrl == _options.Url)
                {
                    //If so, don't resend the URL. 
                    var opt = _options.DeepClone();
                    opt.Url = null;

                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, opt);
                }
                else
                {
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
                }
            }
        }

        #endregion
    }
}