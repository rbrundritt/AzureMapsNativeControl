using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AzureMapsWPFSamples.Samples
{
    /// <summary>
    /// Interaction logic for GeolocationSample.xaml
    /// </summary>
    public partial class GeolocationSample : Page
    {
        /*
         * Since this is a .NET Core application, we don't have access to System.Device.Location, however there is a port for it available here: https://github.com/dotMorten/System.Device
         */
        public GeolocationSample()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                //Geolocation.RequestAccessAsync().ContinueWith(t =>
                //{
                //    if (t.Result == Windows.Devices.Geolocation.GeolocationAccessStatus.Allowed)
                //    {
                //        Dispatcher.Invoke(() =>
                //        {
                //            GetLocationButton.IsEnabled = true;
                //            ToggleLocationTrackingButton.IsEnabled = true;
                //        });
                //    }
                //    else
                //    {
                //        Dispatcher.Invoke(() =>
                //        {
                //            GetLocationButton.IsEnabled = false;
                //            ToggleLocationTrackingButton.IsEnabled = false;
                //        });
                //    }
                //});
            };
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {

        }

        private void GetLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleLocationTrackingButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
