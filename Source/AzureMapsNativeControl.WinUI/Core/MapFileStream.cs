using System;
using System.Threading.Tasks;
using AzureMapsNativeControl.Internal;

#if MAUI
#else
using System.IO;
#endif

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Represents a stream of data that can be used as a response to a Map request.
    /// </summary>
    public class MapFileStream
    {
        /// <summary>
        /// Represents a stream of data that can be used as a response to a Map request.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mimeType"></param>
        public MapFileStream(MemoryStream stream, string? mimeType = null, TimeSpan? maxAge = null, DateTime? expires = null, string? name = null)
        {
            Stream = stream;
            Stream.Position = 0;
            MimeType = mimeType;
            MaxAge = maxAge;
            Expires = expires;
            Name = name;
        }

        internal MapFileStream(RawMapDroppedFileInfo file)
        {
            if(file == null)
                throw new ArgumentNullException(nameof(file));

            if(file.Stream != null)
            {
                Stream = file.Stream;
            }
            else if (file.Data != null)
            {
                var data = file.Data.Substring(file.Data.IndexOf(",") + 1);
                var bytes = Convert.FromBase64String(data);
                Stream = new MemoryStream(bytes);
                Stream.Position = 0;
            } 
            else
            {
                Stream = new MemoryStream();
            }

            MimeType = file.MimeType ?? "text/plain";
            Name = file.Name;
        }

        /// <summary>
        /// Creates a new instance of the MapFileStream class from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static async Task<MapFileStream> FromStream(Stream stream, string? mimeType = null, TimeSpan? maxAge = null, DateTime? expires = null)
        {
            var s = new MemoryStream();
            await stream.CopyToAsync(s);
            stream.Position = 0;
            return new MapFileStream(s, mimeType, maxAge, expires);
        }

        /// <summary>
        /// The stream of the response.
        /// </summary>
        public MemoryStream Stream { get; set; }

        /// <summary>
        /// The MIME type of the response.
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// The file name or path.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The maximum age of the response.
        /// </summary>
        public TimeSpan? MaxAge { get; set; }

        /// <summary>
        /// The expiration date of the response.
        /// </summary>
        public DateTime? Expires { get; set; }
    }
}
