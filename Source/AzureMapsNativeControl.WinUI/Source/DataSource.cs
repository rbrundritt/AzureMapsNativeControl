using Azure.Core.GeoJson;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections;
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
    /// A data source for shapes on the map.
    /// </summary>
    [JsonDerivedType(typeof(GriddedDataSource))]
    [JsonConverter(typeof(BaseSourceIdConverter<DataSource>))]
    public class DataSource : BaseSource, IEnumerable<Feature>
    {
        #region Private Properties

        [JsonIgnore]
        internal static List<string> SupportedFileMimeTypes = new List<string>
        {
            "text/plain",
            "application/json"
        };

        [JsonIgnore]
        internal static List<string> SupportedFileExtensions = new List<string>
        {
            ".json",
            ".geojson",
            ".txt"
        };

        [JsonIgnore]
        private List<Feature> collection = new List<Feature>();

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
        private HashSet<string> _ids = new HashSet<string>();

        [JsonIgnore]
        private bool _syncFeatureUpdates = false;

        [JsonIgnore]
        private List<string> waitingDataImports = new List<string>();

        #endregion

        #region Constructor

        /// <summary>
        /// A data source for shapes on the map.
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
        public DataSource(string? initDataImportUri = null, DataSourceOptions? options = null,string? id = null, bool syncFeatureUpdates = false) : base("atlas.source.DataSource", id)
        {
            _syncFeatureUpdates = syncFeatureUpdates;

            if (options != null)
            {
                DataSourceOptions.Merge(options, this._options);
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
                }
            };
        }

        internal DataSource(string jsNamespace, string? initDataImportUri = null, DataSourceOptions? options = null, string? id = null, bool syncFeatureUpdates = false) : base(jsNamespace, id)
        {
            _syncFeatureUpdates = syncFeatureUpdates;

            if (options != null)
            {
                DataSourceOptions.Merge(options, this._options);
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
                }
            };
        }

        #endregion

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

                if(f != null)
                {
                    features.Add(f);
                }
            }

            AddShapes(features);

            return features;
        }

        /// <summary>
        /// Adds a GeoJson Feature Collection to the data source.
        /// </summary>
        /// <param name="featureCollection">the Feature Collection to add.</param>
        /// <returns>The shapes versions of the added features.</returns>
        public IList<Feature> AddRange(FeatureCollection featureCollection)
        {
            AddShapes(featureCollection.Features);
            return featureCollection.Features;
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

            AddShapes(features);

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

            AddShapes(features);

            return features;
        }

        #endregion

        #region Remove & Clear Methods

        /// <summary>
        /// Clears all features from the data source.
        /// </summary>
        public void Clear()
        {
            //Calling add shapes with an empty array and setShapes to true will clear the data source.
            AddShapes([], true);
        }

        /// <summary>
        /// Removes a feature from the data source.
        /// </summary>
        /// <param name="feature">The shape to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public bool Remove(Feature feature)
        {
            if (feature != null)
            {
               RemoveShapes([feature]);
               return true;
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
            if (string.IsNullOrWhiteSpace(featureId) || !_ids.Contains(featureId))
            {
                return false;
            }

            var feature = collection.FirstOrDefault(s => s.Id == featureId);

            if (feature != null)
            {
                RemoveShapes([feature]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a collection of features from the data source.
        /// </summary>
        /// <param name="features">Collection of features to remove.</param>
        public void RemoveRange(IList<Feature> features)
        {
            if (features != null)
            {
                RemoveShapes(features);
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
                var features = collection.Where(f => !string.IsNullOrWhiteSpace(f.Id) && featureIds.Contains(f.Id));
                RemoveShapes(features.ToList());
            }
        }

        #endregion

        #region Contains, Count, IndexOf Methods

        /// <summary>
        /// Specifies if a feature is contained in the data source.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns>True is data source contains feature.</returns>
        public bool Contains(Feature feature)
        {
            if (feature != null)
            {
                return collection.Contains(feature);
            }

            return false;
        }

        /// <summary>
        /// Specifies if a feature is contained in the data source.
        /// </summary>
        /// <param name="featureId">The id of the feature.</param>
        /// <returns>True is data source contains feature.</returns>
        public bool Contains(string featureId)
        {
            if (string.IsNullOrWhiteSpace(featureId) || !_ids.Contains(featureId))
            {
                return false;
            }

            return collection.Any(s => s.Id == featureId);
        }

        /// <summary>
        /// The number of features in the data source.
        /// </summary>
        public int Count
        {
            get { return collection.Count; }
        }

        /// <summary>
        /// Gets the index of a feature in the collection.
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <returns>The index of the feature or -1 if not found.</returns>
        public int IndexOf(Feature feature)
        {
            if (feature != null)
            {
                return collection.IndexOf(feature);
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of a feature by id in the collection.
        /// </summary>
        /// <param name="featureId">A feature id</param>
        /// <returns>The index of a feature with the specified id or -1 if not found.</returns>
        public int IndexOf(string featureId)
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                return -1;
            }

            var shape = collection.FirstOrDefault(s => s.Id == featureId);

            if (shape != null)
            {
                return collection.IndexOf(shape);
            }

            return -1;
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

        #region Enumerator Methods

        /// <summary>
        /// Gets an enumerator for the features in the data source.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Feature> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the features in the data source.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        #region DataSource Specific Methods

        /// <summary>
        /// Gets or sets a feature at a specific index.
        /// </summary>
        /// <param name="index">The index of the feature.</param>
        /// <returns>The feature at the specified index.</returns>
        public Feature this[int index]
        {
            get
            {
                return collection[index];
            }
            set
            {
                //Only add if it isn't already in the collection.
                if (value != null && (string.IsNullOrWhiteSpace(value.Id) || !_ids.Contains(value.Id)))
                {
                    AddShapes([value]);
                }
            }
        }

        /// <summary>
        /// Gets a feature by it's ID.
        /// </summary>
        /// <param name="id">The id of the feature.</param>
        /// <returns>The matched feature or null.</returns>
        public Feature? GetFeatureById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return collection.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Retrieves the bounding box of the data source.
        /// </summary>
        /// <returns></returns>
        public BoundingBox? GetBounds()
        {
            if (_bbox == null)
            {
                _bbox = BoundingBox.FromData(this);
            }

            return _bbox?.DeepClone();
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
        /// Converts the data source to a GeoJson FeatureCollection.
        /// </summary>
        /// <param name="cloneFeatures">Specifies if the features should be cloned. Features will be assigned new IDs.</param>
        /// <returns>A FeatureColleciton version of the data in the data source.</returns>
        public FeatureCollection ToFeatureCollection(bool cloneFeatures = true)
        {
            var fc = new FeatureCollection();

            if (cloneFeatures)
            {
                foreach (var s in collection)
                {
                    fc.Features.Add(s.DeepClone());
                }
            }
            else
            {
                foreach (var s in collection)
                {
                    fc.Features.Add(s);
                }
            }

            return fc;
        }

        /// <summary>
        /// Converts the data source to a GeoJson string.
        /// </summary>
        /// <returns>A string version of the data source.</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(new FeatureCollection(this, _bbox));
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
            if (Utils.IsAzureMapsRestRequest(uri))
            {
                if (Map != null)
                {
                    var r = await Map.MakeGetRequest(uri);

                    if (!string.IsNullOrWhiteSpace(r))
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
                    waitingDataImports.Add(uri);
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

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public void ImportDataFromStream(Stream utf8JsonStream)
        {
            AddRange(FeatureCollection.Parse(utf8JsonStream));
        }

        /// <summary>
        /// Imports a stream of GeoJSON data directly into the data source. 
        /// A copy of the features are stored in both the data source (.NET) and in the map (WebView/JavaScript) 
        /// for seamless integration. However, this can be memory intensive for large datasets. If limited 
        /// interaction is needed, consider using the DataSourceLite class which only maintains a single copy 
        /// of the data within the map (WebView).
        /// </summary>
        /// <param name="utf8JsonStream"></param>
        /// <returns></returns>
        public async Task ImportDataFromStreamAsync(Stream utf8JsonStream)
        {
            await AddShapesAsync(FeatureCollection.Parse(utf8JsonStream).Features);
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
                using(var s2 = s1.AsStreamForRead())
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

        #region Feature Update Methods

        /// <summary>
        /// Refreshes the data source in the map. 
        /// Syncs and unsynced feature changes.
        /// </summary>
        public async void RefreshMap()
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setShapes", Id, collection);
            }
        }

        /// <summary>
        /// Refreshes the data source in the map. 
        /// Syncs and unsynced feature changes.
        /// </summary>
        public async Task RefreshMapAsync()
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setShapes", Id, collection);
            }
        }

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
                if (string.IsNullOrWhiteSpace(feature.Id) || !_ids.Contains(feature.Id))
                {
                    await AddShapesAsync([feature]);
                }
                else
                {
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

                //Make sure the features are in the data source.
                foreach (var f in features)
                {
                    if (string.IsNullOrWhiteSpace(f.Id) || !_ids.Contains(f.Id))
                    {
                        needsAdding.Add(f);
                    }
                }

                if (needsAdding.Count > 0)
                {
                    await AddShapesAsync(needsAdding);
                }

                //updateShapes(sourceId, features) 
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateShapes", Id, features);
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
            if (Map != null && properties != null && !string.IsNullOrWhiteSpace(featureId) && _ids.Contains(featureId))
            {
                //updateShapeProperties(sourceId, featureId, properties, mergeProperties)
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "updateShapeProperties", Id, featureId, properties, mergeProperties);
            }
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
            return ((!string.IsNullOrEmpty(fileName) && SupportedFileMimeTypes.Contains(Path.GetExtension(fileName.ToLowerInvariant()))) ||
                (!string.IsNullOrEmpty(mimeType) && SupportedFileMimeTypes.Contains(mimeType.ToLowerInvariant())));
        }

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
                    else if(!_ids.Contains(f.Id) && !string.IsNullOrWhiteSpace(f.SourceId))
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
                    //For any existing features that are not in the new collection, remove references and monitoring to this data source.
                    foreach (var f in collection)
                    {
                        if (string.IsNullOrWhiteSpace(f.Id) || !newIds.Contains(f.Id))
                        {
                            //Remove reference to this source.
                            f.SourceId = null;

                            if (_syncFeatureUpdates)
                            {
                                //Stop monitoring the feature.
                                f.PropertyChanged -= FeatureProperty_Changed;
                            }
                        }
                    }

                    collection.Clear();

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
                
                collection.AddRange(newFeatures);

                if (Map != null)
                {
                    if (setShapes)
                    {
                        await Map.JsInterlop.InvokeJsMethodAsync(Map, "setShapes", Id, newFeatures);
                    }
                    else
                    {
                        await Map.JsInterlop.InvokeJsMethodAsync(Map, "addShapes", Id, newFeatures);
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

        private async void RemoveShapes(IList<Feature>? oldShapes)
        {
            if (oldShapes != null && oldShapes.Count() > 0)
            {
                //Reset the bounding box. 
                _bbox = null;

                var removeIds = new List<string>();

                foreach (var f in oldShapes)
                {
                    //All features in the data source will have an ID.
                    if (!string.IsNullOrWhiteSpace(f.Id) && collection.Remove(f))
                    {
                        //Capture the ID of the feature being removed.
                        removeIds.Add(f.Id);

                        //Remove from the main ID list.
                        _ids.Remove(f.Id);

                        //Remove reference to this source.
                        f.SourceId = null;

                        if (_syncFeatureUpdates)
                        {
                            //Stop monitoring the feature.
                            f.PropertyChanged -= FeatureProperty_Changed;
                        }
                    }
                }

                if (Map != null && removeIds.Count > 0)
                {
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "removeShapesById", Id, removeIds);
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
