using AzureMapsNativeControl.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureMapsNativeControl
{
    public static class AzureMapsModules
    {
        /// <summary>
        /// The Animation module is used to create animations in Azure Maps. https://github.com/Azure-Samples/azure-maps-animations
        /// </summary>
        public static MapModuleInfo AnimationModule
        {
            get
            {
                return new MapModuleInfo("azure-maps-animation",
                    new List<string> { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-animations.min.js" });
            }
        }

        /// <summary>
        /// The Gridded Data Source module is used to create a gridded data source for Azure Maps. https://github.com/Azure-Samples/azure-maps-gridded-data-source
        /// </summary>
        public static MapModuleInfo GriddedDataSourceModule 
        {
            get { 
                return new MapModuleInfo("azure-maps-gridded-data-source", 
                    new List<string> { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-gridded-data-source.min.js" }); 
            }
        }

        /// <summary>
        /// The Overview Map module is used to create an overview map for Azure Maps. https://github.com/Azure-Samples/azure-maps-overview-map
        /// </summary>
        public static MapModuleInfo OverviewMapModule
        {
            get
            {
                return new MapModuleInfo("azure-maps-overview-map",
                    new List<string>() { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-overview-map.min.js" });
            }
        }

        /// <summary>
        /// An Azure Maps Web SDK module that provides a control that makes it easy to bring any data loaded on the map into view. https://github.com/Azure-Samples/azure-maps-bring-data-into-view-control
        /// </summary>
        public static MapModuleInfo BringDataIntoViewModule
        {
            get
            {
                return new MapModuleInfo("azure-maps-bring-data-into-view-control",
                    new List<string>() { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-bring-data-into-view-control.min.js" });
            }
        }

        public static MapModuleInfo GeolocationControlModule
        {
            get
            {
                return new MapModuleInfo("azure-maps-geolocation-control",
                    new List<string>() { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-geolocation-control.min.js" });
            }
        }

        /// <summary>
        /// An Azure Maps Web SDK module that provides UI controls for dynamically styling layers and displaying legends on the map. https://github.com/Azure-Samples/azure-maps-layer-legend/
        /// </summary>
        public static MapModuleInfo LayerLegendModule
        {
            get
            {
                return new MapModuleInfo("azure-maps-layer-legend",
                new List<string> { "proxy?operation=embeddedResource&resourceName=js/modules/azure-maps-layer-legend.min.js" },
                new List<string> { "proxy?operation=embeddedResource&resourceName=css/modules/azure-maps-layer-legend.min.css" });
            }
        }
    }
}
