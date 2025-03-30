using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Source;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Base constructor for all sources.
    /// </summary>
    [JsonDerivedType(typeof(DataSource))]
    [JsonDerivedType(typeof(DataSourceLite))]
    [JsonDerivedType(typeof(GriddedDataSource))]
    [JsonDerivedType(typeof(TileSource))]
    [JsonDerivedType(typeof(MBTileSource))]
    [JsonDerivedType(typeof(ZipFileTileSource))]
    [JsonDerivedType(typeof(AzureMapsSource))]
    public abstract class BaseSource : MapEntity<BaseSourceOptions>
    {
        #region Constructor

        /// <summary>
        /// Base constructor for all sources.
        /// </summary>
        /// <param name="jsNamespace"> The JavaScript namespace of the source class.</param>
        /// <param name="id">A unique ID for the source.</param>
        public BaseSource(string jsNamespace, string? id = null): base(jsNamespace, id)
        {
        }

        /// <summary>
        /// Base constructor for all sources.
        /// </summary>
        /// <param name="jsNamespace"></param>
        /// <param name="id"></param>
        /// <param name="allowNonUniqueId"></param>
        internal BaseSource(string jsNamespace, string? id = null, bool allowNonUniqueId = false) : base(jsNamespace, id, allowNonUniqueId)
        {
        }

        #endregion

        #region Internal Methods

        internal void RemoveLinkedLayers(Map oldMap)
        {
            //When detaching a source, any layers using it must also be removed.
            if(oldMap != null)
            {
                //Find all layers that use this source.
                var layers = new List<BaseLayer>();

                foreach (var layer in oldMap.Layers)
                {
                    if (layer.Source == this)
                    {
                        layers.Add(layer);
                    }
                }

                //Remove all layers that use this source.
                oldMap.Layers.RemoveRange(layers);
            }
        }

        #endregion
    }
}
