using AzureMapsNativeControl;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    /// <summary>
    /// Interaction logic for SynchronizeMapsSample.xaml
    /// </summary>
    public partial class SynchronizeMapsSample : Page
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
