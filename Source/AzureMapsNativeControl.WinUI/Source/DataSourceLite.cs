using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#if (MAUI && WINDOWS) || WINUI
using Windows.Storage.Streams;
#endif

namespace AzureMapsNativeControl.Source
{
    /// <summary>
    /// A light weight version of the Data Source class that does not store a copy of the data in .NET (only on the web map SDK). 
    /// </summary>
    [JsonConverter(typeof(BaseSourceIdConverter<DataSourceLite>))]
    public class DataSourceLite: BaseSource
    {
        #region Private Properties

        [JsonInclude]
        [JsonPropertyName("options")]
        internal DataSourceOptions _options = new DataSourceOptions()
        {
            Buffer = 128,
            Cluster = false,
            ClusterMaxZoom = 21,
            ClusterRadius = 50,
            LineMetrics = false,
            MaxZoom = 18,
            Tolerance = 0.375,
            MinZoom = 0,
            PromoteId = Constants.AzureMapsShapeID
        };

        [JsonIgnore]
        internal BoundingBox? _bbox = null;

        [JsonIgnore]
        private List<string> waitingDataImports = new List<string>();

        [JsonIgnore]
        private List<Feature> waitingFeatures = new List<Feature>();

        [JsonIgnore]
        private HashSet<string> _ids = new HashSet<string>();

        [JsonIgnore]
        private bool _syncFeatureUpdates = false;

        #endregion

        #region Constructor

        /// <summary>
        /// A light weight version of the Data Source class that does not store a copy of the data in .NET (only on the web map SDK). 
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
        public DataSourceLite(string? initDataImportUri = null, DataSourceOptions? options = null, string? id = null, bool syncFeatureUpdates = false) : base("atlas.source.DataSource", id)
        {
            if (options != null)
            {
                DataSourceOptions.Merge(options, _options);
            }

            if (!string.IsNullOrWhiteSpace(initDataImportUri))
            {
                ImportDataFromUrl(initDataImportUri);
            }

            //Handle when the map is added or removed.
            MapUpdated = (Map? oldMap, Map? newMap) =>
            {
                if (oldMap != null)
                {
                    //If the source is being detached, remove any layers that are using this source.
                    base.RemoveLinkedLayers(oldMap);
                }

                if (newMap != null)
                {
                    //Import any waiting data.
                    foreach (var url in waitingDataImports)
                    {
                        ImportDataFromUrl(url);
                    }
                    waitingDataImports.Clear();

                    //Import waiting features.
                    AddShapes(waitingFeatures.ToList(), false);
                    waitingFeatures.Clear();
                }
            };
        }

        #endregion


        /// <summary>
        /// Checks to see if a file name, based on file extension, or MimeType is supported by this data source.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="mimeType">The file mime type.</param>
        /// <returns></returns>
        public static bool IsSupportedFileType(string? fileName = null, string? mimeType = null)
        {
            //Both should support the same file types. This can be modified in the future if different file types are supported.
            return DataSource.IsSupportedFileType(fileName, mimeType);
        }

        #region Add Methods

        /// <summary>
        /// Adds a GeoJson Feature object into the data source.
        /// </summary>
        /// <param name="feature">The GeoJson object to add.</param>
        /// <returns>A shape version of the GeoJson Feature.</returns>
        public Feature Add(Feature feature)
        {
            AddShapes([feature]);
            return feature;
        }

