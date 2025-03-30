using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsWPFSamples.Samples
{
    /// <summary>
    /// Interaction logic for SwipeMapSample.xaml
    /// </summary>
    public partial class SwipeMapSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to use the SwipeMap control. This allows you to have two maps side by side where the 
        * camera is seamlessly synced. Add different data sets to the maps to as a way to create a comparison view.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?search=swipe&sample=swipe-between-two-maps
        *********************************************************************************************************/

        #region Private Properties

        private DataSourceLite primaryDataSource;
        private DataSourceLite secondaryDataSource;

        #endregion

        public SwipeMapSample()
        {
            InitializeComponent();
        }

        private void MySwipeMap_OnReady(object sender, EventArgs e)
        {
            //Both maps are now ready. 

            //Create a data source for the primary map and load some data into the data source.
            primaryDataSource = new DataSourceLite("data/geojson/US_County_Unemployment_2017.geojson");
            MySwipeMap.PrimaryMap.Sources.Add(primaryDataSource);

            //Add a layer to display the data in the primary map. In this case, a choropleth based on unemployment rates in the US counties.
            var unemploymentRateLayer = new PolygonLayer(primaryDataSource, new PolygonLayerOptions
            {
                FillColor = new Expression<string>()
                {
                    "step",
                    new object[] { "get", "unemployment_rate" },
                    "#FFEDA0",
                    3, "#FED976",
                    4, "#FD8D3C",
                    5, "#E31A1C",
                    6, "#800026"
                },
                FillOpacity = Expression<double>.Literal(0.8)
            });
            MySwipeMap.PrimaryMap.Layers.Add(unemploymentRateLayer, "labels");

            //Create a legend control that dynamically generates based on the unemploymentRateLayer options.
            MySwipeMap.PrimaryMap.Controls.Add(new LegendControl()
            {
                Title = "Unemployment rate (%)",
                ShowToggle = false,
                Legends = [new DynamicLegend(unemploymentRateLayer) {
                SubtitleFallback = "none",
                DefaultCategory = new CategoryLegendDefaults() {
                    Shape = "square",
                    Layout = CssFlexDirection.ColumnReverse
                }
            }],
                Position = ControlPosition.TopLeft
            });

            //Create a data source for the secondary map and load some data into the data source.
            secondaryDataSource = new DataSourceLite("data/geojson/US_County_Unemployment_2017.geojson");
            MySwipeMap.SecondaryMap.Sources.Add(secondaryDataSource);

            //Add a layer to display the data in the primary map. In this case, a choropleth based on the size of the labor force in the US counties.
            var labourSizeLayer = new PolygonLayer(secondaryDataSource, new PolygonLayerOptions
            {
                FillColor = new Expression<string>()
                {
                    "step",
                    new object[] { "get", "labor_force" },
                    "#fff7f3",
                    10000, "#fcc5c0",
                    50000, "#dd3497",
                    100000, "#48006a"
                },
                FillOpacity = Expression<double>.Literal(0.8)
            });
            MySwipeMap.SecondaryMap.Layers.Add(labourSizeLayer, "labels");

            //Create a legend control that dynamically generates based on the labourSizeLayer options.
            MySwipeMap.SecondaryMap.Controls.Add(new LegendControl()
            {
                Title = "Size of labor force",
                ShowToggle = false,
                Legends = [new DynamicLegend(labourSizeLayer) {
                    SubtitleFallback = "none",
                    DefaultCategory = new CategoryLegendDefaults() {
                        Shape = "square",
                        Layout = CssFlexDirection.ColumnReverse
                    },
                    CssClass = "labour-force-legend"
                }],
                Position = ControlPosition.TopRight
            });
        }
    }
}