using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data
{
    /// <summary>
    /// A table of GeoJson Feature properties.
    /// </summary>
    [JsonConverter(typeof(PropertiesTableConverter))]
    public class PropertiesTable : ObservableDictionary<string, object?>, IEquatable<IDictionary<string, object?>>, IDeepCloneable<PropertiesTable>
    {
        #region Private Properties

        /// <summary>
        /// Serializer options for cloning the PropertiesTable.
        /// </summary>
        private static JsonSerializerOptions cloneSerializerOptions = new JsonSerializerOptions() { 
            WriteIndented = false, 
            Converters = { new PropertiesTableConverter() } 
        };

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the PropertiesTable class.
        /// </summary>
        public PropertiesTable() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PropertiesTable class that contains elements copied from the specified dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        public PropertiesTable(IDictionary<string, object?> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PropertiesTable class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection"></param>
        public PropertiesTable(IEnumerable<KeyValuePair<string, object?>> collection)
           : base(collection)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the PropertiesTable.
        /// </summary>
        /// <returns></returns>
        public new PropertiesTable DeepClone()
        {
            //Since we can't be sure of the cloneability of the elements of the table or the depth and
            //complexity of the table, safest to serialize and deserialize.
            try
            {
                var json = JsonSerializer.Serialize(this, cloneSerializerOptions);

                if (!string.IsNullOrWhiteSpace(json))
                {

                    var copy = JsonSerializer.Deserialize<PropertiesTable>(json, cloneSerializerOptions);

                    if (copy != null)
                    {
                        return copy;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error making deep copy of PropertiesTable: " + ex.Message);
            }

            return new PropertiesTable(this);
        }

        /// <summary>
        /// Determines whether the PropertiesTable contains value at the specified property path.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns></returns>
        public new bool ContainsKey(string propertyPath)
        {
            return ContainsKey(propertyPath, 0);
        }

        /// <summary>
        /// Gets or sets the value of the property at the specified path. 
        /// Examples: 
        ///     object? city = table["address/city"]
        /// </summary>
        /// <param name="propertyPath">Path to the object in the properties table.</param>
        /// <returns></returns>
        public new object? this[string propertyPath]
        {
            get
            {
                return GetValue(propertyPath);
            }
            set
            {
                TrySetValue(propertyPath, value);
            }
        }

        /// <summary>
        /// Sets the value of the property at the specified path.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetValue(string propertyPath, object? value)
        {
            if (this == null)
            {
                return false;
            }

            var pathParts = GetKeyPath(propertyPath);

            if (pathParts.Length == 0)
            {
                return false;
            }

            // Recursively navigate the object tree.
            object? currentTable = this;

            for (int i = 0; i < pathParts.Length; i++)
            {
                var part = pathParts[i];

                if (currentTable is IDictionary<string, object?> table)
                {
                    //Check to see if we have reached the end of the path.
                    if (i == pathParts.Length - 1)
                    {
                        table[part] = value;
                        RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);
                        return true;
                    }

                    //Try getting the existing subpath. 
                    if (table.TryGetValue(part, out var tableValue))
                    {
                        currentTable = tableValue;
                    }
                    else
                    {
                        //Subpath not found, need to create one.
                        //Check the next part of the path. If it is a number, create an array, otherwise create a dictionary.
                        if (int.TryParse(pathParts[i + 1], out int _))
                        {
                            var newList = new List<object?>();
                            table[part] = newList;
                            currentTable = newList;
                        }
                        else
                        {
                            var newTable = new Dictionary<string, object?>();
                            table[part] = newTable;
                            currentTable = newTable;
                        }
                    }
                }
                else if (currentTable is IList list)
                {
                    //Check to see if current part is a number that indicates an array position.
                    if (int.TryParse(part, out int index))
                    {
                        //Check to see if the index is valid.
                        if (index >= 0)
                        {
                            //Check to see if the array is long enough.
                            while (list.Count <= index)
                            {
                                list.Add(null);
                            }

                            //Check to see if we have reached the end of the path.
                            if (i == pathParts.Length - 1)
                            {
                                list[index] = value;
                                RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);
                                return true;
                            }

                            //Try getting the existing subpath. 
                            if (list[index] != null)
                            {
                                currentTable = list[index];
                            }
                            else
                            {
                                //Subpath not found, need to create one.
                                //Check the next part of the path. If it is a number, create an array, otherwise create a dictionary.
                                if (int.TryParse(pathParts[i + 1], out int _))
                                {
                                    var newList = new List<object?>();
                                    list[index] = newList;
                                    currentTable = newList;
                                }
                                else
                                {
                                    var newTable = new Dictionary<string, object?>();
                                    list[index] = newTable;
                                    currentTable = newTable;
                                }
                            }
                        } 
                        else
                        {
                            //Invalid index. Property names can't be a number, so invalid path.
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                if (currentTable == null)
                {
                    return false;
                }
            }

            return false;
        }

        #region Get Value Methods

        /// <summary>
        /// Gets the object value of the property at the specified path. 
        /// Examples: 
        ///     object? city = GetValue("address/city")
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path or null.</returns>
        public object? GetValue(string propertyPath)
        {
            if (TryGetValue(propertyPath, out var value))
            {
                return value;
            }

            return default;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as a bool.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="defaultValue">The value returned if unable to locate property, or parse it as the specified type.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public bool GetBool(string propertyPath, bool defaultValue = false)
        {
            if (TryGetBool(propertyPath, out bool value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as a double.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public double GetDouble(string propertyPath, double defaultValue = 0)
        {
            if (TryGetDouble(propertyPath, out double value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as a int.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public int GetInt32(string propertyPath, int defaultValue = 0)
        {
            if (TryGetInt32(propertyPath, out int value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as a string.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public string? GetString(string propertyPath, string? defaultValue = null)
        {
            if (TryGetString(propertyPath, out string? value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as a list of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public IList<T> GetList<T>(string propertyPath) where T : class
        {
            TryGetList(propertyPath, out IList<T> value);
            return value;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as an array of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public T[] GetArray<T>(string propertyPath) where T : class
        {
            TryGetArray(propertyPath, out T[] value);
            return value;
        }

        /// <summary>
        /// Gets the value of the property at the specified path as the specified type.
        /// </summary>
        /// <typeparam name="T">The class type to try and case to.</typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns>The value at the specified path, casted to the specified type, or null.</returns>
        public T? GetValue<T>(string propertyPath) where T: class
        {
            if (TryGetValue<T>(propertyPath, out T value))
            {
                return value;
            }

            return default;
        }

        #endregion

        #region Try Get Methods

        /// <summary>
        /// Tries to get the value of the property at the specified path.
        /// Examples: 
        ///     TryGetValue("address/city", out object city)
        ///     TryGetValue("address.city", out object? city)
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property.</returns>
        public new bool TryGetValue(string propertyPath, out object? value)
        {
            return TryGetValue(propertyPath, out value, 0);
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a double.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetBool(string propertyPath, out bool value)
        {
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToBool(obj, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a double.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetDouble(string propertyPath, out double value)
        {
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToDouble(obj, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a int.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetInt32(string propertyPath, out int value)
        {
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToInt32(obj, out value))
            {
                return true;
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a string.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetString(string propertyPath, out string value)
        {
            value = string.Empty;

            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToString(obj, out value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a list of objects.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetList(string propertyPath, out IList<object?> value)
        {
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToList(obj, out value))
            {
                return true;
            }

            value = new List<object?>();
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as a list of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetList<T>(string propertyPath, out IList<T?> value) where T : class
        {
            //Fallback to checking is the object is a list or array, then checking each value to see if it can be cast to the specified type.
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToList(obj, out value))
            {
                return true;
            }

            value = new List<T?>();
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as an array of objects.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetArray(string propertyPath, out object?[] value)
        {
            //Fallback to checking is the object is a list or array, then checking each value to see if it can be cast to the specified type.
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToList(obj, out IList<object?> list))
            {
                value = list.ToArray();
                return true;
            }

            value = new object[0];
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as an array of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        public bool TryGetArray<T>(string propertyPath, out T?[] value) where T : class
        {
            //Fallback to checking is the object is a list or array, then checking each value to see if it can be cast to the specified type.
            if (TryGetValue(propertyPath, out object? obj) && Utils.TryConvertToList(obj, out IList<T?> list))
            {
                value = list.ToArray();
                return true;
            }

            value = new T[0];
            return false;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path as the specified type.
        /// </summary>
        /// <typeparam name="T">The class type to try and case to.</typeparam>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="value">The output value.</param>
        /// <returns>A boolean value indicating if it was able to successfully get the property and parse it as the specified type.</returns>
        //public new bool TryGetValue<T>(string propertyPath, out T? value) where T : class
        //{ 
        //    if(TryGetValue(propertyPath, out object? obj) && obj is T castValue)
        //    {
        //        value = castValue;
        //        return true;
        //    }

        //    value = default;
        //    return false;
        //}

        #endregion

        #region Comparison Methods

        /// <inheritdoc />
        public bool Equals(IDictionary<string, object?>? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as IDictionary<string, object?>));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(PropertiesTable? left, IDictionary<string, object?>? right)
        {
            return (left == right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(PropertiesTable? left, IDictionary<string, object?>? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            //Check if the dictionaries contain the same content. Only go down one level.
            if (left.Count != right.Count)
            {
                return false;
            }

            foreach (var key in left.Keys)
            {
                if (!right.ContainsKey(key) || !Equals(left[key], right[key]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public static bool operator !=(PropertiesTable? left, IDictionary<string, object?>? right)
        {
            return !(left == right);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses a property path into a string array.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <returns></returns>
        private string[] GetKeyPath(string propertyPath)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
            {
                return new string[0];
            }
            return propertyPath.Split(new char[] { '/', '.' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Determines whether the PropertiesTable contains value at the specified property path.
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="depth">The recursion depth.</param>
        /// <returns></returns>
        private bool ContainsKey(string propertyPath, int depth = 0)
        {
            const int maxDepth = 100; // Adjust as needed
            if (depth > maxDepth)
            {
                Debug.WriteLine("Maximum recursion depth exceeded when search properties table.");
                return false;
            }

            if (this == null)
            {
                return false;
            }

            var pathParts = GetKeyPath(propertyPath);

            if (pathParts.Length == 0)
            {
                return false;
            }

            // Recursively navigate the object tree.
            object? currentTable = this;

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (currentTable == null)
                {
                    return false;
                }

                var part = pathParts[i];

                if (currentTable is PropertiesTable table)
                {
                    if (table.Keys.Contains(part))
                    {
                        currentTable = table[part];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (currentTable is IList list)
                {
                    if (int.TryParse(part, out int index) && index >= 0 && index < list.Count)
                    {
                        currentTable = list[index];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            //If we have reached the end of the path, then the property exists.
            return true;
        }

        /// <summary>
        /// Tries to get the value of the property at the specified path.
        /// Examples: 
        ///     TryGetValue("address/city", out object city)
        ///     TryGetValue("address.city", out object? city)
        /// </summary>
        /// <param name="propertyPath">Path within the properties table to the desired property.</param>
        /// <param name="depth">The recursion depth.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryGetValue(string propertyPath, out object? value, int depth = 0)
        {
            value = null; // Initialize the out parameter

            const int maxDepth = 100; // Adjust as needed
            if (depth > maxDepth)
            {
                Debug.WriteLine("Maximum recursion depth exceeded when search properties table.");
                return false;
            }

            if (this == null)
            {
                return false;
            }

            var pathParts = GetKeyPath(propertyPath);

            if (pathParts.Length == 0)
            {
                value = this;
                return true;
            }

            // Recursively navigate the object tree.
            object? currentTable = this;

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (currentTable == null)
                {
                    return false;
                }

                var part = pathParts[i];

                if (currentTable is IDictionary<string, object?> table)
                {
                    if (table.Keys.Contains(part))
                    {
                        currentTable = table[part];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (currentTable is IList list)
                {
                    if (int.TryParse(part, out int index) && index >= 0 && index < list.Count)
                    {
                        currentTable = list[index];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            //If we have reached the end of the path, then the property exists. But may be null.
            value = currentTable;
            return true;
        }

        #endregion
    }
}
