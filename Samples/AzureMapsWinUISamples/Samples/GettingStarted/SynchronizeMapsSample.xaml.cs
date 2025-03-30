using AzureMapsNativeControl;
using Microsoft.UI.Xaml.Controls;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class SynchronizeMapsSample : Page
    {
        /*********************************************************************************************************
       * This sample shows how to synchronize the camera of multiple map instances.
       *********************************************************************************************************/

        public SynchronizeMapsSample()
        {
            InitializeComponent();
        }

        private void MyMap1_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            var synchronizer = new MapSynchronizer([MyMap1, MyMap2, MyMap3, MyMap4]);
        }
    }
}
