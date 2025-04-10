using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureMapsWinUISamples.Samples
{
    public partial class GeolocationControlSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the Geolocation control to get the users current location (e.g. GPS).
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=geolocation-control-options
        * https://samples.azuremaps.com/?sample=geolocation-control
        * 
        * TIP: All IMapView classes (Map, SwipeMap) have a method for getting the users current location without 
        * the need for the geolocation control call `GetCurrentGeolocationPosition()`.
        *********************************************************************************************************/

        public GeolocationControlSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Attach some events to the geolocation control.
            MyMap.Events.Add("geolocationsuccess", MyGeolocationControl, OnGeolocationSuccess);
            MyMap.Events.Add("geolocationerror", MyGeolocationControl, OnGeolocationError);
            MyMap.Events.Add("compassheadingchanged", MyGeolocationControl, OnCompassHeadingChanged);
        }

        private void OnGeolocationSuccess(object? sender, MapEventArgs e)
        {
            if (e is GeolocationControlEventArgs args && args.Feature != null)
            {

                //Get the geolocation position.
                var point = (args.Feature.Geometry as PointGeometry);

                //Display the position in the label.
                if (point != null)
                {
                    GeolocationLabel.Text = $"Latitude: {point.Coordinates[1]}, Longitude: {point.Coordinates[0]}";
                }
            }
        }

        private void OnGeolocationError(object? sender, MapEventArgs e)
        {
            if (e is GeolocationControlEventArgs args && args.Error != null)
            {
                GeolocationLabel.Text = $"Error: {args.Error.Message}";
            }
        }

        private void OnCompassHeadingChanged(object? sender, MapEventArgs e)
        {
            if (e is GeolocationControlEventArgs args && args.CompassHeading != null)
            {
                CompassHeadingLabel.Text = $"Compass Heading: {args.CompassHeading}";
            }
        }

        private void StylePicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyGeolocationControl != null)
            {
                var styleString = Helpers.GetSelectedPickerString(sender);

                //Update the style of the legend.
                switch (styleString)
                {
                    case "Light":
                        MyGeolocationControl.Style = ControlStyle.Light;
                        break;
                    case "Dark":
                        MyGeolocationControl.Style = ControlStyle.Dark;
                        break;
                    case "Auto":
                    default:
                        MyGeolocationControl.Style = ControlStyle.Auto;
                        break;
                }
            }
        }

        private void MarkerColorPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyGeolocationControl != null)
            {
                string markerColor = Helpers.GetSelectedPickerString(sender);

                MyGeolocationControl.MarkerColor = markerColor;
            }
        }

        private void ShowUserLocationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.ShowUserLocation = cbx.IsChecked == true;
            }
        }

        private void TrackUserLocationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.TrackUserLocation = cbx.IsChecked == true;

                if (MyGeolocationControl.TrackUserLocation)
                {
                    //Updating the max age allowed of the geolocation position to increase tracking frequency.
                    MyGeolocationControl.PositionOptions = new GeolocationPositionOptions
                    {
                        MaximumAge = 5000
                    };
                }
                else
                {
                    //Revert to default settings.
                    MyGeolocationControl.PositionOptions = new GeolocationPositionOptions();
                }
            }
        }

        private void CalculateMissingValuesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.CalculateMissingValues = cbx.IsChecked == true;
            }
        }

        private void UpdateMapCameraCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.UpdateMapCamera = cbx.IsChecked == true;
            }
        }

        private void EnableCompassCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.EnableCompass = cbx.IsChecked == true;
            }
        }

        private void SyncMapCompassHeadingCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (MyGeolocationControl != null && sender is CheckBox cbx)
            {
                MyGeolocationControl.SyncMapCompassHeading = cbx.IsChecked == true;
            }
        }
    }
}
