using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's popups. Exposed through the popups property of the atlas.Map class. Cannot be instantiated by the user.
    /// </summary>
    public sealed class PopupManager: BaseMapEntityCollection<Popup, PopupOptions>
    {
        #region Constructor

        /// <summary>
        /// A manager for the map control's popups. Exposed through the popups property of the atlas.Map class. Cannot be instantiated by the user.
        /// </summary>
        /// <param name="map">Map instance manager is attached to.</param>
        public PopupManager(Map map): base(map, "addPopup", "removePopups")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Closes all popups.
        /// </summary>
        public async Task CloseAll()
        {
            if(_map != null)
            {
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "closeAllPopups");
            }
        }

        #endregion
    }
}
