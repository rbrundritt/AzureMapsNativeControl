using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class HeatMapLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to create a simple heat map from a data set of point features.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=heat-map-layer-options
        * https://samples.azuremaps.com/?sample=consistent-zoomable-heat-map
        * 
        * The data in this sample is sourced from the USGS Earthquake Hazards Program (https://www.usgs.gov/programs/earthquake-hazards) 
        * and consists of points that represents significant earthquakes that have occurred in the last 30 days.
        *********************************************************************************************************/

        #region Private Properties

        private DataSourceLite dataSource;
        private HeatMapLayer heatMapLayer;

        #endregion

        public HeatMapLayerSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //Create a style control and add it to the map.
            MyMap.Controls.Add(new StyleControl()
            {
                Position = ControlPosition.TopRight
            });

            //Create a data source and add it to the map.
            //Using a DataSourceLite since we don't need to edit or access the data from the .NET side. 
            //DataSourceLite is more efficient as it only maintains a version of the data on the map side (JavaScript) and not a copy of it in .NET.
            //We can also pass in an initial JSON data feed to load into the data source. This feed is a set of points that represent all earthquakes from the past 30 days.
            dataSource = new DataSourceLite("https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_month.geojson");
            MyMap.Sources.Add(dataSource);

            //Create a heat map layer and add it to the map.
            heatMapLayer = new HeatMapLayer(dataSource);
            MyMap.Layers.Add(heatMapLayer);

            //For this sample, set the initial heat gradient option from the picker.
            HeatGradientPicker.SelectedIndex = 0;

            //Create a legend control that dynamically updates based on the heat map layer options.
            MyMap.Controls.Add(new LegendControl()
            {
                Title = "Earthquake Heat Map",
                Legends = [new DynamicLegend(heatMapLayer) {
                    SubtitleFallback = "none",
                    DefaultCategory = new CategoryLegendDefaults() {
                        Shape = "square"
                    }
                }],
                Position = ControlPosition.BottomLeft
            });
        }

        private void HeatGradientPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var heatGradient = Helpers.GetSelectedPickerString(sender);
            Expression<string> heatGradientExp;

            switch (heatGradient)
            {
                case "default_with_mask":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0,0,0.5)",
                        0.01, "royalblue",
                        0.3, "cyan",
                        0.5, "lime",
                        0.7, "yellow",
                        1, "red"
                    };
                    break;
                case "color_spectrum":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0,0,0)",
                        0.01, "navy",
                        0.25, "blue",
                        0.5, "green",
                        0.75, "yellow",
                        1, "red"
                    };
                    break;
                case "incandescent":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0,0,0)",
                        0.01, "black",
                        0.33, "darkred",
                        0.66, "yellow",
                        1, "white"
                    };
                    break;
                case "heated_metal":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0,0,0)",
                        0.01, "black",
                        0.25, "purple",
                        0.5, "red",
                        0.75, "yellow",
                        1, "white"
                    };
                    break;
                case "fire":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0,0,0)",
                        0.01, "red",
                        0.66, "yellow",
                        1, "white"
                    };
                    break;
                case "sunrise":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                         0, "rgba(0,0,0,0)",
                        0.01, "#feb24c",
                        0.03, "#feb24c",
                        0.5, "#fd8d3c",
                        0.7, "#fc4e2a",
                        1, "#e31a1c"
                    };
                    break;
                case "light_blue_to_red":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(33,102,172,0)",
                        0.2, "rgb(103,169,207)",
                        0.4, "rgb(209,229,240)",
                        0.6, "rgb(253,219,199)",
                        0.8, "rgb(239,138,98)",
                        1, "rgb(178,24,43)"
                    };
                    break;
                case "gray_to_aqua_green":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(236,222,239,0)",
                        0.2, "rgb(208,209,230)",
                        0.4, "rgb(166,189,219)",
                        0.6, "rgb(103,169,207)",
                        0.8, "rgb(28,144,153)"
                    };
                    break;
                case "purple_pink_light_blue":
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "transparent",
                        0.01, "purple",
                        0.5, "#fb00fb",
                        1, "#00c3ff"
                    };
                    break;
                case "stepped_colors_ngyr":
                    heatGradientExp = new Expression<string>() {
                        "step",
                        new object[] { "heatmap-density" },
                        "transparent",
                        0.01, "navy",
                        0.25, "green",
                        0.50, "yellow",
                        0.75, "red"
                    };
                    break;
                case "stepped_colors_wpp":
                    heatGradientExp = new Expression<string>() {
                        "step",
                        new object[] { "heatmap-density" },
                        "transparent",
                        0.01, "#fff7f3",
                        0.12, "#fde0de",
                        0.23, "#fcc5c0",
                        0.34, "#f99fb5",
                        0.45, "#f767a1",
                        0.56, "#dd3497",
                        0.67, "#ae017e",
                        0.78, "#790277",
                        0.89, "#48006a"
                    };
                    break;
                case "stepped_colors_tr":
                    heatGradientExp = new Expression<string>() {
                       "step",
                        new object[] { "heatmap-density" },
                        "transparent",
                        0.01, "#03939c",
                        0.17, "#5ebabf",
                        0.33, "#bae1e2",
                        0.49, "#f8c0aa",
                        0.66, "#dd7755",
                        0.83, "#c22e00"
                    };
                    break;
                case "default":
                default:
                    heatGradientExp = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "heatmap-density" },
                        0, "rgba(0,0, 255,0)",
                        0.1, "royalblue",
                        0.3, "cyan",
                        0.5, "lime",
                        0.7, "yellow",
                        1, "red"
                    };
                    break;
            }

            heatMapLayer.SetOptions(new HeatMapLayerOptions()
            {
                //Set the heat gradient to use.
                Color = heatGradientExp
            });
        }

        private void RadiusSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;
            double radius = Math.Round(slider.Value);
            var currentRadius = heatMapLayer.GetOptions().Radius;

            //Make sure that radius value has changed.
            if (currentRadius == null || !currentRadius.IsLiteralEquals(radius))
            {
                //Set the radius of the heat map.
                heatMapLayer.SetOptions(new()
                {
                    Radius = Expression<double>.Literal(radius)
                });

                RadiusLabel.Text = $"Radius: {radius}";
            }
        }

        private void ZoomScaledRadiusCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            Expression<double> radiusExp;
            string radiusValue;

            if (checkBox.IsChecked != null && checkBox.IsChecked.Value)
            {
                //Create an expression that scales the radius based on the zoom level.
                radiusExp = new Expression<double>()
                {
                    "interpolate",
                    new object[] { "exponential", 2 },
                    new object[] { "zoom" },

                    //For all zoom levels 10 or lower, set the radius to 2 pixels.
                    10, 2,

                    //Between zoom level 10 and 22, exponentially scale the radius from 2 pixels to 50000 pixels.
                    22, 50000
                };

                radiusValue = "zoom scaled";
            }
            else
            {
                //Default to the radius slider value.
                radiusExp = Expression<double>.Literal(RadiusSlider.Value);
                radiusValue = Math.Round(RadiusSlider.Value, 2).ToString();
            }

            //Update the radius option.
            heatMapLayer.SetOptions(new()
            {
                Radius = radiusExp
            });

            RadiusLabel.Text = $"Radius: {radiusValue}";
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var opacity = (double)Math.Round(slider.Value, 2);

            //Set the opacity of the heat map.
            heatMapLayer.SetOptions(new()
            {
                Opacity = opacity
            });

            OpacityLabel.Text = $"Opacity: {opacity}";
        }

        private void IntensitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            //Set the intensity of the heat map.
            heatMapLayer.SetOptions(new()
            {
                Intensity = slider.Value
            });

            IntensityLabel.Text = $"Intensity: {Math.Round(slider.Value, 2)}";
        }

        private void WeightedCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }
            var checkBox = (CheckBox)sender;

            Expression<double> weigthExp;

            if (checkBox.IsChecked != null && checkBox.IsChecked.Value)
            {
                //Use an expression to set the weight of the heat map points based on the density of the points.
                weigthExp = new Expression<double>()
                {
                    "interpolate",
                    new object[] { "exponential", 2 },  //Using an exponential interpolation since earthquake magnitudes are on an exponential scale.
                    new object[] { "get", "mag" },      //Retrieve the magnitude of the earthquake from the point data.
                    0, 0,
                    10, 1
                };
            }
            else
            {
                //Reset the weight to 1.
                weigthExp = Expression<double>.Literal(1);
            }

            heatMapLayer.SetOptions(new()
            {
                Weight = weigthExp
            });
        }

        private void BeforeLayerPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (heatMapLayer == null)
            {
                return;
            }

            var beforeLayerId = Helpers.GetSelectedPickerString(sender);

            if (beforeLayerId.Equals("undefined"))
            {
                beforeLayerId = string.Empty;
            }

            //Move the heat map layer before the selected layer within the map.
            //The two main built-in layer IDs are "labels" and "roads", but you can also pass in the ID of any of your layers as well.
            MyMap.Layers.Move(heatMapLayer, beforeLayerId);
        }
    }
}
