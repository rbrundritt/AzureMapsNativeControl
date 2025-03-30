using System.IO;
using System.Windows;
using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;

namespace AzureMapsWPFSamples.Samples
{
    public partial class ImageLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the image layer.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=image-layer-options
        * https://samples.azuremaps.com/?sample=simple-image-layer
        * https://samples.azuremaps.com/?sample=kml-ground-overlay-as-image-layer
        *********************************************************************************************************/

        #region Private Properties

        private ImageLayer imageLayer;

        /// <summary>
        /// A dictionary of image layer options for different images in this sample.
        /// The ImageLayerOptionsInfo class is a helper class in this sample app that contains an image layer option and center/zoom to set the map to.
        /// </summary>
        private Dictionary<string, ImageLayerOptionsInfo> _imageLayerOptionsInfo = new Dictionary<string, ImageLayerOptionsInfo>();

        #endregion

        #region Constructor

        public ImageLayerSample()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for when the map is ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Preload the images for this sample.
            await PreloadImageOptions();

            //For this sample, set the initial image source from the picker.
            ImageSourcePicker.SelectedIndex = 0;
        }

        /// <summary>
        /// Preloads the image layer options for the sample.
        /// </summary>
        /// <returns></returns>
        private async Task PreloadImageOptions()
        {
            //For this sample, clearing just in case the page has been reloaded.
            _imageLayerOptionsInfo.Clear();

            //The image layer supports the following image formats: JPEG, PNG, BMP, GIF(no animations)

            /****************************************************************
            * Load image_asset_url image.
            * 
            * This loads an image using a URL that points to the Raw/map_resources 
            * folder which the uses map as a local host for resources.
            *****************************************************************/

            _imageLayerOptionsInfo.Add("image_asset_url", new ImageLayerOptionsInfo(
                new ImageLayerOptions(
                    "images/image-overlays/Foto_aérea_de_Unaí_detalhando_o_córrego_Canabrava_3.jpg",
                    [
                        new Position(-46.896948, -16.367295),   //Top Left Corner
                        new Position(-46.895306, -16.364333),   //Top Right Corner
                        new Position(-46.893289, -16.365362),   //Bottom Right Corner
                        new Position(-46.89493, -16.368324)     //Bottom Left Corner
                    ]
                ),
                new Position(-46.89511, -16.36632), 16));

            /****************************************************************
            * Load image_from_stream image.
            * 
            * This loads an image from a stream. You can create an image stream 
            * in a lot of ways (download an image, create an image using drawing tools...).
            * 
            * This sample simply loads an existing image from the file system.
            *****************************************************************/

            //Load an image from a stream. This may be useful if you use a drawing library like SkiaSharp to generate images.            
            using (var imageStream = new FileInfo("map_resources/images/image-overlays/Chicago_1872_Map.jpg").OpenRead())
            {
                _imageLayerOptionsInfo.Add("image_from_stream", new ImageLayerOptionsInfo(
                    new ImageLayerOptions(
                        //Simply pass the stream into the image layer options.
                        imageStream,
                        [
                            new Position(-87.732, 41.938),  //Top Left Corner
                            new Position(-87.592, 41.9381), //Top Right Corner
                            new Position(-87.589, 41.811),  //Bottom Right Corner
                            new Position(-87.7298, 41.8105) //Bottom Left Corner
                        ]
                    ), new Position(-87.65, 41.87), 11));
            }

            /****************************************************************
            * Load kml_ground_overlay image.
            * 
            * KML ground overlays provide north, south, east, and west coordinates 
            * and a counter-clockwise rotation,where as the image layer expects 
            * coordinates for each corner of the image. 
            * 
            * Based on: https://samples.azuremaps.com/?sample=kml-ground-overlay-as-image-layer
            *****************************************************************/

            //KML Ground Overlay information extracted from: https://commons.wikimedia.org/wiki/File:Chartres.svg/overlay.kml
            string kmlImageUrl = "/images/image-overlays/1600px-Chartres.svg.png";
            double north = 48.4482;
            double south = 48.447372;
            double east = 1.488833;
            double west = 1.486788;
            double kmlRotation = 46.44;

            //Calculate the corner coordinates of the ground overlay.
            //KML rotations are counter-clockwise, subtrack from 360 to make them clockwise.
            var cornerPositions = ImageLayer.GetCoordinatesFromEdges(north, south, east, west, 360 - kmlRotation);

            _imageLayerOptionsInfo.Add("kml_ground_overlay", new ImageLayerOptionsInfo(
                new ImageLayerOptions(kmlImageUrl, cornerPositions),
                new Position(1.487811, 48.44779), 17));

            /****************************************************************
            * Load image_data_uri image.
            * 
            * Data URI's can be loaded directly into the image layer as a URL.
            * 
            * This sample is a simple image of the Manhatten flood plains.
            *****************************************************************/

            _imageLayerOptionsInfo.Add("image_data_uri", new ImageLayerOptionsInfo(
                new ImageLayerOptions(
                    "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAbIAAAIACAYAAADnmen/AAABvnpUWHRSYXcgcHJvZmlsZSB0eXBlIGV4aWYAAHjabVNRdsQgCPz3FD1CBARzHLPR93qDHr8Dmt3stuQZcYARUFP/+R7py6XwlqRY1V11g8guOzUodQvJeU6bxD/koO1C3/CEQaG5A2PmaTjbnLGe+Aq45kzveHoaykfAIswNSHnh/eI73vHUrzIe/+9M9QOntY+X5vq5iMaVAE1Dlrke19a9suP0uJXkbaQqgR+LiPs06F7t3rzRptYGcATwItKFH3niZW2cmj6TnlJfQ+p7mX/WhnM+C/iYEnXOyL0zs8wS2Ydww1zjr3DMobutQTf47YghLryzAtGEEMVS8BHIqhY64V4QbHB1UoVzhr3CT0MrQO+e0SNPbA0YjCXCOXYznrJDr6Ag+Enkb1iDDJ47vuoZebow07o4nGkOJ5coKkpdiFvoiZYXmm6w9wKdijzMA9CH6eibobQbmUTOeFhyavbjS6pGOvQkQDgDbCJDRIqcNIpoMYI9C4YxuhLvcWtli6imRdkEb3Uk7TB2zXq6GzuhwTzUnMA29SeN24r798BaaGAeiHK6ol13E2MvDaEqKvZAJse6I3hg/z2XS9IvrK3fmqOiR/wAAAGEaUNDUElDQyBwcm9maWxlAAB4nH2RPUjDUBSFT1OlRSqCdhBxyFCd7KIijqUVi2ChtBVadTB56R80aUhSXBwF14KDP4tVBxdnXR1cBUHwB8TZwUnRRUq8Lym0iPHC432cd8/hvfsAoVVjqtkXA1TNMjLJuJgvrIqBVwQRgA/D8EnM1FPZxRw86+ueOqnuojzLu+/PGlSKJgN8InGM6YZFvEE8t2npnPeJw6wiKcTnxFMGXZD4keuyy2+cyw4LPDNs5DIJ4jCxWO5huYdZxVCJZ4kjiqpRvpB3WeG8xVmtNVjnnvyFoaK2kuU6rXEksYQU0hAho4EqarAQpV0jxUSGzuMe/jHHnyaXTK4qGDkWUIcKyfGD/8Hv2ZqlmWk3KRQH+l9s+2MCCOwC7aZtfx/bdvsE8D8DV1rXX28B85+kN7ta5AgY2gYurruavAdc7gCjT7pkSI7kpyWUSsD7GX1TARi5BQbW3Ll1znH6AORoVss3wMEhMFmm7HWPdwd75/ZvT2d+PxVlcoFbPjRhAAANdmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iWE1QIENvcmUgNC40LjAtRXhpdjIiPgogPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iCiAgICB4bWxuczpzdEV2dD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlRXZlbnQjIgogICAgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIgogICAgeG1sbnM6R0lNUD0iaHR0cDovL3d3dy5naW1wLm9yZy94bXAvIgogICAgeG1sbnM6dGlmZj0iaHR0cDovL25zLmFkb2JlLmNvbS90aWZmLzEuMC8iCiAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgIHhtcE1NOkRvY3VtZW50SUQ9ImdpbXA6ZG9jaWQ6Z2ltcDo2YmU3MDQ1Ny01Y2YzLTQ5N2EtYmQzOS01NjE5MDEzZWFlNTUiCiAgIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6ZWI2NWMwYjUtNDJiNi00ZWY0LWE3YTItZmVmOTcyMjE5Nzg5IgogICB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InhtcC5kaWQ6NGVmNTdkMGUtNmI0Yy00NGIzLWIzZTAtY2EyZWY5MDFjZDBlIgogICBkYzpGb3JtYXQ9ImltYWdlL3BuZyIKICAgR0lNUDpBUEk9IjIuMCIKICAgR0lNUDpQbGF0Zm9ybT0iV2luZG93cyIKICAgR0lNUDpUaW1lU3RhbXA9IjE3MjI5OTM2MTkzNjAxNzciCiAgIEdJTVA6VmVyc2lvbj0iMi4xMC4zNCIKICAgdGlmZjpPcmllbnRhdGlvbj0iMSIKICAgeG1wOkNyZWF0b3JUb29sPSJHSU1QIDIuMTAiCiAgIHhtcDpNZXRhZGF0YURhdGU9IjIwMjQ6MDg6MDZUMTg6MjA6MTctMDc6MDAiCiAgIHhtcDpNb2RpZnlEYXRlPSIyMDI0OjA4OjA2VDE4OjIwOjE3LTA3OjAwIj4KICAgPHhtcE1NOkhpc3Rvcnk+CiAgICA8cmRmOlNlcT4KICAgICA8cmRmOmxpCiAgICAgIHN0RXZ0OmFjdGlvbj0ic2F2ZWQiCiAgICAgIHN0RXZ0OmNoYW5nZWQ9Ii8iCiAgICAgIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6NzY2ZDIxYmYtNzliMi00ZjU3LWIyODYtMGIzNzNlOTEzOThhIgogICAgICBzdEV2dDpzb2Z0d2FyZUFnZW50PSJHaW1wIDIuMTAgKFdpbmRvd3MpIgogICAgICBzdEV2dDp3aGVuPSIyMDI0LTA4LTA2VDE4OjIwOjE5Ii8+CiAgICA8L3JkZjpTZXE+CiAgIDwveG1wTU06SGlzdG9yeT4KICA8L3JkZjpEZXNjcmlwdGlvbj4KIDwvcmRmOlJERj4KPC94OnhtcG1ldGE+CiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAKPD94cGFja2V0IGVuZD0idyI/PtshjOsAAAAGYktHRACkAKUAopuP32YAAAAJcEhZcwAACxMAAAsTAQCanBgAAAAHdElNRQfoCAcBFBMwtryLAAAgAElEQVR42uzddZgd1f3H8ffZGCHBXUshWIMV+KGF4lBcC4VCDtKW2QAFKlCKFisVrJCDlZ7F3WmBoinuXlyLFQ0JEZLs+f0xk7IsK/funbk78nk9zz6Q3Ssz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",
                    ImageLayer.GetCoordinatesFromEdges(40.7828, 40.6931, -73.9325, -74.0326)
                ),
                new Position(-73.98255, 40.73795), 12));

            /****************************************************************
            * Load image_from_web image.
            * 
            * Simply pass in a URL to a web hosted image and the corner coordinates of the image.
            * 
            * Image sourced from: https://commons.wikimedia.org/wiki/File:St_Lucia_map.png
            *****************************************************************/

            _imageLayerOptionsInfo.Add("image_from_web", new ImageLayerOptionsInfo(
                new ImageLayerOptions(
                    "https://upload.wikimedia.org/wikipedia/commons/d/df/St_Lucia_map.png",
                    ImageLayer.GetCoordinatesFromEdges(14.1264989, 13.693956, -60.8388, -61.107421)
                ),
                new Position(-60.97311, 13.91022), 10));

            /****************************************************************
            * Load image_from_web_no_cors image.
            * 
            * The Azure Maps Web SDK requires hosted assets to be on a CORs enabled endpoint. 
            * This library automatically downloads images for image layers using 
            * a proxy so you simply just pass in a URL and the map will do the rest.
            *****************************************************************/

            _imageLayerOptionsInfo.Add("image_from_web_no_cors", new ImageLayerOptionsInfo(
               new ImageLayerOptions(
                    "https://samples.azuremaps.com/images/image-overlays/newark_nj_1922.jpg",
                    [
                        new Position(-74.22655, 40.773941), //Top Left Corner
                        new Position(-74.12544, 40.773941), //Top Right Corner
                        new Position(-74.12544, 40.712216), //Bottom Right Corner
                        new Position(-74.22655, 40.712216)  //Bottom Left Corner
                    ]
               ),
               new Position(-74.18, 40.740), 12));
        }

        /// <summary>
        /// Event handler for when the image source picker changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageSourcePicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_imageLayerOptionsInfo.Count == 0)
            {
                return;
            }

            var imageSelectionName = Helpers.GetSelectedPickerString(sender);

            if (_imageLayerOptionsInfo.ContainsKey(imageSelectionName))
            {
                var info = _imageLayerOptionsInfo[imageSelectionName];

                //If there isn't an image layer already, create it and add to the map.
                if (imageLayer == null)
                {
                    imageLayer = new ImageLayer(info.Options);
                    MyMap.Layers.Add(imageLayer);
                }
                else
                {
                    //Update the options of the image layer.
                    imageLayer.SetOptions(info.Options);
                }

                //Update the camera for the sample.
                MyMap.SetCamera(new CameraOptions
                {
                    Center = info.Center,
                    Zoom = info.Zoom
                });
            }
        }

        /// <summary>
        /// Event that is triggered when the opacity slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if(imageLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var opacity = (double)Math.Round(slider.Value, 1);

            //Set the opacity of the image layer.
            imageLayer.SetOptions(new ImageLayerOptions
            {
                Opacity = opacity
            });

            OpacityLabel.Text = $"Opacity: {opacity}";
        }

        /// <summary>
        /// Event that is triggered when the hue rotation slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueRotationSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (imageLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var hueRotation = (double)Math.Round(slider.Value);

            //Set the hue rotation of the image layer.
            imageLayer.SetOptions(new ImageLayerOptions
            {
                HueRotation = hueRotation
            });

            HueRotationLabel.Text = $"Hue Rotation: {hueRotation}";
        }

        /// <summary>
        /// Event that is triggered when the contrast slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContrastSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (imageLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var contrast = (double)Math.Round(slider.Value, 1);

            //Set the contrast of the image layer.
            imageLayer.SetOptions(new ImageLayerOptions
            {
                Contrast = contrast
            });

            ContrastLabel.Text = $"Contrast: {contrast}";
        }

        /// <summary>
        /// Event that is triggered when the saturation slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaturationSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (imageLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var saturation = (double)Math.Round(slider.Value, 1);

            //Set the saturation of the image layer.
            imageLayer.SetOptions(new ImageLayerOptions
            {
                Saturation = saturation
            });

            SaturationLabel.Text = $"Saturation: {saturation}";
        }

        /// <summary>
        /// Event that is triggered when the before layer picker value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeLayerPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageLayer == null)
            {
                return;
            }

            var beforeLayerId = Helpers.GetSelectedPickerString(sender);

            if (beforeLayerId.Equals("undefined"))
            {
                beforeLayerId = string.Empty;
            }

            //Move the image layer before the selected layer within the map.
            //The two main built-in layer IDs are "labels" and "roads", but you can also pass in the ID of any of your layers as well.
            MyMap.Layers.Move(imageLayer, beforeLayerId);
        }

        #endregion
    }


    /// <summary>
    /// A helper class for this sample that contains options for an image layer and center/zoom to set the map to.
    /// </summary>
    public class ImageLayerOptionsInfo
    {
        public ImageLayerOptionsInfo(ImageLayerOptions options, Position center, int zoom)
        {
            Options = options;
            Center = center;
            Zoom = zoom;
        }

        public ImageLayerOptions Options { get; set; }

        public Position Center { get; set; }

        public int Zoom { get; set; }
    }
}
