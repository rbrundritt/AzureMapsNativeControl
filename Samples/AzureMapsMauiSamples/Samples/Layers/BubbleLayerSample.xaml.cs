using AzureMapsNativeControl;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;

namespace AzureMapsMauiSamples.Samples;

public partial class BubbleLayerSample : ContentPage
{
    /*********************************************************************************************************
    * This sample shows the different ways to display points using a bubble layer.
    * 
    * This sample is based on these Azure Maps Web SDK samples: 
     * 
     * https://samples.azuremaps.com/?sample=data-driven-bubble-layer-styling
     * https://samples.azuremaps.com/?sample=bubble-layer-options
     *********************************************************************************************************/

    #region Private Properties

    //GeoJSON feed of significant earthquakes from the past 30 days. Sourced from the USGS Earthquake Hazards Program (https://www.usgs.gov/programs/earthquake-hazards) 
    private const string earthquakeFeed = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/significant_month.geojson";

    private BubbleLayer bubbleLayer;

    #endregion

    public BubbleLayerSample()
	{
		InitializeComponent();
	}

    //Wait for the map to be ready before interacting with it.
    private void MyMap_OnReady(object? sender, MapEventArgs e)
    {
        //Create a data source.
        var dataSource = new DataSourceLite(earthquakeFeed);
        MyMap.Sources.Add(dataSource);

        //Create a bubble layer.
        bubbleLayer = new BubbleLayer(dataSource, new BubbleLayerOptions
        {
            //Bubbles are made semi-transparent.
            Opacity = Expression<double>.Literal(0.75),

            //Color of each bubble based on the value of "mag" property using a color gradient of green, yellow, orange, and red.
            Color = new Expression<string>
            {
                "interpolate",
                new object[] { "linear" },
                new object[] { "get", "mag" },
                0, "green",
                5, "yellow",
                6, "orange",
                7, "red"
            },

            /*
            * Radius for each data point scaled based on the value of "mag" property.
            * When "mag" = 0, radius will be 2 pixels.
            * When "mag" = 8, radius will be 40 pixels.
            * All other "mag" values will be a linear interpolation between these values.
            */
            Radius = new Expression<double>
            {
                "interpolate",
                new object[] { "linear" },
                new object[] { "get", "mag" },
                0, 2,
                8, 40
            },
        });

        //Create a symbol layer using the same data source to render the magnitude as text above each bubble and add it to the map.
        var labelLayer = new SymbolLayer(dataSource, new SymbolLayerOptions
        {
            IconOptions = new IconOptions
            {
                //Hide the icon image.
                Image = Expression<string>.Literal("none")
            },
            TextOptions = new TextOptions
            {
                //Ensure all labels are displayed.
                AllowOverlap = true,
                IgnorePlacement = true,

                //An expression is used to concerte the "mag" property value into a string and appends the letter "m" to the end of it.
                TextField = new Expression<string> {
                    "concat",
                    new object[] { "to-string", new object[] { "get", "mag" } }, "m"
                },
                Size = Expression<int>.Literal(12)
            }
        });
        
        //Add the two layers to the map at the same time. This will trigger a single repaint of the map which is more efficient than adding the layers one by one.
        MyMap.Layers.AddRange(new List<BaseLayer> { bubbleLayer, labelLayer });

        //For this sample, set the initial color option from the picker.
        ColorPicker.SelectedIndex = 0;
    }

    private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Any CSS color value can be used as the color: https://www.w3schools.com/cssref/css_colors_legal.php
        //TIP: You can use the `Expression.Color` method to convert a .NET Color object into a CSS color string expression.

        var selectedColor = Helpers.GetSelectedPickerString(sender);

        Expression<string> colorExp;

        if (selectedColor.Equals("data_driven_style"))
        {
            //Set the color to a data driven style based on the "mag" property.
            colorExp = new Expression<string>
                {
                    "interpolate",
                    new object[] { "linear" },
                    new object[] { "get", "mag" },
                    0, "green",
                    5, "yellow",
                    6, "orange",
                    7, "red"
                };
        }
        else
        {
            //Use a CSS color string as the color. 
            colorExp = Expression<string>.Literal(selectedColor);
        }

        bubbleLayer.SetOptions(new BubbleLayerOptions
        {
            Color = colorExp
        });
    }

    private void UpdateRadius(object sender, EventArgs e)
    {
        //Check to see if the radius should be a data driven style based on the "mag" property.
        if (DataDrivenRadiusCheckBox.IsChecked)
        {
            //Disable the radius slider.
            RadiusSlider.IsEnabled = false;

            //Set the radius to a data driven style based on the "mag" property.
            bubbleLayer.SetOptions(new BubbleLayerOptions
            {
                Radius = new Expression<double>
                {
                    "interpolate",
                    new object[] { "linear" },
                    new object[] { "get", "mag" },
                    0, 2,
                    8, 40
                }
            });
        } 
        else
        {
            //Enable the radius slider.
            RadiusSlider.IsEnabled = true;

            var radius = Math.Round(RadiusSlider.Value, 2);

            //Set the radius to a fixed value.
            bubbleLayer.SetOptions(new BubbleLayerOptions
            {
                Radius = Expression<double>.Literal(radius)
            });

            RadiusLabel.Text = $"{radius}";
        }
    }

    private void OpacitySlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;
        var opacity = (double)Math.Round(slider.Value, 2);

        //Set the opacity.
        bubbleLayer.SetOptions(new()
        {
            Opacity = Expression<double>.Literal(opacity)
        });

        OpacityLabel.Text = $"Opacity: {opacity}";
    }

    private void StrokeWidthSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;

        //Set the stroke width.
        bubbleLayer.SetOptions(new()
        {
            StrokeWidth = Expression<int>.Literal((int)Math.Round(slider.Value))
        });

        StrokeWidthLabel.Text = $"Stroke Width: {Math.Round(slider.Value)}";
    }

    private void StrokeOpacitySlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = (Slider)sender;

        //Set the stroke opacity.
        bubbleLayer.SetOptions(new()
        {
            StrokeOpacity = Expression<double>.Literal(slider.Value)
        });

        StrokeOpacityLabel.Text = $"Stroke Opacity: {Math.Round(slider.Value, 2)}";
    }

    private void PitchAlignmentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var pitchAlignmentString = Helpers.GetSelectedPickerString(sender);

        //Convert the string value to the enum value.
        if (Enum.TryParse<PitchAlignment>(pitchAlignmentString, out var pitchAlignment))
        {
            //Set the pitch alignment.
            bubbleLayer.SetOptions(new()
            {
                PitchAlignment = pitchAlignment
            });
        }
    }
}