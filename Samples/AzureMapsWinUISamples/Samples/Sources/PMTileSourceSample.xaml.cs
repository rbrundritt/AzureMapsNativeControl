using Microsoft.UI.Xaml.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System.Threading.Tasks;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class PMTileSourceSample : Page
    {
        /*********************************************************************************************************
        * This sample showcases the use of Protomaps to display Overture buildings data on a map. The data is stored 
        * in a PMTiles file that contains a vector tileset of buildings data. The data is filtered by the building 
        * subtype and displayed as polygons on the map. Click on a polygon to view more information about the building.
        * 
        * https://samples.azuremaps.com/?sample=pmtiles-buildings
        *********************************************************************************************************/

        private Popup popup;

        public PMTileSourceSample()
        {
            InitializeComponent();
        }

        private async void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //Create a reusable popup.
            popup = new Popup();

            //Create a vector tile source that uses the "pmtiles:" protocol to load a PMTiles file from the Azure Maps service.
            var source = new TileSource(
                //Since the tiles are PMTiles, we must use the "pmtiles:" protocol to load the tiles.
                tileUrl: "pmtiles://https://overturemaps-tiles-us-west-2-beta.s3.amazonaws.com/2024-07-22/buildings.pmtiles",

                //We must specify that the tile source contains vector tiles by setting the "isVectorTiles" property to true.
                isVectorTiles: true
            );
            await MyMap.Sources.AddAsync(source);

            await Task.Delay(1000);

            var layer = new PolygonExtrusionLayer(source, new PolygonExtrusionLayerOptions
            {
                //Specify the internal layer ID in the tiles.
                SourceLayer = "building",

                Height = new Expression<double>(["get", "height"]),
                FillOpacity = 1,
                FillColor = new Expression<string>([
                    "match",

                    new object[] { "get", "subtype" },

                    "agricultural", "wheat",
                    "civic", "teal",
                    "commercial", "blue",
                    "education", "aqua",
                    "entertainment", "pink",
                    "industrial", "yellow",
                    "medical", "red",
                    "military", "darkgreen",
                    "outbuilding", "white",
                    "religious", "khaki",
                    "residential", "green",
                    "service", "gold",
                    "transportation", "orange",

                    //Default color.
                    "grey"
                ])
            });

            //Add the layer to the map.
            MyMap.Layers.Add(layer);

            //Add a click event to the layer.
            MyMap.Events.Add("click", layer, FeatureLayerClicked);
        }

        /// <summary>
        /// Event that is triggered when a feature is clicked on the map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeatureLayerClicked(object? sender, MapEventArgs e)
        {
            if (e is MapMouseEventArgs args && args.Shapes.Count > 0)
            {
                //Update the content and position of the popup.
                popup.SetOptions(new PopupOptions
                {
                    //Assign a template to the properties of the feature. 
                    PopupTemplate = new PopupTemplate(args.Shapes[0].Properties),
                    Position = args.Position
                });

                //Open the popup.
                popup.Open(MyMap);
            }
        }
    }
}
