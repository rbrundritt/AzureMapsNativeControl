using System;
using AzureMapsNativeControl.Core;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

#if !MAUI
using System.IO;
using System.Net.Http;
#endif

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// An HTTP Client Manager for requests related to the Azure Maps Control.
    /// </summary>
    internal class MapHttpClientManager
    {
        //https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/

        #region Private Properties

        private static ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>();

        internal static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36 Edg/127.0.0.0";

        #endregion

        #region Public Methods

        public static async Task<MapFileStream?> GetStreamAsync(Uri url)
        {
            var client = GetClient(url);

            var response = await client.GetAsync(url);//, HttpCompletionOption.ResponseHeadersRead);

            try
            {
                if (response.IsSuccessStatusCode && response.Content is object)
                {
                    using(var s= response.Content.ReadAsStreamAsync())
                    {
                        var stream = new MemoryStream();
                        await s.Result.CopyToAsync(stream);
                        stream.Position = 0;

                        string? mimeType = response.Content.Headers.ContentType?.MediaType;

                        if (string.IsNullOrWhiteSpace(mimeType))
                        {
                            mimeType = Utils.GetMimeType(url.AbsolutePath);
                        }

                        TimeSpan? maxAge = response.Headers.CacheControl?.MaxAge;
                        DateTime? expires = null;
                        
                        if (maxAge != null)
                        {
                            expires = DateTime.Now + maxAge;
                        }

                        return new MapFileStream(stream, mimeType, maxAge, expires);
                    }
                }
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error making request with MapHttpClientManager", ex);
            }
            finally
            {
                response.Dispose();
            }

            return null;
        }

        /// <summary>
        /// Tries to get a stream from the given URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<MapFileStream?> TryGetStreamAsync(string url)
        {
            if (IsValidUri(url))
            {
                return await Task.Run<MapFileStream?>(async () =>{
                    return await TryGetStreamAsync(new Uri(url));
                });
            }

            return null;
        }

        /// <summary>
        /// Tries to get a stream from the given URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<MapFileStream?> TryGetStreamAsync(Uri uri)
        {
            if (IsValidUri(uri))
            {
                return await GetStreamAsync(uri);
            }

            return null;
        }

        /// <summary>
        /// Checks if the given URL is a valid URI.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsValidUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && 
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Checks if the given URI is a valid URI.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsValidUri(Uri uri)
        {
            return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an HttpClient for the base address of the given URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static HttpClient GetClient(Uri url)
        {
            var baseAddress = GetBaseAddress(url);

            if (_clients.ContainsKey(baseAddress))
            {
                return _clients[baseAddress];
            }

            var client = new HttpClient(new SocketsHttpHandler()
            {
                MaxConnectionsPerServer = 16,
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            })
            {
                DefaultRequestVersion = HttpVersion.Version20,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
                Timeout = TimeSpan.FromMinutes(5)
            };

            //Ensure resources can be accessed on non-CORS enabled servers.
            client.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            if(_clients.TryAdd(baseAddress, client))
            {
                return client;
            }

            //If we get here, a similar client was added while a second one was being made.
            return _clients[baseAddress];
        }

        /// <summary>
        /// Gets the base address of the given URL.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetBaseAddress(Uri uri)
        {
            return $"{uri.Scheme}://{uri.Host}";
        }

        #endregion
    }
}
