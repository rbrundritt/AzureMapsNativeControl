using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class AnimateChoroplethMapSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to create a choropleth map and animate it over time. For simplicity this sample is 
    * using a single GeoJSON file that includes all data to iterate over.
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animate-a-choropleth-map
    *********************************************************************************************************/

    #region Private Properties

    private PolygonLayer? polygonLayer = null;
    private List<Expression<string>> colorExpressions = new List<Expression<string>>();
    
    private double maxScale = 30;

    private int frameIndex = 0;
    private int frameDuration = 1000;
    private IDispatcherTimer timer;

    #endregion

    #region Constructor 

    public AnimateChoroplethMapSample()
	{
		InitializeComponent();

        //Create an animation timer.
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(frameDuration); 

        this.Unloaded += (s, e) =>
        {
            //Stop the timer when the page is unloaded.
            timer.Stop();
        };

        this.Loaded += (s, e) =>
        {
            if (polygonLayer != null)
            {
                //Restart timer.
                timer.Start();
            }
        };
    }

    #endregion

    #region Private Methods

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Create list of color expressions to use for the animation. Each expression is a gradient based on a specific property of the data.
        for (int i = 1; i <= 11; i++)
        {
            colorExpressions.Add(new Expression<string>(new object[] {
                "interpolate",
                new object[] { "linear" },
                new object[] { "get", "PopChange" + (2000 + i) },
                -maxScale, "rgb(255,0,255)",       // Magenta
                -maxScale / 2, "rgb(0,0,255)",     // Blue
                0, "rgb(0,255,0)",                 // Green
                maxScale / 2, "rgb(255,255,0)",    // Yellow
                maxScale, "rgb(255,0,0)"           // Red
            }));
        }

        //Create a data source and add it to the map.
        var dataSource = new DataSource();
        MyMap.Sources.Add(dataSource);

        //Load data into data source.
        dataSource.ImportDataFromUrl("data/geojson/US_County_2000_Annual_Population_Change.json");

        //Create a layer to render the polygon data.
        polygonLayer = new PolygonLayer(dataSource, new PolygonLayerOptions
        {
            FillColor = colorExpressions[0]
        });
        MyMap.Layers.Add(polygonLayer);

        //Start the animation timer.
        timer.Tick += (s, e) => AnimateNextFrame();
        timer.Start();
    }

    private void PlayPauseToggleButton_Clicked(object sender, EventArgs e)
    {
        //Toggle the timer between running and paused.
        if (timer.IsRunning)
        {
            //Pause the animation.
            timer.Stop();
        }
        else
        {
            //Resume the animation.
            timer.Start();
        }
    }

    private void AnimateNextFrame()
    {
        //Animate the map by changing the fill color of the polygons over time.
        if (polygonLayer != null)
        {
            frameIndex = (frameIndex + 1) % colorExpressions.Count;
            polygonLayer.SetOptions(new PolygonLayerOptions
            {
                FillColor = colorExpressions[frameIndex]
            });

            //Update label to show the current year.
            var year = 2000 + frameIndex;
            YearLabel.Text = $"Year: {year}";
        }
    }

    #endregion
}