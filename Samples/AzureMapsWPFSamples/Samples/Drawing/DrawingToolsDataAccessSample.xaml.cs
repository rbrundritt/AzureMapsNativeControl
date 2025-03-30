using System.Windows.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Drawing;
using System.Text.Json;
using System.Windows;

namespace AzureMapsWPFSamples.Samples
{
    public partial class DrawingToolsDataAccessSample : Page
    {
        /*********************************************************************************************************
         * This sample shows how to import/export data when using the drawing tools. 
         * This additionally shows how you can put a shape into edit mode programmatically.
         * 
         * This sample is based on: 
         * https://samples.azuremaps.com/?sample=load-data-into-drawing-manager
         * https://samples.azuremaps.com/?sample=get-drawn-shapes-from-drawing-manager
         *********************************************************************************************************/

        private string firstFeatureId = "myFirstFeatureId";
        private DrawingManager drawingManager;

        public DrawingToolsDataAccessSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create an instance of the drawing manager and display the drawing toolbar.
            drawingManager = new DrawingManager(MyMap)
            {
                ToolbarOptions = new DrawingToolbarOptions
                {
                    //Position the toolbar at the top right of the map.
                    Position = ControlPosition.TopRight
                }
            };

            //Import some data into the drawing manager.

            //The source property of the data source is a DataSourceLite instance. 
            //There are several ways to add/import data into a DataSourceLite instance.

            //Example 1: Create a shape fature and add it to the map. 
            drawingManager.Source.Add(new Feature(new LineString([
                new Position(-122.27577,47.55938),
                new Position(-122.29705,47.60662),
                new Position(-122.22358,47.6367)
                ]), new PropertiesTable() {
                    { "name", "My Line" }
                }, firstFeatureId)); //Note: that we are setting the id of the feature here and will use the ID later to put the feature into edit mode.

            //Example 2: Import GeoJSON data from a file.
            drawingManager.Source.ImportDataFromUrl("data/geojson/randomFeatures.json");
        }

        private async void GetDrawnFeaturesButton_Clicked(object sender, RoutedEventArgs e)
        {
            //Since the drawing manager uses a DataSourceLite instance as its source,
            //we have to asynchronously retrieve the features from the source since they are not stored in .NET.
            var features = await drawingManager.Source.GetFeaturesAsync();

            if (features != null)
            {
                //Now that we have the features, we can do something with them.

                //For this example, we will just display the features as a string in a text window.
                GeoJsonTextWindow.Text = JsonSerializer.Serialize(features, new JsonSerializerOptions() { WriteIndented = true });
            }
        }

        private void EditFeatureWithIdButton_Clicked(object sender, RoutedEventArgs e)
        {
            //We can use the "Edit" method of the DrawingManager to put a feature into edit mode. We can pass in either a feature instance or its ID.
            //If the feature doesn't exist in the data source, it will be added. 

            //For this example, we will use the ID of the first feature that we added to the data source.
            drawingManager.Edit(firstFeatureId);

            GeoJsonTextWindow.Text = $"Putting Feature with ID \"{firstFeatureId}\" into edit mode.";
        }

        private async void EditLastFeaturesButton_Clicked(object sender, RoutedEventArgs e)
        {
            //Since the drawing manager uses a DataSourceLite instance as its source,
            //we have to asynchronously retrieve the features from the source since they are not stored in .NET.
            var fc = await drawingManager.Source.GetFeaturesAsync();

            if (fc != null && fc.Features.Count > 0)
            {
                //Get the last feature.
                var feature = fc.Features[fc.Features.Count - 1];

                //Put the shape into edit mode.
                //We can use the "Edit" method of the DrawingManager to put a feature into edit mode. We can pass in either a feature instance or its ID.
                //If the feature doesn't exist in the data source, it will be added. 
                drawingManager.Edit(feature);

                GeoJsonTextWindow.Text = $"Putting Feature with ID \"{feature.Id}\" into edit mode.";
            }
        }
    }
}
