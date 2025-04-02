using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    /// <summary>
    /// Interaction logic for ScreenshotSample.xaml
    /// </summary>
    public partial class ScreenshotSample : Page
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
                var sfd = new SaveFileDialog()
                {
                    DefaultExt = ".png",
                    FileName = "map_screenshot",
                    Filter = "PNG File (*.png)|*.png|JPEG File (*.jpg)|*.jpg"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (var s = sfd.OpenFile())
                    {
                        screenshotStream.CopyTo(s);
                    }
                    
                    MessageBox.Show("Screenshot saved successfully!", "Success");

                    //Open the image using the default image viewer of the platform.
                    Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("Unable to save screenshot!", "Failed");
                }

                screenshotStream.Dispose();
            }
            else
            {
                MessageBox.Show("Unable to generate screenshot!", "Failed");
            }
        }
    }
}
