using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Windows;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    public partial class LegendControlSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the legend control. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=create-a-legend-control
        * https://samples.azuremaps.com/?sample=legend-control-options
        * https://samples.azuremaps.com/?sample=category-legend-options
        * https://samples.azuremaps.com/?sample=legend-control-options
        * https://samples.azuremaps.com/?sample=gradient-legend-options
        *********************************************************************************************************/

        private LegendControl? legendControl = null;

        public LegendControlSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //For this sample we will add in custom CSS classes to the map as some of the legends will use these.
            MyMap.JsInterlop.AddRawCss(".darkText { color: black; text-shadow: white 0 0 2px; } .lightText { color: white; text-shadow: black 0 0 2px; }");

            //For this sample a dynamic legend will be generated from a layer. So add a data source, load some data, and create a heat map layer.
            //Load a data set of points, in this case all earthquakes from the past 30 days.
            var dataSource = new DataSourceLite("https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_month.geojson");
            MyMap.Sources.Add(dataSource);

            //Create a heat map layer and add it to the map. 
            var heatMapLayer = new HeatMapLayer(dataSource, new HeatMapLayerOptions
            {
                Color = new Expression<string>
                {
                    "step",
                    new object[] { "heatmap-density" },
                    "transparent",
                    0.01, "#03939c",
                    0.17, "#5ebabf",
                    0.33, "#bae1e2",
                    0.49, "#f8c0aa",
                    0.66, "#dd7755",
                    0.83, "#c22e00"
                }
            });
            MyMap.Layers.Add(heatMapLayer);

            //Create the legend control.
            legendControl = new LegendControl()
            {
                Position = ControlPosition.BottomLeft,

                //Global title to display for the legend.
                Title = "My Legend",

                //All legend cards to display within the legend control.
                Legends = [
                    //A category legend that uses a combination of shapes and icons.
                    new CategoryLegend()
                {
                    Subtitle = "Category",
                    Footer = "A category legend that uses a combination of shapes and icons.",
                    StrokeWidth = 2,
                    Items = [
                        new CategoryLegendItem()
                        {
                            Label = "label1",
                            Color = "DodgerBlue",
                            Shape = "images/CoffeeIcon.png" //A url to an image that's in the Raw/map_resources/images folder. The Raw/map_resources folder is where the map looks for resources with relative paths.
                        },
                        new CategoryLegendItem()
                        {
                            Label = "label2",
                            Color = "Yellow",
                            Shape = "square"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "Ricky",
                            Color = "Orange",
                            Shape = "line"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "is",
                            Color = "Red",
                            Shape = "circle"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "awesome!",
                            Color = "purple",
                            Shape = "triangle"
                        }
                    ]
                },

                //A category legend that scales the shapes/icons.
                new CategoryLegend()
                {
                    Subtitle = "Category - Scaled shapes",
                    Layout = CssFlexDirection.ColumnReverse,
                    ItemLayout = CssFlexDirection.Row,
                    Footer = "A category legend that scales the shapes/icons.",
                    Shape = "circle",
                    Color = "transparent",

                    //Setting fitItems to true will allow all shapes to be equally spaced out and centered.
                    FitItems = true,

                    //Set the shape size for each item.
                    Items = [
                        new CategoryLegendItem()
                        {
                            Label = "10",
                            ShapeSize = 10
                        },
                        new CategoryLegendItem()
                        {
                            Label = "20",
                            ShapeSize = 20
                        },
                        new CategoryLegendItem()
                        {
                            Label = "30",
                            ShapeSize = 30
                        },
                        new CategoryLegendItem()
                        {
                            Label = "40",
                            ShapeSize = 40
                        }
                    ]
                },

                //A category legend with different size lines.
                new CategoryLegend()
                {
                    Subtitle = "Category - Scaled lines",
                    Layout = CssFlexDirection.ColumnReverse,
                    ItemLayout = CssFlexDirection.Row,
                    Footer = "A category legend that scales lines. Scale line thickness by using stroke width. Use shape size to specify length.",
                    Shape = "line",

                    //Setting fitItems to true will allow all shapes to be equally spaced out and centered.
                    FitItems = true,

                    //Use shape size to set line length.
                    ShapeSize = 20,

                    //Scale lines by using stroke width.
                    Items = [
                        new CategoryLegendItem()
                        {
                            Label = "1",
                            StrokeWidth = 1,
                            Color = "red"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "2",
                            StrokeWidth = 2,
                            Color = "orange"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "4",
                            StrokeWidth = 4,
                            Color = "purple"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "6",
                            StrokeWidth = 6,
                            Color = "blue"
                        }
                    ]
                },

                //A category legend that collapses the space between items, overlaps the labels with the shapes, and uses custom CSS to collapse the space between the category items. 
                new CategoryLegend()
                {
                    Subtitle = "Category - Custom CSS",
                    Layout = CssFlexDirection.Column,
                    ItemLayout = CssFlexDirection.Column,
                    Footer = "A category legend that collapses the space between items, overlaps the labels with the shapes, and uses custom CSS to collapse the space between the category items.",
                    Shape = "square",

                    //Have the labels overlap the shape. Otherwise the text span size may push the shape away from the other shapes.
                    LabelsOverlapShapes = true,

                    //Collapse the space between the items.
                    Collapse = true,

                    //Optionally remove the stroke around the shapes.
                    StrokeWidth = 0,

                    //Set the shape size for each item.
                    Items = [
                        new CategoryLegendItem()
                        {
                            Label = "low",
                            Color = "white",

                            //Ensure the label will always appear when overlaid on top of the item. 
                            CssClass = "darkText"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#d24fa0"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#8a32d7"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#144bed"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#479702"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#72b403"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#93c701"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#ffd701"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#f05514"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#dc250e"
                        },
                        new CategoryLegendItem()
                        {
                            Color = "#ba0808"
                        },
                        new CategoryLegendItem()
                        {
                            Label = "high",
                            Color = "black",

                            //Ensure the label will always appear when overlaid on top of the item. 
                            CssClass = "lightText"
                        }
                    ]
                },

                //A simple legend that loads an image.
                new ImageLegend()
                {
                    Subtitle = "Image legend",

                    //URL to legend image.
                    Url = "https://i0.wp.com/gisgeography.com/wp-content/uploads/2016/05/Map-Example-Legend.png",
                    
                    //Alt text should always be provided for legend images.
                    AltText = "A legend for the icons on the map.",

                    //Optionally specify a max width or height for the image.
                    MaxHeight = 200,

                    Footer = "This legend was created using an image."
                },

                //A simple HTML legend created using an HTML string.
                new HtmlLegend()
                {
                    Subtitle = "Simple HTML legend",
                    Html = "<span class=\"simple-legend\">This <b>legend</b> is created using an <i>HTML</i> string.</span>"
                },

                //A gradient legend that uses multiple color stops, some having labels.
                new GradientLegend()
                {
                    Subtitle = "Gradient legend",
                    Footer = "A gradient legend that uses multiple color stops, some having labels.",
                    Stops = [
                        new GradientLegendStop(0, "royalblue", "low"),
                        new GradientLegendStop(0.25, "cyan"),
                        new GradientLegendStop(0.5, "lime", "medium"),
                        new GradientLegendStop(0.75, "yellow"),
                        new GradientLegendStop(1, "red", "high")
                    ]
                },

                //A gradient legend that has transparency
                new GradientLegend()
                {
                    Subtitle = "Gradient legend - Transparent",
                    Footer = "A gradient legend that has transparency.",
                    Orientation = AzureMapsNativeControl.MapOrientation.Vertical,
                    BarLength = 75,
                    Stops = [
                        new GradientLegendStop(0, "rgba(0, 0, 0, 0)", "Transparent"),
                        new GradientLegendStop(1, "black", "Solid")
                    ]
                },

                //A gradient legend with stepped color stops.
                new GradientLegend()
                {
                    Subtitle = "Gradient legend - stepped",
                    Footer = "In this gradient each color has two stops, a starting stop and an end stop. The end stop has the same offset as the next colors starting stop.",
                    Stops = [
                        new GradientLegendStop(0, "#03939c", "< -1"),
                        new GradientLegendStop(0.167, "#03939c"),
                        new GradientLegendStop(0.167, "#5ebabf"),
                        new GradientLegendStop(0.334, "#5ebabf"),
                        new GradientLegendStop(0.334, "#bae1e2"),
                        new GradientLegendStop(0.501, "#bae1e2"),
                        new GradientLegendStop(0.501, "#f8c0aa", "0"),
                        new GradientLegendStop(0.668, "#f8c0aa"),
                        new GradientLegendStop(0.668, "#dd7755"),
                        new GradientLegendStop(0.835, "#dd7755"),
                        new GradientLegendStop(0.835, "#c22e00"),
                        new GradientLegendStop(1, "#c22e00", "> 1")
                    ]
                },

                //Add a legend that is dynamically generated by a layers settings. This legend will update if the layers style changes.
                new DynamicLegend(heatMapLayer)
                {
                    //By default the dynamic legend will try and find a suitable subtitle for the layer, such as the layers Id, if we don't explicitly set one.
                    Subtitle = "Dynamic legend - 30 day Earthquake density",
                    Footer = "A legend that is dynamically generated by a layers settings. This legend will update if the layers style changes.",
                    DefaultCategory = new CategoryLegendDefaults()
                    {
                        Shape = "square"
                    }
                }
                ],

                //Optional resource key value pairs used to replace labels and titles with localized values specified in this object.
                ResourceStrings = new Dictionary<string, string>()
                {
                    { "label1", "Hello" },
                    { "label2", "World!" }
                }
            };

            //Add the legend control to the map.
            MyMap.Controls.Add(legendControl);
        }

        #region Options input handlers

        private void TitleEntry_Changed(object sender, TextChangedEventArgs e)
        {
            if (legendControl != null && sender is TextBox tb)
            {
                //Update the title property.
                legendControl.Title = tb.Text;
            }
        }

        private void LayoutPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (legendControl != null)
            {
                //Update the layout of the legend.
                var layoutString = Helpers.GetSelectedPickerString(sender);

                if (Enum.TryParse(typeof(ControlLayout), layoutString, true, out object? layout))
                {
                    legendControl.Layout = (ControlLayout)layout;
                }
            }
        }

        private void StylePicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (legendControl != null)
            {
                var styleString = Helpers.GetSelectedPickerString(sender);

                //Update the style of the legend.
                switch (styleString)
                {
                    case "Light":
                        legendControl.Style = ControlStyle.Light;
                        break;
                    case "Dark":
                        legendControl.Style = ControlStyle.Dark;
                        break;
                    case "CSS Color":
                        //Use any CSS color to set the background color of the legend.
                        legendControl.StyleColor = "#6010df";
                        break;
                    case "Auto":
                    default:
                        legendControl.Style = ControlStyle.Auto;
                        break;
                }
            }
        }

        private void MaxWidthSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (legendControl != null && sender is Slider sld)
            {
                int maxWidth = (int)sld.Value;

                if (legendControl.MaxWidth != maxWidth)
                {
                    //Update the max width of the legend. This limit how wide the legend can be.
                    legendControl.MaxWidth = maxWidth;
                    MaxWidthLabel.Text = $"MaxWidth: {maxWidth}";
                }
            }
        }

        private void ShowToggleCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (legendControl != null && sender is CheckBox cbx)
            {
                //Specify if the toggle button for minimizing the legend is displayed or not.
                legendControl.ShowToggle = cbx.IsChecked == true;
            }
        }

        private void MiminizedCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (legendControl != null && sender is CheckBox cbx)
            {
                //Update the minimized state, and either collapse or expand the legend.
                legendControl.Miminized = cbx.IsChecked == true;
            }
        }

        private void VisibleCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (legendControl != null && sender is CheckBox cbx)
            {
                //Update the visible property.
                legendControl.Visible = cbx.IsChecked == true;
            }
        }

        #endregion
    }
}