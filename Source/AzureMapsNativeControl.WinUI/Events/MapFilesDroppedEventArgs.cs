using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event object returned by the maps when one or more files are dropped onto it.
    /// </summary>
    public class MapFilesDroppedEventArgs: MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the maps when one or more files are dropped onto it.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapFilesDroppedEventArgs(Map map, string eventName) : base(map, eventName)
        {
            Files = [];
        }

        /// <summary>
        /// Event object returned by the maps when one or more files are dropped onto it.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapFilesDroppedEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            if(eventData.Files == null)
            {
                Files = [];
                return;
            }

            // Deserialize the files array
            Files = eventData.Files.Select(file => new MapFileStream(file)).ToList();
        }

        internal MapFilesDroppedEventArgs(Map map, string eventName, Stream stream, string? fileName = null, string? fileExtension = null) : base(map, eventName)
        {
            string? mimeType = null;

            if(string.IsNullOrWhiteSpace(fileExtension))
            {
                if (Utils.TryGetMimeType(fileExtension, out string mt))
                {
                    mimeType = mt;
                }
            }

            if (mimeType == null && string.IsNullOrWhiteSpace(fileName))
            {
                if (Utils.TryGetMimeType(fileName, out string mt))
                {
                    mimeType = mt;
                }
            }

            if (mimeType == null)
            {
                mimeType = "plain/text";
            }

            var ms = new MemoryStream();

            //Copy the stream to the memory stream.
            stream.CopyTo(ms);
            ms.Position = 0;

            // Deserialize the files array
            Files = new List<MapFileStream>
            {
                new MapFileStream(ms, mimeType, null, null, fileName)
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// The array of dropped file information.
        /// </summary>
        [JsonPropertyName("files")]
        public IList<MapFileStream> Files { get; set; }

        #endregion
    }
}
