using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Source;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's sources. Exposed through the sources property of the atlas.Map class. Cannot be instantiated by the user.
    /// </summary>
    public sealed class SourceManager : BaseMapEntityCollection<BaseSource, BaseSourceOptions>
    {
        #region Constructor 

        /// <summary>
        /// A manager for the map control's sources. Exposed through the sources property of the atlas.Map class. Cannot be instantiated by the user.
        /// </summary>
        /// <param name="map"></param>
        public SourceManager(Map map): base(map, "addSource", "removeSources")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets information on the non-user added sources in the map. 
        /// These are typically sources that are added by the map control itself such as the base map sources.
        /// </summary>
        public async Task<IList<AzureMapsSource>> GetBasemapSources()
        {
            var sources = new List<AzureMapsSource>();

            if (_map != null)
            {
                //Get all the ID's of use created sources.
                var userSourceIds = new List<string>();

                foreach (var s in this)
                {
                    userSourceIds.Add(s.Id);
                }

                //Get all the sources on the map.
                var info = await _map.JsInterlop.InvokeJsMethodAsync<List<RawBasemapSourceInfo>>(_map, "getBasemapSourcesInfo", userSourceIds);

                if (info != null)
                {
                    foreach (RawBasemapSourceInfo i in info)
                    {
                        var l = new AzureMapsSource(i);
                        l.Map = _map;
                        sources.Add(l);
                    }
                }
            }

            return sources;
        }

        /// <summary>
        /// Returns a boolean indicating if the source is loaded or not.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>A boolean indicating if the source is loaded or not.</returns>
        public async Task<bool> IsSourceLoaded(BaseSource source)
        {
            return await IsSourceLoaded(source.Id);
        }

        /// <summary>
        /// Returns a boolean indicating if the source is loaded or not.
        /// </summary>
        /// <param name="sourceId">the Id of the source</param>
        /// <returns>A boolean indicating if the source is loaded or not.</returns>
        public async Task<bool> IsSourceLoaded(string sourceId)
        {
            if (_map != null && !string.IsNullOrEmpty(sourceId))
            {
                return await _map.JsInterlop.InvokeJsMethodAsync<bool>(_map, "isSourceLoaded", sourceId);
            }

            return false;
        }

        /// <summary>
        /// Gets the state of a feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>The state of the feature.</returns>
        public async Task<KeyValuePair<string, object>?> GetFeatureState(Feature feature)
        {
            //Find which data source the feature belongs to.
            if (feature != null && !string.IsNullOrWhiteSpace(feature.Id))
            {
                var source = FindSource(feature);

                if (source != null)
                {
                    return await GetFeatureState(feature.Id, source.Id);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the state of a feature.
        /// </summary>
        /// <param name="featureId">The feature Id to set the state on.</param>
        /// <param name="sourceId">The id of the source the feature is in.</param>
        /// <param name="sourceLayer">A source layer if the shape is in a vector tile source.</param>
        /// <returns>The state of the feature.</returns>
        public async Task<KeyValuePair<string, object>?> GetFeatureState(string featureId, string sourceId, string? sourceLayer = null)
        {
            if (_map != null && !string.IsNullOrEmpty(featureId) && !string.IsNullOrEmpty(sourceId))
            {
                return await _map.JsInterlop.InvokeJsMethodAsync<KeyValuePair<string, object>>(_map, "getFeatureState", featureId, sourceId, sourceLayer);
            }

            return null;
        }

        /// <summary>
        /// Removes the state or a single key value of the state of a feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="stateKey">The state key to remove.</param>
        public async Task RemoveFeatureState(Feature feature, string? stateKey = null)
        {
            //Find which data source the shape belongs to.
            if (feature != null && !string.IsNullOrWhiteSpace(feature.Id))
            {
                var source = FindSource(feature);

                if (source != null)
                {
                    await RemoveFeatureState(feature.Id, source.Id, null, stateKey);
                }
            }
        }

        /// <summary>
        /// Removes the state or a single key value of the state of a feature.
        /// </summary>
        /// <param name="featureId">The feature or feature Id to set the state on.</param>
        /// <param name="sourceId">The id of the source the feature or feature is in.</param>
        /// <param name="sourceLayer">A source layer if the feature is in a vector tile source.</param>
        /// <param name="stateKey">The state key to remove.</param>
        public async Task RemoveFeatureState(string featureId, string sourceId, string? sourceLayer = null, string? stateKey = null)
        {
            if (_map != null && !string.IsNullOrEmpty(featureId) && !string.IsNullOrEmpty(sourceId))
            {
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeFeatureState", featureId, sourceId, sourceLayer, stateKey);
            }
        }

        /// <summary>
        /// Sets the state of the feature by passing in a key value pair object.
        /// </summary>
        /// <param name="feature">The feature to set the state on.</param>
        /// <param name="state">The state information.</param>
        public async Task SetFeatureState(Feature feature, KeyValuePair<string, object> state)
        {
            //Find which data source the shape belongs to.
            if (feature != null && !string.IsNullOrWhiteSpace(feature.Id))
            {
                var source = FindSource(feature);

                if (source != null) {
                    await SetFeatureState(feature.Id, source.Id, state);
                }
            }
        }

        /// <summary>
        /// Sets the state of the feature by passing in a key value pair object.
        /// </summary>
        /// <param name="featureId">The feature Id to set the state on.</param>
        /// <param name="sourceId">The id of the source the feature is in.</param>
        /// <param name="state">The state information.</param>
        /// <param name="sourceLayer">A source layer if the feature is in a vector tile source.</param>
        public async Task SetFeatureState(string featureId, string sourceId, KeyValuePair<string, object> state, string? sourceLayer = null)
        {
            if (_map != null && !string.IsNullOrEmpty(featureId) && !string.IsNullOrEmpty(sourceId))
            {
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "setFeatureState", featureId, sourceId, state, sourceLayer);
            }
        }

        /// <summary>
        /// Returns all rendered shape features in a source that match a specified filter. If the source is a vector tile source, a source layer name needs to be specified.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <param name="sourceLayer">The data layer within a vector tile source.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>> GetRenderedShapes(BaseSource source, Expression<bool>? filter = null, string? sourceLayer = null)
        {
            return await GetRenderedShapes(source.Id, filter, sourceLayer);
        }

        /// <summary>
        /// Returns all rendered shape features in a source that match a specified filter. If the source is a vector tile source, a source layer name needs to be specified.
        /// </summary>
        /// <param name="sourceId">The id of the source.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <param name="sourceLayer">The data layer within a vector tile source.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>> GetRenderedShapes(string sourceId, Expression<bool>? filter = null, string? sourceLayer = null)
        {
            if (_map != null)
            {
                var response = await _map.JsInterlop.InvokeJsMethodAsync<RawMapMsg>(_map, "getSourceRenderedShapes", sourceId, filter, sourceLayer);

                if (response != null)
                {
                    return response.GetFeatures(_map);
                }
            }

            return new List<Feature>(0);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tries to find the source that a feature belongs to.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>The source the feature belongs to or null.</returns>
        internal BaseSource? FindSource(Feature feature)
        {
            var f = GetDataSourceFeature(feature);

            if (f != null && !string.IsNullOrWhiteSpace(feature.SourceId))
            {
                return GetById(feature.SourceId);
            }

            return null;
        }

        /// <summary>
        /// Tries to find the source that a feature belongs to.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal Feature? GetDataSourceFeature(Feature feature)
        {
            if (string.IsNullOrWhiteSpace(feature.Id))
            {
                return null;
            }

            //Check to see if a source id is set on the feature.
            if (!string.IsNullOrWhiteSpace(feature.SourceId))
            {
                var source = GetById(feature.SourceId);

                if (source is DataSource ds)
                {
                    return ds.GetFeatureById(feature.Id);
                }
            }

            if (!string.IsNullOrWhiteSpace(feature.Id))
            {
                //Loop through the sources and try to find the feature.
                foreach (var s in this)
                {
                    if (s is DataSource ds)
                    {
                        var f = ds.GetFeatureById(feature.Id);

                        if (f != null)
                        {
                            f.SourceId = ds.Id;
                            return f;
                        }
                    }
                }
            }

            return null;
        }

        internal override async Task UpdateEntitiesAsync(IList<BaseSource>? newEntities, bool replaceAll = false)
        {
            if (replaceAll)
            {
                await RemoveEntitiesAsync(null, replaceAll);
            }

            //Add new entities.
            if (newEntities != null && newEntities.Count > 0)
            {
                var addTasks = new List<Task>();

                foreach (var s in newEntities)
                {
                    //Make sure the entity is not null or an Azure Maps source. 
                    if (s != null && s is not AzureMapsSource)
                    {
                        //Check to see if the source is used in a different map. Ensure the source is not already in the collection.
                        if (s.Map == null || s.Map != _map)
                        {
                            s.Map = null;
                            _collection.Add(s);

                            //Specifies if the source is a raster tile source, other than elevation tile source.
                            bool isTileSource = false;

                            //Specifies if the tile source should be added (vector and elevation tile sources should).
                            bool shouldAddTileSource = false;

                            if (s is TileSource ts)
                            {
                                isTileSource = true;
                                shouldAddTileSource = ts.IsVectorTiles || ts.ElevationEncoding != null;
                            }

                            //Don't need/allow adding AzureMapsSource, Raster tiles sources or elevation source.
                            if (!(isTileSource && !shouldAddTileSource))
                            {
                                if (isTileSource)
                                {
                                    addTasks.Add(_map.JsInterlop.InvokeJsMethodAsync(_map, "addSource", s.JsNamespace, s.Id, s));
                                }
                                else if (s is GriddedDataSource gds)
                                {
                                    var fc = gds.ToFeatureCollection(false);

                                    addTasks.Add(_map.JsInterlop.InvokeJsMethodAsync(_map, "addSource", gds.JsNamespace, gds.Id, gds.GetOptions(), fc));
                                }
                                else if (s is DataSource ds)
                                {
                                    var fc = ds.ToFeatureCollection(false);

                                    addTasks.Add(_map.JsInterlop.InvokeJsMethodAsync(_map, "addSource", ds.JsNamespace, ds.Id, ds.GetOptions(), fc));
                                }
                                else if (s is DataSourceLite dsl)
                                {
                                    addTasks.Add(_map.JsInterlop.InvokeJsMethodAsync(_map, "addSource", dsl.JsNamespace, dsl.Id, dsl.GetOptions()));
                                }
                            }

                            s.Map = _map;
                        }
                    }
                }

                await Task.WhenAll(addTasks);

                foreach(var e in newEntities)
                {
                    if (e is IMapEventTarget el)
                    {
                        _map.Events.ReAddEvents(el);
                    }
                }
            }
        }

        #endregion
    }
}
