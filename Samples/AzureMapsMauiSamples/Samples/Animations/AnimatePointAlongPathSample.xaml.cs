using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class AnimatePointAlongPathSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to easily animate a point along a path on the map. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animate-point-along-path
    * https://samples.azuremaps.com/?sample=animate-marker-along-path
    *********************************************************************************************************/

    #region Private Properties

    private IPlayableAnimation? _animation;

    private bool isMarkerScenario = false;

    private HtmlMarker? marker = null;
    private Feature pointFeature;

    private DataSourceLite dataSource;
    private SymbolLayer? symbolLayer = null;

    //Create an array of points to define a path to animate along.
    private List<Position> path = new List<Position>
    {
        new Position(-122.34758, 47.62155),
        new Position(-122.34764, 47.61859),
        new Position(-122.33787, 47.61295),
        new Position(-122.34217, 47.60964)
    };

    #endregion

    #region Constructor

    public AnimatePointAlongPathSample()
	{
		InitializeComponent();

        PointFeatureBtn.IsChecked = true;
    }

    #endregion

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Create an HTML marker to animate. Hide for now.
        marker = new HtmlMarker(new HtmlMarkerOptions
        {
            Position = path[0],
            Color = "red",
            Visible = false
        });

        //Add the marker to the map.
        MyMap.Markers.Add(marker);

        //Create a data source and add it to the map.
        dataSource = new DataSourceLite();
        MyMap.Sources.Add(dataSource);

        //Add a line for the path as a visual reference.
        dataSource.Add(new LineString(path));

        //Create a point feature to animate along the path.
        pointFeature = new Feature(new PointGeometry(path[0]));
        dataSource.Add(pointFeature);

        //Add a layer for rendering line data. 
        MyMap.Layers.Add(new LineLayer(dataSource), "labels");

        //Create a symbol layer to render the point feature.
        symbolLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
        {
            IconOptions = new IconOptions
            {
                //The rotation of the icon is set to the heading of the point feature. The animation will update the heading of the point feature.
                Rotation = new Expression<double> { "get", "heading" },

                //Have the rotation align with the map.
                RotationAlignment = PitchAlignment.Map,

                //For smoother animation, ignore the placement of the icon. This skips the label collision calculations and allows the icon to overlap map labels.
                IgnorePlacement = true,

                //For smoother animation, allow symbol to overlap all other symbols on the map.
                AllowOverlap = true
            },

            //Only render Points in this layer.    
            Filter = new Expression<bool> { "==", "$type", "Point" }
        });

        //Add the symbol layer to the map.
        MyMap.Layers.Add(symbolLayer);
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        PlayAnimation();
    }

    private async void PlayAnimation()
    {
        if (_animation != null)
        {
            //Restart the animation.
            _animation.Reset();
            _animation.Play();
        }
        else
        {
            if (isMarkerScenario)
            {
                //Create a marker animation to animate the marker along the path.
                _animation = await MapAnimations.MoveAlongPath(path, marker, new MapPathAnimationOptions
                {
                    Duration = 2000,
                    CaptureMetadata = true,
                    AutoPlay = true
                }, AnimateMapCbx.IsChecked);
            }
            else
            {
                //Create a point animation to animate the point feature along the path.
                _animation = await MapAnimations.MoveAlongPath(path, pointFeature, dataSource, new MapPathAnimationOptions
                {
                    Duration = 2000,
                    CaptureMetadata = true,
                    AutoPlay = true
                }, AnimateMapCbx.IsChecked);
            }

            //Start the animation.
            _animation?.Play();
        }
    }

    //Switch between scenarios.
    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var radioBtn = sender as RadioButton;

        if (radioBtn != null && radioBtn.IsChecked && marker != null && symbolLayer != null)
        {
            //Stop and dispose of the animation if it is running.
            _animation?.Stop();
            _animation?.Dispose();
            _animation = null;

            if (radioBtn == PointFeatureBtn)
            {
                //Hide the marker, show the symbol layer.
                marker.SetOptions(new HtmlMarkerOptions
                {
                    Visible = false
                });

                symbolLayer.SetOptions(new SymbolLayerOptions
                {
                    Visible = true
                });

                isMarkerScenario = false;
            }
            else
            {
                //Show the marker, hide the symbol layer.
                marker.SetOptions(new HtmlMarkerOptions
                {
                    Visible = true
                });

                symbolLayer.SetOptions(new SymbolLayerOptions
                {
                    Visible = false
                });

                isMarkerScenario = true;
            }
        }
    }

    private void AnimateMap_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Stop and dispose of the animation if it is running.
        _animation?.Stop();
        _animation?.Dispose();
        _animation = null;

        PlayAnimation();
    }
}