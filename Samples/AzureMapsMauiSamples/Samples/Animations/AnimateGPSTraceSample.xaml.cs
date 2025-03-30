using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class AnimateGPSTraceSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to smoothly animate a symbol along a route path taking into consideration timestamps 
    * for each point in the route path. This sample also includes controls and options for the animation. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animate-a-gps-trace
    * https://samples.azuremaps.com/?sample=animate-along-a-route-path
    *********************************************************************************************************/

    #region Private Properties

    private PlayableAnimation? animation;

    private Feature pointFeature;
    private IList<Feature> route;

    private string gpsTraceUrl = "data/geojson/GPS_Trace.json";

    //The name of the property in the GPS trace that has timestamp information.
    private string timestampProperty = "time";

    #endregion

    #region Constructor

    public AnimateGPSTraceSample()
    {
        InitializeComponent();
    }

    #endregion

    private async void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Load a custom image icon into the map resources.
        await MyMap.ImageSprite.CreateFromTemplateAsync("arrow-icon", "marker-arrow", "teal", "white");

        //Create a data source and add it to the map.
        var dataSource = new DataSource();
        MyMap.Sources.Add(dataSource);

        //Create a second data source for the animated pin.
        var pinSource = new DataSourceLite();
        MyMap.Sources.Add(pinSource);

        //Add a layer for rendering the data.
        MyMap.Layers.Add(new BubbleLayer(dataSource));

        //Create a layer to render a symbol which we will animate.
        MyMap.Layers.Add(new SymbolLayer(pinSource, new SymbolLayerOptions
        {
            IconOptions = new IconOptions
            {
                //Pass in the id of the custom icon that was loaded into the map resources.
                Image = Expression<string>.Literal("arrow-icon"),

                //Anchor the icon to the center of the image.
                Anchor = PositionAnchor.Center,

                //Rotate the icon based on the rotation property on the point data.
                //The arrow icon being used in this case points down, so we have to rotate it 180 degrees.
                Rotation = new Expression<double> { "+", 180, new object[] { "get", "heading" } },

                //Have the rotation align with the map.
                RotationAlignment = PitchAlignment.Map,

                //For smoother animation, ignore the placement of the icon. This skips the label collision calculations and allows the icon to overlap map labels.
                IgnorePlacement = true,

                //For smoother animation, allow symbol to overlap all other symbols on the map.
                AllowOverlap = true
            }
        }));

        //Load the GPS trace data from a GeoJSON file.
        await dataSource.ImportDataFromUrlAsync(gpsTraceUrl);

        MyMap.SetCamera(new CameraOptions
        {
            //Set the camera to the bounding box of the data.
            Bounds = BoundingBox.FromData(dataSource),            

            Padding = new Padding(40)
        });

        //Extract route waypoints from the features.
        route = MapAnimations.ExtractRoutePoints(dataSource, timestampProperty);

        //Create a point feature to animate and add to pinSource.
        pointFeature = new Feature(new PointGeometry((route[0].Geometry as PointGeometry).Coordinates));
        pinSource.Add(pointFeature);

        animation = await MapAnimations.MoveAlongRoute(route, pointFeature, pinSource, new RoutePathAnimationOptions
        {
            //Capture metadata so that the "heading" value will be calculated at available to the rotation expression of the rendering layer.
            CaptureMetadata = true,

            //Speed up the animation.
            SpeedMultiplier = 16,

            //Set the animation to play automatically.
            AutoPlay = true
        });

        MyMap.Events.Add("onprogress", animation, (s, e) =>
        {
            PlayableAnimationEvent animationEvent = (PlayableAnimationEvent)e;
            InfoLabel.Text = $"Speed: {AtlasMath.ConvertSpeed(animationEvent.Speed, SpeedUnits.MetersPerSecond, SpeedUnits.KilometersPerHour, 1)} km/h  Time: {AtlasMath.FromJsonDateTime(animationEvent.Timestamp).ToString("T")}";
        });
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        animation?.Play();
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
        //Stop the animation.
        animation?.Stop();
    }

    private void ResetButton_Clicked(object sender, EventArgs e)
    {
        //Reset the animation.
        animation?.Reset();
    }

    private void PauseButton_Clicked(object sender, EventArgs e)
    {
        animation?.Pause();
    }
}