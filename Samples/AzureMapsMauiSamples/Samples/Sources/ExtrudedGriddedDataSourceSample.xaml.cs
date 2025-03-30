using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class ExtrudedGriddedDataSourceSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to create a 3D polygon extrusions from a gridded data source. 
    * Change the grid type to see the data be re-aggregated into a different grid system. 
    * 
    * The data in this sample consists of 86,576 data points that represent each address in Fort Collins, Colorado. 
    * Once downloaded, the data points are aggregated into cells of a grid very quickly. 
    * Each cell is a hexgaon that is 0.25 miles wide.
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=extruded-gridded-data-source
    *********************************************************************************************************/

    #region Private Properties

    private GriddedDataSource? dataSource = null;
    private PolygonExtrusionLayer polygonLayer;
    private PolygonExtrusionLayer polygonHoverLayer;
    private Popup popup;

    #endregion

    #region Contrsutor

    public ExtrudedGriddedDataSourceSample()
    {
        InitializeComponent();

        //Set default grid type in picker.
        GridTypePicker.SelectedIndex = 1;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Event that is triggered when the map is ready.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //Load the Gridded data source module.
        await MyMap.LoadModuleAsync(AzureMapsModules.GriddedDataSourceModule);

        //Create a reusable popup and add it to the map.
        popup = new Popup();
        MyMap.Popups.Add(popup);

        //Create a grudded data source and add it to the map.
        dataSource = new GriddedDataSource("data/geojson/address.json", new GriddedDataSourceOptions
        {
            CellWidth = 0.25,
            DistanceUnits = AzureMapsNativeControl.DistanceUnits.Miles,

            //Reduce the coverage to give a bit of space between the grid basrs for better visual effect.
            Coverage = 0.8,

            //Adjust the pixel scaling of the grid such that spatial distances are accurate near the center of the initial map view.
            CenterLatitude = 40.54,

            AggregateProperties = new Dictionary<string, Expression>
            {
                { "Sum_Floors", new Expression { "+", new object[] { "get", "FLOOR" }}}
            }
        });
        MyMap.Sources.Add(dataSource);

        //Define some expressions that will be used with multiple layers.

        //Create a stepped expression based on the color scale.
        var colorExp = new Expression<string>(
            "step",
            new object[] { "get", "point_count" },

            //Default color.
            "#ffffb2",
            100, "#fecc5c",
            200, "#fd8d3c",
            300, "#f03b20",
            400, "#bd0026"
        );

        //Create an interpolate expression for height based on the `point_count` value of each cell. 
        var heightExp = new Expression<double>(
            "interpolate",
            new object[] { "linear" },
            new object[] { "get", "point_count" },
            1, 10,      //When point count is 1, height is 10 meters.
            1000, 5000  //When point_count is 1000, height is 1000 meters. All values between 1 and 5000 are interpolated.
        );

        //Create a polygon extrusion layer.
        polygonLayer  = new PolygonExtrusionLayer(dataSource, new PolygonExtrusionLayerOptions
        {
            //Height of the extrusion in meters.
            Height = heightExp,

            //Color of the extrusion based on the value of the "population" property.
            FillColor = colorExp,

            FillOpacity = 0.8
        });

        MyMap.Layers.Add(polygonLayer , "labels");

        //Create a second polygon extrusion layer to use a to highlight hovered grid cells by giving them a solid opacity.
        polygonHoverLayer = new PolygonExtrusionLayer(dataSource, new PolygonExtrusionLayerOptions
        {
            //Height of the extrusion in meters.
            Height = heightExp,

            //Color of the extrusion based on the value of the "population" property.
            FillColor = colorExp,

            FillOpacity = 1,

            Filter = new Expression<bool>("==", new object[] { "get", "cell_id" }, "")
        });

        MyMap.Layers.Add(polygonHoverLayer, "labels");

        // When the user moves their mouse over the polygonLayer, we'll update the filter in
        // the polygonHoverLayer to only show the matching state, thus creating a hover effect.
        MyMap.Events.Add("mousemove", (s, e) =>
        {
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0) {
                polygonHoverLayer.SetOptions(new PolygonExtrusionLayerOptions
                {
                    Filter = new Expression<bool>("==", new object[] { "get", "cell_id" }, args.Shapes[0].Properties.GetString("cell_id"))
                });

                MyMap.SetMouseCursor("pointer");
            }
        });

        // Reset the polygonHoverLayer layer's filter when the mouse leaves the layer.
        MyMap.Events.Add("mouseleave", polygonLayer , (s, e) =>
        {
            polygonHoverLayer.SetOptions(new PolygonExtrusionLayerOptions
            {
                Filter = new Expression<bool>("==", new object[] { "get", "cell_id" }, "")
            });

            MyMap.SetMouseCursor("grab");
        });

        MyMap.Events.Add("click", polygonLayer, LayerClicked);

        //Add a legend control to the map to add some context to the data colors/styles.
        MyMap.Controls.Add(new LegendControl()
        {
            Title = "Addresses per cell",
            ShowToggle = false,
            Legends = [
                new CategoryLegend() {
                    Items = [
                        new CategoryLegendItem("#ffffb2", "1 - 100"),
                        new CategoryLegendItem("#fecc5c", "100 - 200"),
                        new CategoryLegendItem("#fd8d3c", "200 - 300"),
                        new CategoryLegendItem("#f03b20", "300 - 400"),
                        new CategoryLegendItem("#bd0026", "400+")
                    ],
                    Shape = "square"
                }
            ],
            Position = ControlPosition.TopLeft
        });
    }

    private void LayerClicked(object? sender, MapEventArgs e)
    {
        if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
        {
            var props = args.Shapes[0].Properties;

            //Show a popup with the density value.
            popup.SetOptions(new PopupOptions
            {
                Position = args.Position,  //Show the popup by the mouse cursor.
                PopupTemplate = new PopupTemplate(props)
            });

            popup.Open();
        }
    }

    /// <summary>
    /// Event that is triggered when the grid type picker is changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dataSource != null)
        {
            popup.Close();

            var gridType = Helpers.GetSelectedPickerString(sender);

            Enum.TryParse(gridType, true, out GridType gridTypeEnum);

            dataSource.SetOptions(new GriddedDataSourceOptions
            {
                GridType = gridTypeEnum
            });
        }
    }

    #endregion
}