using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Information on a module that is required for a custom extension to Azure Maps.
    /// </summary>
    public class MapModuleInfo
    {
        /// <summary>
        /// Information on a module that is required for a custom extension to Azure Maps.
        /// </summary>
        /// <param name="name">Unique name of the module.</param>
        /// <param name="jsResources">The JavaScript resources to load.</param>
        /// <param name="cssResources">The CSS style resources to load.</param>
        public MapModuleInfo(string name, IList<string> jsResources, IList<string>? cssResources = null)
        {
            Name = name;
            JsResources = jsResources.ToArray();
            CssResources = cssResources != null ? cssResources.ToArray() : Array.Empty<string>();
        }

        /// <summary>
        /// Unique name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of JavaScript resources that need to be loaded for the module.
        /// </summary>
        public string[] JsResources { get; internal set; }

        /// <summary>
        /// List of CSS resources that need to be loaded for the module.
        /// </summary>
        public string[] CssResources { get; internal set; }
    }
}
