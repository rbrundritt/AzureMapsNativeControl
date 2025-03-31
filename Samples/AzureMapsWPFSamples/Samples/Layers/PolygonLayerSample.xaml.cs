using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    public partial class PolygonLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways to display polygons.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=polygon-fill-pattern
        * https://samples.azuremaps.com/?sample=fill-polygon-with-built-in-icon-template
        * https://samples.azuremaps.com/?sample=polygon-masks
        * https://samples.azuremaps.com/?sample=add-custom-icon-template-to-atlas-namespace
        *********************************************************************************************************/

        #region Private Properties

        private DataSource dataSource;

        private PolygonLayer polygonLayer;
        private LineLayer polygonOutlineLayer;
        private BubbleLayer polygonPointLayer;

        #endregion

        #region Constructor 

        public PolygonLayerSample()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event that is triggered when the map is ready to be used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Add images for fill pattern to the maps image sprite.
            await CreateFillPatterns();

            //Create a data source and add it to the map.
            dataSource = new DataSource();
            MyMap.Sources.Add(dataSource);

            //Add a layer for rendering the polygons. A polygon layer only renders the fill of the polygons.
            polygonLayer = new PolygonLayer(dataSource, new PolygonLayerOptions
            {
                //TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
                FillColor = Expression<string>.Literal("#1e90ff")
            });

            //Polygon data can also be rendered as lines (it's edges) and points (it's individual coordinates).

            //Use a line layer to render the outline of polygons.
            polygonOutlineLayer = new LineLayer(dataSource, new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("black")
            });

            //Use a bubble or symbol layer to render the points of the polygons.
            polygonPointLayer = new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                Visible = false
            });

            /*
             * TIP
             * If you have a data source with a mix of geometries, you may not want polygons to be rendered by all layers.
             * To prevent this, add a filter to the layer options that limits the geometry types rendered by that layer. 
             * The Expression class has static methods for these to simplify the code.
             * 
             * Expression.PointTypeFilter()
             * Expression.LineStringTypeFilter()
             * Expression.PolygonTypeFilter()
             */

            //Add the layers to the map. 
            MyMap.Layers.AddRange([polygonLayer, polygonOutlineLayer, polygonPointLayer]);
            //MyMap.Layers.AddRange([polygonLayer, polygonOutlineLayer], "labels"); //Typically you will want polygons and lines below the "labels" or "roads" layer.

            //Load some data for the sample.
            LoadData();
        }

        /// <summary>
        /// Changes the fill color of the polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillColorPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonLayer == null)
            {
                return;
            }

            //Reset fill pattern picker.
            FillPatternPicker.SelectedIndex = 0;

            //Any CSS color value can be used as the fill color: https://www.w3schools.com/cssref/css_colors_legal.php

            string cssFillColor = Helpers.GetSelectedPickerString(sender);

            Expression<string> colorExp;

            if (cssFillColor.Equals("data_driven_style"))
            {
                //Instead of using a single color, use property information on the features to specify how to style them.
                colorExp = new Expression<string>() {
                    "case",
                    new object[] { "has", "myColor" }, //Check if the feature has a property called "myColor".
                    new object[] { "get", "myColor" }, //If it does, use the value of the "myColor" property.
                    "#1e90ff" //If it doesn't, use a default color.
                };
            }
            else
            {
                //Use a CSS color string as the fill color. 
                colorExp = Expression<string>.Literal(cssFillColor);

                //TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.
            }

            polygonLayer.SetOptions(new PolygonLayerOptions
            {
                FillColor = colorExp
            });
        }

        /// <summary>
        /// Changes the fill pattern of the polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillPatternPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (polygonLayer == null)
            {
                return;
            }

            //NOTE: Fill patterns are a great way to make your maps more visually accessible.

            string patternImageName = Helpers.GetSelectedPickerString(sender);

            //Set the fill pattern of the polygons.
            var exp = Expression<string>.Literal(patternImageName);

            if (patternImageName.Equals("undefined"))
            {
                exp = Expression<string>.Undefined(); //Use an undefined expression to remove the fill pattern.
            }
            else if (patternImageName.Equals("data_driven_style"))
            {
                //Instead of using a single image pattern, use property information on the features to specify how to style them.
                exp = new Expression<string>() {
                    "case",
                    new object[] { "has", "myPattern" }, //Check if the feature has a property called "myPattern".
                    new object[] { "get", "myPattern" }, //If it does, use the value of the "myPattern" property.
                    "image_data_uri" //If it doesn't, use a default image pattern name.
                };
            }

            polygonLayer.SetOptions(new PolygonLayerOptions
            {
                FillPattern = exp
            });
        }

        /// <summary>
        /// Changes the opacity of the fill of the polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillOpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if(polygonLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var opacity = (double)Math.Round(slider.Value, 2);

            //Set the fill opacity of the polygons.
            polygonLayer.SetOptions(new PolygonLayerOptions
            {
                FillOpacity = Expression<double>.Literal(opacity)
            });

            OpacityLabel.Text = $"Fill Opacity: {opacity}";
        }

        /// <summary>
        /// Shows or hides the outline of the polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutlinePolygonCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (polygonLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            //Show or hide the outline of the polygons.
            polygonOutlineLayer.SetOptions(new LayerOptions
            {
                Visible = checkBox.IsChecked
            });
        }

        /// <summary>
        /// Shows or hides the points of the polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPolygonPointsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (polygonLayer == null)
            {
                return;
            }

            var checkBox = (CheckBox)sender;

            //Show or hide the points of the polygons.
            polygonPointLayer.SetOptions(new LayerOptions
            {
                Visible = checkBox.IsChecked
            });
        }

        /// <summary>
        /// Moves the polygon layer before the selected layer within the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeLayerPicker_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (polygonLayer == null)
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
            MyMap.Layers.Move(polygonLayer, beforeLayerId);

            //Keep the outline and point layers above all others for this sample.
            MyMap.Layers.Move(polygonOutlineLayer);
            MyMap.Layers.Move(polygonPointLayer);
        }

        /// <summary>
        /// Loads some polygon data into the data source.
        /// </summary>
        private void LoadData()
        {
            //The following shows different ways to add polygon data into a data source.
            //Note that polygon layers can also be used with vector tiles.

            //Load polygon data from a geojson file. In this case a GeoJson file is being retrieved from the "Raw/map_resources" folder of the app.
            dataSource.ImportDataFromUrl("data/geojson/south_africa_mainland.json");

            //Alternatively you can create GeoJson objects in code. 

            //Features can be added to a data source using the Add or Insert methods.
            //However, if you have a large number of features to add, it is more efficient to use the AddRange or InsertRange method as this will trigger a single map update.
            //Similarly, if you want to replace the data in the datasource, use the ReplaceRange method. This will clear the data source and update the map in a single render pass.

            //Here is a simple polygon that only has an outer ring.
            var simplePolygon = new Feature(new Polygon(new PositionCollection
                {
                    new Position(-23,-13.2), new Position(-23,-19.7), new Position(-1.5,-19.7), new Position(-1.5,-13.2), new Position(-23,-13.2)
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "myColor", "#FF0000" },
                    { "myPattern", "template_teal_stripes" }
                });
            dataSource.Add(simplePolygon);

            //Here is a polygon that has both an outer ring and a hole cut out of it.
            var polygon = new Feature(new Polygon(new List<PositionCollection>
                {
                    new PositionCollection { new Position(-23.2,-25.6),new Position(-23.2,-33.1),new Position(-1.0,-33.1),new Position(-1.0,-25.6),new Position(-23.2,-25.6) },
                    new PositionCollection { new Position(-16.0,-28.0),new Position(-16.0,-31.1),new Position(-6.0,-31.1),new Position(-6.0,-28.0),new Position(-16.0,-28.0) }
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "myColor", "#0000FF" },
                    { "myPattern", "custom_template" }
                });
            dataSource.Add(polygon);

            //MultiPolygon represent multiple polygons. Each polygon has an outer ring and can have holes in them.
            var multiPolygon = new Feature(new MultiPolygon(new List<List<PositionCollection>>
                {
                    new () {
                        new PositionCollection { new Position(-47.90039, -14.94478), new Position(-51.59179, -19.91138), new Position(-41.11083, -21.30984), new Position(-43.39599, -15.39013), new Position(-47.90039, -14.94478) },
                        new PositionCollection { new Position(-46.62597, -17.14079), new Position(-47.54882, -16.80454), new Position(-46.23046, -16.69934), new Position(-45.35156, -19.31114), new Position(-46.62597, -17.14079) },
                        new PositionCollection { new Position(-44.40673, -18.37537), new Position(-43.52783, -17.60213), new Position(-42.93457, -18.97902), new Position(-44.42871, -20.09720), new Position(-44.40673, -18.37537) }
                    },
                    new () {
                        new PositionCollection { new Position(-40.58349, -18.33366), new Position(-38.36425, -17.99963), new Position(-38.49609, -11.22151), new Position(-42.56103, -14.75363), new Position(-40.58349, -18.33366) }
                    }
                }),
                new PropertiesTable() //Add some custom properties.
                {
                    { "myColor", "#00FF00" },
                    { "myPattern", "inline_svg_string" }
                });
            dataSource.Add(multiPolygon);
        }

        /// <summary>
        /// Logic for adding custom images to the map image sprite.
        /// Once images are added to the map, they can be used as fill patterns, or symbol icons.
        /// </summary>
        /// <returns></returns>
        private async Task CreateFillPatterns()
        {
            //Create a custom template from an inline SVG and add it to the map.
            await MyMap.ImageSprite.AddImageTemplateAsync("anchor-fill", "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 80 80\" width=\"calc(80px * {scale})\" height=\"calc(80px * {scale})\"><rect x=\"0\" y=\"0\" width=\"80\" height=\"80\" fill=\"{secondaryColor}\"/><path fill=\"{color}\" d=\"M14 16H9v-2h5V9.87a4 4 0 1 1 2 0V14h5v2h-5v15.95A10 10 0 0 0 23.66 27l-3.46-2 8.2-2.2-2.9 5a12 12 0 0 1-21 0l-2.89-5 8.2 2.2-3.47 2A10 10 0 0 0 14 31.95V16zm40 40h-5v-2h5v-4.13a4 4 0 1 1 2 0V54h5v2h-5v15.95A10 10 0 0 0 63.66 67l-3.47-2 8.2-2.2-2.88 5a12 12 0 0 1-21.02 0l-2.88-5 8.2 2.2-3.47 2A10 10 0 0 0 54 71.95V56zm-39 6a2 2 0 1 1 0-4 2 2 0 0 1 0 4zm40-40a2 2 0 1 1 0-4 2 2 0 0 1 0 4zM15 8a2 2 0 1 0 0-4 2 2 0 0 0 0 4zm40 40a2 2 0 1 0 0-4 2 2 0 0 0 0 4z\"></path></svg>");

            //TIP: You can load multiple images into the map asynchronously for faster loading.
            //But make sure to add custom image templates before trying to create images from them. 
            await Task.WhenAll(new List<Task>() { 

                //Create fill patterns from built-in icon templates.
                //Input is: custom icon name, template name, icon color, secondary icon color, scale. The last three are optional.
                MyMap.ImageSprite.CreateFromTemplateAsync("template_teal_stripes", "diagonal-stripes-down", "teal", "transparent"),
                MyMap.ImageSprite.CreateFromTemplateAsync("template_red_dots", "dots", "red", "transparent"),

                //Create fill pattern from custom icon template.
                MyMap.ImageSprite.CreateFromTemplateAsync("custom_template", "anchor-fill", "navy", "rgba(0,150,150,0.5)"),

                //Create a fill pattern from an image in raw/map_resources folder.
                MyMap.ImageSprite.AddImageFromUrl("png_asset_url", "images/dog_park_pattern.png"),

                //Create a fill pattern from SVG file in raw/map_resources folder.
                MyMap.ImageSprite.AddImageFromUrl("svg_asset_url", "images/orchard_banana_pattern.svg"),

                //Create fill pattern from inline SVG string.
                MyMap.ImageSprite.AddImageFromUrl("inline_svg_string", "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"15\" height=\"15\" viewBox=\"0 0 120 120\"><rect x=\"0\" y=\"0\" width=\"120\" height=\"120\" fill=\"transparent\"/><polygon fill=\"purple\" points=\"120 120 60 120 90 90 120 60 120 0 120 0 60 60 0 0 0 60 30 90 60 120 120 120\"/></svg>"),

                //Create fill pattern from an image data URI.
                MyMap.ImageSprite.AddImageFromUrl("image_data_uri", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAAAyCAMAAADbXS0mAAAAVFBMVEXR57utxZXF26+xyJirw5GowI/I3bHP5brO47jK4LW50KHC2Ky70qS3zp+/1qiju4m1zJ2zy5y60aOzy5qmvo22zZ+906XB16mhuYefuIXA1am70qIRKgrLAAADQUlEQVR42n2WBbrgOAyDVdSfx4z3v+dC4thfFzRspqgj2fobIJAkWR1IXQ0NkCwhQag73DW2Kjx+d3c0QwLpiYmsVgUDkgzMmGDJ3RprGE0L7Ono8ErfHtdpn2UjYTQVhBqk4dB/M9fIkdp9WuHMGA0OSVei7KGiu7s3EYKEc2iz1xrlam3r6kDoAPf60uQC2xLO65iKuvOMYE1b5GtVV4CtK2xcllUtwBoJwGExEX6u4weYK4nDnTczYzi66zrj7meySeXtJhKR9AJs/S/W/djKwUFZs4mriyskSs2ch3Xu/Z+tqdz16NNmOAClC9OD3awSC332lKeGoNFtwyvq8tz3eSxIeth3AAyUfEBuriOMItQe6e7uaQ3DZaz0xGUwHr9ZlnxE5M+bWE40U9dlYrzZjAHa40O3YbkvBweR7gJbgOdr9nH2iOCbMZUrHDwT/wLyZncBhdpyOr4MwkoYTG9kR2y6by/wUed+OUmE7XzPjhuiHfv+w5ufjmPrxYEQzMPGgGXL2FwKASPaY/Sx4qkKCxp5tyYOPLSSaV/uhd9+t9p5pf75CILnZ35BWzHgtr0/5SHj+Wro0rYcWwNixp7FjkpPv/gUIILLklQavK20I1uslY5/80hO2AiSxeD2CXzzGYLsMk/z6fU+FLmFUYWP4xb73OP1XDgfi5vb+4cHegWOJPE4X9dBr+sSM7h+ZNqtnrU2WyJ5JWa+xF7bgXRlFveOeS8BcT1qb89d6PulTd43AUE6uRxArOH5DgYXfWyFmsraFVRjSgwPctiL+I3CFqS1QTTqQ7aDSZZTuuPFN+oMDHGIHnkr/Y3qLc9e27LvN3rU3XHsLSdVudPRcMY3rYXWlvga/8oGHRwzFkcDEJr0BKwenyc/Xg8AnH8DcGsY4cKo/r4Nh6Z4ibl6mYJ8PMzCnp+2M7ZahgymTgHq2pUMvrYX34Jsu6fO7dtBycm8lq7YWyw6jONfPZ5zsCh7Nw0AHpb3K0vZWLjb2rJtM3xxNmlL593veVfcCdgZjjEcx9fRGgHxg151nqO8pHba5Tsc8XN9kw9eo6eUypWn64oMTJsInneVgG5dFx+7vPwXFHB+aHKNJis2MrKNLPEHwYMrdiL0jgIAAAAASUVORK5CYII="),

                //Load image from web.
                MyMap.ImageSprite.AddImageFromUrl("image_from_web", "https://upload.wikimedia.org/wikipedia/commons/9/9d/Development_test_%28Minetest%29_texture_testnodes_buildable_to.png"),

                //Load image from web that doesn't have CORs enabled. Image will be loaded via a proxy in native code.
                MyMap.ImageSprite.AddImageFromUrl("image_from_web_no_cors", "https://samples.azuremaps.com/images/icons/showers.png")
            });

            //Load an image from a stream. This may be useful if you use a drawing library like SkiaSharp to generate images.
            using (var s = new FileInfo("map_resources/images/rough_grass_texture.jpg").OpenRead())
            {
                await MyMap.ImageSprite.AddImageFromStreamAsync("image_from_stream", s, "image/jpg");
            }
        }

        #endregion
    }
}