        /// <summary>
        /// Adds a GeoObject into the data source.
        /// </summary>
        /// <param name="geoObject">The GeoObject to add.</param>
        /// <returns>A shape version of the GeoObject.</returns>
        public Feature? Add(GeoObject geoObject)
        {
            if (geoObject != null)
            {
                var f = Feature.FromGeoObject(geoObject);

                if (f != null)
                {
                    AddShapes([f]);
                    return f;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a GeoJson Geometry object into the data source.
        /// </summary>
        /// <param name="geom">The GeoJson object to add.</param>
        /// <param name="properties">The properties of the geometry.</param>
        /// <param name="id">The ID of the geometry.</param>
        /// <returns>A shape version of the GeoJson Geometry.</returns>
        public Feature Add(Geometry geom, PropertiesTable? properties = null, string? id = null)
        {
            var f = new Feature(geom, properties, id);
            AddShapes([f]);
            return f;
        }

        /// <summary>
        /// Adds a collection of GeoObjects into the data source.
        /// </summary>
        /// <param name="geoCollection">The GeoCollection to add.</param>
        /// <returns>The shapes versions of the added GeoObjects.</returns>
        public IList<Feature> AddRange(GeoCollection geoCollection)
        {
            var features = new List<Feature>(geoCollection.Count);

            foreach (var g in geoCollection)
            {
                var f = Feature.FromGeoObject(g);

                if (f != null)
                {
                    features.Add(f);
                }
            }

            AddRange(features);

            return features;
        }

        /// <summary>
        /// Adds a GeoJson Feature Collection to the data source.
        /// </summary>
        /// <param name="featureCollection">the Feature Collection to add.</param>
        /// <returns>The shapes versions of the added features.</returns>
        public IList<Feature> AddRange(FeatureCollection featureCollection)
        {
            return AddRange(featureCollection.Features);
        }

        /// <summary>
        /// Adds a collection of GeoObjects into the data source.
        /// </summary>
        /// <param name="geoObjects">The GeoObjects to add.</param>
        /// <returns>The shapes versions of the added GeoObjects.</returns>
        public IList<Feature> AddRange(IList<GeoObject> geoObjects)
        {
            var features = new List<Feature>(geoObjects.Count());

            foreach (var g in geoObjects)
            {
                var f = Feature.FromGeoObject(g);

                if (f != null)
                {
                    features.Add(f);
                }
            }

            AddRange(features);

            return features;
        }

        /// <summary>
        /// Adds a collection of GeoJson Features to the data source.
        /// </summary>
        /// <param name="features">The features to add.</param>
        /// <returns>The shapes versions of the added features.</returns>
        public IList<Feature> AddRange(IList<Feature> features)
        {
            AddShapes(features);
            return features.ToList();
        }

        /// <summary>
        /// Adds a collection of GeoJson Geometries to the data source.
        /// </summary>
        /// <param name="geometries">The GeoJson geometries to add.</param>
        /// <returns>The shapes versions of the added geometries.</returns>
        public IList<Feature> AddRange(IList<Geometry> geometries)
        {
            var features = new List<Feature>(geometries.Count());

            foreach (var g in geometries)
            {
                if (g != null)
                {
                    features.Add(new Feature(g));
                }
            }

            AddRange(features);

            return features;
        }

        #endregion

        /// <summary>
        /// Checks to see if there is any feature in the data source with the specified ID.
        /// </summary>
        /// <param name="featureId">The feature ID to check.</param>
        /// <returns></returns>
        public bool Contains(string featureId)
        {
            return _ids.Contains(featureId);
        }

        #region Count Methods

        /// <summary>
        /// The number of features in the data source.
        /// </summary>
        public int Count
        {
            get
            {
                return _ids.Count;
            }
        }

        #endregion

        #region Remove & Clear Methods

        /// <summary>
        /// Clears all features from the data source.
        /// </summary>
        public async void Clear()
        {
            _ids.Clear();

            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "clearDataSource", Id);
            }
        }

        /// <summary>
        /// Removes a feature from the data source.
        /// </summary>
        /// <param name="feature">The shape to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public bool Remove(Feature feature)
        {
            if (feature != null && !string.IsNullOrWhiteSpace(feature.Id))
            {
                return Remove(feature.Id);
            }

            return false;
        }

        /// <summary>
        /// Removes a feature by id from the data source.
        /// </summary>
        /// <param name="featureId">The id of the feature to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public bool Remove(string? featureId)
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                return false;
            }

            //Remove the feature Id from the main ID list.
            _ids.Remove(featureId);

            RemoveShapes([featureId]);
            return true;
        }

