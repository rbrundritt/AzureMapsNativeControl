using Azure.Core.GeoJson;
using System.Collections.Generic;
using System.Diagnostics;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// A class that generates unique IDs for objects.
    /// </summary>
    internal static class UniqueId
    {
        private static int UniqueIdNumber = 0;
        private static readonly HashSet<string> UniqueIds = new HashSet<string>();

        /// <summary>
        /// Generates a unique ID using a combination of type name (useful for debugging) and a unique number.
        /// </summary>
        /// <param name="typeName">The type of object the ID is for. This helps with debugging.</param>
        /// <param name="properties">Optional properties to check for an ID.</param>
        /// <param name="id">Optional ID to use. If not provided, a new unique ID will be generated.</param>
        /// <returns></returns>
        internal static string Get(string typeName, IDictionary<string, object?>? properties = null, string? id = null)
        { 
            //Check to see if the id is unique.
            if (!string.IsNullOrWhiteSpace(id) && UniqueId.Has(id))
            {
                //The id is not unique.
                Debug.WriteLine($"The id \"{id}\" is not unique. An id must be unique. A new unique id will be generated.");
                id = null;
            }

            //If id is null, check properties for possible Id.
            if (string.IsNullOrWhiteSpace(id) && properties != null && UniqueId.TryGet(properties, out id)) { }

            //If id is still null, generate a new unique id.
            if (string.IsNullOrWhiteSpace(id))
            {
                //Loop until we find a unique ID and then return it.
                while (true)
                {
                    id = $"{typeName}_{UniqueIdNumber++}";

                    //If the ID is unique, return it.
                    if (UniqueIds.Add(id)) //Checks if the ID is already in the set and adds it if it isn't.
                    {
                        return id;
                    }
                }
            }

            return id;
        }

        /// <summary>
        /// Generates a unique ID using a combination of type name (useful for debugging) and a unique number.
        /// </summary>
        /// <param name="typeName">The type of object the ID is for. This helps with debugging.</param>
        /// <param name="geoObject"></param>
        /// <returns></returns>
        internal static string Get(string typeName, GeoObject geoObject)
        {
            //Try and get a unique ID from the geoObject.
            if (UniqueId.TryGet(geoObject, out string? id))
            {
                return id;
            }

            return UniqueId.Get(typeName);
        }

        /// <summary>
        /// Checks if an ID is already in use.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static bool Has(string id)
        {
            return UniqueIds.Contains(id);
        }

        /// <summary>
        /// Checks if an ID is unique.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static bool IsUnique(string id)
        {
            return !UniqueIds.Contains(id);
        }

        /// <summary>
        /// Removes a single ID from the unique ID set.
        /// </summary>
        /// <param name="id"></param>
        internal static void Remove(string id)
        {
            UniqueIds.Remove(id);
        }

        /// <summary>
        /// Removes multiple IDs from the unique ID set.
        /// </summary>
        /// <param name="ids"></param>
        internal static void Remove(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                UniqueIds.Remove(id);
            }
        }

        /// <summary>
        /// Tries to get an ID from a set of properties.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static bool TryGet(IDictionary<string, object?> properties, out string id)
        {
            if (properties != null && properties.TryGetValue("id", out var idValue) && idValue is string idString)
            {
                id = idString;
                UniqueIds.Add(id);
                return true;
            }

            if (properties != null &&
                (properties.TryGetValue("id", out object? idObject) ||
                  properties.TryGetValue("Id", out idObject) ||
                  properties.TryGetValue("ID", out idObject) ||
                  properties.TryGetValue(Constants.AzureMapsShapeID, out idObject)) &&
                  Utils.TryConvertToString(idObject, out string? tempID) &&
                  !string.IsNullOrWhiteSpace(tempID) &&
                  IsUnique(tempID))
            {
                id = tempID;
                UniqueIds.Add(id);
                return true;
            }

            id = string.Empty;
            return false;
        }

        /// <summary>
        /// Tries to get an ID from a GeoJSON object.
        /// </summary>
        /// <param name="geoObject"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static bool TryGet(GeoObject geoObject, out string id)
        {
            if (geoObject != null &&
                (geoObject.TryGetCustomProperty("id", out object? idObject) ||
                  geoObject.TryGetCustomProperty("Id", out idObject) ||
                  geoObject.TryGetCustomProperty("ID", out idObject) ||
                  geoObject.TryGetCustomProperty(Constants.AzureMapsShapeID, out idObject)) &&
                idObject != null)
            {
                try
                {
                    id = idObject.ToString();

                    //If the ID is not null or empty and is unique, add it to the set.
                    if (!string.IsNullOrWhiteSpace(id) && IsUnique(id))
                    {
                        UniqueIds.Add(id);
                        return true;
                    }
                }
                catch { }
            }

            id = string.Empty;
            return false;
        }
    }
}
