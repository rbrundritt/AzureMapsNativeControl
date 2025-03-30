using HybridWebView;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Internal
{
    internal class EmbeddedResourceProvider
    {
        #region Private Properties

        /// <summary>
        /// This assembly.
        /// </summary>
        private static readonly Assembly ThisAssembly = typeof(EmbeddedResourceProvider).Assembly;

        #endregion 

        #region Public Methods

        /// <summary>
        /// Reads an embedded resource.
        /// </summary>
        /// <param name="resourceName">File name if the resource</param>
        /// <returns></returns>
        public static Stream? ReadResource(string resourceName)
        {            
            return ThisAssembly.GetManifestResourceStream("AzureMapsNativeControl.EmbeddedResources." + resourceName.Replace("/", ".").Replace("\\", "."));
        }

        /// <summary>
        /// Loads an embedded resource into a HybridWebViewProxyEventArgs.
        /// </summary>
        /// <param name="resourceName">File name if the resource</param>
        /// <param name="args">HybridWebViewProxyEventArgs</param>
        public static async Task LoadResource(string resourceName, HybridWebViewProxyEventArgs args)
        {
            using (var fs = ReadResource(resourceName))
            {
                if (fs != null)
                {
                    var ms = new MemoryStream();
                    await fs.CopyToAsync(ms);
                    ms.Position = 0;

                    args.ResponseStream = ms;
                    args.ResponseContentType = Utils.GetMimeType(resourceName);
                }
            }
        }

        #endregion
    }
}
