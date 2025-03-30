using AzureMapsNativeControl.Control.Layers;
using AzureMapsNativeControl.Control.Legends;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A group of layer items. 
    /// </summary>
    public class LayerGroup
    {
        #region Constructor

        /// <summary>
        /// A group of layer items. 
        /// </summary>
        public LayerGroup()
        {

        }

        /// <summary>
        /// A group of layer items. 
        /// </summary>
        /// <param name="layers">One or more layers that are impacted by the layer state.</param>
        public LayerGroup(IEnumerable<BaseLayer> layers)
        {
            SetLayers(layers);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A CSS class to add to the generated layer group. 
        /// </summary>
        [JsonPropertyName("cssClass")]
        public string? CssClass { get; set; }

        /// <summary>
        /// How the layer state items are presented.
        /// </summary>
        [JsonPropertyName("layout")]
        public LayerListLayout Layout { get; set; } = LayerListLayout.Checkbox;

        /// <summary>
        /// The title of the layer group.
        /// </summary>
        [JsonPropertyName("groupTitle")]
        public string? GroupTitle { get; set; }

        /// <summary>
        /// One or more layers that are impacted by the layer state.
        /// </summary>
        [JsonPropertyName("layers")]
        public IList<string>? Layers { get; set; }

        /// <summary>
        /// The states of the layer.
        /// </summary>
        [JsonPropertyName("items")]
        public IList<ILayerState>? Items { get; set; }

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
