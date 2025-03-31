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
    public sealed partial class DropAnimationSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways to animate the dropping of point data on the map.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=drop-symbol-animation
        * https://samples.azuremaps.com/?sample=drop-multiple-symbols-animation
        * https://samples.azuremaps.com/?sample=drop-multiple-symbols-on-interval
        * https://samples.azuremaps.com/?sample=drop-multiple-markers-animation
        * https://samples.azuremaps.com/?sample=drop-markers-on-interval
        *********************************************************************************************************/

        #region Private Properties

        DataSourceLite _source;
        IPlayableAnimation? _animation;

        PointGeometry singleSamplePoint = new PointGeometry(-122.335167, 47.608013);
        PointGeometry[] multipleSamplePoints = new PointGeometry[]
        {
            new PointGeometry(-123.27822, 47.31070),
            new PointGeometry(-122.45246, 47.86309),
            new PointGeometry(-120.91693, 47.44491),
            new PointGeometry(-123.28695, 46.89312),
            new PointGeometry(-122.14761, 46.65194),
            new PointGeometry(-122.29528, 48.27177),
            new PointGeometry(-120.99257, 46.62546),
            new PointGeometry(-121.84341, 46.95814),
            new PointGeometry(-123.96604, 46.93563),
            new PointGeometry(-123.60179, 47.40814),
            new PointGeometry(-124.21808, 47.39068),
            new PointGeometry(-123.46525, 47.82393),
            new PointGeometry(-121.49282, 48.24169),
            new PointGeometry(-121.81540, 48.48334),
            new PointGeometry(-122.60533, 47.28673)
        };

        #endregion

        #region Constructor

        public DropAnimationSample()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create a data source and add it to the map.
            _source = new DataSourceLite();
            MyMap.Sources.Add(_source);

            //Add a layer for rendering the data. 
            MyMap.Layers.Add(new SymbolLayer(_source, new SymbolLayerOptions
            {
                IconOptions = new IconOptions
                {
                    //Grab the offset from the shape.
                    Offset = new Expression<Pixel> { "get", "offset" },

                    //Grab the opacity from the shape. Opacity will be used to hide the shape when progress of the animation is 0.
                    Opacity = new Expression<double> { "get", "opacity" },

                    //For smoother animation, ignore the placement of the icon. This skips the label collision calculations and allows the icon to overlap map labels.
                    IgnorePlacement = true,

                    //For smoother animation, allow symbol to overlap all other symbols on the map.
                    AllowOverlap = true
                }
            }));

            //Load the initial scenario by default.
            AnimationScenarioPicker.SelectedIndex = 0;
        }

        private async void AnimationScenarioPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //Dispose the existing animation.
            if (_animation != null)
            {
                _animation.Dispose();
                _animation = null;
            }

            //Clear the source to start fresh.
            _source.Clear();

            //Clear any attached markers.
            MyMap.Markers.Clear();

            ProgressLabel.Text = "";

            string scenario = Helpers.GetSelectedPickerString(sender);

            switch (scenario)
            {
                case "Drop Point Geometry":
                    _animation = await MapAnimations.DropAsync(singleSamplePoint, _source, null, new PlayableAnimationOptions
                    {
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                        Duration = 1000,
                        AutoPlay = true
                    });
                    break;
                case "Drop Point Feature":
                    //Create a feature to drop.
                    var feature = new Feature(singleSamplePoint);

                    _animation = await MapAnimations.DropAsync(feature, _source, null, new PlayableAnimationOptions
                    {
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                        Duration = 1000,
                        AutoPlay = true
                    });
                    break;
                case "Drop HTML Marker":
                    //Create a marker to drop.
                    var marker = new HtmlMarker(new HtmlMarkerOptions
                    {
                        Position = singleSamplePoint.Coordinates,
                        Color = "red"
                    });

                    _animation = await MapAnimations.DropMarkersAsync(marker, MyMap, null, new PlayableAnimationOptions
                    {
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                        Duration = 1000,
                        AutoPlay = true
                    });
                    break;
                case "Drop Multiple Points":
                    //NOTE: You can alternatively pass in an array of Feature points.
                    _animation = await MapAnimations.DropAsync(multipleSamplePoints, _source, null, new PlayableAnimationOptions
                    {
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                        Duration = 1000,
                        AutoPlay = true
                    });
                    break;
                case "Drop Multiple HTML Markers":
                    //Create multiple markers to drop.
                    var markers = new List<HtmlMarker>();
                    foreach (var point in multipleSamplePoints)
                    {
                        markers.Add(new HtmlMarker(new HtmlMarkerOptions
                        {
                            Position = point.Coordinates,
                            Color = "red"
                        }));
                    }

                    _animation = await MapAnimations.DropMarkersAsync(markers, MyMap, null, new PlayableAnimationOptions
                    {
                        Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                        Duration = 1000,
                        AutoPlay = true
                    });
                    break;
                case "Drop Multiple Points on Interval":
                    //Create an animation for each point.
                    var dropAnimations = new List<IPlayableAnimation?>();

                    foreach (var point in multipleSamplePoints)
                    {
                        dropAnimations.Add(await MapAnimations.DropAsync(point, _source, null, new PlayableAnimationOptions
                        {
                            Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                            Duration = 1000

                            //NOTE: For this scenario we are not setting AutoPlay to true for the individual point animations, this will be handled by the GroupAnimation.
                        }));
                    }

                    //Create an interval animation to offset each drop animation.
                    _animation = await MapAnimations.GroupAnimationsAsync(dropAnimations, new GroupAnimationOptions
                    {
                        PlayType = GroupAnimationPlayType.Interval,
                        Interval = 100,
                        AutoPlay = true
                    });
                    break;
                case "Drop Multiple HTML Markers on Interval":
                    //Create an animation for each point.
                    var dropMarkersAnimations = new List<IPlayableAnimation?>();

                    foreach (var point in multipleSamplePoints)
                    {
                        //Create a marker for the point.
                        var m = new HtmlMarker(new HtmlMarkerOptions
                        {
                            Position = point.Coordinates,
                            Color = "red"
                        });

                        //Create an animation for the individual marker.
                        dropMarkersAnimations.Add(await MapAnimations.DropMarkersAsync(m, MyMap, null, new PlayableAnimationOptions
                        {
                            Easing = AzureMapsNativeControl.Animations.Easing.EaseOutBounce,
                            Duration = 1000
                        }));
                    }

                    //Create an interval animation to offset each drop animation.
                    _animation = await MapAnimations.GroupAnimationsAsync(dropMarkersAnimations, new GroupAnimationOptions
                    {
                        PlayType = GroupAnimationPlayType.Interval,
                        Interval = 100,
                        AutoPlay = true
                    });
                    break;
            }

            AddEventsToAnimation();
        }

        private void AddEventsToAnimation()
        {
            if (_animation != null)
            {
                MyMap.Events.Add("oncomplete", _animation, (s, e) =>
                {
                    ProgressLabel.Text = "Animation complete";
                });

                //Group animations don't have an "onprogress" event to attach to.
                if (_animation is PlayableAnimation)
                {
                    MyMap.Events.Add("onprogress", _animation, (s, e) =>
                    {
                        ProgressLabel.Text = $"Animation progress: {Math.Round((e as PlayableAnimationEvent).Progress * 100)}%";
                    });
                }
            }
        }

        private void PlayButton_Clicked(object sender, RoutedEventArgs e)
        {
            _animation?.Play();
        }

        private void StopButton_Clicked(object sender, RoutedEventArgs e)
        {
            _animation?.Stop();
        }

        private void ResetButton_Clicked(object sender, RoutedEventArgs e)
        {
            _animation?.Reset();
        }

        #endregion
    }
}