        /// <summary>
        /// Removes a collection of features from the data source.
        /// </summary>
        /// <param name="features">Collection of features to remove.</param>
        public void RemoveRange(IList<Feature> features)
        {
            if (features != null)
            {
                var featureIds = new List<string>();

                foreach (var f in features)
                {
                    if (f.Id != null)
                    {
                        //Remove the feature Id from the main ID list.
                        _ids.Remove(f.Id);

                        featureIds.Add(f.Id);
                    }
                }

                RemoveShapes(featureIds);
            }
        }

        /// <summary>
        /// Removes a collection of features by id's from the data source.
        /// </summary>
        /// <param name="featureIds">List of feature id's.</param>
        public void RemoveRange(IList<string> featureIds)
        {
            if (featureIds != null)
            {
                //Remove the feature Ids from the main ID list.
                foreach (var id in featureIds)
                {
                    _ids.Remove(id);
                }
                RemoveShapes(featureIds);
            }
        }

        #endregion

        #region Replace Methods

        /// <summary>
        /// Replaces the data in the data source with a GeoJson Feature.
        /// </summary>
        /// <param name="feature"></param>
        public void Replace(Feature feature)
        {
            if (feature != null)
            {
                AddShapes([feature], true);
            }
        }

        /// <summary>
        /// Replaces the data in the data source with a GeoObject.
        /// </summary>
        /// <param name="geoObject"></param>
        public void Replace(GeoObject geoObject)
        {
            var f = Feature.FromGeoObject(geoObject);

            if (f != null)
            {
                AddShapes([f], true);
            }
        }

        /// <summary>
        /// Replaces the data in the data source with a GeoJson Geometry.
        /// </summary>
        /// <param name="geometry"></param>
        public void Replace(Geometry geometry)
        {
            if (geometry != null)
            {
                AddShapes([new Feature(geometry)], true);
            }
        }

        /// <summary>
        /// Replaces the data in the data source with a collection of GeoJson features.
        /// </summary>
        /// <param name="features"></param>
        public void ReplaceRange(IList<Feature> features)
        {
            if (features != null)
            {
                AddShapes(features, true);
            }
        }

        /// <summary>
        /// Replaces the data in the data source with a collection of GeoObjects.
        /// </summary>
        /// <param name="geoObjects"></param>
        public void ReplaceRange(IList<GeoObject> geoObjects)
        {
            var features = new List<Feature>();

            foreach (var g in geoObjects)
            {
                var f = Feature.FromGeoObject(g);

                if (f != null)
                {
                    features.Add(f);
                }
            }

            AddShapes(features, true);
        }

        /// <summary>
        /// Replaces the data in the data source with a collection of GeoJson geometries.
        /// </summary>
        /// <param name="geometries"></param>
        public void ReplaceRange(IList<Geometry> geometries)
        {
            var features = new List<Feature>();

            foreach (var g in geometries)
            {
                features.Add(new Feature(g));
            }

            AddShapes(features, true);
        }

        /// <summary>
        /// Replaces the data in the data source with a Feature collection.
        /// </summary>
        /// <param name="featureCollection"></param>
        public void ReplaceRange(FeatureCollection featureCollection)
        {
            AddShapes(featureCollection.Features, true);
        }

        #endregion

        #region Feature Update Methods

        /// <summary>
        /// Updates a feature in the data source. 
        /// If the feature has an ID that matches with the ID of a feature in the data source, it will be updated (replaced). 
        /// Otherwise it will be added as a new feature.
        /// </summary>
        /// <param name="feature"></param>
        public async void UpdateFeature(Feature feature)
        {
            await UpdateFeatureAsync(feature);
        }

