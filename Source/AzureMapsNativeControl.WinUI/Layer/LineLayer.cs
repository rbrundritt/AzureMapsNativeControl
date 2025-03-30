using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Layer
{
    /// <summary>
    /// Renders line data on the map. Can be used with SimpleLine, SimplePolygon, CirclePolygon, LineString, MultiLineString, Polygon, and MultiPolygon objects.
    /// </summary>
    public class LineLayer : BaseLayer, IMapEventTarget
    {
        #region Private Properties 

        [JsonInclude]
        [JsonPropertyName("options")]
        private LineLayerOptions _options = LineLayerOptions.Defaults();

        #endregion

        #region Constructor 

        /// <summary>
        /// Renders line data on the map. Can be used with SimpleLine, SimplePolygon, CirclePolygon, LineString, MultiLineString, Polygon, and MultiPolygon objects.
        /// </summary>
        /// <param name="source">Data source for the layer.</param>
        /// <param name="options">Options for the layer.</param>
        /// <param name="id">A unique ID for the layer.</param>
        public LineLayer(BaseSource source, LineLayerOptions? options = null, string? id = null) : 
            base("atlas.layer.LineLayer", source, id)
        {
            if (options != null)
            {
                LineLayerOptions.Merge(options, _options);
            }
        }

        internal LineLayer(BaseSource source, LineLayerOptions? options = null, string? id = null, bool allowNonUniqueId = false) :
           base("atlas.layer.LineLayer", source, id, allowNonUniqueId)
        {
            if (options != null)
            {
                LineLayerOptions.Merge(options, _options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the layer.
        /// </summary>
        /// <returns></returns>
        public override LineLayerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Removes the stroke dash array from the layer.
        /// </summary>
        public async void ClearStrokeDashArray()
        {
            if (Map != null)
            {
                _options.StrokeDashArray = null;
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "clearStrokeDashArray", Id);
            }
        }

        /// <summary>
        /// Sets the options of the layer.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(LineLayerOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the layer on the map. 
            if (LineLayerOptions.Merge(options, _options) && Map != null)
            {
                //Logic to work around the complexity of the StrokeDashArray property.
                if (options.StrokeDashArray != null && options.StrokeDashArray.Count == 0)
                {
                    _options.StrokeDashArray = null;
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "clearStrokeDashArray", Id);
                }

                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setLayerOptions", Id, _options);
            }
        }

        #endregion
    }
}