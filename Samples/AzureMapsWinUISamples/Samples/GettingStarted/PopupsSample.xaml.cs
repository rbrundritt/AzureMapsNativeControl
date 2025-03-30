using AzureMapsNativeControl;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class PopupsSample : Microsoft.UI.Xaml.Controls.Page
    {
        /*********************************************************************************************************
        * This sample shows how to use popups and popup templates.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=popups-on-shapes
        * https://samples.azuremaps.com/?sample=reusing-popup-with-multiple-pins
        * https://samples.azuremaps.com/?sample=show-popup-on-hover
        * https://samples.azuremaps.com/?sample=customize-a-popup
        * https://samples.azuremaps.com/?sample=popup-with-media-content
        * https://samples.azuremaps.com/?sample=popup-templates
        *********************************************************************************************************/

        #region Private Properties

        private DataSource dataSource;
        private DataSource dataSource2;
        private Popup popup;
        private Dictionary<string, PopupTemplate> popupTemplates;

        #endregion

        #region Constructor

        public PopupsSample()
        { 
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// Event handler for when the map is ready.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //Create a data source and add it to the map.
            dataSource = new DataSource();
            MyMap.Sources.Add(dataSource);

            //Load some sample data.
            dataSource.ImportDataFromUrl("data/geojson/MtStHelensNorthTrails.json");

            //Create a second data source for one of the other scenarios.
            dataSource2 = new DataSource();
            MyMap.Sources.Add(dataSource2);

            //Set the initial scenario for the sample.
            PopupScenarioPicker.SelectedIndex = 0;
        }

        /// <summary>
        /// Scenario selection handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PopupScenarioPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //Remove existing popups and layers.
            MyMap.Popups.Clear();
            await MyMap.Layers.ClearAsync();
            dataSource2.Clear();

            //Remove any map events.
            MyMap.Events.Remove("mousemove", MapMouse_Moved);

            BoundingBox scenarioViewArea = new BoundingBox(-122.29113, 46.27582, -122.16417, 46.32537);

            string scenarioName = Helpers.GetSelectedPickerString(sender);

            switch (scenarioName)
            {
                case "Popups with multiple shapes":
                    PopupsOnMultipleShapes();
                    break;
                case "Show popup on hover":
                    ShowPopupOnHover();
                    break;
                case "Custom popup style":
                    CustomPopupStyle();
                    break;
                case "Popup with media content":
                    PopupWithMediaContent();
                    break;
                case "Popup templates":
                    PopupTemplates();

                    scenarioViewArea = new BoundingBox(-26.83858, -29.42513, 50.53342, 29.45034);
                    break;
                default:
                    break;
            }

            //Update the map's camera to the viewable area.
            MyMap.SetCamera(new CameraOptions
            {
                Bounds = scenarioViewArea
            });
        }

        #region Popup with Multiple Shape Scenario

        /// <summary>
        /// Shows how to open a popup when a shape is clicked, and how to position it depending on the shape type.
        /// 
        /// Based on: 
        /// https://samples.azuremaps.com/?sample=popups-on-shapes
        /// https://samples.azuremaps.com/?sample=reusing-popup-with-multiple-pins
        /// </summary>
        private void PopupsOnMultipleShapes()
        {
            //Create a popup that we can reuse.
            popup = new Popup();

            var layers = AddSampleLayers();

            //Add click events to the layers.
            foreach (var l in layers)
            {
                if (l is IMapEventTarget)
                {
                    MyMap.Events.Add("click", (IMapEventTarget)l, FeatureLayer_Clicked);
                }
            }
        }

        /// <summary>
        /// Event handler for when a feature is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeatureLayer_Clicked(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                //Get the first feature.
                var f = args.Shapes[0];

                //If the shape is a Point, use its coordinates to position the popup, otherwise use the mouse location.
                var pos = (f.Geometry is PointGeometry pg) ? pg.Coordinates : args.Position;

                //Create some content for the popup from the properties of the feature.
                var description = f.Properties.ContainsKey("description") ? f.Properties["description"] : "";
                string htmlContent = $"<div style=\"padding:10px;max-width:350px;white-space:normal;\"><b>{f.Properties["title"]}</b><br/>{description}</div>";

                //Update the content and position of the popup.
                popup.SetOptions(new PopupOptions
                {
                    Content = htmlContent,
                    Position = pos
                });

                //Open the popup.
                popup.Open(MyMap); //Alternatively you can add the popup to the map using MyMap.Popups.Add(popup); then call popup.Open() to open it.
            }
        }

        #endregion

        #region Show popup on hover Scenario

        /// <summary>
        /// Shows how to display a popup when hovering over a shape.
        /// 
        /// Based on: https://samples.azuremaps.com/?sample=show-popup-on-hover
        /// </summary>
        private void ShowPopupOnHover()
        {
            //Create a popup that we can reuse.
            popup = new Popup(new PopupOptions
            {
                PixelOffset = new Pixel(0, -5)
            });

            var layers = AddSampleLayers();

            //When using feature layers, it's possible that two features may partially overlap, and thus the mouse enter and leave events would not fire.
            //Instead we use mouse move events to detect which feature the mouse it over. We can also use a mouse even on the map to close the popup.


            //Close the popup when the mouse moves on the map.
            MyMap.Events.Add("mousemove", MapMouse_Moved);

            //Add mouse move and touch start events to the layers.
            foreach (var l in layers)
            {
                if (l is IMapEventTarget target)
                {
                    MyMap.Events.Add("mousemove", target, FeatureHovered);

                    //Use touch events for mobile devices, where a hover user experience would be to display the popup while touching, and hide it when not.
                    MyMap.Events.Add("touchstart", target, FeatureHovered);
                    MyMap.Events.Add("touchend", target, MapMouse_Moved);
                }
            }
        }

        /// <summary>
        /// Event handler for when a feature is hovered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeatureHovered(object? sender, MapEventArgs e)
        {
            //We can update a popup similar to ehe first scenario, but since we are hovering, we don't need the close button displayed.
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                //Get the first feature.
                var f = args.Shapes[0];

                //If the shape is a Point, use its coordinates to position the popup, otherwise use the mouse location.
                var pos = (f.Geometry is PointGeometry pg) ? pg.Coordinates : args.Position;

                //Create some content for the popup from the properties of the feature.
                var description = f.Properties.ContainsKey("description") ? f.Properties["description"] : "";
                string htmlContent = $"<div style=\"padding:10px;max-width:350px;white-space:normal;\"><b>{f.Properties["title"]}</b><br/>{description}</div>";

                //Update the content and position of the popup.
                popup.SetOptions(new PopupOptions
                {
                    Content = htmlContent,
                    Position = pos,
                    ShowCloseButton = false
                });

                //Open the popup.
                popup.Open(MyMap); //Alternatively you can add the popup to the map using MyMap.Popups.Add(popup); then call popup.Open() to open it.
            }
        }

        /// <summary>
        /// Event handler for when the mouse moves on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapMouse_Moved(object? sender, MapEventArgs e)
        {
            //Close the popup.
            popup.Close();
        }

        #endregion

        #region Custom popup scenario

        /// <summary>
        /// This shows how to customize the look of a popup. You can change it's fill color, hide the 
        /// close button and/or pointer and add custom HTML as its content.
        /// 
        /// Based on: https://samples.azuremaps.com/?sample=customize-a-popup
        /// </summary>
        private void CustomPopupStyle()
        {
            //Create a popup with a a media element in the HTML.
            var popup = new Popup(new PopupOptions
            {
                //Add HTML that embeds a video. In this case an iframe is being used, but a a "video" HTML element could be used and even point to a local video in the app.
                Content = "<div style=\"padding:10px;color:white\">Drag me</div>",
                Position = new Position(-122.22694, 46.30031),
                FillColor = "rgba(0,0,0,0.8)",
                ShowCloseButton = false,
                Draggable = true
            });

            popup.Open(MyMap);

            MyMap.Events.Add("drag", popup, (s, e) =>
            {
                if (e is MapMouseEventArgs args)
                    popup.SetOptions(new PopupOptions
                    {
                        Content = $"<div style=\"padding:10px;color:white\">Position: {args.Position.ToString(6)}</div>"
                    });
            });
        }

        #endregion

        #region Popup with media content

        /// <summary>
        /// Shows how media content can be added to a popup.
        /// 
        /// Based on: https://samples.azuremaps.com/?sample=popup-with-media-content
        /// </summary>
        private void PopupWithMediaContent()
        {
            //Create a popup with a a media element in the HTML.
            var popup = new Popup(new PopupOptions
            {
                //Add HTML that embeds a video. In this case an iframe is being used, but a a "video" HTML element could be used and even point to a local video in the app.
                Content = "<iframe style=\"margin:10px\" width=\"560\" height=\"315\" src=\"https://channel9.msdn.com/Shows/Internet-of-Things-Show/Azure-Maps-intro-for-developers/player\" frameborder=\"0\" allow=\"autoplay; encrypted-media\" allowfullscreen></iframe>",
                Position = new Position(-122.22957, 46.31486)
            });

            popup.Open(MyMap);
        }

        #endregion

        #region Popup templates scenario

        /// <summary>
        /// Popup templates provide an easy way to create data driven content for a popup based on a features properties.
        /// This is particularly useful when viewing multiple data sets that have different sets of properties.
        /// 
        /// Based on: https://samples.azuremaps.com/?sample=popup-templates
        /// </summary>
        private void PopupTemplates()
        {
            //For this scenario we will store custom popup templates for features in a lookup table (dictionary). 
            //We don't want to add the popup template in the properties table of the feature as we will end up will a circular reference when doing JSON serialization.
            popupTemplates = new Dictionary<string, PopupTemplate>();

            //Add sample data.

            //Some features that will use the default popup template.
            var features = new List<Feature>() {
                //Doesn't specify a template, will use the default template.
                new Feature(new PointGeometry(-20, 20), new PropertiesTable {
                    { "title", "No template - title/description" },
                    { "description", "This point doesn't have a template defined, fallback to title and description properties." }
                }),

                //Doesn't specify a template, will use the default template.
                new Feature(new PointGeometry(20, 20), new PropertiesTable {
                    { "title",  "No template - property table" },
                    { "message", "This point doesn't have a template defined, fallback to title and table of properties." },
                    { "randomValue", 10 },
                    { "url", "https://samples.azuremaps.com" },

                     //A link to an image in the Raw/map_resources/images folder. The map see Raw/map_resources as its root relative host.
                    { "imageLink", "/images/Pike_Market.jpg" },
                    { "email", "info@microsoft.com" }
                })
            };

            //Some features that have custom popup templates.
            var f = new Feature(new PointGeometry(40, 0), new PropertiesTable {
                { "title",  "No template - hyperlink detection disabled" },
                { "message", "This point doesn't have a template defined, fallback to title and table of properties." },
                { "randomValue", 10 },
                { "url", "https://samples.azuremaps.com" },
                { "email", "info@microsoft.com" }
            });

            //Add the feature to our collection.
            features.Add(f);

            //Add a simple popup template that doesn't make hyperlinks clickable.
            popupTemplates.Add(f.Id, new PopupTemplate() { DetectHyperlinks = false });

            //Add feature that uses a simple popup template that uses a string template with placeholders.
            f = new Feature(new PointGeometry(-20, -20), new PropertiesTable {
                { "title",  "Template 1 - String template" },
                { "value1", 1.2345678 },
                { "value2", new Dictionary<string, object> {
                    { "subValue", "Pizza" }
                }},
                { "arrayValue", new int[] { 3, 4, 5, 6 } }
            });

            features.Add(f);
            popupTemplates.Add(f.Id, new PopupTemplate()
            {
                Content = [new PopupStringContent("This template uses a string template with placeholders.<br/><br/> - Value 1 = {value1}<br/> - Value 2 = {value2/subValue}<br/> - Array value [2] = {arrayValue/2}")],
                NumberFormat = new JSNumberFormatOptions
                {
                    MaximumFractionDigits = 2
                }
            });

            //A feature with a popup template that specifies which properties to display, in which order, and how to format them for display.
            f = new Feature(new PointGeometry(20, -20), new PropertiesTable {
                { "title",  "Template 2 - PropertyInfo" },

                //A .NET date object. The JsonSerializer will convert this to a ISO 8601-1:2019 formatted date string.
                //https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support
                { "createDate", DateTime.Now },     
                
                //A JSON date number. Note that the JSDateTimeFormatOptions class has "ToJSDateNumber" and "FromJSDateNumber" helper methods.
                { "dateNumber", 1569880860542 },
                { "url", "https://samples.azuremaps.com" },
                { "email", "info@microsoft.com" }
            });

            features.Add(f);
            popupTemplates.Add(f.Id, new PopupTemplate()
            {
                Content = [
                    new PopupPropertyInfoContent {
                    //Display the createDate property with the label "Created Date".
                    new PopupPropertyInfo {
                        PropertyPath = "createDate",
                        Label = "Created Date"
                    },

                    //Display the dateNumber property with the label "Formatted date from number" and format it as a date.
                    new PopupPropertyInfo {
                        PropertyPath = "dateNumber",
                        Label = "Formatted date from number",
                        DateFormat = new JSDateTimeFormatOptions
                        {
                            Weekday = "long",
                            Year = "numeric",
                            Month = "long",
                            Day = "numeric",
                            TimeZone = "UTC",
                            TimeZoneName = "short"
                        }
                    },

                    //Display the url property with the label "Code samples" and make it a clickable hyperlink.
                    new PopupPropertyInfo {
                        PropertyPath = "url",
                        Label = "Code samples",
                        HideLabel = true,
                        HyperlinkFormat = new HyperLinkFormatOptions
                        {
                            Label = "Go to code samples!"
                        }
                    },

                    //Display the email property with the label "Email us" and make it a clickable email link.
                    new PopupPropertyInfo {
                        PropertyPath = "email",
                        Label = "Email us",
                        HideLabel = true,
                        HyperlinkFormat = new HyperLinkFormatOptions
                        {
                            Scheme = "mailto:"
                        }
                    }
                }
                ]
            });

            //A feature with a popup template that has multiple pieces of content.
            f = new Feature(new PointGeometry(0, 0), new PropertiesTable {
                { "title",  "Template 3 - Multiple content template"},
                { "value1", 1.2345678 },
                { "value2", new Dictionary<string, object> {
                    { "subValue", "Pizza" }
                }},
                { "arrayValue", new int[] { 3, 4, 5, 6 } },

                //A link to an image in the Raw/map_resources/images folder. The map see Raw/map_resources as its root relative host.
                { "imageLink", "/images/Pike_Market.jpg"}
            });
            features.Add(f);
            popupTemplates.Add(f.Id, new PopupTemplate()
            {
                Content = [
                    new PopupStringContent("This template has two pieces of content; a string template with placeholders and an array of property info which renders a full width image.<br/><br/> - Value 1 = {value1}<br/> - Value 2 = {value2/subValue}<br/> - Array value [2] = {arrayValue/2}"),
                        new PopupPropertyInfoContent {
                            new PopupPropertyInfo {
                                PropertyPath = "imageLink",
                                Label = "Image",
                                HideLabel = true,
                                HyperlinkFormat  = new HyperLinkFormatOptions {
                                    IsImage = true
                                }
                            }
                        }
                    ],

                //Global number foramt option used for all numbers in the popup.
                NumberFormat = new JSNumberFormatOptions
                {
                    MaximumFractionDigits = 2
                }
            });

            //Add the features to the data source.
            dataSource2.AddRange(features);

            //Create a layer for the data source.
            var layer = new BubbleLayer(dataSource2);

            //Add the layer to the map.
            MyMap.Layers.Add(layer);

            //Create a popup that we can reuse.
            popup = new Popup();

            //Add a click event to the layer.
            MyMap.Events.Add("click", layer, PopupTemplateLayer_Clicked);
        }

        /// <summary>
        /// Event handler for when a feature in the popup template scenario is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupTemplateLayer_Clicked(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                //Get the first feature.
                var f = args.Shapes[0];

                //If the shape is a Point, use its coordinates to position the popup, otherwise use the mouse location.
                var pos = (f.Geometry is PointGeometry pg) ? pg.Coordinates : args.Position;

                //Try and get the popup template for the feature from the lookup table.
                PopupTemplate? popupTemplate = popupTemplates.ContainsKey(f.Id) ? popupTemplates[f.Id] : null;

                //Check to see if a popup template was found.
                if (popupTemplate != null)
                {
                    //Set the properties of the feature onto the popup template.
                    popupTemplate.Properties = f.Properties;
                }
                else
                {
                    //Create a default popup template and pass in the properties of the feature.
                    popupTemplate = new PopupTemplate(f.Properties);
                }

                //Update the content and position of the popup.
                popup.SetOptions(new PopupOptions
                {
                    //Instead of setting the "Content" property, set the "PopupTemplate" property.
                    PopupTemplate = popupTemplate,
                    Position = pos
                });

                //Open the popup.
                popup.Open(MyMap); //Alternatively you can add the popup to the map using MyMap.Popups.Add(popup); then call popup.Open() to open it.
            }
        }

        #endregion

        #region Helper methods for the sample

        /// <summary>
        /// Adds layers for displaying points, lines, and polygons.
        /// Used by some of the scenarios.
        /// </summary>
        /// <returns></returns>
        private List<BaseLayer> AddSampleLayers()
        {
            //Add layers for points, lines, and polygons.
            var layers = new List<BaseLayer> {
                new PolygonLayer(dataSource, new PolygonLayerOptions
                {
                    Filter = AzureMapsNativeControl.Expression.PolygonTypeFilter()
                }),

                new LineLayer(dataSource, new LineLayerOptions
                {
                    //Only render lines in this layer.
                    Filter = AzureMapsNativeControl.Expression.LineStringTypeFilter()
                }),

                new BubbleLayer(dataSource, new BubbleLayerOptions
                {
                    //Only render points in this layer.
                    Filter = AzureMapsNativeControl.Expression.PointTypeFilter()
                })
            };

            //Add the layers to the map.
            MyMap.Layers.AddRange(layers);

            return layers;
        }

        #endregion
    }
}