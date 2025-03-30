using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class SimpleLineAnimationSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to animate the position of a line on the map by appending coordinates to it over time.
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=animate-a-line
    *********************************************************************************************************/

    #region Private Properties

    private bool isAnimating = false;
    private IDispatcherTimer timer;
    private double duration = 2500; //How long a single loop of the animation should last in ms.
    private long startTick = 0;
    private double lastProgres = 0;

    private Feature? line = null;
    private DataSource dataSource;

    #endregion

    #region Constructor

    public SimpleLineAnimationSample()
	{
		InitializeComponent(); 
        
        //Create an animation timer.
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(33); //Approximately 30 frames a second.

        this.Unloaded += (s, e) =>
        {
            //Stop the timer when the page is unloaded.
            timer.Stop();
        };

        this.Loaded += (s, e) =>
        {
            if (line != null)
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
        // Create a data source and add it to the map.
        dataSource = new DataSource();
        MyMap.Sources.Add(dataSource);

        //Create a line layer to render the point feature.
        MyMap.Layers.Add(new LineLayer(dataSource, new LineLayerOptions
        {
            StrokeColor = Expression<string>.Literal("red"),
            StrokeWidth = Expression<int>.Literal(5)
        }));

        //Create a line object and wrap it with the Shape class for easier updating.
        line = new Feature(new LineString(new PositionCollection { new Position(0, 0), new Position(0, 0) }));
        dataSource.Add(line);

        //Create the animation timer.
        timer.Tick += (s, e) => UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        //Calculate animation progress as a ratio of the duration between 0 and 1.
        var progress = ((double)((DateTime.Now.Ticks - startTick) / TimeSpan.TicksPerMillisecond)) % duration / duration;

        //Reset the animation if it is at the end.
        if (progress < lastProgres)
        {
            //Reset the animation if it is at the start.
            progress = 0;
            (line.Geometry as LineString).Coordinates = new PositionCollection { new Position(0, 0), new Position(0, 0) };
        }
        else
        {
            //Offset the longitude position based on the progress.
            double x = 360 * progress;

            //Draw a sine wave as our animation.
            double y = Math.Sin(x * Math.PI / 180) * 40; //Scale latitude by a factor of 40.

            //Append new coordinates to the line.
            (line.Geometry as LineString).Coordinates.Add(new Position(x, y));
        }

        lastProgres = progress;

        //Force the data source to update the feature in the map.
        dataSource.UpdateFeature(line);
    }

    private void ToggleAnimation(object sender, EventArgs e)
    {
        if (timer != null)
        {
            var btn = (Button)sender;
            if (isAnimating)
            {
                isAnimating = false;
                btn.Text = "Play";
                timer.Stop();
            }
            else
            {
                isAnimating = true;
                btn.Text = "Pause";
                startTick = DateTime.Now.Ticks;
                timer.Start();
            }
        }
    }

    #endregion
}