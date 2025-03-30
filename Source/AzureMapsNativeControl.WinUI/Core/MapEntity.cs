using AzureMapsNativeControl.Internal;
using System;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Base interface for all entities that can be attached to a map.
    /// </summary>
    [JsonDerivedType(typeof(Popup))]
    [JsonDerivedType(typeof(HtmlMarker))]
    [JsonDerivedType(typeof(BaseSource))]
    [JsonDerivedType(typeof(BaseLayer))]
    public abstract class MapEntity<TMapEntityOption>
    {
        #region Private Properties

        [JsonIgnore]
        private Map? _map = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Base interface for all entities that can be attached to a map.
        /// </summary>
        /// <param name="jsNamespace"> The JavaScript namespace of the entity class.</param>
        /// <param name="id"></param>
        public MapEntity(string jsNamespace, string? id = null)
        {
            JsNamespace = jsNamespace;
            Id = UniqueId.Get(jsNamespace, null, id);
        }

        /// <summary>
        /// Base interface for all entities that can be attached to a map.
        /// </summary>
        /// <param name="jsNamespace"> The JavaScript namespace of the entity class.</param>
        /// <param name="id"></param>
        /// <param name="allowNonUniqueId">If true, the ID can be non-unique. This is useful for custom controls that are not added to the map.</param>
        internal MapEntity(string jsNamespace, string? id = null, bool allowNonUniqueId = false)
        {
            JsNamespace = jsNamespace;
            Id = (!string.IsNullOrWhiteSpace(id) && allowNonUniqueId) ? id : UniqueId.Get(jsNamespace, null, id);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A unique ID for the entity.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The JavaScript namespace of the layer class.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("jsNamespace")]
        public string JsNamespace { get; internal set; }

        /// <summary>
        /// The map the entity is attached to.
        /// </summary>
        [JsonIgnore]
        public Map? Map
        {
            get { return _map; }
            internal set {
                var oldMap = _map;
                _map = value; 

                if (MapUpdated != null)
                {
                    MapUpdated(oldMap, _map);
                }
            }
        }

        #endregion

        #region Public Methods

        public abstract TMapEntityOption GetOptions();

        #endregion

        #region Private Methods

        /// <summary>
        /// Action for when the map reference is changed. Action recieves, (oldMap, newMap).
        /// </summary>
        internal Action<Map?, Map?>? MapUpdated = null;

        #endregion
    }
}
