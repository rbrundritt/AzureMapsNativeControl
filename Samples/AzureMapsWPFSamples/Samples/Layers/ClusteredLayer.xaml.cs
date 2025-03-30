using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Text;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    public partial class ClusteredLayer : Page
    {
        /*********************************************************************************************************
         * This sample is based on: https://samples.azuremaps.com/?sample=point-clusters-in-bubble-layer
         *********************************************************************************************************/

        #region Private Properties

        //GeoJSON feed of significant earthquakes from the past 30 days. Sourced from the USGS Earthquake Hazards Program (https://www.usgs.gov/programs/earthquake-hazards) 
        private const string earthquakeFeed = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_month.geojson";

        //Create a data source.
        private DataSourceLite dataSource = new DataSourceLite(earthquakeFeed, new DataSourceOptions
        {
            //Tell the data source to cluster point data.
            Cluster = true,

            //The radius in pixels to cluster points together.
            ClusterRadius = 45,

            //The maximium zoom level in which clustering occurs.
            //If you zoom in more than this, all points are rendered as symbols.
            ClusterMaxZoom = 15
        });

        private Popup popup;

        #endregion

        public ClusteredLayer()
        {
            InitializeComponent();
        }

        //Wait for the map to be ready before interacting with it.
        private void MyMap_OnReady(object? sender, MapEventArgs e)
        {
            //Create a reusable popup and add it to the map.
            popup = new Popup();
            MyMap.Popups.Add(popup);

            //Add the data source to the map.
            MyMap.Sources.Add(dataSource);

            //Create a bubble layer.
            var clusterBubbleLayer = new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                //Bubbles are made semi-transparent.
                Opacity = Expression<double>.Literal(0.8),

                //Scale the size of the clustered bubble based on the number of points in the cluster.
                Radius = new Expression<double>
                {
                    "step",
                    new object[] { "get", "point_count" },
                    20,         //Default of 20 pixel radius.
                    100, 30,    //If point_count >= 100, radius is 30 pixels.
                    750, 40     //If point_count >= 750, radius is 40 pixels.
                },

                //Change the color of the cluster based on the value on the point_cluster property of the cluster.
                Color = new Expression<string>
                {
                    "step",
                    new object[] { "get", "point_count" },
                    "green",            //Default to green. 
                    100, "yellow",      //If the point_count >= 100, color is yellow.
                    750, "red"          //If the point_count >= 100, color is red.
                },

                StrokeWidth = Expression<int>.Literal(0),

                //Only rendered data points which have a point_count property, which clusters do.
                Filter = new Expression<bool>
                {
                    "has", "point_count"
                }
            });

            //Add a click event to the layer so we can zoom in when a user clicks a cluster.
            MyMap.Events.Add("click", clusterBubbleLayer, ClusterLayer_Clicked);

            //Add mouse events to change the mouse cursor when hovering over a cluster.
            MyMap.Events.Add("mouseenter", clusterBubbleLayer, (s, e) => MyMap.SetMouseCursor("pointer"));
            MyMap.Events.Add("mouseleave", clusterBubbleLayer, (s, e) => MyMap.SetMouseCursor("grab"));

            //Create a layer to render the individual locations.
            var pointLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                //Ensure individual locations are rendered and not hidden by the symbol underlying collision logic of the map.
                IconOptions = new IconOptions
                {
                    //Ensure the numbers are rendered above the clusters
                    AllowOverlap = true,
                    IgnorePlacement = true
                },

                //Filter out clustered points from this layer.
                Filter = new Expression<bool> { "!", new object[] { "has", "point_count" } }
            });

            //Add a click event to the layer so we can zoom in when a user clicks a cluster.
            MyMap.Events.Add("click", pointLayer, IndividualPointLayer_Clicked);

            //Add mouse events to change the mouse cursor when hovering over an individual location.
            MyMap.Events.Add("mouseenter", pointLayer, (s, e) => MyMap.SetMouseCursor("pointer"));
            MyMap.Events.Add("mouseleave", pointLayer, (s, e) => MyMap.SetMouseCursor("grab"));

            //Add the two layers to the map at the same time. This will trigger a single repaint of the map which is more efficient than adding the layers one by one.
            MyMap.Layers.AddRange(new List<BaseLayer> {
                clusterBubbleLayer,

                //Create a symbol layer to render the count of locations in a cluster.
                new SymbolLayer(dataSource, new SymbolLayerOptions
                {
                    IconOptions = new IconOptions
                    {
                        //Hide the icon image.
                        Image = Expression<string>.Literal("none")
                    },
                    TextOptions = new TextOptions
                    {
                        //Ensure the numbers are rendered above the clusters
                        AllowOverlap = true,
                        IgnorePlacement = true,

                        //An expression is used to concerte the "mag" property value into a string and appends the letter "m" to the end of it.
                        TextField = new Expression<string> { "get", "point_count_abbreviated" },
                        Offset = new Pixel(0, 0.4)
                    }
                }),

                pointLayer
            });
        }

        private async void ClusterLayer_Clicked(object? sender, MapEventArgs e)
        {
            //When a cluster is clicked, zoom in to it to break it apart into its smaller clusters and individual points.

            //Close the popup.
            popup.Close();

            //Cluster information is stored in the features property of the event since it is not a user created data object (Shape).
            if (e is MapMouseEventArgs args && args.Shapes != null && args.Shapes.Count > 0)
            {
                //Get the clustered point from the event.
                var cluster = args.Shapes[0];

                if (cluster.Geometry is PointGeometry clusterGeom)
                {
                    //Get the cluster expansion zoom level. This is the zoom level at which the cluster starts to break apart.
                    var zoom = await dataSource.GetClusterExpansionZoomAsync(cluster.Properties.GetInt32("cluster_id"));

                    //Update the map camera to be centered over the cluster. 
                    MyMap.SetCamera(new CameraOptions
                    {
                        Center = clusterGeom.Coordinates,
                        Zoom = zoom
                    }, new CameraAnimationOptions
                    {
                        Type = CameraAnimationType.Ease,
                        Duration = 200
                    });
                }
            }
        }

        private void IndividualPointLayer_Clicked(object? sender, MapEventArgs e)
        {
            //When an individual point is clicked, show a popup with details about the point.
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                //Get the point from the event.
                var point = args.Shapes[0];

                //Create a HTML string to show the details of the point.
                StringBuilder html = new StringBuilder("<div style=\"padding:10px;max-height:200px;overflow-y:scroll;\">");

                //Loop though each property of the point and add it to the HTML.
                foreach (var prop in point.Properties)
                {
                    //Skip internal properties (internal property names start with an underscore). 
                    if (!prop.Key.StartsWith("_"))
                    {
                        html.Append($"<b>{prop.Key}</b>: {prop.Value}<br/>");
                    }
                }

                html.Append("</div>");

                //Update the options of the popup and open it on the map.
                popup.SetOptions(new PopupOptions
                {
                    Position = ((PointGeometry)point.Geometry).Coordinates,
                    PixelOffset = new Pixel(0, -15),
                    Content = html.ToString()
                });

                popup.Open();
            }
        }
    }
}
