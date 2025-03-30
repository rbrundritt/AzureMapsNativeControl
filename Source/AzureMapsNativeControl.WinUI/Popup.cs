using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using System.Threading.Tasks;
using System;

#if WINUI
using Microsoft.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// An information window anchored at a specified position on a map.
    /// </summary>
    public class Popup: MapEntity<PopupOptions>, IMapEventTarget, IDeepCloneable<Popup>
    {
        #region Private Properties

        internal PopupOptions _options = PopupOptions.Defaults();

        #endregion

        #region Constructor

        /// <summary>
        /// An information window anchored at a specified position on a map.
        /// </summary>
        /// <param name="options">Options for the popup</param>
        /// <param name="id">Unique ID for the popup.</param>
        public Popup(PopupOptions? options = null, string? id = null): base("atlas.Popup", id)
        {
            if (options != null)
            {
                PopupOptions.Merge(options, _options);
            }

            //Add/remove default map events when popup added/removed from map.
            MapUpdated = (Map? oldMap, Map? newMap) =>
            {
                if (oldMap != null)
                {
                    //By default attach a drag event that updates the position value in the options.
                    oldMap.Events.Remove("drag", this, OnPopupDrag);
                }

                if (newMap != null)
                {
                    //By default attach a drag event that updates the position value in the options.
                    newMap.Events.Add("drag", this, OnPopupDrag);
                }
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the popup.
        /// Note, original Id is cloned as well. If you want a new Id, use the other Clone method.
        /// </summary>
        /// <returns></returns>
        public Popup DeepClone()
        {
            return Clone(false);
        }

        /// <summary>
        /// Creates a deep copy of the popup.
        /// </summary>
        /// <param name="regenerateId">If true, the popup Id will be regenerated.</param>
        /// <returns></returns>
        public Popup Clone(bool regenerateId = false)
        {
            return new Popup(_options.DeepClone(), regenerateId ? UniqueId.Get("Popup") : Id);
        }

        /// <summary>
        /// Gets a copy of the options of the popup.
        /// </summary>
        public override PopupOptions GetOptions()
        {
            return _options.DeepClone();
        }

        /// <summary>
        /// Set the options of the popup.
        /// </summary>
        /// <param name="options"></param>
        public async void SetOptions(PopupOptions options)
        {
            //Merge the options and check for changes.
            PopupOptions.Merge(options, _options);

            //If changes, update the popup on the map. 
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "setPopupOptions", Id, _options);
            }
        }

        /// <summary>
        /// Returns true if the popup is currently open, otherwise false.
        /// </summary>
        /// <returns>True if popup is open.</returns>
        public async Task<bool> IsOpenAsync()
        {
            if (Map != null)
            {
                return await Map.JsInterlop.InvokeJsMethodAsync<bool>(Map, "isPopupOpen", Id);
            }

            return false;
        }

        /// <summary>
        /// Opens the popup.
        /// </summary>
        public async void Open()
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "openPopup", Id, _options);
            }
        }

        /// <summary>
        /// Opens the popup on a specific map instance.
        /// Note that the popup will be removed from the previous map instance if it was attached to one.
        /// </summary>
        /// <param name="map"></param>
        public void Open(Map map)
        {
            //Check to see if the map instance is changing.
            if (Map != map)
            {
                //Remove the popup if it is already attached to another map.
                if (Map != null)
                {
                    Map.Popups.Remove(this);
                }

                //Add the popup to the new map, if it isn't null.
                if (map != null)
                {
                    map.Popups.Add(this);
                    Map = map;
                }
            }

            Open();
        }

        /// <summary>
        /// Closes the popup on the map. The popup remains attached to the HTML document.
        /// </summary>
        public async void Close()
        {
            if (Map != null)
            {
                await Map.JsInterlop.InvokeJsMethodAsync(Map, "closePopup", Id);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Default event handler for when the popup is dragged.
        /// </summary>
        private static void OnPopupDrag(object? sender, MapEventArgs args)
        {
            if(sender is Popup p)
            {
                p._options.Position = ((MapMouseEventArgs)args).Position;
            }
        }

        #endregion
    }
}
