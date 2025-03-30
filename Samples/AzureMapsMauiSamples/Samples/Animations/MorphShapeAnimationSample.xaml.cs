using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class MorphShapeAnimationSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to animate the morphing of a shape from one geometry to another. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=morph-shape-animation
    *********************************************************************************************************/

    #region Private Properties

    private PlayableAnimation? animation;

    private Feature mainFeature;

    //The geometries to animate through.
    private IList<Geometry> geometries = new List<Geometry>
    {
        new Polygon([new Position(-114.96093,44.08758),new Position(-101.07421,38.95940),new Position(-97.91015,52.37559),new Position(-114.96093,44.08758)]),
        new Polygon([new Position(-73.125,24.36711),new Position(-52.20703,24.36711),new Position(-52.20703,37.43997),new Position(-73.125,37.43997),new Position(-73.125,24.36711)]),
        new LineString([new Position(-83.32031,44.08758),new Position(-74.53125,51.39920),new Position(-59.41406,47.75409),new Position(-50.97656,59.35559),new Position(-37.61718,51.83577),new Position(-45.35156,45.33670),new Position(-51.67968,44.08758)]),
        new PointGeometry(-99.14062,23.56398),
        new PointGeometry(-34.45312,30.75127),
        new PointGeometry(-13.35937,55.77657),
        new MultiPoint([new Position(-68.55468,8.75479),new Position(-31.99218,-7.71099),new Position(-20.39062,23.56398),new Position(-41.48437,8.40716),new Position(-49.57031,15.96132)]),
        new LineString([new Position(-10.54687,31.05293),new Position(7.03125,-14.94478),new Position(52.38281,-31.95216),new Position(74.53125,-24.20688),new Position(67.5,11.86735)]),
        new Polygon([new Position(-10.54687,31.05293),new Position(7.03125,-14.94478),new Position(52.38281,-31.95216),new Position(74.53125,-24.20688),new Position(67.5,11.86735),new Position(59.0625,-17.64402),new Position(43.24218,8.40716),new Position(50.27343,26.11598),new Position(58.35937,37.99616),new Position(49.21875,46.31658),new Position(35.50781,44.59046),new Position(22.85156,34.01624),new Position(9.14062,42.81152),new Position(-10.54687,31.05293)]),
        new MultiLineString([
            new PositionCollection([new Position(-80.85937,-53.95608),new Position(-15.11718,-53.95608),new Position(-15.11718,-22.59372),new Position(-80.85937,-22.59372),new Position(-80.85937,-53.95608)]),
            new PositionCollection([new Position(-63.28125,-47.75409),new Position(-28.47656,-47.75409),new Position(-28.47656,-31.95216),new Position(-63.28125,-31.95216),new Position(-63.28125,-47.75409)])]
        ),
        new MultiPolygon([
            [new PositionCollection([new Position(22.14843,-42.03297),new Position(12.65625,-54.97761),new Position(52.03125,-55.17886),new Position(47.46093,-41.77131),new Position(22.14843,-42.03297)])],
            [new PositionCollection([new Position(52.73437,-41.24477),new Position(60.46875,-54.16243),new Position(88.94531,-39.36827),new Position(78.75,-32.84267),new Position(52.73437,-41.24477)])]
        ])
    };

    private IDispatcherTimer timer;
    private Random random = new Random();

    #endregion

    #region Constructor

    public MorphShapeAnimationSample()
	{
		InitializeComponent();

        //Create an animation timer.
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(2000);

        this.Unloaded += (s, e) =>
        {
            //Stop the timer when the page is unloaded.
            timer.Stop();
        };

        this.Loaded += (s, e) =>
        {
            if (mainFeature != null)
            {
                //Restart timer.
                timer.Start();
            }
        };
    }

    #endregion

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Create a data source and add it to the map.
        var dataSource = new DataSource();
        MyMap.Sources.Add(dataSource);

        //Create the initial feature and add to data source.
        mainFeature = new Feature(geometries[0]);
        dataSource.Add(mainFeature);

        //Add layers for rendering the data. 

        //Create a layer to render a symbol which we will animate.
        MyMap.Layers.AddRange([
            new PolygonLayer(dataSource, new PolygonLayerOptions
            {
                FillColor = Expression<string>.Literal("red"),
                Filter = Expression<bool>.PolygonTypeFilter()
            }),
            new LineLayer(dataSource, new LineLayerOptions
            {
                StrokeColor = Expression<string>.Literal("red"),
                StrokeWidth = Expression<int>.Literal(5),
                Filter = Expression<bool>.LineStringTypeFilter()
            }),
            new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                Filter = Expression<bool>.PointTypeFilter()
            })
        ]);

        //Start the animation timer.
        timer.Tick += async (s, e) =>
        {
            //Dispose previous animation.
            animation?.Dispose();

            //Animate a change to a random geometry.
            animation = await MapAnimations.Morph(mainFeature, geometries[(int)random.NextInt64(0, geometries.Count - 1)], dataSource, new PlayableAnimationOptions
            {
                Duration = 1500,
                AutoPlay = true
            });
        };
        timer.Start();
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        timer.Start();
        animation?.Play();
    }

    private void StopButton_Clicked(object sender, EventArgs e)
    {
        timer.Stop();
        //Stop the animation.
        animation?.Stop();
    }
}