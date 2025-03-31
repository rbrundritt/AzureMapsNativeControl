using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Drawing;
using Microsoft.UI.Xaml.Controls;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class DrawingToolsSample : Page
    {
        /*********************************************************************************************************
          * This sample shows how to use the drawing tools.
          * 
          * This sample is based on: 
          * https://samples.azuremaps.com/?search=drawing&sample=add-drawing-toolbar-to-map
          *********************************************************************************************************/

        public DrawingToolsSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
        {
            //Create an instance of the drawing manager and pass in the map instance.
            //Only create a drawing manager after the map ready event has been fired.
            var drawingManager = new DrawingManager(MyMap)
            {
                ToolbarOptions = new DrawingToolbarOptions
                {
                    //NOTE: Unlike the web SDK, the toolbar is visible by default.
                    //It can be hidden if desired and the drawing modes can be changed programmatically.
                    //Visible = false,

                    //Position the toolbar at the top right of the map.
                    Position = ControlPosition.TopRight
                }
            };
        }
    }
}
