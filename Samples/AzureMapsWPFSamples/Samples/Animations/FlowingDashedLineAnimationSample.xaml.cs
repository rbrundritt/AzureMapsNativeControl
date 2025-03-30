using AzureMapsNativeControl;
using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    public partial class FlowingDashedLineAnimationSample : Page
    {
        /*********************************************************************************************************
        * This sample shows animate a line to make it look like it is flowing. 
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=animate-marker-along-path
        *********************************************************************************************************/

        private PlayableAnimation? animation;

        public FlowingDashedLineAnimationSample()
        {
            InitializeComponent();
        }

        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            // Create a data source and add it to the map.
            var dataSource = new DataSource();
            MyMap.Sources.Add(dataSource);

            //Add a line.
            dataSource.Add(new LineString(new Position[]
            {
                new Position(-122.34758, 47.62155),
                new Position(-122.34764, 47.61859),
                new Position(-122.33787, 47.61295),
                new Position(-122.34217, 47.60964)
            }));

            //Add a layer for rendering line data. 
            var layer = new LineLayer(dataSource, new LineLayerOptions
            {
                StrokeWidth = Expression<int>.Literal(4)
            });

            //Need to add the layer to the map asynchronously as it needs to be loaded before we can add an animation to it.
            await MyMap.Layers.AddAsync(layer);

            //Create a moving dashed line animation.
            animation = await MapAnimations.FlowingDashedLine(layer, new FlowingDashLineOptions
            {
                Duration = 2000,
                AutoPlay = true,
                Loop = true
            });
        }
    }
}
