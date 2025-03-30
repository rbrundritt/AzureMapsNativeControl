using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Source
{
    [JsonConverter(typeof(BaseSourceIdConverter<GriddedDataSource>))]
    public class GriddedDataSource : DataSource
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        new internal GriddedDataSourceOptions _options = new GriddedDataSourceOptions()
        {
            GridType = GridType.Hexagon,
            CellWidth = 25000,
            MinCellWidth = 0,
            Coverage = 1,
            CenterLatitude = 0,
            MaxZoom = 18,
            MinZoom = 0,
            DistanceUnits = DistanceUnits.Meters
        };

        #endregion

        #region Constructor

        /// <summary>
        /// A data source for aggregating point data into cells of a grid system. 
        /// Point features will be extracted from atlas.Shape objects, but this shape will not be data bound.
        /// </summary>
        /// <param name="initDataImportUri">Url to a GeoJson file to import initially.</param>
        /// <param name="options">Options for the data source.</param>
        /// <param name="id">A unique ID for the data source.</param>
        /// <param name="syncFeatureUpdates">
        /// Specifies if features should be monitored for changes and dynamically updated in the map.
        /// This has a potential performance impact when making a lot of feature changes.
        /// Alternatively, after updating features call the "RefreshMap" method on the data source to update the map.
        /// You can also use the "UpdateFeature", UpdateFeatures" or "UpdateFeatureProperties" methods update a subset of features.
        /// </param>
        public GriddedDataSource(string? initDataImportUri = null, GriddedDataSourceOptions? options = null, string? id = null, bool syncFeatureUpdates = false) : base("atlas.source.GriddedDataSource", initDataImportUri, options, id, syncFeatureUpdates)
        {
            if (options != null)
            {
                GriddedDataSourceOptions.Merge(options, _options);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all points that are within the specified grid cell.
        /// </summary>
        /// <param name="cellId">The ID of the grid cell.</param>
        /// <returns></returns>
        public async Task<IList<Feature>?> GetCellChildrenAsync(string cellId)
        {
            if (Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<IList<Feature>?>(Map, "getCellChildren", Id, cellId);
            }

            return null;
        }

        /// <summary>
        /// Gets all grid cell polygons as a GeoJSON FeatureCollection.
        /// </summary>
        /// <returns></returns>
        public async Task<FeatureCollection?> GetGridCellsAsync()
        {
            if (Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<FeatureCollection?>(Map, "getGridCells", Id);
            }

            return null;
        }

        /// <summary>
        /// Gets the options for the data source.
        /// </summary>
        /// <returns></returns>
        public new GriddedDataSourceOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Sets the options for the data source.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(GriddedDataSourceOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the data source on the map. 
            if (GriddedDataSourceOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setDataSourceOptions", Id, _options);
            }
        }

        #endregion
    }
}
