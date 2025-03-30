using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class AnimateTrafficFlowSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to animate the flow of traffic relative to the congestion level using the flowing 
    * dashed line animation.
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animated-traffic-flow
    *********************************************************************************************************/

    public AnimateTrafficFlowSample()
	{
		InitializeComponent();
	}

    private async void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Create a vector tile source and add it to the map.
        var dataSource = new TileSource("https://{azMapsDomain}/traffic/flow/tile/pbf?api-version=1.0&style=relative&zoom={z}&x={x}&y={y}", 
            isVectorTiles: true,
            maxSourceZoom: 22
        );
        MyMap.Sources.Add(dataSource);

        //Common style options for traffic background colors.
        var trafficBackgroundOptions = new LineLayerOptions
        { 
            //The name of the data layer within the data source to pass into this rendering layer.        
            SourceLayer = "Traffic flow",

            //Color the roads based on the level of traffic.
            StrokeColor = new Expression<string>
            {
                "step",
                new object[] { "get", "traffic_level" },
                "#6B0512", //Dark red
                0.01, "#EE2F53", //Red
                0.8, "orange", //Orange
                1, "#66CC99" //Green
            },

            //Scale the width of roads based on the level of traffic.
            StrokeWidth = new Expression<int>
            {
                "interpolate", 
                new object[] { "exponential", 2 },
                new object[] { "zoom" },
                12, 3,
                17, 9
            }
        };

        //Create two line layer for the base traffic flow color. One layer for both direction traffic data, and one layer for single line traffic data.
        MyMap.Layers.Add(new LineLayer(dataSource,  new LineLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.        
            SourceLayer = trafficBackgroundOptions.SourceLayer,
            StrokeColor = trafficBackgroundOptions.StrokeColor,
            StrokeWidth = trafficBackgroundOptions.StrokeWidth,

            //For traffic data that represents one side of the road, offset it.
            Offset = new Expression<double>
            {
                "interpolate",
                new object[] { "exponential", 2 },
                new object[] { "zoom" },
                12, 2,
                17, 6
            },

            Filter = new Expression<bool>
            {
                "==", new object[] { "get", "traffic_road_coverage" }, "one_side"
            }
        }), "labels");

        MyMap.Layers.Add(new LineLayer(dataSource, new LineLayerOptions
        {
            //The name of the data layer within the data source to pass into this rendering layer.        
            SourceLayer = trafficBackgroundOptions.SourceLayer,
            StrokeColor = trafficBackgroundOptions.StrokeColor,
            StrokeWidth = trafficBackgroundOptions.StrokeWidth,

            Filter = new Expression<bool>
            {
                "==", new object[] { "get", "traffic_road_coverage" }, "full"
            }
        }), "labels");

        //Common style options for traffic flow dashed lines.
        var trafficFLowLineOptions = new LineLayerOptions
        {       
            SourceLayer = "Traffic flow",
            StrokeColor =  Expression<string>.Literal("black"),

            //Scale the width of roads based on the level of traffic.
            StrokeWidth = new Expression<int>
            {
                "interpolate",
                new object[] { "exponential", 2 },
                new object[] { "zoom" },
                12, 1,
                17, 4
            }
        };

        //Create an offset for the layers that has two directional traffic data.
        var offsetExp = new Expression<double>
        {
            "interpolate",
            new object[] { "exponential", 2 },
            new object[] { "zoom" },
            12, 3,
            17, 7
        };

        //Create line layers for the different levels of traffic flow.
        var oneSideSlowFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Offset = offsetExp,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "one_side" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0 }, new object[] { "<=", new object[] { "get", "traffic_level" }, 0.01 } }
        });

        var slowFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "full" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0 }, new object[] { "<=", new object[] { "get", "traffic_level" }, 0.01 } }
        });

        var oneSideMidFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Offset = offsetExp,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "one_side" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0.01 }, new object[] { "<=", new object[] { "get", "traffic_level" }, 0.8 } }
        });

        var midFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "full" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0.01 }, new object[] { "<=", new object[] { "get", "traffic_level" }, 0.8 } }
        });

        var oneSideFastFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Offset = offsetExp,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "one_side" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0.8 } }
        });

        var fastFlowLayer = new LineLayer(dataSource, new LineLayerOptions
        {
            SourceLayer = trafficFLowLineOptions.SourceLayer,
            StrokeColor = trafficFLowLineOptions.StrokeColor,
            StrokeWidth = trafficFLowLineOptions.StrokeWidth,
            Filter = new Expression<bool> { "all", new object[] { "==", new object[] { "get", "traffic_road_coverage" }, "full" }, new object[] { ">", new object[] { "get", "traffic_level" }, 0.8 } }
        });

        //Add the layers below the labels to make the map clearer.
        await MyMap.Layers.AddRangeAsync(new BaseLayer[] {
            oneSideSlowFlowLayer, slowFlowLayer, oneSideMidFlowLayer, midFlowLayer, oneSideFastFlowLayer, fastFlowLayer 
        }, "labels");

        await MapAnimations.FlowingDashedLine(oneSideSlowFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 0.25,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
        await MapAnimations.FlowingDashedLine(slowFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 0.25,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
        await MapAnimations.FlowingDashedLine(oneSideMidFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 1,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
        await MapAnimations.FlowingDashedLine(midFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 1,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
        await MapAnimations.FlowingDashedLine(oneSideFastFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 4,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
        await MapAnimations.FlowingDashedLine(fastFlowLayer, new FlowingDashLineOptions
        {
            SpeedMultiplier = 4,
            GapLength = 2,
            DashLength = 2,
            Duration = 2000,
            AutoPlay = true,
            Loop = true
        });
    }
}