using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.Win32;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using Expression = AzureMapsNativeControl.Expression;

namespace AzureMapsWPFSamples.Samples
{
    public partial class DataSourceImportSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways data can be imported/added to a data source.
        * 
        * Layers in this sample are based on this Azure Maps Web SDK sample: 
        * https://samples.azuremaps.com/?sample=drag-and-drop-geojson-file-onto-map
        *********************************************************************************************************/

        private DataSource dataSource;
        private Popup popup;

        public DataSourceImportSample()
        {
            InitializeComponent();

            //Logic for supporting drag and drop of files onto the map.

            //IMPORTANT: Be sure to set the AllowFileDrop property in the map load settings to true.

            //Add/remove the drop event from the map when the page is loaded/unloaded.
            Loaded += (sender, args) =>
            {
                MyMap.OnFilesDropped += FileDroppedOnMap;
            };

            Unloaded += (sender, args) =>
            {
                MyMap.OnFilesDropped -= FileDroppedOnMap;
            };
        }

        private async void FileDroppedOnMap(object sender, MapFilesDroppedEventArgs e)
        {
            if (e.Files != null && e.Files.Count > 0)
            {
                //Clear the data source and close the popup.
                dataSource.Clear();
                popup.Close();

                //Load the files into the data source.
                foreach (var f in e.Files)
                {
                    //Only allow files that the data source supports to be imported.
                    if (DataSourceLite.IsSupportedFileType(f.Name, f.MimeType))
                    {
                        await dataSource.ImportDataFromStreamAsync(f.Stream);
                    }
                }

                UpdateCamera();
            }
        }

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Add a style control to the map.
            MyMap.Controls.Add(new StyleControl()
            {
                Position = ControlPosition.TopRight
            });

            //Create a data source and add it to the map.
            dataSource = new DataSource();
            MyMap.Sources.Add(dataSource);

            //Add a layer for rendering the polygons.
            var polygonLayer = new PolygonLayer(dataSource, new PolygonLayerOptions
            {
                FillColor = Expression<string>.Literal("#1e90ff"),
                Filter = Expression.PolygonTypeFilter() //Only render Polygon or MultiPolygon in this layer.
            });

            //Add a click event to the layer.
            MyMap.Events.Add("click", polygonLayer, Layer_Click);

