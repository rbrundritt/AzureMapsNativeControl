using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class BringDataIntoViewControlSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to use the bring data into view control. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=bring-data-into-view-control
    *********************************************************************************************************/

    public BringDataIntoViewControlSample()
	{
		InitializeComponent();
	}

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //The overview map control was added in the XAML. Can optionally add it here as well.

        //var myControl = new BringDataIntoViewControl()
        //{
        //    Position = ControlPosition.TopRight
        //}
        //MyMap.Controls.Add(myControl);

        //Load some data into the map so that the control has something to bring into view.
        var dataSource = new DataSourceLite("data/geojson/SamplePoiDataSet.json", new DataSourceOptions
        {
            Cluster = true,
            ClusterRadius = 45,
            ClusterMaxZoom = 15
        });

        //Add the data source to the map.
        MyMap.Sources.Add(dataSource);

        //Add a layer for rendering point data as symbols.
        MyMap.Layers.Add(new SymbolLayer(dataSource));

        //Create a HTML marker and add it to the map.
        MyMap.Markers.Add(new HtmlMarker(new HtmlMarkerOptions
        {
            Position = new Position(-122.33, 47.6),
            Text = "10",
            Color = "DodgerBlue"
        }));
    }
}