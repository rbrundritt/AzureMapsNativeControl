using AzureMapsNativeControl;

namespace AzureMapsMauiSamples.Samples;

public partial class SynchronizeMapsSample : ContentPage
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