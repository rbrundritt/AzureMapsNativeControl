using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AzureMapsWPFSamples.Samples
{
    public partial class BubbleLayerPulseAnimationSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to create a pulse animation using a bubble layer as a pulse. A second layer 
        * (symbol, bubble...) can be used to show the main data point.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=pulse-animation-with-bubble-layer
        *********************************************************************************************************/

        #region Private Properties

        private int maxRadius = 20;     //Parameter used to position the point within a circle in degrees.
        private double duration = 2500; //How long a single loop of the animation should last in ms.

        private BubbleLayer? bubbleLayer = null;
        private Random random = new Random();
        private DispatcherTimer timer;

        #endregion

        #region Constructor

        public BubbleLayerPulseAnimationSample()
        {
            InitializeComponent();

            //Create an animation time
            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = TimeSpan.FromMilliseconds(33); //Approximately 30 frames a second.

            this.Unloaded += (s, e) =>
            {
                //Stop the timer when the page is unloaded.
                timer.Stop();
            };

            this.Loaded += (s, e) =>
            {
                if (bubbleLayer != null)
                {
                    //Restart timer.
                    timer.Start();
                }
            };
        }

        #endregion

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create a data source to add your data to.
            var dataSource = new DataSourceLite();
            MyMap.Sources.Add(dataSource);

            //Create random points in the Seattle area.
            var points = new List<Feature>();

            for (var i = 0; i < 100; i++)
            {
                points.Add(new Feature(new PointGeometry(-122.33 + random.NextDouble() - 0.5, 47.6 + random.NextDouble() / 2 - 0.25)));
            }

            dataSource.AddRange(points);

            //Create a bubble layer to show the pulse animation.
            bubbleLayer = new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                Color = Expression<string>.Literal("rgb(0, 204, 255)"),

                //Hide the stroke of the bubble. 
                StrokeWidth = Expression<int>.Literal(0),

                //Make bubbles stay flat on the map when the map is pitched.
                PitchAlignment = PitchAlignment.Map
            });
            //Add the pulse layer to the map.
            MyMap.Layers.Add(bubbleLayer);

            //Create a symbol layer to show the main data point.
            MyMap.Layers.Add(new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    //For smoother animation, ignore the placement of the icon. This skips the label collision calculations and allows the icon to overlap map labels.
                    IgnorePlacement = true,

                    //For smoother animation, allow symbol to overlap all other symbols on the map.
                    AllowOverlap = true
                }
            }));

            //Start the animation timer.
            timer.Tick += (s, e) => UpdateAnimation();
            timer.Start();
        }

        private void UpdateAnimation()
        {
            //Calculate animation progress as a ratio of the duration between 0 and 1.
            var progress = ((double)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)) % duration / duration;

            //Early in the animaiton, make the radius small but don't render it. The map transitions between radiis, which causes a flash when going from large radius to small radius. This resolves that.
            if (progress < 0.1)
            {
                bubbleLayer?.SetOptions(new BubbleLayerOptions
                {
                    Radius = Expression<double>.Literal(0),
                    Opacity = Expression<double>.Literal(0)
                });
            }
            else
            {
                bubbleLayer?.SetOptions(new BubbleLayerOptions
                {
                    Radius = Expression<double>.Literal(maxRadius * progress),

                    //Have the opacity fade as the radius becomes larger.
                    Opacity = Expression<double>.Literal(Math.Max(0.9 - progress, 0))
                });
            }
        }
    }
}
