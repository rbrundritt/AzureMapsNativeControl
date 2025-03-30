using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// Options for a gridded data source.
    /// </summary>
    public class GriddedDataSourceOptions : DataSourceOptions, IDeepCloneable<GriddedDataSourceOptions>
    {
        #region Public Properties

        /// <summary>
        /// Defines custom properties that are calculated using expressions against all the points within each grid cell and added to the properties of each grid cell polygon.
        /// Note that only a subset of expressions are supported. https://github.com/Azure-Samples/azure-maps-gridded-data-source/blob/main/docs/supported-expressions.md
        /// </summary>
        [JsonPropertyName("aggregateProperties")]
        public Dictionary<string, Expression>? AggregateProperties { get; set; }

        /// <summary>
        /// The shape of the data bin to generate. Default: `hexagon`
        /// </summary>
        [JsonPropertyName("gridType")]
        public GridType? GridType { get; set; }

        /// <summary>
        /// The spatial width of each cell in the grid in the specified distance units. Default: `25000`
        /// </summary>
        [JsonPropertyName("cellWidth")]
        public double? CellWidth { get; set; }

        /// <summary>
        /// The minimium cell width to use by the coverage and scaling operations. Will be snapped to the `cellWidth` if greater than that value. Default: `0`
        /// </summary>
        [JsonPropertyName("minCellWidth")]
        public double? MinCellWidth { get; set; }

        /// <summary>
        /// The distance units of the cellWidth option. Default: `meters`
        /// </summary>
        [JsonPropertyName("distanceUnits")]
        public DistanceUnits? DistanceUnits { get; set; }

        /// <summary>
        /// The aggregate property to calculate the min/max values over the whole data set. Can be an aggregate property or `point_count`.
        /// </summary>
        [JsonPropertyName("scaleProperty")]
        public string? ScaleProperty { get; set; }

        /// <summary>
        /// A data driven expression that customizes how the scaling function is done. This expression has access to the properties of the cell (CellInfo) and the following two properties; 
        /// `min` - The minimium aggregate value across all cells in the data source.
        /// `max` - The maximium aggregate value across all cells in the data source.
        /// A linear scaling function based on the "point_count" property is used by default `scale = (point_count - min)/(max - min)`. 
        /// Default: `['/', ['-', ['get', 'point_count'], ['get', 'min']], ['-', ['get', 'max'], ['get', 'min']]]`
        /// Note that only a subset of expressions are supported. https://github.com/Azure-Samples/azure-maps-gridded-data-source/blob/main/docs/supported-expressions.md
        /// </summary>
        [JsonPropertyName("scaleExpression")]
        public Expression? ScaleExpression { get; set; }

        /// <summary>
        ///  A number between 0 and 1 that specifies how much area a cell polygon should consume within the grid cell.
        ///  This applies a multiplier to the scale of all cells.If `scaleProperty` is specified, this will add additional scaling. 
        ///  Default: `1`
        /// </summary>
        [JsonPropertyName("coverage")]
        public double? Coverage { get; set; }

        /// <summary>
        /// The latitude value used to calculate the pixel equivalent of the cellWidth. Default: `0`
        /// </summary>
        [JsonPropertyName("centerLatitude")]
        public double? CenterLatitude { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override GriddedDataSourceOptions DeepClone()
        {
            return new GriddedDataSourceOptions
            {                
                AggregateProperties = AggregateProperties?.ToDictionary(k => k.Key, v => v.Value),
                GridType = GridType,
                CellWidth = CellWidth,
                MinCellWidth = MinCellWidth,
                DistanceUnits = DistanceUnits,
                ScaleProperty = ScaleProperty,
                ScaleExpression = ScaleExpression,
                Coverage = Coverage,
                CenterLatitude = CenterLatitude,

                // Copy other properties from the base classMinZoom = MinZoom,
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
        internal static bool Merge(GriddedDataSourceOptions source, GriddedDataSourceOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = DataSourceOptions.Merge(source, target);

                if (source.AggregateProperties != null && source.AggregateProperties.Count > 0)
                {
                    if (target.AggregateProperties == null)
                    {
                        target.AggregateProperties = source.AggregateProperties.ToDictionary(k => k.Key, v => v.Value);
                        hasChanges = true;
                    }
                    else
                    {
                        foreach (var item in source.AggregateProperties)
                        {
                            if (!target.AggregateProperties.ContainsKey(item.Key) || target.AggregateProperties[item.Key] != item.Value)
                            {
                                target.AggregateProperties[item.Key] = item.Value;
                                hasChanges = true;
                            }
                        }
                    } 
                }

                if (source.GridType != null && source.GridType != target.GridType)
                {
                    target.GridType = source.GridType;
                    hasChanges = true;
                }

                if (source.CellWidth != null && source.CellWidth >= 0 && source.CellWidth != target.CellWidth)
                {
                    target.CellWidth = source.CellWidth;
                    hasChanges = true;
                }

                if (source.MinCellWidth != null && source.MinCellWidth >= 0 && source.MinCellWidth != target.MinCellWidth)
                {
                    target.MinCellWidth = source.MinCellWidth;
                    hasChanges = true;
                }

                if (source.DistanceUnits != null && source.DistanceUnits != target.DistanceUnits)
                {
                    target.DistanceUnits = source.DistanceUnits;
                    hasChanges = true;
                }

                if (source.ScaleProperty != null && source.ScaleProperty != target.ScaleProperty)
                {
                    target.ScaleProperty = source.ScaleProperty;
                    hasChanges = true;
                }

                if (source.ScaleExpression != null && source.ScaleExpression != target.ScaleExpression)
                {
                    target.ScaleExpression = source.ScaleExpression;
                    hasChanges = true;
                }

                if (source.Coverage != null && source.Coverage >= 0 && source.Coverage <= 1 && source.Coverage != target.Coverage)
                {
                    target.Coverage = source.Coverage;
                    hasChanges = true;
                }

                if (source.CenterLatitude != null && source.CenterLatitude >= -90 && source.CenterLatitude <= 90 && source.CenterLatitude != target.CenterLatitude)
                {
                    target.CenterLatitude = source.CenterLatitude;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