        /// <summary>
        /// Updates a feature in the data source. 
        /// If the feature has an ID that matches with the ID of a feature in the data source, it will be updated (replaced).  
        /// Otherwise it will be added as a new feature.
        /// </summary>
        /// <param name="feature"></param>
        public async Task UpdateFeatureAsync(Feature feature)
        {
            if (Map != null && feature != null)
            {
                //Check to see if the feature is in the data source.
                if (string.IsNullOrWhiteSpace(feature.Id) || feature.SourceId != Id)
                {
                    await AddShapesAsync([feature]);
                }
                else
                {
                    //It's possible the feature is already in the data source on the JS side and the .NET version wasn't aware.
                    if(!_ids.Contains(feature.Id))
                    {
                        _ids.Add(feature.Id);

                        //Reset the bounding box. 
                        _bbox = null;
                    }

                    //updateShape(sourceId, feature) 
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateShape", Id, feature);
                }
            }
        }

        /// <summary>
        /// Updates a collection of features in the data source. 
        /// If the feature has an ID that matches with the ID of a feature in the data source, it will be updated (replaced). 
        /// Otherwise it will be added as a new feature.
        /// </summary>
        /// <param name="features"></param>
        public async void UpdateFeatures(IList<Feature> features)
        {
            await UpdateFeaturesAsync(features);
        }

        /// <summary>
        /// Updates a collection of features in the data source. 
        /// If the feature has an ID that matches with the ID of a feature in the data source, it will be updated (replaced). 
        /// Otherwise it will be added as a new feature.
        /// </summary>
        /// <param name="features"></param>
        public async Task UpdateFeaturesAsync(IList<Feature> features)
        {
            if (Map != null && features != null)
            {
                var needsAdding = new List<Feature>();
                var needsUpdating = new List<Feature>();

                //Make sure the features are in the data source.
                foreach (var f in features)
                {
                    if (string.IsNullOrWhiteSpace(f.Id) || f.SourceId != Id)
                    {
                        needsAdding.Add(f);
                    }
                    else
                    {
                        //It's possible the feature is already in the data source on the JS side and the .NET version wasn't aware.
                        if (!_ids.Contains(f.Id))
                        {
                            _ids.Add(f.Id);

                            //Reset the bounding box. 
                            _bbox = null;
                        }

                        needsUpdating.Add(f);
                    }
                }

                if (needsAdding.Count > 0)
                {
                    await AddShapesAsync(needsAdding);
                }

                if (needsUpdating.Count > 0)
                {
                    //updateShapes(sourceId, features) 
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateShapes", Id, needsUpdating);
                }
            }
        }

        /// <summary>
        /// Updates the properties of a feature in the data source. 
        /// </summary>
        /// <param name="featureId">The ide of the feature.</param>
        /// <param name="properties">The properties to update.</param>
        /// <param name="mergeProperties">If true, the properties will be merged with the existing properties. If false, the properties will be replaced.</param>
        public async void UpdateFeatureProperties(string featureId, IDictionary<string, object?> properties, bool mergeProperties = false)
        {
            await UpdateFeaturePropertiesAsync(featureId, properties, mergeProperties);
        }

        /// <summary>
        /// Updates the properties of a feature in the data source. 
        /// </summary>
        /// <param name="featureId">The ide of the feature.</param>
        /// <param name="properties">The properties to update.</param>
        /// <param name="mergeProperties">If true, the properties will be merged with the existing properties. If false, the properties will be replaced.</param>
        public async Task UpdateFeaturePropertiesAsync(string featureId, IDictionary<string, object?> properties, bool mergeProperties = false)
        {
            if (Map != null && properties != null && !string.IsNullOrWhiteSpace(featureId))
            {
                //updateShapeProperties(sourceId, featureId, properties, mergeProperties)
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateShapeProperties", Id, featureId, properties, mergeProperties);
            }
        }

        #endregion

        #region DataSource Specific Methods

        /// <summary>
        /// Retrieves the bounding box of the data source.
        /// </summary>
        /// <returns></returns>
        public async Task<BoundingBox?> GetBoundsAsync()
        {
            if (_bbox != null)
            {
                return _bbox.DeepClone();
            }

            if (Map != null)
            {
                //getDataSourceBoundingBox(sourceId)
                return await Map.JsInterlop.InvokeJsMethodAsync<BoundingBox?>(Map, "getDataSourceBoundingBox", Id);
            }

            return null;
        }

