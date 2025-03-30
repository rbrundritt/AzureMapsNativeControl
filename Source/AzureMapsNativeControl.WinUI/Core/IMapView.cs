using System.Runtime.Serialization;
using System.Threading.Tasks;

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
        public abstract Task LoadModuleAsync(MapModuleInfo moduleInfo);

        public abstract bool IsModuleLoaded(string moduleName);


        internal string MapViewFileName = "MapView.html";

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
    }
}
