using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;


namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class AnimatedTileLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to animate an sequence of tile layers smoothly. 
        * This example uses the AnimatedTileLayer to animate through an array of tile layers. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=animated-tile-layer
        *********************************************************************************************************/

        private AnimatedTileLayer? layer = null;
        private FrameBasedAnimation? animation = null;
        private List<string> frameLabels = new List<string>();

        private string radarTilesetId = "microsoft.weather.radar.main";
        private string infraredTilesetId = "microsoft.weather.infrared.main";

        //Base weather tile layer URL for radar data. {azMapsDomain} is a placeholder that is set automatically by the map, and will also automatically append the map credentials to the request.
        private string urlTemplate = "https://{azMapsDomain}/map/tile?api-version=2024-04-01&tilesetId={tilesetId}&zoom={z}&x={x}&y={y}&timeStamp={timeStamp}&tileSize=256&view=Auto";

        public AnimatedTileLayerSample()
        {
            InitializeComponent();

            RadarWeatherBtn.IsChecked = true;
        }

        private async void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            LoadWeatherLayer(radarTilesetId);
        }

        private async void LoadWeatherLayer(string tilesetId)
        {
            //Remove any existing animation and layer before creating a new one.
            if (animation != null && layer != null)
            {
                animation.Stop();
                MyMap.Events.Remove("onframe", animation, OnAnimationFrame);
                MyMap.Layers.Remove(layer);
                layer = null;
            }

            //'microsoft.weather.infrared.main' availability
            double interval = 10 * 60 * 1000; //10 minute interval
            double past = 2.5 * 60 * 60 * 1000; //Data available up to 3 hours in the past. Look at just the past 2.5 hours.
            double future = 0; //Forecast data not avaiable.

            if (tilesetId == radarTilesetId)
            {
                interval = 5 * 60 * 1000; //5 minute interval
                past = 1 * 60 * 60 * 1000; //Data available up to 1.5 hours in the past. Look at just the past hour.
                future = 1 * 60 * 60 * 1000; //Data available up to 1.5 hours in the future. Look at just the next hour.
            }

            //Calculate the number of timestamps.
            int numTimestamps = (int)Math.Floor((past + future) / interval);
            var now = DateTime.Now;

            var tileSources = new List<TileSource>();
            frameLabels = new List<string>();

            for (var i = 0; i < numTimestamps; i++)
            {
                //Calculate time period for an animation frame. Shift the interval by one as the olds tile will expire almost immediately.
                var time = now.AddMilliseconds(i * interval - past);

                //Create a tile source for each timestamp.
                tileSources.Add(new TileSource(
                    urlTemplate.Replace("{tilesetId}", tilesetId).Replace("{timeStamp}", time.ToString("o")), //Date string must be ISO8601
                    tileSize: 256,
                    maxSourceZoom: 15
                ));

                //Optionally, create a message to display for each frame of the animation based on the time stamp.
                if (time == now)
                {
                    frameLabels.Add("Current");
                }
                else
                {
                    frameLabels.Add($"{(time - now).TotalMinutes} minutes");
                }
            }

            //Create the animation manager.
            layer = new AnimatedTileLayer(tileSources, new MediaLayerOptions()
            {
                Opacity = 0.9
            }, new PlayableAnimationOptions()
            {
                Duration = numTimestamps * 1000, //Allow one second for each frame (tile layer) in the animation.,
                Loop = true,
                AutoPlay = true
            });

            //Add the layer to the map asyncronously as we will want to attach an event to it and want to ensure it is loaded before we do this.
            await MyMap.Layers.AddAsync(layer);

            animation = layer.GetPlayableAnimation();

            if (animation != null)
            {
                MyMap.Events.Add("onframe", animation, OnAnimationFrame);
            }
        }

        private void OnAnimationFrame(object sender, MapEventArgs e)
        {
            //Get the current frame of the animation.
            var frameEvent = (e as FrameBasedAnimationEvent);
            if (frameEvent != null && frameEvent.FrameIdx < frameLabels.Count)
            {
                AnimationFrameInfo.Text = frameLabels[frameEvent.FrameIdx];
            }
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            //Make sure that this doesn't try to load a new scenario before the initial one has been loaded.
            if (btn != null && btn.IsChecked != null && btn.IsChecked.Value && animation != null)
            {
                if (btn.Content.Equals("Radar"))
                {
                    LoadWeatherLayer(radarTilesetId);
                }
                else
                {
                    LoadWeatherLayer(infraredTilesetId);
                }
            }
        }

        private void PlayButton_Clicked(object sender, RoutedEventArgs e)
        {
            animation?.Play();
        }

        private void StopButton_Clicked(object sender, RoutedEventArgs e)
        {
            animation?.Stop();
        }
    }
}
