using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using System.Windows.Controls;
using System.Windows;

namespace AzureMapsWPFSamples.Samples
{
    /// <summary>
    /// Interaction logic for HtmlMarkerSample.xaml
    /// </summary>
    public partial class HtmlMarkerSample : Page
    {
        /*********************************************************************************************************
       * This sample shows how to use the HTML markers.
       * 
       * This sample is based on these Azure Maps Web SDK samples: 
       * 
       * https://samples.azuremaps.com/?sample=simple-html-marker
       * https://samples.azuremaps.com/?sample=css-styled-html-marker
       * https://samples.azuremaps.com/?sample=draggable-html-marker
       * https://samples.azuremaps.com/?sample=html-marker-with-custom-svg-template
       * https://samples.azuremaps.com/?sample=html-marker-with-built-in-icon-template
       * https://samples.azuremaps.com/?sample=all-built-in-icon-templates-as-html-markers
       *********************************************************************************************************/

        private HtmlMarker? currentMarker = null;
        private Random random = new Random();

        public HtmlMarkerSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Load the first scenario.
            HtmlMarkerScenarioPicker.SelectedIndex = 0;
        }

        private async void HtmlMarkerScenarioPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyMap == null)
            {
                return;
            }

            MyMap.Markers.Clear();
            currentMarker = null;
            MarkerEventLabel.Text = "Drag the marker";
            MarkerEventLabel.Visibility = Visibility.Collapsed;

            UpdateMarkerOptionsButton.Visibility = Visibility.Collapsed;

            string scenarioName = Helpers.GetSelectedPickerString(sender);

            switch (scenarioName)
            {
                case "Default Marker":
                    //Create a HTML marker and add it to the map.
                    currentMarker = new HtmlMarker(new HtmlMarkerOptions
                    {
                        Position = new Position(0, 0)
                    });

                    MyMap.Markers.Add(currentMarker);

                    UpdateMarkerOptionsButton.Visibility = Visibility.Visible;
                    break;
                case "Draggable Image Marker":
                    //Based on: https://samples.azuremaps.com/?sample=draggable-html-marker

                    var marker = new HtmlMarker(new HtmlMarkerOptions
                    {
                        //Load an image for the HTML content.
                        HtmlContent = "<img src=\"/images/pin.png\" alt=\"pushpin\" style=\"pointer-events: none;\" />",
                        Position = new Position(0, 0),
                        Anchor = PositionAnchor.Bottom,
                        Draggable = true
                    });

                    //Add the marker to the map.
                    MyMap.Markers.Add(marker);

                    //Add a drag event to get the position of the marker. Markers support drag, dragstart and dragend events.
                    MyMap.Events.Add("drag", marker, (s, e) =>
                    {
                        //When the drag event is attached to the marker. 
                        MarkerEventLabel.Text = $"Marker drag dragged to: {marker.GetOptions().Position}";
                    });

                    MarkerEventLabel.Visibility = Visibility.Visible;
                    break;
                case "CSS Styled Marker":
                    //Based on: https://samples.azuremaps.com/?sample=css-styled-html-marker

                    //For this scenario we will load a CSS file from the Raw/map_resources/styles folder and use it to style the marker. 
                    //This method can also be used for loading JavaScript files.
                    //NOTE: Raw CSS strings can also be added to the Map view by using the MapJsInterlop.AddRawCss methods.
                    await MyMap.JsInterlop.AddWebResourceAsync("styles/BounceAndPulsatePin.css", WebResourceType.Style);

                    //Create a HTML marker and add it to the map.
                    MyMap.Markers.Add(new HtmlMarker(new HtmlMarkerOptions
                    {
                        HtmlContent = "<div><div class='pin bounce'></div><div class='pulse'></div></div>",
                        Position = new Position(0, 0),
                        PixelOffset = new Pixel(5, -18)
                    }));
                    break;
                case "Built-in SVG Template":
                    //Based on: https://samples.azuremaps.com/?sample=html-marker-with-built-in-icon-template

                    //Get the image template from the maps image sprite.
                    var imageTemplate = await MyMap.ImageSprite.GetImageTemplateAsync("marker-arrow");

                    //Create a HTML marker and add it to the map.
                    currentMarker = new HtmlMarker(new HtmlMarkerOptions
                    {
                        HtmlContent = imageTemplate,
                        Color = "red",
                        SecondaryColor = "white",
                        Text = "00",
                        Position = new Position(0, 0)
                    });
                    MyMap.Markers.Add(currentMarker);

                    UpdateMarkerOptionsButton.Visibility = Visibility.Visible;
                    break;
                case "Custom SVG Template":
                    //Based on: https://samples.azuremaps.com/?sample=html-marker-with-custom-svg-template

                    //Create a HTML marker using a custom SVG template and add it to the map.
                    currentMarker = new HtmlMarker(new HtmlMarkerOptions
                    {
                        HtmlContent = "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"30\" height=\"37\" viewBox=\"0 0 30 37\" xml:space=\"preserve\"><rect x=\"0\" y=\"0\" rx=\"8\" ry=\"8\" width=\"30\" height=\"30\" fill=\"{color}\"/><polygon fill=\"{color}\" points=\"10,29 20,29 15,37 10,29\"/><text x=\"15\" y=\"20\" style=\"font-size:16px;font-family:arial;fill:#ffffff;\" text-anchor=\"middle\">{text}</text></svg>",
                        Color = "purple",
                        Text = "2",
                        Position = new Position(0, 0)
                    });
                    MyMap.Markers.Add(currentMarker);

                    UpdateMarkerOptionsButton.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void UpdateMarkerOptionsButton_Clicked(object sender, EventArgs e)
        {
            if (currentMarker != null)
            {
                //Update the marker options with a random color and text value.
                currentMarker.SetOptions(new HtmlMarkerOptions
                {
                    Color = $"rgb({random.NextInt64(0, 255)}, {random.NextInt64(0, 255)}, {random.NextInt64(0, 255)})",
                    SecondaryColor = $"rgb({random.NextInt64(0, 255)}, {random.NextInt64(0, 255)}, {random.NextInt64(0, 255)})",
                    Text = $"{random.NextInt64(0, 100)}"
                });
            }
        }
    }
}