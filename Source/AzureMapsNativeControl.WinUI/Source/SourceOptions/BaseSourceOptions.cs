using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// Base options for a source.
    /// </summary>
    [JsonDerivedType(typeof(DataSourceOptions))]
    [JsonDerivedType(typeof(GriddedDataSourceOptions))]
    public class BaseSourceOptions: IDeepCloneable<BaseSourceOptions>
    {
        /// <summary>
        /// Minimum zoom that data is available in the source.
        /// </summary>
        [JsonPropertyName("minZoom")]
        public int? MinZoom { get; set; }

        /// <summary>
        /// Maximum zoom level that data is available in the source.
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public int? MaxZoom { get; set; }

        #region Public Methods

        /// <inheritdoc/>
        public virtual BaseSourceOptions DeepClone()
        {
            return new BaseSourceOptions
            {
                MinZoom = MinZoom,
                MaxZoom = MaxZoom
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        internal static bool Merge(BaseSourceOptions source, BaseSourceOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.MinZoom != null && source.MinZoom >= 0 && source.MinZoom <= 24 && source.MinZoom != target.MinZoom)
                {
                    target.MinZoom = source.MinZoom;
                    hasChanges = true;
                }

                if (source.MaxZoom != null && source.MaxZoom >= 0 && source.MaxZoom <= 24 && source.MaxZoom != target.MaxZoom)
                {
                    target.MaxZoom = source.MaxZoom;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
