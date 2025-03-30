using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Text;

namespace AzureMapsMauiSamples.Samples;

public partial class ClusterAggregates : ContentPage
{
    /*********************************************************************************************************
     * This sample is based on: https://samples.azuremaps.com/?sample=cluster-aggregates
     *********************************************************************************************************/

    private DataSourceLite dataSource;
    private Popup popup;

    //GeoJSON feed that contains the data we want to map.
    private string geojsonFeed = "/data/geojson/SamplePoiDataSet.json";

    //Colors for each EntityType property in point data: [Gas Station, Grocery Store, Restaurant, School]
    private string[] entityTypes = ["Gas Station", "Grocery Store", "Restaurant", "School"];

    public ClusterAggregates()
    {
        InitializeComponent();
    }

    //Wait for the map to be ready before interacting with it.
    internal void MyMap_OnReady(object? sender, MapEventArgs e)
    {
        //Create a reusable popup and add it to the map.
        popup = new Popup();
        MyMap.Popups.Add(popup);

        //Create a data source and add it to the map.
        dataSource = new DataSourceLite(geojsonFeed, new DataSourceOptions
        {
            Cluster = true,

            //The radius in pixels to cluster points together.
            ClusterRadius = 50,

            //Calculate counts for each entity type in a cluster as custom aggregate properties.
            ClusterProperties = new Dictionary<string, Expression>  {
                { "Gas Station", new Expression { "+", new object[] { "case", new object[] { "==", new object[] { "get", "EntityType" }, "Gas Station" }, 1, 0 }}},
                { "Grocery Store", new Expression { "+", new object[] { "case", new object[] { "==", new object[] { "get", "EntityType" }, "Grocery Store" }, 1, 0 }}},
                { "Restaurant", new Expression { "+", new object[] { "case", new object[] { "==", new object[] { "get", "EntityType" }, "Restaurant" }, 1, 0 }}},
                { "School", new Expression { "+", new object[] { "case", new object[] { "==", new object[] { "get", "EntityType" }, "School" }, 1, 0 }}}
            }
        });

        MyMap.Sources.Add(dataSource);

        //Create a bubble layer for rendering clustered data points.
        var clusterBubbleLayer = new BubbleLayer(dataSource, new BubbleLayerOptions {
            Radius = Expression<double>.Literal(20),
            Color = Expression<string>.Literal("purple"),
            StrokeWidth = Expression<int>.Literal(0),
            Filter = new Expression<bool> { "has", "point_count" } //Only rendered data points which have a point_count property, which clusters do.
        });

        //Add a click event to the layer so a popup can be displayed to show details about the cluster.
        MyMap.Events.Add("click", clusterBubbleLayer, ClusterLayer_Clicked);

        //Create a layer to render the individual locations.
        var pointLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
        {
            Filter = new Expression<bool> { "!", new object[] { "has", "point_count" } } //Filter out clustered points from this layer.
        });

        //Add a click event to the layer so a popup can be displayed to show details about the individual location.
        MyMap.Events.Add("click", pointLayer, IndividualPointLayer_Clicked);

        //Add the clusterBubbleLayer and two additional layers to the map.
        MyMap.Layers.AddRange([
            clusterBubbleLayer,

            //Create a symbol layer to render the count of locations in a cluster.
            new SymbolLayer(dataSource, new SymbolLayerOptions {
                IconOptions = new IconOptions {
                    Image = Expression<string>.Literal("none") //Hide the icon image.
                },
                TextOptions = new TextOptions {
                    TextField =new Expression<string> { "get", "point_count_abbreviated" },
                    Offset = new Pixel(0, 0.4),
                    Color = Expression<string>.Literal("white")
                }
            }),

            pointLayer
        ]);
    }

    private void ClusterLayer_Clicked(object? sender, MapEventArgs e)
    {
        if (e is MapMouseEventArgs args && args.Shapes.Count > 0 && args.Shapes[0].Properties.TryGetBool("cluster", out bool isCluster) && isCluster)
        {
            //Get the clustered point from the event.
            var cluster = args.Shapes[0];

            //Create a HTML string to show the details of the cluster.
            StringBuilder html = new StringBuilder("<div style=\"padding:10px;\">");
            html.Append($"<b>Cluster size: {cluster.Properties.GetString("point_count_abbreviated")}</b>");
            html.Append("<br/><br/>");

            //Loop though each entity type get the count from the clusterProperties of the cluster.
            foreach (var entityType in entityTypes)
            {
                html.Append($"<b>{entityType}</b>: {cluster.Properties.GetInt32(entityType)}<br/>");
            }

            html.Append("</div>");

            //Update the options of the popup and open it on the map.
            popup.SetOptions(new PopupOptions
            {
                Position = ((PointGeometry)cluster.Geometry).Coordinates,
                PixelOffset = new Pixel(0, 0),
                Content = html.ToString()
            });

            popup.Open();
        }
    }

    private void IndividualPointLayer_Clicked(object? sender, MapEventArgs e)
    {
        if (e is MapMouseEventArgs args && args.Shapes.Count > 0 && args.Shapes[0].Properties.TryGetString("Name", out string name))
        {
            //Get the point from the event.
            var point = args.Shapes[0];

            //Create a HTML string to show the details of the point.
            StringBuilder html = new StringBuilder("<div style=\"padding:10px;\">");
            html.Append($"<b>{name}</b>");
            html.Append("<br/><br/>");

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