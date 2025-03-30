using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if MAUI
#elif WINUI
using Microsoft.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// Internal utility methods.
    /// </summary>
    internal static class Utils
    {
        private static Regex NewLineSpaceRx = new Regex("[\n\r]");

        /// <summary>
        /// Checks if a URL is an Azure Maps REST service request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static bool IsAzureMapsRestRequest(string url)
        {
            return url.Contains("{azMapsDomain}", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("atlas.microsoft.com", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("atlas.azure.", StringComparison.OrdinalIgnoreCase); //Could be a gov cloud domain like "atlas.azure.us"
        }

        #region JsonElement helpers

        /// <summary>
        /// Gets a property by name from a JsonElement object. Has an optional fallback to check for all caps version of the name.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="checkAllCaps"></param>
        /// <returns></returns>
        internal static JsonElement? GetJsonElementProperty(JsonElement element, string name, bool checkAllCaps)
        {
            if (element.TryGetProperty(name, out JsonElement property))
            {
                return property;
            }

            if (checkAllCaps)
            {
                if (element.TryGetProperty(name.ToUpperInvariant(), out property))
                {
                    return property;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to read a string property from a JsonElement object.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryReadStringProperty(JsonElement element, string name, out string value)
        {
            value = string.Empty;

            if (element.ValueKind == JsonValueKind.Object)
            {
                var property = Utils.GetJsonElementProperty(element, name, true);
                var t = property?.GetString();

                if (t != null)
                {
                    value = t;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to read a double property from a JsonElement object.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryReadDoubleProperty(JsonElement element, string name, out double value)
        {
            value = 0;

            if (element.ValueKind == JsonValueKind.Object)
            {
                var property = Utils.GetJsonElementProperty(element, name, true);
                var t = property?.GetDouble();

                if (t.HasValue)
                {
                    value = t.Value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to read a int32 property from a JsonElement object.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryReadInt32Property(JsonElement element, string name, out int value)
        {
            value = 0;

            if (element.ValueKind == JsonValueKind.Object)
            {
                var property = Utils.GetJsonElementProperty(element, name, true);
                var t = property?.GetInt32();

                if (t.HasValue)
                {
                    value = t.Value;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Number helpers

        /// <summary>
        /// Checks to see if an object is a number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsNumber(object? value)
        {
            if (value == null)
            {
                return false;
            }

            return value is double || value is float || value is int ||
                value is sbyte || value is byte || value is short || value is ushort ||
                 value is uint || value is long || value is ulong || value is decimal;
        }

        /// <summary>
        /// Compares two double values to see if they are equal within a tolerance of 0.0000000001.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        internal static bool TenDecimalCompare(double num1, double num2)
        {
            return Math.Abs(num1 - num2) < 0.0000000001;
        }

        /// <summary>
        /// Tries to convert an object to a boolean.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToBool(object? obj, out bool value)
        {
            if (obj != null)
            {
                if (obj is bool val)
                {
                    value = val;
                    return true;
                }
                else if (obj is string)
                {
                    if (bool.TryParse(obj.ToString(), out bool val2))
                    {
                        value = val2;
                        return true;
                    }
                }
                else if (Utils.IsNumber(obj))
                {
                    value = Convert.ToDouble(obj) == 1 ? true : false;
                    return true;
                }
            }

            value = false;
            return false;
        }

        /// <summary>
        /// Tries to convert an object to a double.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToDouble(object? obj, out double value)
        {
            if (obj != null)
            {
                if (Utils.IsNumber(obj) || obj is bool)
                {
                    value = Convert.ToDouble(obj);
                    return true;
                }
                else if (obj is string)
                {
                    if (double.TryParse(obj.ToString(), out double val2))
                    {
                        value = val2;
                        return true;
                    }
                }
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Tries to convert an object to an Int32.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToInt32(object? obj, out int value)
        {
            if (obj != null)
            {
                if (Utils.IsNumber(obj) || obj is bool)
                {
                    value = Convert.ToInt32(obj);
                    return true;
                }
                else if (obj is string)
                {
                    if (int.TryParse(obj.ToString(), out int val2))
                    {
                        value = val2;
                        return true;
                    }
                }
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Tries to convert an object to a string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToString(object? obj, out string value)
        {
            if (obj != null)
            {
                if (obj is string sVal)
                {
                    value = sVal;
                    return true;
                }
                else if (Utils.IsNumber(obj) || obj is bool)
                {
                   var v = obj.ToString();
                    if (v != null)
                    {
                        value = v;
                        return true;
                    }
                }
                else
                {
                    try
                    {
                        var v = Convert.ToString(obj);

                        if(v != null){
                            value = v;
                            return true;
                        }
                    }
                    catch { }
                }
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Tries to convert an object to a list of objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToList(object? obj, out IList<object?> value)
        {
            if (obj != null)
            {
                //Simple check for List<T> type.
                if (obj is IList<object?> typedList)
                {
                    value = typedList;
                    return true;
                }

                //Fallback to try to convert to a list.
                if (obj is IList list)
                {
                    value = new List<object?>();

                    foreach (var item in list)
                    {
                        value.Add(item);
                    }

                    return true;
                }
            }

            value = new List<object?>();
            return false;
        }

        /// <summary>
        /// Tries to convert an object to a list of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryConvertToList<T>(object? obj, out IList<T?> value) where T : class
        {
            if (obj != null)
            {
                //Simple check for List<T> type.
                if (obj is IList<T?> typedList)
                {
                    value = typedList;
                    return true;
                }

                //Fallback to try to convert to a list.
                if (obj is IList list)
                {
                    bool isBool = (typeof(T) == typeof(bool));
                    bool isDouble = (typeof(T) == typeof(double));
                    bool isFloat = (typeof(T) == typeof(float));
                    bool isInt = (typeof(T) == typeof(int));
                    bool isString = (typeof(T) == typeof(string));
                    bool isObject = (typeof(T) == typeof(object));

                    value = new List<T?>();

                    foreach (var item in list)
                    {
                        //Simply check for type match.
                        if (item is T itemT)
                        {
                            value.Add(itemT);
                        }

                        //Check for primitive types.
                        else if (isBool)
                        {
                            if (TryConvertToBool(item, out bool val))
                            {
                                value.Add(val as T);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (isDouble)
                        {
                            if (TryConvertToDouble(item, out double val))
                            {
                                value.Add(val as T);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (isFloat)
                        {
                            if (TryConvertToDouble(item, out double val))
                            {
                                value.Add((float)val as T);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (isInt)
                        {
                            if (TryConvertToInt32(item, out int val))
                            {
                                value.Add(val as T);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (isString)
                        {
                            if (TryConvertToString(item, out string? val))
                            {
                                value.Add(val as T);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (isObject)
                        {
                            value.Add(item as T);
                            return true;
                        }
                    }
                }
            }

            value = new List<T?>();
            return false;
        }

        #endregion

        #region File and URL helpers

        /// <summary>
        /// Gets a URL to a proxy endpoint for a given URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlProxy(string url)
        {
            return $"{Constants.ProxyOperation}{Constants.ProxyWebRequestOperation}&url={Uri.EscapeDataString(url)}";
        }

        /// <summary>
        /// Gets a URL to a proxy endpoint for a given tile source.
        /// </summary>
        /// <param name="tileSourceId"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static string GetCustomTileSourceProxy(string tileSourceId, int tileSize = 512)
        {
            return $"{Constants.ProxyOperation}{Constants.CustomTileSourceOperation}&tileSourceId={tileSourceId}&x={{x}}&y={{y}}&z={{z}}&quadkey={{quadkey}}&bbox-epsg-3857={{bbox-epsg-3857}}&tilesize={tileSize}";
        }

        /// <summary>
        /// Gets tile info from a proxy custom tile source request query parameters.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public static TileInfo GetTileInfoFromProxyRequest(IDictionary<string, string> queryParams)
        {
            int x = 0;
            int y = 0;
            int zoom = 0;
            string? quadkey = null;
            string? subdomain = null;
            double[]? bounds3857 = null;
            int tileSize = 512;

            if (queryParams.ContainsKey("x"))
            {
                x = int.Parse(queryParams["x"]);
            }

            if (queryParams.ContainsKey("y"))
            {
                y = int.Parse(queryParams["y"]);
            }

            if (queryParams.ContainsKey("z"))
            {
                zoom = int.Parse(queryParams["z"]);
            }

            if (queryParams.ContainsKey("quadkey"))
            {
                quadkey = queryParams["quadkey"];
            }

            if (queryParams.ContainsKey("subdomain"))
            {
                subdomain = queryParams["subdomain"];
            }

            if (queryParams.ContainsKey("bbox-epsg-3857"))
            {
                var bounds = queryParams["bbox-epsg-3857"].Split(',');

                if (bounds.Length == 4)
                {
                    bounds3857 = new double[] { double.Parse(bounds[0]), double.Parse(bounds[1]), double.Parse(bounds[2]), double.Parse(bounds[3]) };
                }
            }

            if (queryParams.ContainsKey("tilesize"))
            {
                tileSize = int.Parse(queryParams["tilesize"]);
            }

            return new TileInfo(x, y, zoom, tileSize, quadkey, bounds3857);
        }

        /// <summary>
        /// Sanitizes an image URL to be used in the hybrid web view.
        /// Proxies Web hosted images and converts SVGs to data URIs.
        /// </summary>
        /// <param name="imagePathOrUrl">Image URL to sanitize.</param>
        /// <returns></returns>
        public static async Task<string?> SanitizeImageUrl(string? imagePathOrUrl, Map map)
        {
            if (string.IsNullOrWhiteSpace(imagePathOrUrl))
            {
                return imagePathOrUrl;
            }

            if (imagePathOrUrl.StartsWith("//")) //Image URL is a protocol relative URL. Need to add the protocol.
            {
                imagePathOrUrl = "https:" + imagePathOrUrl;
            }

            string urlMimeType = GetMimeType(imagePathOrUrl);

            try
            {
                //Image URL points to a web hosted image. Download from native code to avoid CORs issues.
                //Make sure it is not a request to 0.0.0.0 domain which is used by the hybrid web browser.
                if (!imagePathOrUrl.Contains("0.0.0.0/") && MapHttpClientManager.IsValidUri(imagePathOrUrl))
                {
                    var response = await MapHttpClientManager.TryGetStreamAsync(imagePathOrUrl);

                    if (response != null)
                    {
                        imagePathOrUrl = GetDataUriFromStream(response.Stream, response.MimeType ?? "image/png");
                    }
                }

                //Check to see if the image URL is a relative URL to an SVG file, but is not a data URI or inline SVG
                if (!string.IsNullOrEmpty(imagePathOrUrl) && urlMimeType.Equals("image/svg+xml") &&
                    !imagePathOrUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase) &&
                    !imagePathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                    !imagePathOrUrl.Contains("<svg", StringComparison.OrdinalIgnoreCase))
                {
                    var r = await TryGetFileStreamAsync(Constants.MapWebViewHybridAssetRoot + "/" + imagePathOrUrl);
                    if(r != null)
                    {
                        //The hybrid web view currently does not support local SVG files. Need to convert the SVG to a data URI.
                        imagePathOrUrl = GetDataUriFromStream(r.Stream, "image/svg+xml");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading image: " + ex.Message);
            }

            //If the image is an inline SVG, need to convert it to a data URI.
            if (!string.IsNullOrEmpty(imagePathOrUrl) && imagePathOrUrl.Contains("<svg", StringComparison.OrdinalIgnoreCase))
            {
                imagePathOrUrl = "data:image/svg+xml;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(NewLineSpaceRx.Replace(imagePathOrUrl, "")));
            }

            return imagePathOrUrl;
        }

        /// <summary>
        /// Tries to get the mime type from a file name or URL by looking for valid file extensions or mime types in a data URI.
        /// Input can be a file name, url (with query string), a data uri, mime type, or file extension.
        /// </summary>
        /// <param name="fileNameOrUrl">A file name, url (with query string), a data uri, mime type, or file extension.</param>
        /// <returns>The determined mime type.</returns>
        public static string GetMimeType(string? fileNameOrUrl, string defaultMimeType = "text/plain")
        {
            if (string.IsNullOrWhiteSpace(fileNameOrUrl))
            {
                return defaultMimeType;
            }

            string? ext;
            string? mimeType;

            //Check for a mime type in a data uri.
            if (fileNameOrUrl.Contains("data:") && fileNameOrUrl.Contains(";base64,"))
            {
                ext = fileNameOrUrl.Substring(5, fileNameOrUrl.IndexOf(";base64,") - 5);

                if (TryGetMimeType(ext, out mimeType))
                {
                    return mimeType;
                }
            }

            //Seperate out query string if it exists.
            string queryString = string.Empty;

            if (fileNameOrUrl.Contains("?"))
            {
                queryString = fileNameOrUrl.Substring(fileNameOrUrl.IndexOf("?"));
                fileNameOrUrl = fileNameOrUrl.Substring(0, fileNameOrUrl.IndexOf("?"));
            }

            //If there is still a url or file name, check it for a valid file extension.
            if (!string.IsNullOrWhiteSpace(fileNameOrUrl))
            {
                ext = Path.GetExtension(fileNameOrUrl);

                if (TryGetMimeType(ext, out mimeType))
                {
                    return mimeType;
                }

                //Try passing the whole file name to see if it is a valid mime type. This would work if only a file extension or mimetype was passed in.
                if (TryGetMimeType(fileNameOrUrl, out mimeType))
                {
                    return mimeType;
                }
            }

            //If there is a query string, check it's parameter values to see if it contains something with a valid file extension.
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                var queryParameters = queryString.Split('&');

                foreach (var param in queryParameters)
                {
                    if (param.Contains("="))
                    {
                        ext = param.Substring(param.IndexOf("=") + 1);

                        if (TryGetMimeType(ext, out mimeType))
                        {
                            return mimeType;
                        }
                    }
                }
            }

            //If get here, return plain text mime type.
            return defaultMimeType;
        }

        /// <summary>
        /// Looks up a mime type based on a file extension or mime type.
        /// </summary>
        /// <param name="mimeTypeOrFileExtension">A mimeType or file extension to validate and get the mime type for.</param>
        /// <returns>A boolean indicating if it found a supported mime type.</returns>
        internal static bool TryGetMimeType(string? mimeTypeOrFileExtension, out string mimeType)
        {
            mimeType = string.Empty;

            if (string.IsNullOrWhiteSpace(mimeTypeOrFileExtension))
            {
                return false;
            }

            //If content type starts with a period, remove it. File extension may have been passed in.
            if (mimeTypeOrFileExtension.StartsWith("."))
            {
                mimeTypeOrFileExtension = mimeTypeOrFileExtension.Substring(1);
            }

            //For simplirity, if the content type contains a slash, assume it is a file path extension.
            if (mimeTypeOrFileExtension.Contains("/"))
            {
                //Return the string after the last index of the slash.
                mimeTypeOrFileExtension = mimeTypeOrFileExtension.Substring(mimeTypeOrFileExtension.LastIndexOf("/") + 1);
            }

            //Sanitize the content type.
            mimeType = mimeTypeOrFileExtension.ToLowerInvariant() switch
            {
                //WebAssembly file types
                "wasm" => "application/wasm",

                //Image file types
                "png" => "image/png",
                "jpg" or "jpeg" or "jfif" or "pjpeg" or "pjp" => "image/jpeg",
                "gif" => "image/gif",
                "webp" => "image/webp",
                "svg" or "svg+xml" => "image/svg+xml",
                "ico" or "x-icon" => "image/x-icon",
                "bmp" => "image/bmp",
                "tif" or "tiff" => "image/tiff",
                "avif" => "image/avif",
                "apng" => "image/apng",

                //Video file types
                "mp4" => "video/mp4",
                "webm" => "video/webm",
                "mpeg" => "video/mpeg",

                //Audio file types
                "mp3" => "audio/mpeg",
                "wav" => "audio/wav",

                //Font file types
                "woff" => "font/woff",
                "woff2" => "font/woff2",
                "otf" => "font/otf",

                //JSON and XML based file types
                "json" or "geojson" or "geojsonseq" or "topojson" => "application/json",
                "gpx" or "georss" or "gml" or "citygml" or "czml" or "xml" => "application/xml",
                "kml" or "kml+xml" or "vnd.google-earth.kml+xml" => "application/vnd.google-earth.kml+xml",

                //Office file types
                "doc" or "docx" or "msword" => "application/msword",
                "xls" or "xlsx" or "vnd.ms-excel" => "application/vnd.ms-excel",
                "ppt" or "pptx" or "vnd.ms-powerpoint" => "application/vnd.ms-powerpoint",

                //3D model file types commonly used in web.
                "gltf" or "gltf+json" => "model/gltf+json",
                "glb" or "gltf-binary" => "model/gltf-binary",
                "dae" => "model/vnd.collada+xml",

                //Other binary file types
                "zip" => "application/zip",
                "pbf" or "x-protobuf" => "application/x-protobuf",
                "mvt" or "vnd.mapbox-vector-tile" => "application/vnd.mapbox-vector-tile",
                "kmz" or "vnd.google-earth.kmz" or "shp" or "dbf" or "bin" or "b3dm" or "i3dm" or "pnts" or "subtree" or "octet-stream" => "application/octet-stream",
                "pdf" => "application/pdf",

                //Other map tile file types
                "terrian" => "application/vnd.quantized-mesh",
                "pmtiles" => "application/vnd.pmtiles",

                //Text based file types
                "htm" or "html" => "text/html",
                "xhtml" or "xhtml+xml" => "application/xhtml+xml",
                "js" or "javascript" => "text/javascript",
                "css" => "text/css",
                "csv" => "text/csv",
                "md" => "text/markdown",
                "plain" or "txt" or "wat" => "text/plain",

                _ => string.Empty,
            };

            return !string.IsNullOrWhiteSpace(mimeType);
        }

        /// <summary>
        /// Checks if a mime type is a binary file type.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static bool IsMimeTypeBinary(string mimeType)
        {
            return mimeType switch
            {
                //WebAssembly file types
                "application/wasm" or

                // Image file types
                "image/png" or "image/jpeg" or "image/gif" or "image/webp" or "image/svg+xml" or "image/x-icon" or "image/bmp" or "image/tiff" or "image/avif" or "image/apng" or

                // Video file types
                "video/mp4" or "video/webm" or "video/mpeg" or

                // Audio file types
                "audio/mpeg" or "audio/wav" or

                // Font file types
                "font/woff" or "font/woff2" or "font/otf" or

                //Office file types
                "application/msword" or "application/vnd.ms-excel" or "application/vnd.ms-powerpoint" or

                // GLTF related file types
                "model/gltf-binary" or

                //Other map tile file types
                "application/vnd.pmtiles" or "application/vnd.quantized-mesh" or

                // Other binary file types
                "application/zip" or "application/x-protobuf" or "application/octet-stream" or "application/pdf" => true,

                _ => false,
            };
        }

        /// <summary>
        /// Converts a stream to a data URI.
        /// </summary>
        /// <param name="stream">The data stream.</param>
        /// <param name="contentType">The type of content to stream contains. Can be a mimeType or file extension.</param>
        /// <returns></returns>
        public static string? GetDataUriFromStream(Stream stream, string? contentType = "text/plain")
        {
            string mimeType = GetMimeType(contentType);
            bool isBinaryData = IsMimeTypeBinary(mimeType);

            if (isBinaryData)
            {
                if(stream is MemoryStream ms)
                {
                    stream.Position = 0;
                    return "data:" + contentType + ";base64," + Convert.ToBase64String(ms.ToArray());
                } 
                else
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return "data:" + contentType + ";base64," + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            else
            {
                using (var reader = new StreamReader(stream))
                {
                    return "data:" + contentType + ";base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(NewLineSpaceRx.Replace(reader.ReadToEnd(), "")));
                }
            }
        }

        /// <summary>
        /// Tries get a file stream from a URL (absolute) or a file path (full, relative to asset folder, or app data directory).
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <returns></returns>
        internal static async Task<MapFileStream?> TryGetFileStreamAsync(string filePathOrUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePathOrUrl) || filePathOrUrl.Contains("0.0.0.0/"))
                {
                    return null;
                }

                if (filePathOrUrl.StartsWith("//")) //URL is a protocol relative URL. Need to add the protocol.
                {
                    filePathOrUrl = "https:" + filePathOrUrl;
                }

                string mimeType = GetMimeType(filePathOrUrl);

                //Check to see if the file path is a URL.
                if (Uri.TryCreate(filePathOrUrl, UriKind.Absolute, out Uri fileUri)
                    && (fileUri.Scheme == Uri.UriSchemeHttp || fileUri.Scheme == Uri.UriSchemeHttps))
                {
                    //Use MapHttpClientService to download the file.
                    var response = await MapHttpClientManager.TryGetStreamAsync(fileUri);

                    if (response != null && string.IsNullOrWhiteSpace(response.MimeType))
                    {
                        response.MimeType = mimeType;
                    }

                    return response;
                }

                if (filePathOrUrl.StartsWith("/"))
                {
                    filePathOrUrl = filePathOrUrl.Substring(1);
                }

                if (filePathOrUrl.StartsWith("data:"))
                {
                    //Handle data URI.
                    var data = filePathOrUrl.Substring(filePathOrUrl.IndexOf(",") + 1);
                    var bytes = Convert.FromBase64String(data);
                    return new MapFileStream(new MemoryStream(bytes), mimeType);
                }

                var localFilePath = await TryFindLocalFilePathAsync(filePathOrUrl);

                if (!string.IsNullOrEmpty(localFilePath))
                {
                    return await TryGetLocalFileStreamAsync(localFilePath, mimeType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading file: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Tries to get a data URI from a file path or URL.
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <returns></returns>
        internal static async Task<string?> TryGetDataUri(string filePathOrUrl)
        {
            if (filePathOrUrl.StartsWith("data:"))
            {
                return filePathOrUrl;
            }

            var result = await Utils.TryGetFileStreamAsync(filePathOrUrl);

            if (result != null)
            {
                return Utils.GetDataUriFromStream(result.Stream, result.MimeType);
            }

            return null;
        }

        /// <summary>
        /// Tries to get a local file stream from a file path or URL.
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        internal static async Task<MapFileStream?> TryGetLocalFileStreamAsync(string filePathOrUrl, string mimeType)
        {
            if (!string.IsNullOrEmpty(filePathOrUrl))
            {
#if MAUI
                //Try accessing via App Package.
                if (await FileSystem.Current.AppPackageFileExistsAsync(filePathOrUrl))
                {
                    using (var s = await FileSystem.Current.OpenAppPackageFileAsync(filePathOrUrl))
                    {
                        return await MapFileStream.FromStream(s, mimeType);
                    }
                }
#elif WINUI

#elif WPF
                //Try accessing as a packed resource.
                if(filePathOrUrl.StartsWith("pack://application:"))
                {
                    //Try accessing via WPF pack URI.
                    var sri = Application.GetResourceStream(new Uri(filePathOrUrl, Path.IsPathRooted(filePathOrUrl) ? UriKind.Absolute: UriKind.Relative));

                    if (sri != null)
                    {
                        using (var s = sri.Stream)
                        {
                            return await MapFileStream.FromStream(s, mimeType);
                        }
                    }
                }   
                
#endif

                //Try normal file system access.
                if (File.Exists(filePathOrUrl))
                {
                    //Read the file as a stream.
                    using (var s = File.OpenRead(filePathOrUrl))
                    {
                        return await MapFileStream.FromStream(s, mimeType);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Searches for a local file path.
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <returns></returns>
        internal static async Task<string?> TryFindLocalFilePathAsync(string filePathOrUrl)
        {
            if (filePathOrUrl.StartsWith("/"))
            {
                filePathOrUrl = filePathOrUrl.Substring(1);
            }

            if (filePathOrUrl.StartsWith("data:"))
            {
                return filePathOrUrl;
            }

            filePathOrUrl = filePathOrUrl.Replace("/", "\\");

            var fileName = Path.GetFileName(filePathOrUrl);

#if MAUI
            var filePaths = new List<string>() {
                filePathOrUrl,
                Constants.MapWebViewHybridAssetRoot + "/" + filePathOrUrl,
                Path.Combine(FileSystem.AppDataDirectory, fileName),
                Path.Combine(FileSystem.CacheDirectory, fileName)
            };
#else
            string mapResourceFilePath = Constants.MapWebViewHybridAssetRoot + "\\" + filePathOrUrl;
            var filePaths = new List<string>() {
                Path.IsPathRooted(filePathOrUrl)? filePathOrUrl: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePathOrUrl),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mapResourceFilePath)
            };
#endif

#if WINUI
            filePaths.Add(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", filePathOrUrl));
            filePaths.Add(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", mapResourceFilePath));
#endif

            //Loop through file names and try to open as local file paths.
            foreach (var path in filePaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
#if MAUI
                    //Try accessing via App Package.
                    if (await FileSystem.Current.AppPackageFileExistsAsync(path))
                    {
                        return path;
                    }
#endif

                    //Try normal file system access.
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Make the first character of a string lowercase. Most JS property names are camelCase while .NET property names are PascalCase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            {
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];
            }

            return str;
        }
    }
}
