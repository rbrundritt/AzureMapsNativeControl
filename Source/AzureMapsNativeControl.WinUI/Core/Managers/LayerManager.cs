using AzureMapsNativeControl.Animations;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using AzureMapsNativeControl.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's layers.
    /// </summary>
    public sealed class LayerManager: BaseMapEntityCollection<BaseLayer, LayerOptions>
    {
        #region Constructor

        /// <summary>
        /// A manager for the map control's layers.
        /// </summary>
        /// <param name="map">Map instance manager is attached to.</param>
        public LayerManager(Map map) : base(map, "addLayers", "removeLayers")
        {
        }

        #endregion

        #region Add methods

        /// <summary>
        /// Adds a layer before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layer">The layers to add.</param>
        /// <param name="beforeLayer">The layer to insert before.</param>
        public async void Add(BaseLayer layer, BaseLayer beforeLayer)
        {
            await UpdateLayersAsync([layer], false, beforeLayer?.Id);
        }

        /// <summary>
        /// Adds a layer before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layer">The layers to add.</param>
        /// <param name="beforeLayer">The layer to insert before.</param>
        public async Task AddAsync(BaseLayer layer, BaseLayer? beforeLayer)
        {
            await UpdateLayersAsync([layer], false, beforeLayer?.Id);
        }

        /// <summary>
        /// Adds a layer before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layer">The layers to add.</param>
        /// <param name="beforeLayerId">The id of the layer to insert before.</param>
        public async void Add(BaseLayer layer, string? beforeLayerId)
        {
            await UpdateLayersAsync([layer], false, beforeLayerId);
        }

        /// <summary>
        /// Adds a layer before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layer">The layers to add.</param>
        /// <param name="beforeLayerId">The id of the layer to insert before.</param>
        public async Task AddAsync(BaseLayer layer, string? beforeLayerId)
        {
            await UpdateLayersAsync([layer], false, beforeLayerId);
        }

        /// <summary>
        /// Adds a set of layers before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layers">The layers to add.</param>
        /// <param name="beforeLayer">The layer to insert before.</param>
        public async void AddRange(IList<BaseLayer> layers, BaseLayer beforeLayer)
        {
            await UpdateLayersAsync(layers, false, beforeLayer.Id);
        }

        /// <summary>
        /// Adds a set of layers before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layers">The layers to add.</param>
        /// <param name="beforeLayer">The layer to insert before.</param>
        public async Task AddRangeAsync(IList<BaseLayer> layers, BaseLayer beforeLayer)
        {
            await UpdateLayersAsync(layers, false, beforeLayer.Id);
        }

        /// <summary>
        /// Adds a set of layers before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layers">The layers to add.</param>
        /// <param name="beforeLayerId">The id of the layer to insert before.</param>
        public async void AddRange(IList<BaseLayer> layers, string beforeLayerId)
        {
            await UpdateLayersAsync(layers, false, beforeLayerId);
        }

        /// <summary>
        /// Adds a set of layers before the specified layer. The specified layer may be in the collection, or a base map layer.
        /// </summary>
        /// <param name="layers">The layers to add.</param>
        /// <param name="beforeLayerId">The id of the layer to insert before.</param>
        public async Task AddRangeAsync(IList<BaseLayer> layers, string? beforeLayerId)
        {
            await UpdateLayersAsync(layers, false, beforeLayerId);
        }

        #endregion

        #region Layer Manager Specific Methods

        /// <summary>
        /// Gets information on the non-user added layers in the map. 
        /// These are typically layers that are added by the map control itself such as the base map layers.
        /// </summary>
        public async Task<IList<AzureMapsLayer>> GetBasemapLayersAsync()
        {
            var layers = new List<AzureMapsLayer>();

            if (_map != null)
            {
                //Get all the ID's of use created layers.
                var userLayerIds = new List<string>();

                foreach (var s in this)
                {
                    userLayerIds.Add(s.Id);
                }

                //Get all the sources on the map.
                var info = await _map.JsInterlop.InvokeJsMethodAsync<List<BasemapLayerInfo>>(_map, "getBasemapLayersInfo", userLayerIds);

                if (info != null)
                {
                    var basemapSources = await _map.Sources.GetBasemapSources();

                    foreach (var i in info)
                    {
                        //Get the source for the layer by Id.
                        var source = basemapSources.FirstOrDefault(s => s.Id == i.SourceId);

                        layers.Add(new AzureMapsLayer(i, source, _map));
                    }
                }
            }

            return layers;
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="point">A point that intersects with the shapes.</param>
        /// <param name="layer">The layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(Position point, BaseLayer layer, Expression<bool>? filter = null)
        {
            if (point is null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (layer is null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            return await GetRenderedShapesAsync(point, new List<string>() { layer.Id }, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="point">A point that intersects with the shapes.</param>
        /// <param name="layerId">The layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(Position point, string layerId, Expression<bool>? filter = null)
        {
            return await GetRenderedShapesAsync(point, new List<string>() { layerId }, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="point">A point that intersects with the shapes.</param>
        /// <param name="layers">An array of layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(Position point, IList<BaseLayer> layers, Expression<bool>? filter = null)
        {
            var layerIds = new List<string>();

            foreach (var l in layers)
            {
                layerIds.Add(l.Id);
            }

            return await GetRenderedShapesAsync(point, layerIds, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="point">A point that intersects with the shapes.</param>
        /// <param name="layerIds">An array of layer ids to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(Position? point = null, IList<string>? layerIds = null, Expression<bool>? filter = null)
        {
            if (_map != null)
            {
                var response = await _map.JsInterlop.InvokeJsMethodAsync<RawMapMsg>(_map, "getLayerRenderedShapes", point, layerIds, filter);

                if (response != null)
                {
                    return response.GetFeatures(_map);
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="bbox">A bounding box that intersects with the shapes.</param>
        /// <param name="layer">The layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(BoundingBox bbox, BaseLayer layer, Expression<bool>? filter = null)
        {
            return await GetRenderedShapesAsync(bbox, new List<string>() { layer.Id }, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="bbox">A bounding box that intersects with the shapes.</param>
        /// <param name="layerId">The layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(BoundingBox bbox, string layerId, Expression<bool>? filter = null)
        {
            return await GetRenderedShapesAsync(bbox, new List<string>() { layerId }, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="bbox">A bounding box that intersects with the shapes.</param>
        /// <param name="layers">An array of layer to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(BoundingBox bbox, IList<BaseLayer> layers, Expression<bool>? filter = null)
        {
            var layerIds = new List<string>();

            foreach (var l in layers)
            {
                layerIds.Add(l.Id);
            }

            return await GetRenderedShapesAsync(bbox, layerIds, filter);
        }

        /// <summary>
        /// Retrieve all Shapes and GeoJSON features that are visible on the map that are in a DataSource or VectorTileSource.
        /// </summary>
        /// <param name="bbox">A bounding box that intersects with the shapes.</param>
        /// <param name="layerIds">An array of layer ids to limit the query to.</param>
        /// <param name="filter">A filter to limit the query to.</param>
        /// <returns>Visible shapes and features that are matcht the query requirements.</returns>
        public async Task<IList<Feature>?> GetRenderedShapesAsync(BoundingBox bbox, IList<string> layerIds, Expression<bool>? filter = null)
        {
            if (_map != null)
            {
                var response = await _map.JsInterlop.InvokeJsMethodAsync<RawMapMsg>(_map, "getLayerRenderedShapes", bbox, layerIds, filter);

                if (response != null)
                {
                    return response.GetFeatures(_map);
                }
            }

            return null;
        }

        #endregion

        #region Move Methods

        /// <summary>
        /// Moves a layer to a new position in the layer stack.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="beforeLayerId"></param>
        public async void Move(BaseLayer layer, string beforeLayerId = "")
        {
            //Check to see if the layer has been added to the map yet.
            if (Contains(layer))
            {
                int beforeIdx = IndexOf(beforeLayerId);
                layer.BeforeLayerId = (beforeIdx >= 0) ? beforeLayerId : "";

                //Remove the layer from the collection.
                _collection.Remove(layer);

                //Add the layer if the before layer is not found.
                if (beforeIdx == -1)
                {
                    _collection.Add(layer);
                }
                else
                {
                    //Insert the layer before the specified layer.
                    _collection.Insert(beforeIdx, layer);
                }

                await _map.JsInterlop.InvokeJsMethodAsync(_map, "moveLayer", layer.Id, beforeLayerId);
            }
            else
            {
                //Add the layer to the map before the specified layer.
                Add(layer, beforeLayerId);
            }
        }

        /// <summary>
        /// Moves a layer to a new position in the layer stack.
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="beforeLayerId"></param>
        public void Move(string layerId, string beforeLayerId = "")
        {
            if (!string.IsNullOrWhiteSpace(layerId))
            {
                //Check to see if the layer is in the layer manager.
                var layer = GetById(layerId);

                if (layer != null)
                {
                    Move(layer, beforeLayerId);
                }
            }
        }

        #endregion

        #region Private Methods

        internal override async Task UpdateEntitiesAsync(IList<BaseLayer>? newLayers, bool replaceAll = false)
        {
            await UpdateLayersAsync(newLayers, replaceAll, null);
        }

        internal async Task UpdateLayersAsync(IList<BaseLayer>? newEntities, bool replaceAll = false, string? beforeLayerId = null)
        {
            if (replaceAll)
            {
                await RemoveEntitiesAsync(null, replaceAll);
            }
            
            //Add new entities.
            if (newEntities != null && newEntities.Count > 0)
            {
                var newLayers = new List<BaseLayer>();

                int insertIdx = Count;

                //Determine if the layer should be added before another layer.
                if (!string.IsNullOrWhiteSpace(beforeLayerId))
                {
                    var beforeLayer = GetById(beforeLayerId);

                    //If no before layer ID specified or no match found, just add the layer to the end of the collection.
                    if (beforeLayer != null && Contains(beforeLayer))
                    {
                        insertIdx = IndexOf(beforeLayer);
                    }
                }

                bool requiresAnimationModule = false;

                foreach (var l in newEntities)
                {
                    //Make sure the entity is not null. Ensure the layer is not already in the collection.
                    if (l != null && l._allowManagerAdd && l.Map != _map)
                    {
                        //Capture any sources that haven't been added to the source manager of the map yet.
                        if (l.Source != null && l.Source is BaseSource baseSource && !_map.Sources.Contains(l.Source))
                        {
                            _map.Sources.Add(baseSource);
                        }

                        if(l is AnimatedTileLayer)
                        {
                            requiresAnimationModule = true;
                        }

                        //Check to see if the entity is already attached to a map.
                        if (l.Map != null)
                        {
                            //Remove the entity from the other map.
                            l.Map = null;
                        }

                        if (beforeLayerId != null)
                        {
                            l.BeforeLayerId = beforeLayerId;
                        }

                        //Add to collection of layers that need adding.
                        newLayers.Add(l);
                    }
                }

                if (requiresAnimationModule)
                {
                    await MapAnimations.LoadAnimationModule(_map);
                }

                if (newLayers.Count > 0)
                {
                    _collection.InsertRange(insertIdx, newLayers);
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, _addMethodName, newLayers);

                    //Attach the map.
                    foreach (var l in newLayers)
                    {
                        l.Map = _map;

                        //For animated tile layers we need to register the animation.
                        if (l is AnimatedTileLayer)
                        {
                            var aml = (l as AnimatedTileLayer);
                            var animation = new FrameBasedAnimation(l.Map, l.Id);
                            animation.NumberOfFrames = aml._tileSources.Count();
                            aml._animation = animation;
                        }

                        if(l is IMapEventTarget el)
                        {
                            _map.Events.ReAddEvents(el);
                        }
                    }
                }
            }
        }

        #endregion
    }
}