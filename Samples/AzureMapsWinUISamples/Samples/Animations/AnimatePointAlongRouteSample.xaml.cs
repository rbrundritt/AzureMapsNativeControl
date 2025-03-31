using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class AnimatePointAlongRouteSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to smoothly animate a symbol along a route path taking into consideration timestamps 
        * for each point in the route path. This sample also includes controls and options for the animation. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=animate-along-a-route-path
        * https://samples.azuremaps.com/?sample=animate-a-gps-trace
        *********************************************************************************************************/

        #region Private Properties

        private IPlayableAnimation? _animation;

        private bool isMarkerScenario = false;

        private HtmlMarker? marker = null;
        private Feature pointFeature;

        private DataSourceLite dataSource;
        private SymbolLayer? symbolLayer = null;

        //Create an array of points to define a path to animate along.
        private List<Feature> route = new List<Feature>
        {
            new Feature(new PointGeometry(-122.34758, 47.62155), new Dictionary<string, object?> {{ "_timestamp", AtlasMath.ToJsonDateTime(DateTime.Parse("Tue, 18 Aug 2020 00:53:53 GMT")) }}),
            new Feature(new PointGeometry(-122.34764, 47.61859), new Dictionary<string, object?> {{ "_timestamp", AtlasMath.ToJsonDateTime(DateTime.Parse("Tue, 18 Aug 2020 00:54:53 GMT")) }}),
            new Feature(new PointGeometry(-122.33787, 47.61295), new Dictionary<string, object?> {{ "_timestamp", AtlasMath.ToJsonDateTime(DateTime.Parse("Tue, 18 Aug 2020 00:55:53 GMT")) }}),
            new Feature(new PointGeometry(-122.34217, 47.60964), new Dictionary<string, object?> {{ "_timestamp", AtlasMath.ToJsonDateTime(DateTime.Parse("Tue, 18 Aug 2020 00:59:53 GMT")) }})
        };

        #endregion

        #region Constructor

        public AnimatePointAlongRouteSample()
        {
            InitializeComponent();

            PointFeatureBtn.IsChecked = true;
        }

        #endregion

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            var firstCoord = (route[0].Geometry as PointGeometry).Coordinates;

            //Create an HTML marker to animate. Hide for now.
            marker = new HtmlMarker(new HtmlMarkerOptions
            {
                Position = firstCoord,
                Color = "red",
                Visible = false
            });

            //Add the marker to the map.
            MyMap.Markers.Add(marker);

            //Create a data source and add it to the map.
            dataSource = new DataSourceLite();
            MyMap.Sources.Add(dataSource);

            //Add a line for the path as a visual reference.
            var coords = new List<Position>();
            foreach (var feature in route)
            {
                coords.Add((feature.Geometry as PointGeometry).Coordinates);
            }
            dataSource.Add(new LineString(coords));

            //Create a point feature to animate along the path.
            pointFeature = new Feature(new PointGeometry(firstCoord));
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

        private void PlayButton_Clicked(object sender, RoutedEventArgs e)
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
                    _animation = await MapAnimations.MoveAlongRoute(route, marker, new MapPathAnimationOptions
                    {
                        Duration = 2000,
                        CaptureMetadata = true,
                        AutoPlay = true,

                        //Animate such that 1 second of animation time = 1 minute of data time.
                        SpeedMultiplier = 60
                    }, AnimateMapCbx.IsChecked == true);
                }
                else
                {
                    //Create a point animation to animate the point feature along the path.
                    _animation = await MapAnimations.MoveAlongRoute(route, pointFeature, dataSource, new MapPathAnimationOptions
                    {
                        Duration = 2000,
                        CaptureMetadata = true,
                        AutoPlay = true,

                        //Animate such that 1 second of animation time = 1 minute of data time.
                        SpeedMultiplier = 60
                    }, AnimateMapCbx.IsChecked == true);
                }

                //Start the animation.
                _animation?.Play();
            }
        }

        //Switch between scenarios.
        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            var radioBtn = sender as RadioButton;

            if (radioBtn != null && radioBtn.IsChecked == true && marker != null && symbolLayer != null)
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

        private void AnimateMap_CheckedChanged(object sender, RoutedEventArgs e)
        {
            //Stop and dispose of the animation if it is running.
            _animation?.Stop();
            _animation?.Dispose();
            _animation = null;

            PlayAnimation();
        }
    }
}
