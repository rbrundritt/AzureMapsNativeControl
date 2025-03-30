using System.Windows;
using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsWPFSamples.Samples
{
    public partial class PolygonExtrusionLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways to display polygon extrusions.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=extruded-choropleth-map    
        * https://samples.azuremaps.com/?sample=polygon-extrusion-layer-options
        * https://samples.azuremaps.com/?sample=show-popup-on-hover
        *********************************************************************************************************/

        #region Private Properties

        private DataSourceLite dataSource;
        private PolygonExtrusionLayer polygonExtrusionLayer;
        private Popup popup;

        #endregion

        #region Contrsutor

        public PolygonExtrusionLayerSample()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event that is triggered when the map is ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Add images for fill pattern to the maps image sprite.
            await CreateFillPatterns();

            //Create a reusable popup and add it to the map.
            popup = new Popup(new PopupOptions
            {
                ShowCloseButton = false,            //No need to show the close button, so hide it.
                PixelOffset = new Pixel(0, -5), //Offset the popup slightly so that we can't overlap the popup with the mouse.
            });
            MyMap.Popups.Add(popup);

            //Create a data source and add it to the map.
            dataSource = new DataSourceLite("data/geojson/US_States_Population_Density.json");
            MyMap.Sources.Add(dataSource);

            //Create a polygon extrusion layer.
            polygonExtrusionLayer = new PolygonExtrusionLayer(dataSource, new PolygonExtrusionLayerOptions
            {
                Base = Expression<double>.Literal(0),

                //Height of the extrusion in meters.
                Height = new Expression<double>
                {
                    "interpolate",
                    new object[] { "linear" },
                    new object[] { "get", "density" },
                    0, 100,
                    1200, 960000
                },

                //Color of the extrusion based on the value of the "population" property.
                FillColor = new Expression<string>
                {
                    "step",
                    new object[] { "get", "density" },
                    "#00ff80",
                    10, "#09e076",
                    20, "#0bbf67",
                    50, "#f7e305",
                    100, "#f7c707",
                    200, "#f78205",
                    500, "#f75e05",
                    1000, "#f72505"
                },

                FillOpacity = 0.7
            });

            MyMap.Layers.Add(polygonExtrusionLayer, "labels");

            //Add a mouse move event to the map to close the popup.
            MyMap.Events.Add("mousemove", MapMouseMoved);

            //Add a mouse move event to the layer to show a popup.
            //Mouse move is used as mouse enter and over events will fire when the mouse first goes over a shape in the layer but won't fire again if you move the mouse between two shapes that are side by side. 
            MyMap.Events.Add("mousemove", polygonExtrusionLayer, LayerMouseMoved);

            //Add a legend control to the map to add some context to the data colors/styles.
            MyMap.Controls.Add(new LegendControl()
            {
                Title = "Population Density<br/>(people/mi²)",
                ShowToggle = false,
                Legends = [
                    new CategoryLegend() {
                        Items = [
                            new CategoryLegendItem("#00ff80", "0 - 10"),
                            new CategoryLegendItem("#09e076", "10 - 20"),
                            new CategoryLegendItem("#0bbf67", "20 - 50"),
                            new CategoryLegendItem("#f7e305", "50 - 100"),
                            new CategoryLegendItem("#f7c707", "100 - 200"),
                            new CategoryLegendItem("#f78205", "200 - 500"),
                            new CategoryLegendItem("#f75e05", "500 - 1000"),
                            new CategoryLegendItem("#f72505", "1000+")
                        ],
                        Shape = "square"
                    }
                ],
                Position = ControlPosition.TopLeft
            });
        }

        /// <summary>
        /// Event that is triggered when the mouse moves over the layer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerMouseMoved(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                var props = args.Shapes[0].Properties;

                //Show a popup with the density value.
                popup.SetOptions(new PopupOptions
                {
                    Position = args.Position,  //Show the popup by the mouse cursor.
                    Content = $"<div style='padding:10px'><b>{props.GetString("name")}</b><br/>Density: {props.GetDouble("density")} (people/mi<sup>2</sup>)</div>"
                });

                popup.Open();
            }
        }

        /// <summary>
        /// Event that is triggered when the map is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapMouseMoved(object? sender, MapEventArgs e)
        {
            popup.Close();
        }

        /// <summary>
        /// Event that is triggered when the fill picker is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonExtrusionLayer == null)
            {
                return;
            }

            var fillType = Helpers.GetSelectedPickerString(sender);

            //Pattern selected, use image fill patterns.
            if (fillType.Equals("Pattern"))
            {
                //Set the fill pattern of the polygons.
                polygonExtrusionLayer.SetOptions(new PolygonExtrusionLayerOptions
                {
                    FillPattern = new Expression<string> {
                        "step",
                        new object[] { "get", "density" },
                        "default_pattern",
                        10, "value_10_pattern",
                        20, "value_20_pattern",
                        50, "value_50_pattern",
                        100, "value_100_pattern",
                        200, "value_200_pattern",
                        500, "value_500_pattern",
                        1000, "value_1000_pattern"
                    }
                });

                //NOTE: Fill patterns are a great way to make your maps more visually accessible.
            }
            else
            {
                //Set the fill color of the polygons.
                polygonExtrusionLayer.SetOptions(new PolygonExtrusionLayerOptions
                {
                    FillColor = new Expression<string> {
                        "step",
                        new object[] { "get", "density" },
                        "#00ff80",
                        10, "#09e076",
                        20, "#0bbf67",
                        50, "#f7e305",
                        100, "#f7c707",
                        200, "#f78205",
                        500, "#f75e05",
                        1000, "#f72505"
                    }
                });
            }
        }

        /// <summary>
        /// Event that is triggered when the height scale slider is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeightScaleSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (polygonExtrusionLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            double scale = Math.Round(slider.Value, 2);

            //Use the original expression and scale the values.
            polygonExtrusionLayer.SetOptions(new PolygonExtrusionLayerOptions
            {
                Height = new Expression<double>
                {
                    "interpolate",
                    new object[] { "linear" },
                    new object[] { "get", "density" },
                    0, 100 * scale,
                    1200, 960000 * scale
                }
            });

            HeightScaleLabel.Text = $"Height scale: {scale}";
        }

        /// <summary>
        /// Event that is triggered when the base height slider is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (polygonExtrusionLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            double baseValue = Math.Round(slider.Value);

            //Set the base height of the polygons.
            polygonExtrusionLayer.SetOptions(new PolygonExtrusionLayerOptions
            {
                Base = Expression<double>.Literal(baseValue)
            });

            BaseLabel.Text = $"Base: {baseValue}m";
        }

        /// <summary>
        /// Event that is triggered when the fill opacity slider is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillOpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (polygonExtrusionLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            double opacity = Math.Round(slider.Value, 2);

            //Set the fill opacity of the polygons.
            polygonExtrusionLayer.SetOptions(new PolygonExtrusionLayerOptions
            {
                FillOpacity = opacity
            });

            FillOpacityLabel.Text = $"Fill Opacity: {opacity}";
        }

        /// <summary>
        /// Event that is triggered when the layer picker is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeLayerPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonExtrusionLayer == null)
            {
                return;
            }

            var beforeLayerId = Helpers.GetSelectedPickerString(sender);

            if (beforeLayerId.Equals("undefined"))
            {
                beforeLayerId = string.Empty;
            }

            //Move the polygon layer before the selected layer within the map.
            //The two main built-in layer IDs are "labels" and "roads", but you can also pass in the ID of any of your layers as well.
            MyMap.Layers.Move(polygonExtrusionLayer, beforeLayerId);
        }

        /// <summary>
        /// Create fill patterns for the polygons.
        /// </summary>
        /// <returns></returns>
        private async Task CreateFillPatterns()
        {
            //For simplicity of this sample, we will create a bunch of fill patterns using built-in icon templates.
            //See the polygon layer sample for examples of how to create custom fill patterns from images.

            //TIP: You can load multiple images into the map asynchronously for faster loading.
            await Task.WhenAll(new List<Task>()
            {
                //Input is: custom icon name, template name, icon color, secondary icon color, scale. The last three are optional.
                MyMap.ImageSprite.CreateFromTemplateAsync("default_pattern", "dots", "#00ff80", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_10_pattern", "diagonal-stripes-down", "#09e076", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_20_pattern", "zig-zag", "#0bbf67", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_50_pattern", "checker", "#f7e305", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_100_pattern", "x-fill", "#f7c707", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_200_pattern", "grid-lines", "#f78205", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_500_pattern", "circles-spaced", "#f75e05", "#989898"),
                MyMap.ImageSprite.CreateFromTemplateAsync("value_1000_pattern", "rotated-grid-stripes", "#f72505", "#989898")
            });
        }

        #endregion
    }

}
