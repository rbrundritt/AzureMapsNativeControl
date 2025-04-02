using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class VectorTileSourceSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to use a vactor tile source. 
    * 
    * This sample uses the Azure Maps vector tile traffic flow and incident services: 
    * https://docs.microsoft.com/en-us/rest/api/maps/traffic/gettrafficflowtile
    * https://docs.microsoft.com/en-us/rest/api/maps/traffic/gettrafficincidenttile
    * 
    * Details on the format of these tiles can be found on TomTom's developer site (the data provider for Azure Maps traffic data):
    * https://developer.tomtom.com/traffic-api/traffic-api-documentation-traffic-flow/vector-flow-tiles
    * https://developer.tomtom.com/traffic-api/traffic-api-documentation-traffic-incidents/vector-incident-tiles
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=vector-tile-bubble-layer
    * https://samples.azuremaps.com/?sample=vector-tile-heat-map
    * https://samples.azuremaps.com/?sample=vector-tile-line-layer
    *********************************************************************************************************/

    #region Private Properties

    private TileSource trafficFlowVectorTileSource;
    private TileSource trafficIncidentVectorTileSource;
    private Popup popup;

    private HeatMapLayer heatMapLayer;
    private BubbleLayer incidentClusterLayer;
    private SymbolLayer incidentClusterLabelLayer;
    private BubbleLayer incidentLayer;
    private LineLayer lineLayer;

    #endregion

    public VectorTileSourceSample()
	{
		InitializeComponent();
	}

    private void MyMap_OnReady(object sender, MapEventArgs e)
    {
        //Create a reusable popup.
        popup = new Popup();

        //Create a vector tile source of traffic flow data and add it to the map.
        //Any TileSource class that returns vector tiles and not tile images, is considered a vector tile source.
        //This includes ZipFileTileSource, MBTileSource, and all other classes that derive from the TileSource class.
        trafficFlowVectorTileSource = new TileSource(
            //Since this tile layer is calling an Azure Maps service, using the "{azMapsDomain}" placeholder will
            //result in the map assigning the same domain and auth settings it uses to these requests.
            tileUrl: "https://{azMapsDomain}/traffic/flow/tile/pbf?api-version=1.0&style=relative&zoom={z}&x={x}&y={y}",

            //We must specify that the tile source contains vector tiles by setting the "isVectorTiles" property to true.
            isVectorTiles: true,
            maxSourceZoom: 22
        );
        MyMap.Sources.Add(trafficFlowVectorTileSource);

        //Create a vector tile source of traffic incident data and add it to the map.
        trafficIncidentVectorTileSource = new TileSource(
            tileUrl: "https://{azMapsDomain}/traffic/incident/tile/pbf?api-version=1.0&zoom={z}&x={x}&y={y}",
            
            //We must specify that the tile source contains vector tiles by setting the "isVectorTiles" property to true.
            isVectorTiles: true,

            maxSourceZoom: 22
        );
        MyMap.Sources.Add(trafficIncidentVectorTileSource);

        //Load the scenarios for this sample.
        LoadBubbleLayerScenario();
        LoadLineLayerScenario();
        LoadHeatMapScenario();

        //Select the initial scenario.
        DisplayAsPicker.SelectedIndex = 0;
    }

    #region Scenario loading code.

    /// <summary>
    /// Creates the layers used by the bubble layer scenario.
    /// Based on: https://samples.azuremaps.com/?sample=vector-tile-bubble-layer
    /// </summary>
    private void LoadBubbleLayerScenario()
    {
        //Create a layer for clustered points.
        incidentClusterLayer = new BubbleLayer(trafficIncidentVectorTileSource, new BubbleLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.
            SourceLayer = "Traffic incident POI",

            //Scale the size of the clustered bubble based on the size of the cluster.
            Radius = new Expression<double>(
                "step",
                new object[] { "get", "cluster_size" },
                15,         //Default of 15 pixel radius.
                10, 20,    //If cluster_size >= 10, radius is 20 pixels.
                20, 25     //If cluster_size >= 20, radius is 25 pixels.
            ),

            //Make clusters a single color.
            Color = Expression<string>.Literal("purple"),

            //Hide the stroke outline.
            StrokeWidth = Expression<int>.Literal(0),

            //Only rendered data points which have a cluster_size property.
            Filter = new Expression<bool>("has", "cluster_size")
        });
        MyMap.Layers.Add(incidentClusterLayer);

        //Add a click event to the cluster layer so we can zoom in when a user clicks a cluster.
        MyMap.Events.Add("click", incidentClusterLayer, (s, e) =>
        {
            if (e is MapMouseEventArgs args)
            {
                //Close the popup.
                popup.Close();

                //Zoom the map in one zoom level where the mouse was clicked. Animate the transition.
                MyMap.SetCamera(new CameraOptions
                {
                    Center = args.Position,
                    Zoom = e.Camera.Zoom + 1
                }, new CameraAnimationOptions
                {
                    Type = CameraAnimationType.Ease,
                    Duration = 250
                });
            }
        });

        //Create a layer for displaying the size of the clusters.
        incidentClusterLabelLayer = new SymbolLayer(trafficIncidentVectorTileSource, new SymbolLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.
            SourceLayer = "Traffic incident POI",

            IconOptions = new IconOptions
            {
                Image = Expression<string>.Literal("none"), //Hide the icon image.
            },
            TextOptions = new TextOptions
            {
                TextField = new Expression<string>("get", "cluster_size"),
                Offset = new Pixel(0, 0.4),
                Color = Expression<string>.Literal("white")
            }
        });
        MyMap.Layers.Add(incidentClusterLabelLayer);

        //Create a layer for individual incident points.
        incidentLayer = new BubbleLayer(trafficIncidentVectorTileSource, new BubbleLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.
            SourceLayer = "Traffic incident POI",

            //Give each data point a color based on the level of traffic.
            Color = new Expression<string>(
                "match",
                new object[] { "get", "magnitude" },
                1, "green",	    //Minor
                2, "orange",	//Moderate
                3, "red",		//Major
                "gray"			//Unknown or undefined
            ),

            //Filter out clustered points from this layer.
            Filter = new Expression<bool>("!", new object[] { "has", "cluster_size" })
        });
        MyMap.Layers.Add(incidentLayer);

        //Add a click event to the layer to display details about the incident.
        MyMap.Events.Add("click", incidentLayer, FeatureLayerClicked);

        //Add the layers to the map.
        MyMap.Layers.AddRange([incidentClusterLayer, incidentClusterLabelLayer, incidentLayer]);
    }

    /// <summary>
    /// Event that is triggered when a feature is clicked on the map.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FeatureLayerClicked(object? sender, MapEventArgs e)
    {
        if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
        {
            //Get the first feature.
            var f = args.Shapes[0];

            //If the shape is a Point, use its coordinates to position the popup, otherwise use the mouse location.
            var pos = (f.Geometry is PointGeometry pg) ? pg.Coordinates : args.Position;

            //Update the content and position of the popup.
            popup.SetOptions(new PopupOptions
            {
                //Assign a template to the properties of the feature. 
                PopupTemplate = new PopupTemplate(f.Properties),
                Position = pos
            });

            //Open the popup.
            popup.Open(MyMap);
        }
    }

    /// <summary>
    /// Creates the layers used by the line layer scenario.
    /// Based on: https://samples.azuremaps.com/?sample=vector-tile-line-layer
    /// </summary>
    private void LoadLineLayerScenario()
    {
        //Create a layer for traffic flow lines.
        lineLayer = new LineLayer(trafficFlowVectorTileSource, new LineLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.
            SourceLayer = "Traffic flow",

            //Color the roads based on the level of traffic. 
            StrokeColor = new Expression<string>(
                "step",
                new object[] { "get", "traffic_level" },
                "#6B0512",          //Dark red
                0.01, "#EE2F53",    //Red
                0.8, "orange",      //Orange
                1, "#66CC99"        //Green
            ),

            //Scale the width of roads based on the level of traffic. 
            StrokeWidth = new Expression<int>(
                "interpolate",
                new object[] { "linear" }, 
                new object[] { "get", "traffic_level" },
                0, 6,
                1, 1
            ),

            //Hide initially. For this sample we will simply hide/show the layers based on the picker selection.
            Visible = false
        });

        //Add the traffic flow layer below the labels to make the map clearer.
        MyMap.Layers.Add(lineLayer, "labels");

        //Add a click event to the line layer to display details about the road.
        MyMap.Events.Add("click", lineLayer, FeatureLayerClicked);
    }

    /// <summary>
    /// Creates the layers used by the heat map scenario.
    /// Based on: https://samples.azuremaps.com/?sample=vector-tile-heat-map
    /// </summary>
    private void LoadHeatMapScenario()
    {
        //Create a heat map layer of the traffic data.
        heatMapLayer = new HeatMapLayer(trafficFlowVectorTileSource, new HeatMapLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.
            SourceLayer = "Traffic flow",

            //Give each data point a weight based on the level of traffic.
            //Since traffic flow level goes from 0 - closed, to 1 - free flow, lets intverse this value so slower areas have more weight.
            Weight = new Expression<double>("-", 1, new object[] { "get", "traffic_level" }),

            //Give each data point a radius.
            Radius = Expression<double>.Literal(5),

            //Ignore roads where traffic is travelling at 80% of the posted speed limit or higher.
            Filter = new Expression<bool>("<", new object[] { "get", "traffic_level" }, 0.80),

            //Hide initially. For this sample we will simply hide/show the layers based on the picker selection.
            Visible = false
        });

        //Add the traffic flow layer below the labels to make the map clearer.
        MyMap.Layers.Add(heatMapLayer, "labels");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Event that is triggered when the user changes the scenario selected option in the picker.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayAsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Close the popup.
        popup.Close();

        //Change the visibility of the layers based on the selected option.
        var displayAs = Helpers.GetSelectedPickerString(sender);

        bool bubbleLayerVisible = false;
        bool lineLayerVisible = false;
        bool heatMapVisible = false;

        switch (displayAs)
        {
            case "Bubble Layer":
                bubbleLayerVisible = true;
                break;
            case "Line Layer":
                lineLayerVisible = true;
                break;
            case "Heat Map":
                heatMapVisible = true;
                break;
            default:                
                break;
        }

        //Show the bubble layer scenario.
        incidentClusterLayer.SetOptions(new BubbleLayerOptions
        {
            Visible = bubbleLayerVisible
        });

        incidentClusterLabelLayer.SetOptions(new BubbleLayerOptions
        {
            Visible = bubbleLayerVisible
        });

        incidentLayer.SetOptions(new BubbleLayerOptions
        {
            Visible = bubbleLayerVisible
        });

        //Show the line layer scenario.
        lineLayer.SetOptions(new LineLayerOptions
        {
            Visible = lineLayerVisible
        });

        //Show the heat map scenario.
        heatMapLayer.SetOptions(new HeatMapLayerOptions
        {
            Visible = heatMapVisible
        });
    }

    #endregion
}