        /// <summary>
        /// Retrieves all features in the data source.
        /// </summary>
        /// <returns></returns>
        public async Task<FeatureCollection> GetFeaturesAsync()
        {
            var fc = new FeatureCollection();

            if (Map != null)
            {
                //getDataSourceFeatures(sourceId)
                var r = await Map.JsInterlop.InvokeJsMethodAsync<IList<Feature>>(Map, "getDataSourceFeatures", Id);
                if(r != null)
                {
                    fc.Features = r;
                }
            }

            return fc;
        }

        /// <summary>
        /// Retrieves a feature at the specified index in the data source.
        /// </summary>
        /// <param name="index">The index of the feature to get.</param>
        /// <returns></returns>
        public async Task<Feature?> GetFeatureAtAsync(int index)
        {
            if (Map != null && index >= 0)
            {
                //getDataSourceFeatureAt(sourceId, index)
                var r = await Map.JsInterlop.InvokeJsMethodAsync<Feature>(Map, "getDataSourceFeatureAt", Id, index);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a feature by it's ID.
        /// </summary>
        /// <param name="featureId">The id of the feature.</param>
        /// <returns>The matched feature or null.</returns>
        public async Task<Feature?> GetFeatureByIdAsync(string featureId)
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                return null;
            }

            if (Map != null)
            {
                //getDataSourceFeature(sourceId, featureId)
                return await Map.JsInterlop.InvokeJsMethodAsync<Feature?>(Map, "getDataSourceFeature", Id, featureId);
            }

            return null;
        }

        /// <summary>
        /// Gets the options for the data source.
        /// </summary>
        /// <returns>A copy of the options set on the data source.</returns>
        public override DataSourceOptions GetOptions()
        {
            //Return a copy of the options.
            return _options.DeepClone();
        }

        /// <summary>
        /// Updates the options used by the data source.
        /// </summary>
        /// <param name="options">Options to add to the data source.</param>
        public async void SetOptions(DataSourceOptions options)
        {
            //Merge the options and check for changes.
            //If changes, update the data source on the map. 
            if (DataSourceOptions.Merge(options, _options) && Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setDataSourceOptions", Id, _options);
            }
        }

        /// <summary>
        /// Converts the data source to a GeoJson string.
        /// </summary>
        /// <returns>A string version of the data source.</returns>
        public async Task<string> ToStringAsync()
        {
            var fc = await GetFeaturesAsync();
            return JsonSerializer.Serialize(fc);
        }

        /// <summary>
        /// Downloads a GeoJSON file directly into the maps data source.
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <param name="uri">URI to the GeoJSON file.</param>
        public async void ImportDataFromUrl(string uri)
        {
            await ImportDataFromUrlAsync(uri);
        }

