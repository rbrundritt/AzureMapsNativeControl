using System.IO;
using System.Windows;
using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsWPFSamples.Samples
{
    public partial class SymbolLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the symbol layer.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=symbol-layer-options
        * https://samples.azuremaps.com/?sample=data-driven-symbol-icons
        * https://samples.azuremaps.com/?sample=symbol-layer-with-built-in-icon-template
        * https://samples.azuremaps.com/?sample=all-built-in-icon-templates-as-symbols
        * https://samples.azuremaps.com/?sample=html-marker-with-custom-svg-template
        *********************************************************************************************************/

        #region Private Properties

        private DataSource dataSource;
        private SymbolLayer symbolLayer;
        private Popup popup;

        #endregion

        #region Constructor

        public SymbolLayerSample()
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
            //Load custom icons into the map image sprite.
            await CreateIcons();

            //Create a popup and add it to the map.
            popup = new Popup();
            MyMap.Popups.Add(popup);

            //Create a data source and add it to the map.
            dataSource = new DataSource();
            MyMap.Sources.Add(dataSource);

            //Create a symbol layer and add it to the map.
            symbolLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    //For this sample, make all the icons appear always (by default this layer will hide symbols that collide with other symbols).
                    AllowOverlap = true,
                    IgnorePlacement = true
                },
                TextOptions = new TextOptions
                {
                    //For this sample use the "name" property of the point data as the text of the point.
                    TextField = new Expression<string>("get", "name"),
                    IgnorePlacement = true
                }
            });
            MyMap.Layers.Add(symbolLayer);

            //Add a click event to the layer to display a popup with information about the individual point.
            MyMap.Events.Add("click", symbolLayer, SymbolLayer_Click);

            //Add mouse events to change the mouse cursor when hovering over a cluster.
            MyMap.Events.Add("mouseenter", symbolLayer, (s, e) => MyMap.SetMouseCursor("pointer"));
            MyMap.Events.Add("mouseleave", symbolLayer, (s, e) => MyMap.SetMouseCursor("grab"));

            LoadData();

            //For this sample, set the initial picker options (there is a Maui bug where it doesn't display the initial value unless you set it in code).
            IconPicker.SelectedIndex = 0;
            TextColorPicker.SelectedIndex = 0;
        }

        /// <summary>
        /// Event handler for when a symbol is clicked. Display a popup with information about the point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SymbolLayer_Click(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes != null && args.Shapes.Count > 0)
            {
                //Get the top most feature that was clicked.
                var feature = args.Shapes[0];

                //Get the position of the feature.
                var position = (feature.Geometry as PointGeometry)?.Coordinates ?? args.Position; //If the feature isn't a PointGeometry, will default to the mouse position.

                //Get the properties of the feature.
                var properties = feature.Properties;

                //Create a popup with the information.
                popup.SetOptions(new PopupOptions()
                {
                    Position = position,
                    PopupTemplate = new PopupTemplate(properties),
                    PixelOffset = new Pixel(0, -20)
                });

                //Open the popup.
                popup.Open();
            }
        }

        /// <summary>
        /// Event handler for when the icon picker value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            string imageName = Helpers.GetSelectedPickerString(sender);

            //For this sample, we will remove "built_in_" to get the actual name of the built-in icon.
            if (imageName.StartsWith("built_in_"))
            {
                imageName = imageName.Replace("built_in_", "");
            }

            //Set the icon image of the points. For images created by templates, this would be the custom name of the icon.
            //We use a literal expression to pass a single value that will be used by all data in the layer.
            var exp = Expression<string>.Literal(imageName);

            //NOTE: "none" is a valid value for the icon image option. It will hide the icon image from the symbol.

            //Check to see if this is set to undefined. This would mean reverting to the default icon. 
            if (imageName.Equals("undefined"))
            {
                exp = Expression<string>.Undefined(); //Use an undefined expression to remove a previously set icon and revert to the default.
            }
            else if (imageName.Equals("data_driven_style"))
            {
                //Instead of using a single image pattern, use property information on the features to specify how to style them.
                exp = new Expression<string>() {
                    "case",

                    new object[] {"in", "High", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "High" in it.
                    "marker-red", //Use a red marker if it does.

                    new object[] {"in", "Middle", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "Middle" in it.
                    "marker-yellow", //Use a yellow marker if it does.

                    new object[] {"in", "Elementary", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "Elementary" in it.
                    "marker-blue", //Use a blue marker if it does.

                    //Default fallback icon.
                    "marker-black"
                };
            }

            //Update the icon image option of the layer.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    Image = exp
                }
            });
        }

        /// <summary>
        /// Event handler for when the icon color picker value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconOpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var opacity = (double)Math.Round(slider.Value, 2);

            //Set the icon opacity of the points.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    Opacity = Expression<double>.Literal(opacity)
                }
            });

            IconOpacityLabel.Text = $"Icon Opacity: {opacity}";
        }

        /// <summary>
        /// Event handler for when the icon size slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconSizeSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var size = (double)Math.Round(slider.Value, 2);

            //Set the icon size of the points.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    Size = Expression<double>.Literal(size)
                }
            });

            IconSizeLabel.Text = $"Icon size: {size}";
        }

        /// <summary>
        /// Event handler for when the icon rotation slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconRotationSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var rotation = Math.Round(slider.Value);

            //Set the rotation of the icon image.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    Rotation = Expression<double>.Literal(rotation)
                }
            });

            IconRotationLabel.Text = $"Icon rotation: {rotation}";
        }

        /// <summary>
        /// Event handler for when the show text checkbox value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTextCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            var checkbox = (CheckBox)sender;

            //When checked, get the "name" property of the point data and use it as the text of the point.
            var textFieldExp = new Expression<string>("get", "name");

            if (checkbox.IsChecked == null || !checkbox.IsChecked.Value)
            {
                textFieldExp = Expression<string>.Undefined(); //Use an undefined expression to remove the text.
            }

            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                TextOptions = new TextOptions
                {
                    TextField = textFieldExp
                }
            });
        }

        /// <summary>
        /// Event handler for when the text color picker value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextColorPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            string color = Helpers.GetSelectedPickerString(sender);

            //Initially use the color as a literal expression.
            var colorExp = Expression<string>.Literal(color);

            //Handle special cases in this sample.
            if (color.Equals("undefined"))
            {
                colorExp = Expression<string>.Undefined(); //Use an undefined expression to revert to the default.
            }
            else if (color.Equals("data_driven_style"))
            {
                //Any CSS color string can be used.
                //TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.

                colorExp = new Expression<string>() {
                    "case",

                    new object[] {"in", "High", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "High" in it.
                    "red", //Use red if it does.

                    new object[] {"in", "Middle", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "Middle" in it.
                    "orange", //Use orange if it does.

                    new object[] {"in", "Elementary", new object[] { "get", "name" } }, //Chck to see if the "name" property has the word "Elementary" in it.
                    "blue", //Use blue if it does.

                    //Default fallback color.
                    "black"
                };
            }

            //Set the text color of the points.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                TextOptions = new TextOptions
                {
                    Color = colorExp
                }
            });
        }

        /// <summary>
        /// Event handler for when the text size slider value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextSizeSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (symbolLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var size = (int)Math.Round(slider.Value);

            //Set the text size of the points.
            symbolLayer.SetOptions(new SymbolLayerOptions
            {
                TextOptions = new TextOptions
                {
                    Size = Expression<int>.Literal(size)
                }
            });

            TextSizeLabel.Text = $"Text size: {size}";
        }

        /// <summary>
        /// Loads some point data into the data source.
        /// </summary>
        private void LoadData()
        {
            //The following shows different ways to add point data into a data source.
            //Note that bubble and symbol layers can also be used with vector tiles.

            //Load point data from a geojson file. In this case a GeoJson file is being retrieved from the "Raw/map_resources" folder of the app.
            dataSource.ImportDataFromUrl("data/geojson/fort_collins_schools.json");

            //Alternatively you can create GeoJson objects in code. 

            //Features can be added to a data source using the Add or Insert methods.
            //However, if you have a large number of features to add, it is more efficient to use the AddRange or InsertRange
            //method as this will trigger a single map update.

            //Similarly, if you want to replace the data in the datasource, use the ReplaceRange method. This will clear the data
            //source and update the map in a single render pass.

            //Here is a simple point feature. Note that compared to the Web SDK the "Point" class has been renamed to
            //"PointGeoemtry" in this library to reduce confusion with the "Point" class in the .NET framework.
            var simplePoint = new Feature(new PointGeometry(new Position(-105.105732, 40.693755)),
                new PropertiesTable() //Add some custom properties.
                {
                    { "name", "I'm a simple point feature." }
                });
            dataSource.Add(simplePoint);

            //You can pass in raw geometries that aren't wrapped as features into the data source.
            //The data source will wrap these as Features but they will have no properties.

            //A MultiPoint represents a colleciton of points. For example, the location of all sprinkler heads on a property.
            var multiPoint = new Feature(new MultiPoint(new PositionCollection {
                    new Position(-105.203440, 40.575475), new Position(-105.188499, 40.567112), new Position(-105.192824, 40.599361)
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "name", "I'm part of a MultiPoint feature." }
                });
            dataSource.Add(multiPoint);
        }

        /// <summary>
        /// Logic for adding custom images to the map image sprite.
        /// Once images are added to the map, they can be used as symbol icons, fill patterns.
        /// </summary>
        /// <returns></returns>
        private async Task CreateIcons()
        {
            //The map image sprite is a container that holds images that can be used as icons in the map.
            //By default, the map image sprite contains a number of built-in icons that can be used as symbols in the map.
            //You simply need to specify the name of these icons and don't need to load them. These built-in icons are:
            // - marker-black
            // - marker-blue
            // - marker-darkblue
            // - marker-red
            // - marker-yellow
            // - pin-blue
            // - pin-darkblue
            // - pin-red
            // - pin-round-blue
            // - pin-round-darkblue
            // - pin-round-red

            //Custom icons can be added to the map sprite. The following image formats are supported:
            // - PNG
            // - JPG
            // - SVG
            // - WEBP
            // - BMP
            // - GIF (no animations)

            //Create a custom template from an inline SVG and add it to the map.
            await MyMap.ImageSprite.AddImageTemplateAsync("my_custom_icon_template", "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"30\" height=\"37\" viewBox=\"0 0 30 37\" xml:space=\"preserve\"><rect x=\"0\" y=\"0\" rx=\"8\" ry=\"8\" width=\"30\" height=\"30\" fill=\"{color}\"/><polygon fill=\"{color}\" points=\"10,29 20,29 15,37 10,29\"/><text x=\"15\" y=\"20\" style=\"font-size:16px;font-family:arial;fill:#ffffff;\" text-anchor=\"middle\">{text}</text></svg>");

            //TIP: You can load multiple images into the map asynchronously for faster loading.
            //But make sure to add custom image templates before trying to create images from them. 
            await Task.WhenAll(new List<Task>() { 

                //Create icon from built-in icon templates.
                //Input is: custom icon name, template name, icon color, secondary icon color, scale. The last three are optional.
                MyMap.ImageSprite.CreateFromTemplateAsync("template_marker_arrow", "marker-arrow", "teal", "white"),

                //The tree built in icons are also available as templates, thus allowing you to add additional color options.
                MyMap.ImageSprite.CreateFromTemplateAsync("template_built_in_marker_purple", "marker", "purple", "white"),

                //Create icon from custom icon template.
                MyMap.ImageSprite.CreateFromTemplateAsync("custom_template", "my_custom_icon_template", "red", "white"),

                //Create a icon from an image in raw/map_resources folder.
                MyMap.ImageSprite.AddImageFromUrl("png_asset_url", "images/coffee_icon.png"),

                //Create a icon from SVG file in raw/map_resources folder.
                MyMap.ImageSprite.AddImageFromUrl("svg_asset_url", "images/red_svg_marker.svg"),

                //Create icon from inline SVG string.
                MyMap.ImageSprite.AddImageFromUrl("inline_svg_string", "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"32px\" height=\"32px\" viewBox=\"0 0 16 16\"><path stroke=\"#F00\" stroke-width=\"2\" d=\"m8,2.5 6,12H2z\"/></svg>"),

                //Create icon from an image data URI.
                MyMap.ImageSprite.AddImageFromUrl("image_data_uri", "data:image/png;base64,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"),

                //Load image from web.
                MyMap.ImageSprite.AddImageFromUrl("image_from_web", "https://upload.wikimedia.org/wikipedia/commons/d/d0/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Bomber_%E2%80%93_Industry_%E2%80%93_Classic.png"),

                //Load image from web that doesn't have CORs enabled. Image will be loaded via a proxy in native code.
                MyMap.ImageSprite.AddImageFromUrl("image_from_web_no_cors", "https://samples.azuremaps.com/images/icons/WiFiIcon.png")
            });

            //Load an image from a stream. This may be useful if you use a drawing library like SkiaSharp to generate images.
            using (var s = new FileInfo("map_resources/images/pin.png").OpenRead())
            {
                await MyMap.ImageSprite.AddImageFromStreamAsync("image_from_stream", s, "image/png");
            }
        }

        #endregion
    }
}