            //Add a layer for rendering line data.
            var lineLayer = new LineLayer(dataSource, new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("#1e90ff"),
                StrokeWidth = Expression<int>.Literal(4),
                Filter = Expression.LineStringTypeFilter()  //Only render LineString or MultiLineString in this layer.
            });

            //Add a click event to the layer.
            MyMap.Events.Add("click", lineLayer, Layer_Click);

            //Add a layer for rendering point data.
            var pointLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    AllowOverlap = true,
                    IgnorePlacement = true
                },
                Filter = Expression.PointTypeFilter() //Only render Point or MultiPoints in this layer.
            });

            //Add a click event to the layer.
            MyMap.Events.Add("click", pointLayer, Layer_Click);

            //Add polygon and line layers to the map, below the labels..
            MyMap.Layers.AddRange(new List<BaseLayer>
            {
                polygonLayer,

                //Add a layer for rendering the outline of polygons.
                new LineLayer(dataSource, new LineLayerOptions {
                    StrokeColor = Expression<string>.Literal("black"),
                    Filter = Expression.PolygonTypeFilter()	//Only render Polygon or MultiPolygon in this layer.
                }),

                lineLayer

            }, "labels");

            //Add the point layer to the map.
            MyMap.Layers.Add(pointLayer);

            //Create a popup to display information about the data, and add it to the map.
            popup = new Popup();
            MyMap.Popups.Add(popup);
        }

        private void Layer_Click(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes != null && args.Shapes.Count > 0)
            {
                //By default, show the popup where the mouse event occurred.
                Position position = args.Position;
                Pixel offset = new Pixel(0, 0);

                var shape = args.Shapes[0];

                //Check to see if the shape is a point feature.
                if (shape.Geometry.Type == GeoJsonType.Point)
                {
                    //If the shape is a point feature, show the popup at the points coordinate.
                    position = ((PointGeometry)shape.Geometry).Coordinates;
                    offset = new Pixel(0, -18);
                }

                //Make sure there is some information to actually display.
                if (shape.Properties.Count > 0)
                {
                    popup.SetOptions(new PopupOptions
                    {
                        Position = position,
                        PixelOffset = offset,
                        PopupTemplate = new PopupTemplate(shape.Properties)
                    });
                }
                else
                {
                    popup.SetOptions(new PopupOptions
                    {
                        Position = position,
                        PixelOffset = offset,
                        //Convert content to base64 to avoid issues with special characters.
                        Content = "<div style=\"padding:10px;\">Shape has no properties.</div>"
                    });
                }

                popup.Open();
            }
        }

        /// <summary>
        /// Shows different way to create data in code and add it to the data source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportFeatureShape_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            /* A GeoJson feature is not updatable.
             * The DataSource class will create a new shape for each feature added to it,
             * and you can use the methods on the shape class to update the feature on the map.
            */

            //Create a GeoJSON feature in Code and add to the data source.
            dataSource.Add(new Feature(new PointGeometry(new Position(-122.316209, 47.602483)), new Dictionary<string, object?>
            {
                { "title", "I'm a Feature!" }
            }));

            //Create a GeoJson Geometry and add to the data source.
            //A geometry doesn't have any properties, just a shape. 
            dataSource.Add(new PointGeometry(new Position(-73.985340, 40.725526)));

            //Create a GeoJson Geometry of a Polygon.
            dataSource.Add(new Feature(new AzureMapsNativeControl.Data.Polygon(new PositionCollection[] {
                //A polygon uses an array of LineStrings for rings. The first LineString is the outer ring, and any subsequent LineStrings are holes.
                new PositionCollection {
                    new Position(-44.6874, 37.5794),
                    new Position(-59.4531, 26.1159),
                    new Position(-36.0742, 8.9284),
                    new Position(-19.7265, 30.1451),
                    new Position(-44.6874, 37.5794)
                },

                //A hole in the polygon.
                new PositionCollection{
                    new Position(-46.2695, 29.3821),
                    new Position(-31.8554, 26.2737),
                    new Position(-36.4257, 17.6440),
                    new Position(-46.2695, 29.3821)
                }
                }), new PropertiesTable()
                {
                    { "title", "I'm a Polygon!" }
            }));

            //Convert a GeoJSON string to a Feature and add to the data source.
            string geoJsonString = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[-0.896087, 51.572033]},\"properties\":{\"title\":\"Created from GeoJson string.\"}}";

            if (Feature.TryParse(geoJsonString, out Feature? feature))
            {
                dataSource.Add(feature);
            }

            //Note that the Shape created when adding this feature is returned by the Add method. You can capture it if you want a reference to it.

            //You can also add a shape to the data source by creating a Shape object.
            //The shape class provides a way to update a shape after it has been added to the map.
            var shape = new Feature(new PointGeometry(new Position(0, 0)), new PropertiesTable
            {
                { "title", "I'm a Shape!" },

                //The shape class also provides support for Azure Maps extensions of GeoJson, like circle: https://learn.microsoft.com/en-us/azure/azure-maps/extend-geojson
                { "subType", "Circle" },
                { "radius", 1000000 } //Radius in meters.
            });

            //Add the shape to the data source.
            dataSource.Add(shape);

            UpdateCamera();
        }

        /// <summary>
        /// Imports a GeoJson file from the app package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportFromStream_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Get the GeoJson file.
            using (var stream = new FileInfo("Assets/map_resources/data/geojson/SamplePoiDataSet.json").OpenRead())
            {
                await dataSource.ImportDataFromStreamAsync(stream);
            }

            UpdateCamera();
        }

        /// <summary>
        /// Imports a GeoJson file from the "Raw/map_resources" folder of the app. 
        /// The map controls local host points to that folder and these files can be references using a relative URL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportRawFolderFileUrl_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Add a GeoJson file from the "Raw/map_resources" folder of the app.
            //Loading async so that the map has time to load the data before we update the camera.
            //If you don't need to access the data right after it is added, you can use the non-async version of the method.
            await dataSource.ImportDataFromUrlAsync("data/geojson/US_County_Boundaries.geojson");

            UpdateCamera();
        }

        /// <summary>
        /// Imports a GeoJson file from the web.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportWebUrl_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Add a GeoJson file from the web (GeoJSON feed of significant earthquakes from the past 30 days. Sourced from the USGS).
            //Since this is data is from the web and we want to wait for the data to be populated in
            //the data source so we can calculate the bounds, we need to use the async version of the method.
            //Unless you need to explictily access the data right after it is added, you can use the non-async version.
            await dataSource.ImportDataFromUrlAsync("https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/significant_month.geojson");

            UpdateCamera();
        }

        /// <summary>
        /// Imports a GeoJson file from the file picker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportWithFilePicker_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Create a file picker to select a GeoJson file.
            var ofd = new OpenFileDialog()
            {
                Title = "Select a GeoJson file",
                Filter = "GeoJson files (*.json, *.geojson)|*.json;*.geojson|All files (*.*)|*.*",
                Multiselect = false
            };

            var ofdResult = ofd.ShowDialog();

            //Check if the user selected a file.
            if (ofdResult != null && ofdResult.Value)
            {
                using (var stream = ofd.OpenFile())
                {
                    await dataSource.ImportDataFromStreamAsync(stream);
                    UpdateCamera();
                }
            }
        }

        /// <summary>
        /// Some of the Azure Maps REST services return a GeoJson object as a response and can easily be imported into a data source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportAzureMapsRestGeoJsonResponse_Clicked(object sender, RoutedEventArgs e)
        {
            /************************
             * NOTE: 
             * 
             * Azure Maps has a .NET REST client that is currently in preview but it does not include the latest versions of the REST services 
             * that return GeoJson objects (such as the Search Geocoding API), however these will likely be added in the future.
             * 
             * Since those clients leverage the Azure.Core library, it is likely that the responses from these clients in the 
             * future will be Azure.Core.GeoJson.GeoObjects. These can easily be converted into features in this library using the static 
             * Feature.FromGeoObject method.
             * 
             * https://learn.microsoft.com/en-us/azure/azure-maps/how-to-dev-guide-csharp-sdk
             */

            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Create an Azure Maps REST Geocoding REST service request URL.
            string geocodeRequest = "https://atlas.microsoft.com/geocode?api-version=2023-06-01&addressLine=15127 NE 24th Street&adminDistrict=WA&locality=Redmond";

            //Since the response from this service is GeoJson, we can import it directly into the data source.
            await dataSource.ImportDataFromUrlAsync(geocodeRequest);

            UpdateCamera();
        }

        private async void ImportGenericRestResponse_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            //Clear the data source.
            dataSource.Clear();

            //Not all Azure Maps REST services return GeoJson objects (most v1 REST services don't), but you can download
            //the data and convert it into GeoJson data then import it into the data source.
            //Similarly, you can use this method with other REST services that have geospatial insights that are not in GeoJson format.

            //Call the Azure Maps REST V1 routing service. Documentation: https://learn.microsoft.com/en-us/rest/api/maps/route/get-route-directions?view=rest-maps-2024-04-01
            string routeRequest = "https://atlas.microsoft.com/route/directions/json?api-version=1.0&query=47.60323,-122.33028:47.67491,-122.124";

            //Make an HTTP request to the Azure Maps REST service.
            //The Map class has helper methods MakeGetRequest and MakePostRequest that will sign and process the requests with the same auth info and Azure Maps domain used by the map.
            //Lets have the response deserialized into a JsonObject object.
            var json = await MyMap.MakeGetRequest<JsonObject>(routeRequest);

            //Manually traverse the response to get the location we want data.
            if (json != null && json["routes"] != null)
            {
                var route = json["routes"]?.AsArray().FirstOrDefault();

                //Get the first route response object.
                if (route != null)
                {
                    //Loop through the route legs and get all the points.
                    var routePath = new List<Position>();
                    {
                        var legs = route["legs"]?.AsArray();

                        if (legs != null)
                        {
                            foreach (var leg in legs)
                            {
                                if (leg != null)
                                {
                                    var points = leg["points"]?.AsArray();

                                    if (points != null)
                                    {
                                        foreach (JsonObject? point in points)
                                        {
                                            if (point != null)
                                            {
                                                routePath.Add(new Position(
                                                    point["longitude"].GetValue<double>(),
                                                    point["latitude"].GetValue<double>()
                                                ));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Convert JsonObject to Dictionary<string, object> to pass as properties.
                    var properties = new Dictionary<string, object?>();
                    foreach (var item in route["summary"] as JsonObject)
                    {
                        properties.Add(item.Key, item.Value?.ToString());
                    }

                    //Create a LineString Feature from the route path. Pass the route summary object as the properties.
                    var routeLine = new Feature(new LineString(routePath), properties);

                    //Add the route line to the data source.
                    dataSource.Add(routeLine);
                }
            }

            UpdateCamera();
        }

        /// <summary>
        /// Clears the data source, and automatically clears the map of all shapes that were in that data source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDataSource_Clicked(object sender, RoutedEventArgs e)
        {
            //Close the popup.
            popup.Close();

            dataSource.Clear();
        }

        /// <summary>
        /// Updates the camera to focus on the data source.
        /// </summary>
        private void UpdateCamera()
        {
            //Calculate the bounding box of the data source so we can have the map focus on the data.
            var bounds = BoundingBox.FromData(dataSource);

            if (bounds != null)
            {
                //Update the map view.
                MyMap.SetCamera(new CameraOptions()
                {
                    Bounds = bounds,
                    Padding = new Padding(40)
                });
            }
        }
    }
}
