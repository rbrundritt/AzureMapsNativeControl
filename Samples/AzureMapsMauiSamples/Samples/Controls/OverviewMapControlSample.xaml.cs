using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;

namespace AzureMapsMauiSamples.Samples;

public partial class OverviewMapControlSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows how to use the bring data into view control. 
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
    * 
    * https://samples.azuremaps.com/?sample=mini-overview-map-options
    * https://samples.azuremaps.com/?sample=mini-overview-map
    *********************************************************************************************************/

    public OverviewMapControlSample()
    {
        InitializeComponent();
    }

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
    {
        //The overview map control was added in the XAML. Can optionally add it here as well.

        //var myControl =  new OverviewMapControl()
        //{
        //    Position = ControlPosition.TopRight
        //}
        //MyMap.Controls.Add(myControl);
    }

    #region Options input handlers

    private void OverlayPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            var overlayName = Helpers.GetSelectedPickerString(sender);

            //Convert string name to OverviewMapOverlay enum.
            if (Enum.TryParse(overlayName, out OverviewMapOverlay overlay))
            {
                MyOverviewMap.Overlay = overlay;
            }
        }
    }

    private void MapStylePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            var styleName = Helpers.GetSelectedPickerString(sender);

            //Convert string name to MapStyle enum.
            if (Enum.TryParse(styleName, out MapStyle style))
            {
                MyOverviewMap.MapStyle = style;
            }
        }
    }

    private void ShapePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            var shapeName = Helpers.GetSelectedPickerString(sender);

            //Convert string name to OverviewMapShape enum.
            if (Enum.TryParse(shapeName, out OverviewMapShape shape))
            {
                MyOverviewMap.Shape = shape;
            }
        }
    }

    private void StylePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            var styleString = Helpers.GetSelectedPickerString(sender);

            //Update the style of the control.
            switch (styleString)
            {
                case "Light":
                    MyOverviewMap.Style = ControlStyle.Light;
                    break;
                case "Dark":
                    MyOverviewMap.Style = ControlStyle.Dark;
                    break;
                case "CSS Color":
                    //Use any CSS color to set the background color of the control.
                    MyOverviewMap.StyleColor = "#6010df";
                    break;
                case "Auto":
                default:
                    MyOverviewMap.Style = ControlStyle.Auto;
                    break;
            }
        }
    }

    private void HeightSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            int height = (int)e.NewValue;

            if (MyOverviewMap.Height != height)
            {
                //Update the height of the overview map.
                MyOverviewMap.Height = height;
                HeightLabel.Text = $"Height: {height}";
            }
        }
    }

    private void WidthSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int width = (int)e.NewValue;

        if (MyOverviewMap.Width != width)
        {
            //Update the width of the overview map.
            MyOverviewMap.Width = width;
            WidthLabel.Text = $"Width: {width}";
        }
    }

    private void ZoomOffsetSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            int zoomOffset = (int)e.NewValue;

            if (MyOverviewMap.ZoomOffset != zoomOffset)
            {
                //Update the zoom offset of the overview map.
                MyOverviewMap.ZoomOffset = zoomOffset;
                ZoomOffsetLabel.Text = $"Zoom offset: {zoomOffset}";
            }
        }
    }

    private void SyncZoomCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the sync zoom property.
            MyOverviewMap.SyncZoom = e.Value;
        }
    }

    private void SyncBearingPitchCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the sync bearing pitch property.
            MyOverviewMap.SyncBearingPitch = e.Value;
        }
    }

    private void InteractiveCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the interactive property.
            MyOverviewMap.Interactive = e.Value;
        }
    }

    private void MiminizedCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the minimized property.
            MyOverviewMap.Minimized = e.Value;
        }
    }

    private void ShowToggleCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the show toggle property.
            MyOverviewMap.ShowToggle = e.Value;
        }
    }

    private void VisibleCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Update the visible property.
            MyOverviewMap.Visible = e.Value;
        }
    }

    private void MarkerOptionsDraggableCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (MyOverviewMap != null)
        {
            MyOverviewMap.MarkerOptions = new HtmlMarkerOptions()
            {
                Draggable = e.Value
            };
        }
    }

    private void RandomizeMarkerStyle_Clicked(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Radomize the HTML marker style of the marker overlay.
            MyOverviewMap.MarkerOptions = new HtmlMarkerOptions()
            {
                Color = Helpers.GetRandomColorString(),
                Text = Helpers.Rand.Next(100).ToString()
            };
        }
    }

    private void RandomizeLineStyle_Clicked(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Radomize the line style of the area overlay.
            MyOverviewMap.LineLayerOptions = new AzureMapsNativeControl.Layer.LineLayerOptions()
            {
                StrokeColor = Expression<string>.Literal(Helpers.GetRandomColorString()),
                StrokeWidth = Expression<int>.Literal(Helpers.Rand.Next(1, 3))
            };
        }
    }

    private void RandomizePolygonStyle_Clicked(object sender, EventArgs e)
    {
        if (MyOverviewMap != null)
        {
            //Radomize the polygon style of the area overlay.
            MyOverviewMap.PolygonLayerOptions = new AzureMapsNativeControl.Layer.PolygonLayerOptions()
            {
                FillColor = Expression<string>.Literal(Helpers.GetRandomColorString())
            };
        }
    }

    #endregion
}