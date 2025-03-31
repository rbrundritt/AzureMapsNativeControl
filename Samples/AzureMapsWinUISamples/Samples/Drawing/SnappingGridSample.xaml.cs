using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Drawing;
using AzureMapsNativeControl.Layer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class SnappingGridSample : Page
    {
        /*********************************************************************************************************
      * This sample shows how to use the snapping grid feature of the drawing tools.
      * 
      * This sample is based on: 
      * https://samples.azuremaps.com/?sample=snap-grid-options
      * https://samples.azuremaps.com/?sample=use-a-snapping-grid
      *********************************************************************************************************/

        private SnapGridManager snapGrid;
        private DrawingManager? drawingManager = null;

        public SnappingGridSample()
        {
            InitializeComponent();
        }

        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Create an instance of the Snap grid manager.
            snapGrid = new SnapGridManager(MyMap)
            {
                ShowGrid = true
            };

            //Use a draggable marker as a method to test snapping.
            var marker = new HtmlMarker(new HtmlMarkerOptions
            {
                Draggable = true,
                Position = new Position(-122.33, 47.6)
            });

            MyMap.Markers.Add(marker);

            //When the marker is done being dragged, snap it to the grid.
            MyMap.Events.Add("dragend", marker, async (s, e) =>
            {
                var position = marker.GetOptions().Position;

                if (position != null)
                {
                    //Snap the marker to the grid.
                    var snappedPositions = await snapGrid.SnapPositionsAsync([position]);
                    marker.SetOptions(new HtmlMarkerOptions { Position = snappedPositions[0] });
                }
            });

            //Create an instance of the drawing manager and display the drawing toolbar.
            drawingManager = new DrawingManager(MyMap)
            {
                ToolbarOptions = new DrawingToolbarOptions
                {
                    //Position the toolbar at the top right of the map.
                    Position = ControlPosition.TopRight
                }
            };

            //Since the drawing manager needs to asynchronously connect to the map, lets wait for it to be initialized before trying to add events to it.
            drawingManager.OnInitialized += (s, e) =>
            {
                //Monitor for when a line drawing has been completed.
                MyMap.Events.Add("drawingcomplete", drawingManager, OnDrawingComplete);
            };
        }

        private async void OnDrawingComplete(object? sender, MapEventArgs eventArgs)
        {
            //Convert the map event args to a DrawingManagerEventArgs object.
            var e = (DrawingManagerEventArgs)eventArgs;

            // Exit drawing mode.
            drawingManager.Mode = DrawingMode.Idle;

            if (e.Feature != null)
            {
                var snappedFeature = await snapGrid.SnapFeatureAsync(e.Feature);

                //The drawing manager uses a DataSourceLite instance to store the features, so we have to manually tell it to update the feature in the source.
                //Since the input feature and the snapped feature have the same ID, the data source will be able to locate and update the feature in the source.
                drawingManager.Source.UpdateFeature(snappedFeature);
            }
        }

        private void EnabledCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (drawingManager != null && sender is CheckBox cbx)
            {
                //Enable or disable the snapping function.
                snapGrid.Enabled = cbx.IsChecked == true;
            }
        }

        private void RemoveDuplicatesCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (drawingManager != null && sender is CheckBox cbx)
            {
                //Specifies duplicate sequential positions should be removed when snapping.
                snapGrid.RemoveDuplicates = cbx.IsChecked == true;
            }
        }

        private void ResolutionSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (drawingManager != null && sender is Slider slider)
            {
                //The resolution specifies the size of the snapping grid in pixels.
                //The grid will be square and relative to the nearest integer zoom level.
                //The grid will scale by a factor of 2 relative to physical real-world area with each zoom level.

                var resolution = (int)Math.Round(slider.Value);

                if (snapGrid.Resolution != resolution)
                {
                    snapGrid.Resolution = resolution;
                    Resolution.Text = $"Resolution: {resolution}";
                }
            }
        }

        private void ShowGridCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (drawingManager != null && sender is CheckBox cbx)
            {
                //Specifies if grid lines should be displayed on the map.
                snapGrid.ShowGrid = cbx.IsChecked == true;
            }
        }

        private void SimplifyCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (drawingManager != null && sender is CheckBox cbx)
            {
                //Specifies if a Douglas-Peucker simplification should occur while snapping to create smoother lines.
                snapGrid.Simplify = cbx.IsChecked == true;
            }
        }

        private void GridColorPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drawingManager != null)
            {
                var colorString = Helpers.GetSelectedPickerString(sender);

                //Set the grid layer stroke color option.
                snapGrid.GridLayerOptions = new LineLayerOptions { StrokeColor = Expression<string>.Literal(colorString) };
            }
        }
    }
}