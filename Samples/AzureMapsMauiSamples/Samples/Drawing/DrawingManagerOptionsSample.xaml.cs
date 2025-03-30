using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Drawing;
using AzureMapsNativeControl.Layer;

namespace AzureMapsMauiSamples.Samples;

public partial class DrawingManagerOptionsSample : ContentPage
{
    /*********************************************************************************************************
     * This sample shows how to set the options of a drawing manager.
     * 
     * This sample is based on: 
     * https://samples.azuremaps.com/?sample=drawing-manager-options
     * https://samples.azuremaps.com/?sample=change-drawing-rendering-style
     * https://samples.azuremaps.com/?sample=drawing-toolbar-options
     *********************************************************************************************************/

    private DrawingManager drawingManager;
    private Random random = new Random();

    public DrawingManagerOptionsSample()
    {
        InitializeComponent();
    }

    private void MyMap_OnReady(object sender, AzureMapsNativeControl.MapEventArgs e)
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
    }

    #region Options input handlers

    private void DrawingModePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Programmatically set the mode of the drawing manager.
        drawingManager.Mode = (DrawingMode)Enum.Parse(typeof(DrawingMode), Helpers.GetSelectedPickerString(sender));
    }

    private void InteractionTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Specify how the user can interact with the map to draw shapes.
        drawingManager.InteractionType = (DrawingInteractionType)Enum.Parse(typeof(DrawingInteractionType), Helpers.GetSelectedPickerString(sender));
    }

    private void FreehandIntervalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Specify the minimum pixel distance the mouse must move before a new position is added to the shape when drawing freehand.
        drawingManager.FreehandInterval = int.Parse(Helpers.GetSelectedPickerString(sender));
    }

    private void ShapeDraggingEnabledCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Specify if shapes should be draggable when editting.
        drawingManager.ShapeDraggingEnabled = e.Value;
    }

    private void ShapeRotationEnabledCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Specify if shapes should be rotatable when editting.
        drawingManager.ShapeRotationEnabled = e.Value;
    }

    private void ShowToolbarCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Show or hide the drawing toolbar.
        drawingManager.ToolbarOptions.Visible = e.Value;
    }

    private void ToolbarButtonItemCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        //Get a list of all the drawing mode buttons that are selected.

        var selectedButtons = new List<DrawingMode>();

        if (DrawPoint.IsChecked)
        {
            selectedButtons.Add(DrawingMode.DrawPoint);
        }

        if (DrawLine.IsChecked)
        {
            selectedButtons.Add(DrawingMode.DrawLine);
        }

        if (DrawPolygon.IsChecked)
        {
            selectedButtons.Add(DrawingMode.DrawPolygon);
        }

        if (DrawCircle.IsChecked)
        {
            selectedButtons.Add(DrawingMode.DrawCircle);
        }

        if (DrawRectangle.IsChecked)
        {
            selectedButtons.Add(DrawingMode.DrawRectangle);
        }

        if (EditGeometry.IsChecked)
        {
            selectedButtons.Add(DrawingMode.EditGeometry);
        }

        if (EraseGeometry.IsChecked)
        {
            selectedButtons.Add(DrawingMode.EraseGeometry);
        }

        //Set the toolbar button list.
        drawingManager.ToolbarOptions.Buttons = selectedButtons;
    }

    private string[] symbolIcons = ["marker-blue", "marker-black", "marker-darkblue", "marker-red", "marker-yellow", "pin-blue", "pin-darkblue", "pin-red"];

    private void RandomizeLayerStyles_Clicked(object sender, EventArgs e)
    {
        //Change the line widths.
        var lineWidth = random.Next(1, 10);
        
        //Make the preview lines dashed.
        var dashSize = random.Next(3, 10);
        var dashArray = new List<int> { dashSize, dashSize };

        //Setting layer options on the drawing manager will append the options to the existing options.
        drawingManager.LineLayerOptions = new LineLayerOptions { 
            StrokeColor = Expression<string>.Literal(Helpers.GetRandomColorString()),
            StrokeWidth = Expression<int>.Literal(lineWidth)
        };

        drawingManager.LinePreviewLayerOptions = new LineLayerOptions
        {
            StrokeColor = Expression<string>.Literal(Helpers.GetRandomColorString()),
            StrokeWidth = Expression<int>.Literal(lineWidth),
            StrokeDashArray = dashArray
        };

        drawingManager.PolygonLayerOptions = new PolygonLayerOptions
        {
            FillColor = Expression<string>.Literal(Helpers.GetRandomColorString())
        };

        drawingManager.PolygonPreviewLayerOptions = new PolygonLayerOptions
        {
            FillColor = Expression<string>.Literal(Helpers.GetRandomColorString())
        };

        lineWidth = random.Next(1, 10);

        drawingManager.PolygonOutlineLayerOptions = new LineLayerOptions
        {
            StrokeColor = Expression<string>.Literal(Helpers.GetRandomColorString()),
            StrokeWidth = Expression<int>.Literal(lineWidth)
        };

        drawingManager.PolygonOutlinePreviewLayerOptions = new LineLayerOptions
        {
            StrokeColor = Expression<string>.Literal(Helpers.GetRandomColorString()),
            StrokeWidth = Expression<int>.Literal(lineWidth),
            StrokeDashArray = dashArray
        };

        //Create a random scale an apply it to the point layer icons.
        //Only a subset of symbol layer icon options are supported by the drawing manager.
        drawingManager.PointLayerOptions = new DrawingPointLayerOptions
        {
            //Assign a random icon for the main and preview images. Custom icons can be loaded into the map image sprite and used here as well.
            Image = symbolIcons[random.Next(0, symbolIcons.Length)],
            PreviewImage = symbolIcons[random.Next(0, symbolIcons.Length)],

            //Scale the images.
            Size = random.NextDouble() + 0.5
        };
    }

    #endregion
}