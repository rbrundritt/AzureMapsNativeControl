using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AzureMapsNativeControl.Data;


#if WINUI
using Microsoft.UI.Xaml.Controls;
#elif WPF
using System.Windows.Controls;
#endif

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Interface for a map view control.
    /// </summary>
    [KnownType(typeof(Map))]
    [KnownType(typeof(SwipeMap))]
    public abstract class IMapView: Grid
    {
        #region Private Properties

        internal string MapViewFileName = "MapView.html";

        #endregion

        #region Public Properties
        /// <summary>
        /// Manages the communication between the JavaScript Map API and the .NET wrapper.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MapViewJsInterlop JsInterlop { get; internal set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to retrieve the users device current geolocation (e.g. GPS position).
        /// Leverages the navigator.geolocation API of the WebView.
        /// </summary>
        /// <param name="options">Options for the geolocation request.</param>
        /// <returns>A feature containing the devices current geolocation, or null if unsuccessful.</returns>
        public async Task<Feature?> GetCurrentGeolocationPosition(GeolocationPositionOptions? options = null)
        {
            return await JsInterlop.InvokeJsMethodAsync<Feature?>("MapUtils.getCurrentPosition", options);
        }

        #endregion

        #region Abstract methods

        public abstract Task LoadModuleAsync(MapModuleInfo moduleInfo);

        public abstract bool IsModuleLoaded(string moduleName);

        /// <summary>
        /// Handler for when the web page is loaded and ready to initialize the map view.
        /// </summary>
        /// <param name="config">The configuration options for the map control.</param>
        /// <returns></returns>
        internal abstract Task InitView(AzureMapsConfiguration? config);

        /// <summary>
        /// Handler for when the web page is unloaded.
        /// </summary>
        internal abstract void WebPageUnloaded();

        #endregion
    }
}
