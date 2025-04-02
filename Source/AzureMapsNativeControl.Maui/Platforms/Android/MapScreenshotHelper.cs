using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;

namespace AzureMapsNativeControl.Platforms
{
    internal static class MapScreenshotHelper
    {
        public static async Task<Stream?> CaptureAsync(HybridWebView.HybridWebView webView)
        {
            //Since the map uses WebGL extra effort required to capture a screenshot of the map. Need to grab the rendered pixels of the whole window then crop the map out of it.
            if (webView.Handler?.PlatformView is Android.Webkit.WebView androidWebView)
            {
                var activity = androidWebView.Context as Activity;
                var window = activity?.Window; // Safely get the window

                if (window != null)
                {
                    var (windowWidth, windowHeight) = GetWindowSize(activity);

                    //Take screenshot of the whole window.
                    var taskCompletionSource = new TaskCompletionSource<bool>();
                    Bitmap bitmap = Bitmap.CreateBitmap(windowWidth, windowHeight, Bitmap.Config.Argb8888);
                    PixelCopy.Request(window, bitmap, new PixelCopyFinishedListener((b) =>
                    {
                        if(b!= null)
                        {
                            taskCompletionSource.SetResult(true);
                        }
                        else
                        {
                            taskCompletionSource.SetResult(false);
                        }
                    }, bitmap), new Handler(Looper.MainLooper));

                    if (await taskCompletionSource.Task)
                    {
                        //Now crop the screenshot to the WebView bounds.

                        // Get WebView location on screen
                        int[] location = new int[2];
                        androidWebView.GetLocationOnScreen(location);
                        int x = location[0]; // X position
                        int y = location[1]; // Y position

                        // Get WebView dimensions
                        int width = androidWebView.Width;
                        int height = androidWebView.Height;

                        // Ensure valid crop dimensions
                        if (width <= 0 || height <= 0 || x + width > windowWidth || y + height > windowHeight)
                            return null;

                        // Crop the WebView portion from the full screenshot
                        bitmap = Bitmap.CreateBitmap(bitmap, x, y, width, height);

                        var ms = new MemoryStream();
                        bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                        ms.Position = 0;
                        return ms;
                    }
                }
            }

            return null;
        }

        public static (int width, int height) GetWindowSize(Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R) // Android 11+
            {
                var windowMetrics = activity.WindowManager.CurrentWindowMetrics;
                var insets = windowMetrics.WindowInsets.GetInsetsIgnoringVisibility(WindowInsets.Type.SystemBars());
                var bounds = windowMetrics.Bounds;

                int width = bounds.Width() - insets.Left - insets.Right;
                int height = bounds.Height() - insets.Top - insets.Bottom;

                return (width, height);
            }
            else // Older Android versions
            {
                var displayMetrics = new Android.Util.DisplayMetrics();
                activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                return (displayMetrics.WidthPixels, displayMetrics.HeightPixels);
            }
        }

        // Proper PixelCopy Finished Listener Implementation
        public class PixelCopyFinishedListener : Java.Lang.Object, PixelCopy.IOnPixelCopyFinishedListener
        {
            private readonly Action<Bitmap?> callback;
            private readonly Bitmap bitmap; // Store bitmap reference

            public PixelCopyFinishedListener(Action<Bitmap?> callback, Bitmap bitmap)
            {
                this.callback = callback;
                this.bitmap = bitmap; // Assign bitmap
            }

            public void OnPixelCopyFinished(int result)
            {
                if (result == 0)
                {
                    callback?.Invoke(bitmap);
                } 
                else
                {
                    callback?.Invoke(null);
                }
            }
        }

        //public static Task<Bitmap> CaptureViewToBitmap(Android.Webkit.WebView view)
        //{
        //    var taskCompletionSource = new TaskCompletionSource<Bitmap>();

        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //    {
        //        var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
        //        var locationOfViewInWindow = new int[2];
        //        view.GetLocationInWindow(locationOfViewInWindow);

        //        Android.Graphics.Rect rect = new Android.Graphics.Rect(locationOfViewInWindow[0], locationOfViewInWindow[1], locationOfViewInWindow[0] + view.Width, locationOfViewInWindow[1] + view.Height);
        //        // Corrected part: Get the Window from the context
        //        var window = (view.Context as Activity)?.Window; // Safely get the window

        //        if (window != null)
        //        {
        //                PixelCopy.Request(
        //                    view,//window.DecorView.GetRootView(),
        //                    rect,
        //                    bitmap,
        //                    (int copyResult) =>
        //                    {
        //                        if (copyResult == PixelCopyResult.Success)
        //                        {
        //                            taskCompletionSource.SetResult(bitmap);
        //                        }
        //                        else
        //                        {
        //                            Log.Error("PixelCopy", $"PixelCopy failed: {copyResult}");
        //                            taskCompletionSource.SetResult(null);
        //                        }
        //                    },
        //                    new Handler(Looper.MainLooper)
        //                );
        //        }
        //    }
        //    else
        //    {
        //        // For older Android versions, you might need to revert to older, less reliable methods.
        //        // Or inform the user that their version is not supported.
        //        taskCompletionSource.SetResult(null);
        //    }

        //    return taskCompletionSource.Task;
        //}
    }
}
