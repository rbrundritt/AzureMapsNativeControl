using CommunityToolkit.Maui.Storage;

namespace AzureMapsMauiSamples.Samples;

public partial class ScreenshotSample : ContentPage
{
    /*
     * This sample demonstrates how to take a screenshot of the map and save it to the device.
     * Permissions need to be set to access file storage.
     */

    public ScreenshotSample()
	{
		InitializeComponent();
	}

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        MapScreenshotBtn.IsEnabled = true;
    }

    private async void MapScreenshotButton_Click(object sender, EventArgs e)
    {
        var screenshotStream = await MyMap.CaptureScreenshotAsync();
        if (screenshotStream != null)
        {
            var result = await FileSaver.Default.SaveAsync("map_screenshot.png", screenshotStream);

            if(result.IsSuccessful)
            {
                await DisplayAlert("Success", "Screenshot saved successfully!", "OK");

                //Open the image using the default image viewer of the platform.
                var fileUri = new Uri(result.FilePath);
                //await Launcher.OpenAsync(fileUri);
            }
            else
            {
                await DisplayAlert("Failed", "Unable to save screenshot!", "OK");
            }

            screenshotStream.Dispose();
        } 
        else
        {
            await DisplayAlert("Failed", "Unable to generate screenshot!", "OK");
        }
    }
}