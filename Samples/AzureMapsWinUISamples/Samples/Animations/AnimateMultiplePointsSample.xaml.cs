using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class AnimateMultiplePointsSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to animate multiple points on the map. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=animate-multiple-points
        *********************************************************************************************************/

        #region Private Properties

        private List<Feature> points = new List<Feature>();
        private int numPoints = 50;
        private GroupAnimation? animation = null;

        private DataSourceLite dataSource;

        #endregion

        public AnimateMultiplePointsSample()
        {
            InitializeComponent();
        }

        #region Public Methods

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create a data source and add it to the map.
            dataSource = new DataSourceLite();
            MyMap.Sources.Add(dataSource);

            for (var i = 0; i < numPoints; i++)
            {
                //Create an AnimatedPoint object with a random position and color.
                points.Add(new Feature(new PointGeometry(GetRandomPosition()), new Dictionary<string, object?>
                {
                    { "color", Helpers.GetRandomColorString() } //Calculate a random hex color. 
                }));
            }

            //Add the points to the data source.
            dataSource.AddRange(points);

            //Create a bubble layer to render the point feature.
            MyMap.Layers.Add(new BubbleLayer(dataSource, new BubbleLayerOptions
            {
                Color = new Expression<string> { "get", "color" }
            }));
        }

        private void PlayTogether(object sender, RoutedEventArgs e)
        {
            animation?.Dispose();
            PlayAnimation(GroupAnimationPlayType.Together);
        }

        private void PlaySequentially(object sender, RoutedEventArgs e)
        {
            animation?.Dispose();
            PlayAnimation(GroupAnimationPlayType.Sequential);

        }

        private void PlayInterval(object sender, RoutedEventArgs e)
        {
            animation?.Dispose();
            PlayAnimation(GroupAnimationPlayType.Interval);
        }

        private async void PlayAnimation(GroupAnimationPlayType playType)
        {
            var animations = new List<IPlayableAnimation?>();

            //Animate each point to a new random coordinate over a random duration between 100ms and 2000ms
            for (int i = 0; i < numPoints; i++)
            {
                animations.Add(await MapAnimations.SetCoordinates(points[i], GetRandomPosition(), dataSource, new MapPathAnimationOptions
                {
                    Duration = Helpers.Rand.Next(100, 300)
                }));
            }

            var options = new GroupAnimationOptions
            {
                PlayType = playType
            };

            if (playType == GroupAnimationPlayType.Interval)
            {
                options.Interval = 100;
            }

            animation = await MapAnimations.GroupAnimationsAsync(animations, options);
            animation?.Play();
        }

        private void StopAnimation(object sender, RoutedEventArgs e)
        {
            animation?.Stop();
        }

        private Position GetRandomPosition()
        {
            return new Position(Helpers.Rand.Next(-180, 180), Helpers.Rand.Next(-85, 85));
        }

        #endregion
    }
}
