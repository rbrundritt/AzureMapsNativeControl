using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Layers
{
    /// <summary>
    /// States for an input range that defines style options to be applied to layers when a range slider changes.
    /// </summary>
    public class RangeLayerState : ILayerState
    {
        #region Constructor

        /// <summary>
        /// States for an input range that defines style options to be applied to layers when a range slider changes.
        /// </summary>
        public RangeLayerState()
        {

        }

        /// <summary>
        /// States for an input range that defines style options to be applied to layers when a range slider changes.
        /// </summary>
        /// <param name="layers">One or more layers that are impacted by the layer state.</param>
        public RangeLayerState(IEnumerable<BaseLayer> layers)
        {
            SetLayers(layers);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum value of the range input. 
        /// </summary>
        [JsonPropertyName("min")]
        public double Min { get; set; } = 0;

        /// <summary>
        /// The maximum value of the range input.
        /// </summary>
        [JsonPropertyName("max")]
        public double Max { get; set; } = 1;

        /// <summary>
        /// The incremental step value of the range input.
        /// </summary>
        [JsonPropertyName("step")]
        public double Step { get; set; } = 0.1;

        /// <summary>
        /// The initial value of the range input.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; } = 1;

        /// <summary>
        /// Style options to apply to layer when state changes. Use a placeholder of '{rangeValue}' in your expression. 
        /// This will be replaced with the value from the range.
        /// </summary>
        [JsonPropertyName("style")]
        public LayerOptions? Style { get; set; }

        /// <summary>
        /// The title of the layer state to display.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; } = "{rangeValue}";

        /// <summary>
        /// Specifies if color and opacity styles replicated with other layer types that don't have an equivalent style defined. 
        /// This allows a single style to be used across different types of layers. For example, for a polygon layer, "color" would map to "fillColor".
        /// </summary>
        [JsonPropertyName("inflateStyles")]
        public bool InflateStyles { get; set; } = false;

        /// <summary>
        /// Specifies if the style should be updated when the oninput event fires (while sliding). 
        /// Warning, this can trigger multiple updates in a very short period of time.
        /// </summary>
        [JsonPropertyName("updateOnInput")]
        public bool UpdateOnInput { get; set; } = false;

        /// <summary>
        /// One or more layers that are impacted by the layer state.
        /// </summary>
        [JsonPropertyName("layers")]
        public IList<string>? Layers { get; set; }

        /// <summary>
        /// One or more legends to display for the layer group. These legends only hide based on zoom level.
        /// </summary>
        [JsonPropertyName("legends")]
        public IList<BaseLegend>? Legends { get; set; }

        /// <summary>
        /// Min zoom level that this layer group should appear. 
        /// </summary>
        [JsonPropertyName("minZoom")]
        public int MinZoom { get; set; } = 0;

        /// <summary>
        /// Max zoom level that this layer group should appear. 
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public int MaxZoom { get; set; } = 24;

        /// <summary>
        /// Specifies how a layer group or state should be treated when the map zoom level falls outside of the items min and max zoom range. 
        /// </summary>
        [JsonPropertyName("zoomBehavior")]
        public ZoomBehavior ZoomBehavior { get; set; } = ZoomBehavior.Disable;

        /// <summary>
        /// The number formatting to apply to the value when displaying it in the label. 
        /// Uses [Number.toLocaleString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/toLocaleString).
        /// </summary>
        [JsonPropertyName("numberFormat")]
        public JSNumberFormatOptions NumberFormat { get; set; } = new JSNumberFormatOptions
        {
            MaximumSignificantDigits = 2
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the layers.
        /// </summary>
        /// <param name="layers"></param>
        public void SetLayers(IEnumerable<BaseLayer> layers)
        {
            Layers = new List<string>();

            foreach (var layer in layers)
            {
                Layers.Add(layer.Id);
            }
        }

        #endregion
    }
}