using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Drawing;
using DistanceUnits = AzureMapsNativeControl.DistanceUnits;

namespace AzureMapsMauiSamples.Samples;

public partial class MeasuringToolSample : ContentPage
{
    /*********************************************************************************************************
     * This sample shows how to use the drawing tools to create a measuring tool.
     * 
     * This sample is based on: 
     * https://samples.azuremaps.com/?sample=create-a-measuring-tool
     *********************************************************************************************************/

    private DrawingManager drawingManager;

    public MeasuringToolSample()
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
                //Limit the buttons to line and area type shapes drawing and editting.
                Buttons = [
                    DrawingMode.DrawLine,
                    DrawingMode.DrawPolygon,
                    DrawingMode.DrawRectangle,
                    DrawingMode.DrawCircle,
                    DrawingMode.EditGeometry
                ],

                //Position the toolbar at the top right of the map.
                Position = ControlPosition.TopRight
            }
        };

        //Since the drawing manager needs to asynchronously connect to the map, lets wait for it to be initialized before trying to add events to it.
        drawingManager.OnInitialized += (s, e) =>
        {
            //Clear the data source of the drawing manager when the user enters into a drawing mode.
            MyMap.Events.Add("drawingmodechanged", drawingManager, (s, e) =>
            {
                if (e is DrawingManagerEventArgs args)
                {
                    switch (args.Mode)
                    {
                        case DrawingMode.DrawLine:
                        case DrawingMode.DrawPolygon:
                        case DrawingMode.DrawRectangle:
                        case DrawingMode.DrawCircle:
                            drawingManager.Source.Clear();
                            break;
                    }
                }
            });

            //Monitor the drawing process.
            MyMap.Events.Add("drawingchanging", drawingManager, DrawingChanged);
            MyMap.Events.Add("drawingchanged", drawingManager, DrawingChanged);

            //When the drawing is complete, put drawing manager into idle mode.

            MyMap.Events.Add("drawingcomplete", drawingManager, (s, e) =>
            {
                drawingManager.Mode = DrawingMode.Idle;

                if (e is DrawingManagerEventArgs args)
                {
                    MeasureFeature(args.Feature);
                }
            });
        };
    }

    private void DrawingChanged(object? sender, MapEventArgs e)
    {
        if (e is DrawingManagerEventArgs args)
        {
            MeasureFeature(args.Feature);
        }
    }

    private void MeasureFeature(Feature? feature)
    {
        if(feature != null)
        {
            string msg = "";

            //If the feature is a circle, create a polygon from its circle coordinates.
            if (feature.IsCircle())
            {
                var r = AtlasMath.ConvertDistance(feature.Properties.GetDouble("radius"), DistanceUnits.Meters, DistanceUnits.Miles, 2);
                var a = Math.Round(2 * Math.PI * r * r * 100) / 100;
                var p = Math.Round(2 * Math.PI * r * 100) / 100;

                msg = $"Radius: {r} mi\tArea: {a} sq mi\tPerimeter: {p} mi";
            }
            else
            {
                var g = feature.Geometry;
                Polygon? polygon = null;

                switch (g.Type)
                {
                    case GeoJsonType.LineString:
                        var l = (LineString)g;
                        var len = Math.Round(AtlasMath.GetLengthOfPath(l.Coordinates, DistanceUnits.Miles), 2);
                        msg = $"Length: {len} mi";

                        //Polygon's are rendered as lines when initially being drawn. 
                        if (drawingManager.Mode == DrawingMode.DrawPolygon)
                        {
                            polygon = new Polygon(l.Coordinates);
                        }
                        break;
                    case GeoJsonType.Polygon:
                        polygon = (Polygon)g;

                        //Get the perimeter of the polygon. The outer ring is the first element in the coordinates array.
                        var p = Math.Round(AtlasMath.GetLengthOfPath(polygon.Coordinates[0], DistanceUnits.Miles), 2);
                        msg = $"Perimeter: {p} mi";
                        break;
                }

                if (polygon != null)
                {
                    msg += $"\tArea: {AtlasMath.GetArea(polygon, AreaUnits.SquareMiles, 2)} sq mi";
                }
            }

            MeasureInfoLabel.Text = msg;
        }
    }
}