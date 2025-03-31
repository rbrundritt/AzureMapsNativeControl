using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class AnimateNewPointPositionSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to animate a point on the map to a new coordinate.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=animate-to-new-position-of-marker
        * https://samples.azuremaps.com/?sample=animate-to-new-position-of-point
        *********************************************************************************************************/

        #region Private Properties

        private HtmlMarker? marker = null;
        private Feature pointFeature;

        private DataSourceLite dataSource;
        private SymbolLayer? symbolLayer = null;

        #endregion

        public AnimateNewPointPositionSample()
        {
            InitializeComponent();

            PointFeatureBtn.IsChecked = true;
        }

        #region Public Methods

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create an HTML marker to animate. Hide for now.
            marker = new HtmlMarker(new HtmlMarkerOptions
            {
                Position = new Position(-122.33825, 47.53945),
                Color = "red",
                Visible = false
            });

            //Add the marker to the map.
            MyMap.Markers.Add(marker);

            //Create a data source and layer to render a point feature.
            pointFeature = new Feature(new PointGeometry(-122.33825, 47.53945));

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
            MyMap.Layers.Add(symbolLayer);

            //Add a click event and update the position of the marker or point feature.
            MyMap.Events.Add("click", async (s, e) =>
            {
                var mosueEvent = (MapMouseEventArgs)e;

                //Animate to a position.
                if (PointFeatureBtn.IsChecked == true)
                {
                    await MapAnimations.SetCoordinates(pointFeature, mosueEvent.Position, dataSource, new MapPathAnimationOptions
                    {
                        Duration = 2000,
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseInElastic,
                        AutoPlay = true
                    });
                }
                else
                {
                    await MapAnimations.SetCoordinates(marker, mosueEvent.Position, new MapPathAnimationOptions
                    {
                        Duration = 2000,
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseInElastic,
                        AutoPlay = true
                    });
                }
            });
        }

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
