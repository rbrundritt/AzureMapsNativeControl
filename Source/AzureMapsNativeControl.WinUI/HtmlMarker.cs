using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Internal;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// A class that represents points on the map using HTML elements.
    /// </summary>
    public class HtmlMarker : MapEntity<HtmlMarkerOptions>, IMapEventTarget
    {
        #region Private Properties

        internal HtmlMarkerOptions _options = HtmlMarkerOptions.Defaults();

        /// <summary>
        /// Properties of the entity.
        /// </summary>
        internal PropertiesTable _properties;

        #endregion

        #region Contstructor

        /// <summary>
        /// A class that represents points on the map using HTML elements.
        /// </summary>
        /// <param name="options">Options for the marker.</param>
        /// <param name="properties">Properties to associate with the marker.</param>
        /// <param name="id">A unique ID for the marker.</param>
        public HtmlMarker(HtmlMarkerOptions? options = null, PropertiesTable? properties = null, string? id = null):
            base("atlas.HtmlMarker", id)
        {
            _properties = (properties != null) ? properties : new PropertiesTable();

            if (options != null)
            {
                HtmlMarkerOptions.Merge(options, _options);
            }

            //Add/remove default map events when merker added/removed from map.
            MapUpdated = (Map ? oldMap, Map ? newMap) =>
            {
                if (oldMap != null)
                {
                    //By default attach a drag event that updates the position value in the options.
                    oldMap.Events.Remove("drag", this, OnMarkerDrag);
                }

                if (newMap != null)
                {
                    //By default attach a drag event that updates the position value in the options.
                    newMap.Events.Add("drag", this, OnMarkerDrag);
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Properties of the feature.
        /// </summary>
        [JsonIgnore]
        public PropertiesTable Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                if (value != null)
                {
                    _properties = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the options of the HTML marker. 
        /// </summary>
        /// <returns></returns>
        public override HtmlMarkerOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Set the options of the marker.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(HtmlMarkerOptions options)
        {
            //Merge the options and check for changes.
            if (HtmlMarkerOptions.Merge(options, _options))
            {
                //If changes, update the marker on the map. 
                if (Map != null)
                {
                    //callGenericItemFunction(id, cacheName, functionName, args)
                    await Map.JsInterlop.InvokeJsMethodAsync(Map, "callGenericItemFunction", Id, Constants.MarkerCache, "setOptions", _options);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Default event handler for when the popup is dragged.
        /// </summary>
        private static void OnMarkerDrag(object? sender, MapEventArgs e)
        {
            if (sender is HtmlMarker m && e is MapMouseEventArgs args)
            {
                m._options.Position = args.Position;
            }
        }

        #endregion
    }
}