        /// <summary>
        /// Asynchronously downloads a GeoJSON file directly into the maps data source.
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <param name="uri">URI to the GeoJSON file.</param>
        /// <returns>A task that can be waited for. When complete, a copy of the data will be available in the .NET version of the data source.</returns>
        public async Task ImportDataFromUrlAsync(string uri)
        {
            if (Map != null)
            {
                if (Utils.IsAzureMapsRestRequest(uri))
                {
                    var r = await Map.MakeGetRequest(uri);

                    if(r != null)
                    {
                        var fc = FeatureCollection.Parse(r);

                        if (fc != null)
                        {
                            await AddShapesAsync(fc.Features);
                        }
                    }
                }
                else
                {
                    //Try and load the data.
                    var response = await Utils.TryGetFileStreamAsync(uri);

                    if (response != null)
                    {
                        //Import it as a stream into the data source. This will then send a copy to the map.
                        await ImportDataFromStreamAsync(response.Stream);
                    }
                }
            }
            else
            {
                waitingDataImports.Add(uri);
            }
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <typeparam name="T">The GeoJSON object type the stream contains (FeatureCollection, Feature, Point, LineString, Polygon,  MultiPoint, MultiLineString, MultiPolygon</typeparam>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public async void ImportDataFromStream(Stream utf8JsonStream)
        {
            var fc = FeatureCollection.Parse(utf8JsonStream);
            await AddShapesAsync(fc.Features);
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <typeparam name="T">The GeoJSON object type the stream contains (FeatureCollection, Feature, Point, LineString, Polygon,  MultiPoint, MultiLineString, MultiPolygon</typeparam>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public async Task ImportDataFromStreamAsync(Stream utf8JsonStream)
        {
            var fc = FeatureCollection.Parse(utf8JsonStream);
            await AddShapesAsync(fc.Features);
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <typeparam name="T">The GeoJSON object type the stream contains (FeatureCollection, Feature, Point, LineString, Polygon,  MultiPoint, MultiLineString, MultiPolygon</typeparam>
        /// <param name="mapFile"></param>
        /// <returns></returns>
        public void ImportDataFromStream(MapFileStream mapFile)
        {
            AddRange(FeatureCollection.Parse(mapFile.Stream));
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <param name="mapFile"></param>
        /// <returns></returns>
        public async Task ImportDataFromStreamAsync(MapFileStream mapFile)
        {
            await AddShapesAsync(FeatureCollection.Parse(mapFile.Stream).Features);
        }

#if (MAUI && WINDOWS) || WINUI
        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source.
        /// </summary>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public async Task ImportDataFromStreamAsync(IRandomAccessStreamReference utf8JsonStream)
        {
            using (var s1 = await utf8JsonStream.OpenReadAsync())
            {
                using (var s2 = s1.AsStreamForRead())
                {
                    await ImportDataFromStreamAsync(s2);
                }
            }
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source.
        /// </summary>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public async Task ImportDataFromStreamAsync(IRandomAccessStream utf8JsonStream)
        {
            using (var s = utf8JsonStream.AsStreamForRead())
            {
                await ImportDataFromStreamAsync(s);
            }
        }
#endif

        /// <summary>
        /// Retrieves the children of the given cluster on the next zoom level. 
        /// This may be a combination of shapes and sub-clusters. 
        /// The sub-clusters will be features with properties matching ClusteredProperties.
        /// </summary>
        /// <param name="clusterId"></param>
        /// <returns></returns>
        public async Task<IList<Feature>> GetClusterChildrenAsync(int clusterId)
        {
            if (Map != null)
            {
                var response = await Map.JsInterlop.InvokeJsMethodAsync<RawMapMsg>(Map, "getClusterChildren", Id, clusterId);

                if (response != null)
                {
                    return response.GetFeatures(Map);
                }
            }

            return new List<Feature>();
        }

        /// <summary>
        /// Calculates a zoom level at which the cluster will start expanding or break apart.
        /// </summary>
        /// <param name="clusterId"></param>
        /// <returns></returns>
        public async Task<int?> GetClusterExpansionZoomAsync(int clusterId)
        {
            if (Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<int?>(Map, "getClusterExpansionZoom", Id, clusterId);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the leaves of the given cluster.
        /// </summary>
        /// <param name="clusterId"></param>
        /// <param name="limit">The maximum number of features to return. Set to Infinity to return all shapes.</param>
        /// <param name="offset">The number of shapes to skip. Allows you to page through the shapes in the cluster.</param>
        /// <returns></returns>
        public async Task<IList<Feature>> GetClusterLeavesAsync(int clusterId, int limit, int offset)
        {
            if (Map != null)
            {
                var response = await Map.JsInterlop.InvokeJsMethodAsync<RawMapMsg>(Map, "getClusterLeaves", Id, clusterId, limit, offset);
                if (response != null)
                {
                    return response.GetFeatures(Map);
                }
            }

            return new List<Feature>();
        }

        #endregion

        #region Private Methods

        private async void AddShapes(IList<Feature>? newFeatures, bool setShapes = false)
        {
            await AddShapesAsync(newFeatures, setShapes);
        }

        private async Task AddShapesAsync(IList<Feature>? newFeatures, bool setShapes = false)
        {
            if (newFeatures != null)
            {
                //Reset the bounding box. 
                _bbox = null;

                if (Map != null)
                {

                    //Create a lookup table of new IDs.
                    var newIds = new HashSet<string>();
                    var noReAddFeature = new List<Feature>();

                    foreach (var f in newFeatures)
                    {
                        //Create an ID if needed. Don't need to check for duplicates as the UniqueId.Get method will ensure it is unique.
                        if (string.IsNullOrWhiteSpace(f.Id))
                        {
                            f.Id = UniqueId.Get("Feature", f.Properties);
                        }
                        //Check to see if the feature is in another data source.
                        else if (!_ids.Contains(f.Id) && !string.IsNullOrWhiteSpace(f.SourceId))
                        {
                            //Remove the feature from the other source.
                            RemoveFeatureFromOtherSource(f);
                        }

                        //Make sure the feature is not already in the data source. Don't re-add.
                        if (!_ids.Contains(f.Id))
                        {
                            newIds.Add(f.Id);

                            //Add reference to this source.
                            f.SourceId = this.Id;

                            if (_syncFeatureUpdates)
                            {
                                //Monitor the feature for changes.
                                f.PropertyChanged += FeatureProperty_Changed;
                            }
                        }
                        else if (setShapes) //If setting the shapes, then it's ok to re-add.
                        {
                            newIds.Add(f.Id);
                        }
                        else
                        {
                            //Otherwise we need to remove it from the new features so we don't re-add it.
                            noReAddFeature.Add(f);
                        }
                    }

                    if (setShapes)
                    {
                        //Replace the main ID list.
                        _ids = newIds;
                    }
                    else
                    {
                        //If a feature already exists, remove it from the new collection.
                        if (noReAddFeature.Count > 0)
                        {
                            foreach (var f in noReAddFeature)
                            {
                                newFeatures.Remove(f);
                            }
                        }

                        //Add the new IDs to the main ID list.
                        _ids.UnionWith(newIds);
                    }

                    if (setShapes)
                    {
                        await Map.JsInterlop.InvokeJsMethodAsync(Map, "setShapes", Id, newFeatures);
                    }
                    else
                    {
                        await Map.JsInterlop.InvokeJsMethodAsync(Map, "addShapes", Id, newFeatures);
                    }
                }
                else
                {
                    //If not added to the map, add to waiting list.
                    if (setShapes)
                    {
                        waitingFeatures = new List<Feature>(newFeatures);
                        waitingDataImports.Clear();
                    }
                    else
                    {
                        waitingFeatures.AddRange(newFeatures);
                    }
                }
            }
        }

        private async void FeatureProperty_Changed(object? sender, PropertyChangedEventArgs e)
        {
            if (_syncFeatureUpdates && Map != null && sender is Feature f && !string.IsNullOrWhiteSpace(f.Id))
            {
                //Reset the bounding box, as the shape geometry/coordinates or extended type (circle/rectangle properties) may have changed.
                _bbox = null;

                await Map.JsInterlop.InvokeJsMethodAsync("updateShape", Id, f.Id, f);
            }
        }

        private async void RemoveShapes(IList<string>? oldShapeIds)
        {
            if (oldShapeIds != null && oldShapeIds.Count() > 0)
            {
                //Reset the bounding box. 
                _bbox = null;

                if (Map != null)
                {
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "removeShapesById", Id, oldShapeIds);
                }
            }
        }

        private void RemoveFeatureFromOtherSource(Feature feature)
        {
            //Remove any existing source reference if in a different source.
            if (Map != null && !string.IsNullOrEmpty(feature.SourceId) && !feature.SourceId.Equals(this.Id))
            {
                var s = Map.Sources.GetById(feature.SourceId);

                if (s != null)
                {
                    if (s is GriddedDataSource gds)
                    {
                        gds.Remove(feature);
                    }
                    else if (s is DataSource ds)
                    {
                        ds.Remove(feature);
                    }
                    else if (s is DataSourceLite dsl)
                    {
                        dsl.Remove(feature);
                    }
                }
            }
        }

        #endregion
    }
}
