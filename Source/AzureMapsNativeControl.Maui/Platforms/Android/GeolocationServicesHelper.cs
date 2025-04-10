using Android.Webkit;
using AndroidX.Activity;

namespace AzureMapsNativeControl.Platforms
{
    internal class GeolocationServicesHelper
    {
        public static void EnableWebViewGeolocation(HybridWebView.HybridWebView webView)
        {
            if (webView.Handler?.PlatformView is Android.Webkit.WebView androidWebView)
            {
                var activity = androidWebView.Context as ComponentActivity;

                // Enable geolocation
                var webSettings = androidWebView.Settings;

                if (!webSettings.JavaScriptEnabled)
                {
                    webSettings.JavaScriptEnabled = true;
                }

                webSettings.SetGeolocationEnabled(true);
               // webSettings.SetGeolocationDatabasePath(androidWebView.Context?.FilesDir?.Path);

                androidWebView.SetWebChromeClient(new GeolocationWebChromeClient());
            }
        }
    }

    internal class GeolocationWebChromeClient : WebChromeClient
    {
        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
                return status;
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                return status;
            }
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status;
        }

        public override async void OnGeolocationPermissionsShowPrompt(string? origin, GeolocationPermissions.ICallback? callback)
        {
            PermissionStatus permissionStatus = await CheckAndRequestLocationPermission();
            base.OnGeolocationPermissionsShowPrompt(origin, callback);
            callback.Invoke(origin, true, false);
        }
    }
}
