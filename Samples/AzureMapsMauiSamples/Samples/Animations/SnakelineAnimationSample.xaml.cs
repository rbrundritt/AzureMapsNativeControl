using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class SnakelineAnimationSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to animate a LineString such that its path is drawn out smoothly over time on top 
    * of the map using what is called a snakeline animation. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animate-a-snakeline
    *********************************************************************************************************/

    #region Private Properties

    private Feature pathFeature = new Feature(new LineString(new PositionCollection
    {
        new Position(-122.33825, 47.53945),
        new Position(-122.26135, 47.36115),
        new Position(-122.37121, 47.19717),
        new Position(-122.71179, 47.12247),
        new Position(-122.82165, 47.06263),
        new Position(-122.93151, 46.77373),
        new Position(-122.61840, 46.50217),
        new Position(-122.23388, 46.45678),
        new Position(-121.83837, 46.45678),
        new Position(-121.58020, 46.60039),
        new Position(-121.49780, 46.75491),
        new Position(-121.44836, 47.00273),
        new Position(-121.39892, 47.20091),
        new Position(-121.42089, 47.42065),
        new Position(-121.32202, 47.56911),
        new Position(-121.10229, 47.57652),
        new Position(-120.86059, 47.52461),
        new Position(-120.67382, 47.36115),
        new Position(-120.45410, 47.07760)
    }));

    private DataSourceLite dataSource;

    private PlayableAnimation? animation = null;

    private MapPathAnimationOptions animationOptions = new MapPathAnimationOptions
    {
        Duration = 5000, 
        Easing = AzureMapsNativeControl.Animations.Easing.EaseInCubic,
        AutoPlay = true
    };

    #endregion

    public SnakelineAnimationSample()
	{
		InitializeComponent();
	}

    #region Public Methods

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Create a data source and add the path feature to it.
        dataSource = new DataSourceLite();
        dataSource.Add(pathFeature);

        //Add the data source to the map.
        MyMap.Sources.Add(dataSource);

        //Create a line layer to render the point feature.
        MyMap.Layers.Add(new LineLayer(dataSource, new LineLayerOptions
        {
            StrokeColor = Expression<string>.Literal("red"),
            StrokeWidth = Expression<int>.Literal(5)
        }));
    }

    #endregion

    private void AnimateMap_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Dispose the animation and start a new one.
        animation?.Dispose();
        animation = null;

        PlayAnimation();
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        PlayAnimation();
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

    private void Pause_Clicked(object sender, EventArgs e)
    {
        animation?.Pause();
    }

    private async void PlayAnimation()
    {
        if (animation == null)
        {
            //Create a snakeline animation.
            animation = await MapAnimations.Snakeline(pathFeature, dataSource, animationOptions, AnimateMapCbx.IsChecked);
        }

        //Start the animation.
        animation?.Play();
    }
}