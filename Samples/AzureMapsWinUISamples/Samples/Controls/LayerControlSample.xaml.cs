using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Layers;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Expression = AzureMapsNativeControl.Expression;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class LayerControlSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the layer control. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=create-a-layer-control
        * https://samples.azuremaps.com/?sample=layer-control-options
        *********************************************************************************************************/

        private LegendControl legend;
        private LayerControl layerControl;

        public LayerControlSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //Create a legend control that the layer control will leverage.
            legend = new LegendControl()
            {
                Title = "Legend",
                Position = ControlPosition.BottomLeft
            };

            //Add the legend control to the map.
            MyMap.Controls.Add(legend);

            //Create a layer control.
            layerControl = new LayerControl()
            {
                Position = ControlPosition.TopRight,

                Title = "Layer options",

                //Attach the legend control.
                LegendControl = legend,

                DynamicLayerGroup = new DynamicLayerGroup
                {
                    GroupTitle = "Dynamic layers",
                    Layout = LayerListLayout.Checkbox
                },

                //Specify the layer groups for the control. 
                LayerGroups = [
                    //A dropdown layer group option.
                    new LayerGroup() {
                    Layout = LayerListLayout.Dropdown,

                    //A title for this layer group.
                    GroupTitle = "Symbol label",

                    //The layers affected by this group. 
                    Layers = ["earthquake-labels"],

                    Items = [
                        new LayerState {
                            Label = "mag",

                            //Optionally specify if the state is enabled or not initially.
                            IsEnabled = true,

                            //The style to apply to the layers when this state is enabled.
                            EnabledStyle = new SymbolLayerOptions {
                                TextOptions = new TextOptions {
                                    //An expression is used to concerte the "mag" property value into a string and appends the letter "m" to the end of it.
                                    TextField = new Expression<string>("concat", new object[] { "to-string", new object[] { "get", "mag" } }, "m")
                                }
                            }
                        },
                        new LayerState {
                            Label = "place",

                            //The style to apply to the layers when this state is enabled.
                            EnabledStyle = new SymbolLayerOptions {
                                TextOptions = new TextOptions {
                                    TextField = new Expression<string>("get", "place")
                                }
                            }
                        }
                    ]
                },

                //A group of check box options.
                new LayerGroup() {
                    Layout = LayerListLayout.Checkbox,

                    //A title for this layer group.
                    GroupTitle = "Magnitude filter",

                    //The layers affected by this group. 
                    Layers = ["earthquake-circles", "earthquake-labels"],

                    Items = [
                        new LayerState {
                            Label = "mag > 6",

                            //Optionally specify if the state is enabled or not initially.
                            IsEnabled = false,

                            //Style to apply when a checkbox is checked.
                            EnabledStyle = new LayerOptions {
                                Filter = new Expression<bool> (">", new object[] { "get", "mag" }, 6)
                            },

                            //Style to apply when a checkbox is unchecked.
                            DisabledStyle = new LayerOptions {
                                Filter = Expression.ClearFilter()
                            }
                        }
                    ]
                },

                //A range sliders layer group option. 
                new LayerGroup()
                {
                    Layout = LayerListLayout.Range,

                    //A title for this layer group, in this case using a value that has a match in the localization resources (resx).
                    GroupTitle = "rangeSlider",

                    //The layers affected by this group. 
                    Layers = ["earthquake-circles"],

                    //Settings for one or more range sliders.
                    Items = [
                        new RangeLayerState {
                            //Optionally format the label.
                            Label = "{rangeValue}px",

                            //Set the initial value. 
                            Value = 0.5,

                            //Specify the style applied to the layers as the range sliders value changes.
                            Style = new BubbleLayerOptions {
                                Opacity = new Expression<double>("to-number", "{rangeValue}"),

                                //Use a style expression to apply custom logic, in this case, applying the opposite value to the stroke. 
                                StrokeOpacity = new Expression<double>("-", 1, new object[] { "to-number", "{rangeValue}" })
                            },

                            //Specify is the layer styles should update as the slider is moving, rather than waiting until it has finished moving.
                            UpdateOnInput = true
                        }
                    ]
                },

                //A radio button layer group options. 
                new LayerGroup()
                {
                    Layout = LayerListLayout.Radio,

                    //A title for this layer group.
                    GroupTitle = "Stroke Width",

                    //The layers affected by this group. 
                    Layers = ["earthquake-circles"],
                    Items = [
                        new LayerState {
                            Label = "5px",

                            //Style to apply when a radio item is selected.
                            EnabledStyle = new BubbleLayerOptions {
                                StrokeWidth = Expression<int>.Literal(5)
                            }
                        },
                        new LayerState {
                            Label = "10px",

                            //Style to apply when a radio item is selected.
                            EnabledStyle = new BubbleLayerOptions {
                                StrokeWidth = Expression<int>.Literal(10)
                            },

                            //Optionally limit the zoom level that certain options appear.
                            //MinZoom = 3,
                            //MaxZoom = 7
                        },
                        new LayerState {
                            Label = "15px",

                            //Style to apply when a radio item is selected.
                            EnabledStyle = new BubbleLayerOptions {
                                StrokeWidth = Expression<int>.Literal(15)
                            }
                        }
                    ]
                }
                ],

                ResourceStrings = new Dictionary<string, string>
                {
                    { "rangeSlider", "Opacity slider" }
                }
            };

            //Add the layer control to the map.
            MyMap.Controls.Add(layerControl);

            /* Add some mock data and layers for the sample. */

            //Create a data source and load a data set of points, in this case all significant earthquakes from the past month.
            var source = new DataSourceLite("https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/significant_month.geojson");
            MyMap.Sources.Add(source);

            MyMap.Layers.AddRange([
                //Create a layer that defines how to render the shapes in the data source and add it to the map.
                new BubbleLayer(source, new BubbleLayerOptions {
                    //Bubbles are made semi-transparent.
                    Opacity = Expression<double>.Literal(0.75),

                    //Color of each bubble based on the value of "mag" property using a color gradient of green, yellow, orange, and red.
                    Color = new Expression<string>(
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "get", "mag" },
                        0, "green",
                        5, "yellow",
                        6, "orange",
                        7, "red"
                    ),

                    /*
                    * Radius for each data point scaled based on the value of "mag" property.
                    * When "mag" = 0, radius will be 2 pixels.
                    * When "mag" = 8, radius will be 40 pixels.
                    * All other "mag" values will be a linear interpolation between these values.
                    */
                    Radius = new Expression<double>(
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "get", "mag" },
                        0, 2,
                        8, 40
                    )

                }, "earthquake-circles"),

                //Create a symbol layer using the same data source to render the magnitude as text above each bubble and add it to the map.
                new SymbolLayer(source, new SymbolLayerOptions {
                    IconOptions = new IconOptions {
                        //Hide the icon image.
                        Image = Expression<string>.Literal("none")
                    },
                    TextOptions = new TextOptions {
                        //An expression is used to concerte the "mag" property value into a string and appends the letter "m" to the end of it.
                        TextField = new Expression<string>("concat", new object[] { "to-string", new object[] { "get", "mag" } }, "m"),
                        Size = Expression<int>.Literal(12)
                    }
                }, "earthquake-labels")
            ]);

            //Add a style control to the map.
            MyMap.Controls.Add(new StyleControl()
            {
                Position = ControlPosition.TopLeft
            });
        }

        #region Options input handlers

        private void TitleEntry_Changed(object sender, TextChangedEventArgs e)
        {
            if (layerControl != null && sender is TextBox tbx)
            {
                //Update the title property.
                layerControl.Title = tbx.Text;
            }
        }

        private void LayoutPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (layerControl != null)
            {
                //Update the layout of the layer control.
                var layoutString = Helpers.GetSelectedPickerString(sender);

                if (Enum.TryParse(typeof(ControlLayout), layoutString, true, out object? layout))
                {
                    layerControl.Layout = (ControlLayout)layout;
                }
            }
        }

        private void StylePicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (layerControl != null)
            {
                var styleString = Helpers.GetSelectedPickerString(sender);

                //Update the style of the layer control.
                switch (styleString)
                {
                    case "Light":
                        layerControl.Style = ControlStyle.Light;
                        break;
                    case "Dark":
                        layerControl.Style = ControlStyle.Dark;
                        break;
                    case "CSS Color":
                        //Use any CSS color to set the background color of the layer control.
                        layerControl.StyleColor = "#6010df";
                        break;
                    case "Auto":
                    default:
                        layerControl.Style = ControlStyle.Auto;
                        break;
                }
            }
        }

        private void MaxWidthSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (layerControl != null && sender is Slider slider)
            {
                int maxWidth = (int)slider.Value;

                if (layerControl.MaxWidth != maxWidth)
                {
                    //Update the max width of the layer control. This limit how wide the layer control can be.
                    layerControl.MaxWidth = maxWidth;
                    MaxWidthLabel.Text = $"MaxWidth: {maxWidth}";
                }
            }
        }

        private void ShowToggleCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (layerControl != null && sender is CheckBox cbx)
            {
                //Specify if the toggle button for minimizing the layer control is displayed or not.
                layerControl.ShowToggle = cbx.IsChecked == true;
            }
        }

        private void MiminizedCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (layerControl != null && sender is CheckBox cbx)
            {
                //Update the minimized state, and either collapse or expand the layer control.
                layerControl.Miminized = cbx.IsChecked == true;
            }
        }

        private void VisibleCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (layerControl != null && sender is CheckBox cbx)
            {
                //Update the visible property.
                layerControl.Visible = cbx.IsChecked == true;
            }
        }

        #endregion
    }
}