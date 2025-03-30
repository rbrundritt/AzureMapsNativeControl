using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using System.Windows.Controls;

namespace AzureMapsWPFSamples.Samples
{
    public partial class HtmlMarkerPulseAnimationSample : Page
    {
        /*********************************************************************************************************
        * This sample shows how to pulse animate the position of a HTML marker on the map using CSS.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=html-marker-pulse-animation
        *********************************************************************************************************/

        public HtmlMarkerPulseAnimationSample()
        {
            InitializeComponent();
        }

        private void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //Add custom CSS animation class to the map view.
            MyMap.JsInterlop.AddRawCss(
                @".pulseIcon {
                    display: block;
                    width: 10px;
                    height: 10px;
                    border-radius: 50%;
                    background: orange;
                    border: 2px solid white;
                    cursor: pointer;
                    box-shadow: 0 0 0 rgba(0, 204, 255, 0.4);
                    animation: pulse 3s infinite;
                }

                .pulseIcon:hover {
                    animation: none;
                }

                @keyframes pulse {
                0% { box-shadow: 0 0 0 0 rgba(0, 204, 255, 0.4); }
                70% { box-shadow: 0 0 0 50px rgba(0, 204, 255, 0); }
                100% { box-shadow: 0 0 0 0 rgba(0, 204, 255, 0); }
            }");

            //Create a HTML marker and add it to the map.
            var marker = new HtmlMarker(new HtmlMarkerOptions
            {
                HtmlContent = "<div class=\"pulseIcon\"></div>",
                Position = new Position(-110, 45)
            });

            MyMap.Markers.Add(marker);
        }
    }
}
