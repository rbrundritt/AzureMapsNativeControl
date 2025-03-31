using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AzureMapsWPFSamples.Samples
{
    public partial class SimplePointAnimationSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to create a simple animation on point data, both using a data source with a symbol 
        * layer, and using an HTML marker.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=simple-html-marker-animation
        * https://samples.azuremaps.com/?sample=simple-symbol-animation
        *********************************************************************************************************/

        #region Private Properties

        private int radius = 20;     //Parameter used to position the point within a circle in degrees.
        private double duration = 2500; //How long a single loop of the animation should last in ms.

        private HtmlMarker? marker = null;
        private Feature pointFeature;

        private DataSourceLite dataSource;
        private SymbolLayer? symbolLayer = null;

        private DispatcherTimer timer;

        #endregion

        #region Constructor

        public SimplePointAnimationSample()
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
                if (marker != null)
                {
                    //Restart timer.
                    timer.Start();
                }
            };

            PointFeatureBtn.IsChecked = true;
        }

        #endregion

        #region Public Methods

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create an HTML marker to animate. Hide for now.
            marker = new HtmlMarker(new HtmlMarkerOptions
            {
                Position = new Position(0, 0),
                Color = "red",
                Visible = false
            });

            //Add the marker to the map.
            MyMap.Markers.Add(marker);

            //Create a data source and layer to render a point feature.
            pointFeature = new Feature(new PointGeometry(0, 0));

            dataSource = new DataSourceLite();
            dataSource.Add(pointFeature);

            //Add the data source to the map.
            MyMap.Sources.Add(dataSource);

            //Create a symbol layer to render the point feature.
            symbolLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    //For smoother animation, ignore the placement of the icon. This skips the label collision calculations and allows the icon to overlap map labels.
                    IgnorePlacement = true,

                    //For smoother animation, allow symbol to overlap all other symbols on the map.
                    AllowOverlap = true
                }
            });

            //Add the symbol layer to the map.
            MyMap.Layers.Add(symbolLayer);

            //Start the animation timer.
            timer.Tick += (s, e) => UpdateAnimation();
            timer.Start();
        }

        private void UpdateAnimation()
        {
            //Calculate animation progress as a ratio of the duration between 0 and 1.
            var progress = ((double)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)) % duration / duration;

            //Calculate the position on a circle for an angle of animation.
            var angle = progress * Math.PI * 2;

            var position = new Position(
                Math.Cos(angle) * radius,
                Math.Sin(angle) * radius
            );

            //Update the position of the marker for the animation frame.
            marker.SetOptions(new HtmlMarkerOptions
            {
                Position = position
            });

            //Update the position of the point feature for the animation frame.
            (pointFeature.Geometry as PointGeometry).Coordinates = position;
            dataSource.UpdateFeature(pointFeature);
        }

        //Switch between scenarios.
        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            var radioBtn = sender as RadioButton;

            if (radioBtn != null && radioBtn.IsChecked == true && marker != null && symbolLayer != null)
            {
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
                }
            }
        }

        #endregion
    }
}
