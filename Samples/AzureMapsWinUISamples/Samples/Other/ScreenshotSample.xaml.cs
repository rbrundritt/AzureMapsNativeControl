using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using WinRT.Interop;


namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class ScreenshotSample : Page
    {
        public ScreenshotSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            MapScreenshotBtn.IsEnabled = true;
        }

        private async void MapScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            var screenshotStream = await MyMap.CaptureScreenshotAsync();
            if (screenshotStream != null)
            {
                var savePicker = new FileSavePicker()
                {
                    CommitButtonText = "Save"
                };

                savePicker.FileTypeChoices.Add("PNG File", new string[] { ".png" });

                var windowHandle = WindowNative.GetWindowHandle(App.Window);
                InitializeWithWindow.Initialize(savePicker, windowHandle);

                var pickedFile = await savePicker.PickSaveFileAsync();

                //Check if the user selected a file.
                if (pickedFile != null)
                {
                    using (var os = await pickedFile.OpenStreamForWriteAsync())
                    {
                        await screenshotStream.CopyToAsync(os);
                    }

                    await new ContentDialog
                    {
                        Title = "Success",
                        Content = "Screenshot saved successfully!",
                        CloseButtonText = "OK",
                        XamlRoot = App.Window.Content.XamlRoot // Associate dialog with window
                    }.ShowAsync();
                                        
                    //Open the image using the default image viewer of the platform.
                    var fileUri = new Uri(pickedFile.Path);
                    await Launcher.LaunchFileAsync(pickedFile);
                }
                else
                {
                    await new ContentDialog
                    {
                        Title = "Failed",
                        Content = "Unable to save screenshot!",
                        CloseButtonText = "OK",
                        XamlRoot = App.Window.Content.XamlRoot // Associate dialog with window
                    }.ShowAsync();
                }

                screenshotStream.Dispose();
            }
            else
            {
                await new ContentDialog
                {
                    Title = "Failed",
                    Content = "Unable to generate screenshot!",
                    CloseButtonText = "OK",
                    XamlRoot = App.Window.Content.XamlRoot // Associate dialog with window
                }.ShowAsync();
            }
        }
    }
}
