using System.Windows;
using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsWPFSamples.Samples
{
    public partial class LineLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways to display lines.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=line-layer-options
        * https://samples.azuremaps.com/?sample=line-with-stroke-gradient
        * https://samples.azuremaps.com/?sample=line-layer-with-built-in-icon-template
        *********************************************************************************************************/

        #region Private Properties

        private DataSource dataSource;

        private LineLayer lineLayer;
        private BubbleLayer linePointLayer;
        private SymbolLayer iconLayer;

        #endregion

        #region Constructor

        public LineLayerSample()
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
            //Add icons for displaying along the lines.
            await MyMap.ImageSprite.CreateFromTemplateAsync("myTemplatedIcon", "car", "DodgerBlue", "#fff");

            //Create a data source and add it to the map.
            dataSource = new DataSource(null, new DataSourceOptions
            {
                //Enable line metrics on the data source. This is needed to enable support for strokeGradient.
                LineMetrics = true
            });
            MyMap.Sources.Add(dataSource);

            //Create a layer for rendering the lines.
            lineLayer = new LineLayer(dataSource);

            //Create a layer displaying icons along the lines.
            iconLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                LineSpacing = 100,               //Distance in pixels between each icon.
                Placement = LabelPlacement.Line, //Place the icons along the line.
                IconOptions = new IconOptions
                {
                    Image = Expression<string>.Literal("myTemplatedIcon"),
                    AllowOverlap = true,
                    Anchor = PositionAnchor.Center,
                    Rotation = Expression<double>.Literal(90)   //Since the car points up by default, rotate it 90 degrees so it points left. This will make the car point in the direction of the line.
                },
                Visible = false
            });

            //LineString data can also be rendered as points (individual coordinates of the lines).

            //Use a bubble or symbol layer to render the points of the lines.
            linePointLayer = new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                Visible = false
            });

            /*
             * TIP
             * If you have a data source with a mix of geometries, you may not want lines to be rendered by point layers (Bubble, Symbol).
             * To prevent this, add a filter to the layer options that limits the geometry types rendered by that layer. 
             * The Expression class has static methods for these to simplify the code.
             * 
             * Expression.PointTypeFilter()
             * Expression.LineStringTypeFilter()
             * Expression.PolygonTypeFilter()
             */

            //Add the layers to the map. 
            MyMap.Layers.AddRange([lineLayer, linePointLayer, iconLayer]);
            //MyMap.Layers.AddRange([lineLayer, linePointLayer, iconLayer], "labels"); //Typically you will want polygons and lines below the "labels" or "roads" layer so they don't cover up map labels.

            //Load some data for the sample.
            LoadData();
        }

        /// <summary>
        /// Event that is triggered when the stroke color picker value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokeColorPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            //Any CSS color value can be used as the color: https://www.w3schools.com/cssref/css_colors_legal.php
            //TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.

            string cssFillColor = Helpers.GetSelectedPickerString(sender);

            //Check to see if stroke gradient was selected.
            if (cssFillColor.Equals("Stroke Gradient"))
            {
                //Use a stroke gradient to style the lines.
                lineLayer.SetOptions(new LineLayerOptions
                {
                    StrokeGradient = new Expression<string>() {
                        "interpolate",
                        new object[] { "linear" },
                        new object[] { "line-progress" },
                        0, "blue",
                        0.1, "royalblue",
                        0.3, "cyan",
                        0.5, "lime",
                        0.7, "yellow",
                        1, "red"
                    }
                });

                //NOTE: The stroke gradient option is disabled when the stroke dash array is set.
                //The Stroke gradient can only be used with the DataSource class when the lineMetrics option is set to true.
            }
            else
            {
                //A solid color will be used to render the lines.

                Expression<string> colorExp;

                if (cssFillColor.Equals("Data driven style"))
                {
                    //Instead of using a single color, use property information on the features to specify how to style them.
                    colorExp = new Expression<string>() {
                        "case",
                        new object[] { "has", "myColor" }, //Check if the feature has a property called "myColor".
                        new object[] { "get", "myColor" }, //If it does, use the value of the "myColor" property.
                        "yellow" //If it doesn't, use a default color.
                    };
                }
                else
                {
                    //Use a CSS color string as the fill color. 
                    colorExp = Expression<string>.Literal(cssFillColor);
                }

                lineLayer.SetOptions(new LineLayerOptions
                {
                    StrokeColor = colorExp
                });
            }
        }

        /// <summary>
        /// Event that is triggered when the stroke opacity slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokeOpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if(lineLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;
            var opacity = (double)Math.Round(slider.Value, 2);

            //Set the opacity of the lines.
            lineLayer.SetOptions(new LineLayerOptions
            {
                StrokeOpacity = Expression<double>.Literal(opacity),
            });

            StrokeOpacityLabel.Text = $"Stroke Opacity: {opacity}";
        }

        /// <summary>
        /// Event that is triggered when the stroke width slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokeWidthSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;
            var strokeWidth = (int)Math.Round(slider.Value);

            //Set the width of the lines.
            lineLayer.SetOptions(new LineLayerOptions
            {
                StrokeWidth = Expression<int>.Literal(strokeWidth)
            });

            StrokeWidthLabel.Text = $"Stroke width: {strokeWidth}";
        }

        /// <summary>
        /// Event that is triggered when the show icons on lines checkbox is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowIconsOnLinesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            //Show the icon layer that places icons along the lines.
            iconLayer.SetOptions(new SymbolLayerOptions
            {
                Visible = checkBox.IsChecked
            });
        }

        /// <summary>
        /// Event that is triggered when the dash lines checkbox is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DashLinesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            if (checkBox.IsChecked != null && checkBox.IsChecked.Value)
            {
                //Set the stroke dash array with a pattern.
                lineLayer.SetOptions(new LineLayerOptions
                {
                    StrokeDashArray = new int[] { 5, 5 }
                });
            }
            else
            {
                //Clearing the stroke dash array is a bit more complex within Azure Maps as it requires explicitly setting it to undefined.
                //However, we often want to set other options without always having to explictily passing in the dash array value.
                //To simplify this, simply pass in an empty array/list and the layer will remove the dash array and set the StrokeDashArray option to null.
                lineLayer.SetOptions(new LineLayerOptions
                {
                    StrokeDashArray = new int[0]
                });
            }
        }

        /// <summary>
        /// Event that is triggered when the show line points checkbox is checked or unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowLinePointsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            //Show or hide the points of the lines.
            linePointLayer.SetOptions(new LayerOptions
            {
                Visible = checkBox.IsChecked
            });
        }

        /// <summary>
        /// Event that is triggered when the line layer is moved before another layer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeLayerPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lineLayer == null)
            {
                return;
            }

            var beforeLayerId = Helpers.GetSelectedPickerString(sender);

            if (beforeLayerId.Equals("undefined"))
            {
                beforeLayerId = string.Empty;
            }

            //Move the line layer before the selected layer within the map.
            //The two main built-in layer IDs are "labels" and "roads", but you can also pass in the ID of any of your layers as well.        
            MyMap.Layers.Move(lineLayer, beforeLayerId);

            //Keep the icon and line point layers above all others.
            MyMap.Layers.Move(linePointLayer);
            MyMap.Layers.Move(iconLayer);
        }

        /// <summary>
        /// Load some line data into the data source.
        /// </summary>
        private void LoadData()
        {
            //The following shows different ways to add linestring data into a data source.
            //Note that line layers can also be used with vector tiles.
            //Polygon's data can also be rendered using a line layer. 

            //Load data from a geojson file. In this case a GeoJson file is being retrieved from the "Raw/map_resources" folder of the app.
            dataSource.ImportDataFromUrl("data/geojson/BridleTrailsStatePark_Footpaths.geojson");

            //Alternatively you can create GeoJson objects in code. 

            //Features can be added to a data source using the Add or Insert methods.
            //However, if you have a large number of features to add, it is more efficient to use the AddRange or InsertRange method as this will trigger a single map update.
            //Similarly, if you want to replace the data in the datasource, use the ReplaceRange method. This will clear the data source and update the map in a single render pass.

            //Here is a simple linestring.
            var simpleLineString = new Feature(new LineString(new PositionCollection
                {
                    new Position(-122.18822, 47.63208),
                    new Position(-122.18204, 47.63196),
                    new Position(-122.17243, 47.62976),
                    new Position(-122.16419, 47.63023),
                    new Position(-122.15852, 47.62942),
                    new Position(-122.15183, 47.62988),
                    new Position(-122.14256, 47.63451),
                    new Position(-122.13483, 47.64041),
                    new Position(-122.13466, 47.64422),
                    new Position(-122.13844, 47.65440),
                    new Position(-122.13277, 47.66515),
                    new Position(-122.12779, 47.66712),
                    new Position(-122.11595, 47.66712),
                    new Position(-122.11063, 47.66735),
                    new Position(-122.10668, 47.67035),
                    new Position(-122.10565, 47.67498)
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "myColor", "red" }
            });
            dataSource.Add(simpleLineString);

            //MultiLineString represent multiple lines. This is good for rendering collection lines that may not be connected but are related in some way.
            //For example: All paths in a park (network graph), or a collection of roads.
            var multiLineString = new Feature(new MultiLineString(new List<PositionCollection>
                {
                    new PositionCollection { new Position(-122.14658,47.65840),new Position(-122.14868,47.65863),new Position(-122.14905,47.66000),new Position(-122.15013,47.66029) },
                    new PositionCollection { new Position(-122.15189, 47.65971), new Position(-122.15008, 47.65954), new Position(-122.14953, 47.65854) },
                    new PositionCollection { new Position(-122.15240, 47.65853), new Position(-122.15077, 47.65821) }
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "myColor", "Purple" }
            });
            dataSource.Add(multiLineString);
        }

        #endregion
    }
}
