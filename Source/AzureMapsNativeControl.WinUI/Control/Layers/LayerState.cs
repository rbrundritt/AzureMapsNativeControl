using AzureMapsNativeControl.Control.Legends;
using AzureMapsNativeControl.Layer;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Layers
{
    /// <summary>
    /// States that define style options to be applied to layers when they are enabled/disabled.
    /// </summary>
    public class LayerState: ILayerState
    {
        #region Constructor

        /// <summary>
        /// States that define style options to be applied to layers when they are enabled/disabled.
        /// </summary>
        public LayerState()
        {

        }

        /// <summary>
        /// States that define style options to be applied to layers when they are enabled/disabled.
        /// </summary>
        /// <param name="layers">One or more layers that are impacted by the layer state.</param>
        public LayerState(IEnumerable<BaseLayer> layers)
        {
            SetLayers(layers);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The title of the layer state to display.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// A boolean indicating if the layer state is enabled or disabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Style options to apply to layer when state is enabled.
        /// </summary>
        [JsonPropertyName("enabledStyle")]
        public LayerOptions? EnabledStyle { get; set; }

        /// <summary>
        /// Style options to apply to layer when state is disabled.
        /// </summary>
        [JsonPropertyName("disabledStyle")]
        public LayerOptions? DisabledStyle { get; set; }

        /// <summary>
        /// Specifies if color and opacity styles replicated with other layer types that don't have an equivalent style defined. 
        /// This allows a single style to be used across different types of layers. For example, for a polygon layer, "color" would map to "fillColor".
        /// </summary>
        [JsonPropertyName("inflateStyles")]
        public bool InflateStyles { get; set; } = false;

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
