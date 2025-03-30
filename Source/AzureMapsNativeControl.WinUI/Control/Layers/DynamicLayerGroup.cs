using AzureMapsNativeControl.Control.Legends;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Layers
{
    /// <summary>
    /// Options for a dynamic list of users layers within the map.
    /// </summary>
    public class DynamicLayerGroup
    {
        #region Constructor

        /// <summary>
        /// Options for a dynamic list of users layers within the map.
        /// </summary>
        public DynamicLayerGroup()
        {

        }

        /// <summary>
        /// Options for a dynamic list of users layers within the map.
        /// </summary>
        /// <param name="layerFilter">A list of layers to filter on.</param>
        public DynamicLayerGroup(IEnumerable<BaseLayer> layerFilter)
        {
            SetLayerFilter(layerFilter);
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
        /// One or more layer ids to filter out.
        /// </summary>
        [JsonPropertyName("layerFilter")]
        public IList<string>? LayerFilter { get; set; }

        /// <summary>
        /// One or more legends to display for the layer group. These legends only hide based on zoom level.
        /// </summary>
        [JsonPropertyName("legends")]
        public IList<BaseLegend>? Legends { get; set; }

        /// <summary>
        /// Property name of the layers metadata that should be used as a label. If not specified, the layers ID will be used. 
        /// Values will be passed through the resx option to support localization if specified.
        /// </summary>
        [JsonPropertyName("labelProperty")]
        public string? LabelProperty { get; set; }

        /// <summary>
        /// The index to insert this layer group within controls other layer group collections.
        /// </summary>
        [JsonPropertyName("layerGroupIdx")]
        public int LayerGroupIdx { get; set; } = 0;

        /// <summary>
        /// Specifies how a layer group or state should be treated when the map zoom level falls outside of the items min and max zoom range. 
        /// </summary>
        [JsonPropertyName("zoomBehavior")]
        public ZoomBehavior ZoomBehavior { get; set; } = ZoomBehavior.Disable;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the layer filter.
        /// </summary>
        /// <param name="layerFilter"></param>
        public void SetLayerFilter(IEnumerable<BaseLayer> layerFilter)
        {
            LayerFilter = new List<string>();

            foreach (var layer in layerFilter)
            {
                LayerFilter.Add(layer.Id);
            }
        }

        #endregion
    }
}
