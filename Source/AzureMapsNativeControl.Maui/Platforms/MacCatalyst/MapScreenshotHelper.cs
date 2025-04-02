using Foundation;
using WebKit;

namespace AzureMapsNativeControl.Platforms
{
    internal static class MapScreenshotHelper
    {
        public static async Task<Stream?> CaptureAsync(HybridWebView.HybridWebView webView)
        {
            if (webView.Handler!.PlatformView is WKWebView)
            {
                var image = await (webView.Handler.PlatformView as WKWebView).TakeSnapshotAsync(new WKSnapshotConfiguration()
                {
                    Rect = webView.Bounds
                });

                NSData imageData = image.AsPNG();
                return imageData.AsStream();
            }

            return null;
        }
    }
}
