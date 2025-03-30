using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// Options for a data source.
    /// </summary>
    public class DataSourceOptions : BaseSourceOptions, IDeepCloneable<DataSourceOptions>
    {
        #region Public Properties

        /// <summary>
        /// The size of the buffer around each tile. 
        /// A buffer value of 0 will provide better performance but will be more likely to generate artifacts when rendering. 
        /// Larger buffers will produce left artifacts but will result in slower performance. 
        /// </summary>
        [JsonPropertyName("buffer")]
        public int? Buffer { get; set; }

        /// <summary>
        /// A boolean indicating if Point features in the source should be clustered or not. 
        /// If set to true, points will be clustered together into groups by radius. 
        /// </summary>
        [JsonPropertyName("cluster")]
        public bool? Cluster { get; set; }

        /// <summary>
        /// The radius of each cluster in pixels.
        /// </summary>
        [JsonPropertyName("clusterRadius")]
        public int? ClusterRadius { get; set; }

        /// <summary>
        /// The maximum zoom level in which to cluster points. 
        /// Defaults to one zoom less than maxZoom so that last zoom features are not clustered.
        /// </summary>
        [JsonPropertyName("clusterMaxZoom")]
        public double? ClusterMaxZoom { get; set; }

        /// <summary>
        /// Minimum number of points necessary to form a cluster if clustering is enabled.
        /// </summary>
        [JsonPropertyName("clusterMinPoints")]
        public int? ClusterMinPoints { get; set; }

        /// <summary>
        /// Defines custom properties that are calculated using expressions against all the points within each cluster and added to the properties of each cluster point.
        /// Schema: [operator: string, initialValue?: boolean | number, mapExpression: Expression] operator: 
        /// An expression function that is then applied to against all values calculated by the mapExpression for each point in the cluster. 
        /// Supported operators: 
        ///     - For numbers: +, *, max, min 
        ///     - For Booleans: all, any initialValue
        /// Optional, an initial value in which the first calculated value is aggregated against. 
        /// mapExpression: An expression that is applied against each point in the data set.
        /// </summary>
        [JsonPropertyName("clusterProperties")]
        public IDictionary<string, Expression>? ClusterProperties { get; set; }

        /// <summary>
        /// Specifies whether to calculate line distance metrics. This is required for line layers that specify lineGradient values.
        /// </summary>
        [JsonPropertyName("lineMetrics")]
        public bool? LineMetrics { get; set; }

        /// <summary>
        /// The Douglas-Peucker simplification tolerance that is applied to the data when rendering (higher means simpler geometries and faster performance). 
        /// </summary>
        [JsonPropertyName("tolerance")]
        public double? Tolerance { get; set; }

        /// <summary>
        /// An expression for filtering features prior to processing them for rendering.
        /// </summary>
        [JsonPropertyName("filter")]
        public Expression? Filter { get; set; }

        /// <summary>
        /// A property to use as a feature id (for feature state). 
        /// </summary>
        [JsonPropertyName("promoteId")]
        public string? PromoteId { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override DataSourceOptions DeepClone()
        {
            return new DataSourceOptions()
            {
                Buffer = Buffer,
                Cluster = Cluster,
                ClusterRadius = ClusterRadius,
                ClusterMaxZoom = ClusterMaxZoom,
                ClusterMinPoints = ClusterMinPoints,
                ClusterProperties = ClusterProperties,
                LineMetrics = LineMetrics,
                Tolerance = Tolerance,
                Filter = Filter?.DeepClone(),
                PromoteId = PromoteId,
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
        internal static bool Merge(DataSourceOptions source, DataSourceOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = BaseSourceOptions.Merge(source, target);

                if (source.Buffer != null && source.Buffer >= 0 && source.Buffer != target.Buffer)
                {
                    target.Buffer = source.Buffer;
                    hasChanges = true;
                }

                if (source.Cluster != null && source.Cluster != target.Cluster)
                {
                    target.Cluster = source.Cluster;
                    hasChanges = true;
                }

                if (source.ClusterRadius != null && source.ClusterRadius > 0 && source.ClusterRadius != target.ClusterRadius)
                {
                    target.ClusterRadius = source.ClusterRadius;
                    hasChanges = true;
                }

                if (source.ClusterMaxZoom != null && source.ClusterMaxZoom > 0 && source.ClusterMaxZoom <= 24 && source.ClusterMaxZoom != target.ClusterMaxZoom)
                {
                    target.ClusterMaxZoom = source.ClusterMaxZoom;
                    hasChanges = true;
                }

                if (source.ClusterMinPoints != null && source.ClusterMinPoints >= 2 && source.ClusterMinPoints != target.ClusterMinPoints)
                {
                    target.ClusterMinPoints = source.ClusterMinPoints;
                    hasChanges = true;
                }

                if (source.ClusterProperties != null && source.ClusterProperties != target.ClusterProperties)
                {
                    target.ClusterProperties = source.ClusterProperties;
                    hasChanges = true;
                }

                if (source.LineMetrics != null && source.LineMetrics != target.LineMetrics)
                {
                    target.LineMetrics = source.LineMetrics;
                    hasChanges = true;
                }

                if (source.Tolerance != null && source.Tolerance >= 0 && source.Tolerance != target.Tolerance)
                {
                    target.Tolerance = source.Tolerance;
                    hasChanges = true;
                }

                if (source.Filter != null && source.Filter != target.Filter)
                {
                    target.Filter = source.Filter;
                    hasChanges = true;
                }

                if (source.PromoteId != null && source.PromoteId != target.PromoteId)
                {
                    target.PromoteId = source.PromoteId;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
