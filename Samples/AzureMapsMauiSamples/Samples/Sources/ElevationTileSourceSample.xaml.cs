using AzureMapsNativeControl;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class ElevationTileSourceSample : ContentPage
{
    /// <summary>
    /// An elevation tile source.
    /// Any tile source can be used: CustomTileSource, MBTileSource, ZipFileTileSource, etc.
    /// </summary>
    private TileSource elvSource = new TileSource(
        tileUrl: "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png",
        elevationEncoding: ElevationEncoding.Terrarium,
        maxSourceZoom: 15,
        tileSize: 256
    );

    public ElevationTileSourceSample()
	{
		InitializeComponent();
	}

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Enable the elevation tile source. If it hasn't been added to the map source manager, this method will do that.
        MyMap.EnableElevation(elvSource);

        //To enhance the experience, lets add a CSS background for the sky to give the map a more immersive 3D look.
        MyMap.SetStyle(new StyleOptions ()
        {
            //This is using a CSS linear gradient. 
            BackgroundStyle = "linear-gradient(#93b0d2 0%, #ebbac4 20%, #c0b5ca 40%, #7788a8 60%, #646c7c 80%)"

            //Alternatively, try a background image.
            //BackgroundStyle = "url('images/sky_stars_sunset.jpg') center center / cover"
        });
    }

    private void ExaggerationSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;

        double exaggeration = Math.Round(slider.Value, 1);

        MyMap.EnableElevation(elvSource, exaggeration);

        ExaggerationLabel.Text = $"Exaggeration: {exaggeration}";
    }

    private void DisableElevationCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkbox = (CheckBox)sender;

        if(checkbox.IsChecked)
        {
            MyMap.DisableElevation();
        }
        else
        {
            MyMap.EnableElevation(elvSource);   //No need to set the exaggeration value as it will use the last value set.
        }
    }
}
