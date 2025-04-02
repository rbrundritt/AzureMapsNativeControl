using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace AzureMapsNativeControl.Platforms
{
    internal static class MapScreenshotHelper
    {
        public static async Task<Stream?> CaptureAsync(HybridWebView.HybridWebView webView)
        {
            if (webView.Handler!.PlatformView is WebView2)
            {
                var ms = new MemoryStream();
                await (webView.Handler.PlatformView as WebView2).CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, ms.AsRandomAccessStream());
                ms.Position = 0;
                return ms;
            }

            return null;
        }
    }
}
