using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// JSON converter for GeoJSON feature properties.
    /// Json values are converted to the following types:
    /// - String: string
    /// - Number: double
    /// - True/False: bool
    /// - Object: Dictionary<string, object?>
    /// - Array: List<object?>
    /// </summary>
    public class PropertiesTableConverter: JsonConverter<PropertiesTable>
    {
        /// <summary>
        /// A list of properties not to write.
        /// </summary>
        private static string[] propertiesNotToWrite = new string[] { 
            Constants.AzureMapsShapeID 
        };

        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IDictionary<string, object?>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override PropertiesTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return ReadProperties(document.RootElement, true);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PropertiesTable value, JsonSerializerOptions? options = null)
        {
            WriteProperties(writer, value, options);
        }

        /// <inheritdoc />
        public void Write(Utf8JsonWriter writer, IDictionary<string, object?> value, JsonSerializerOptions? options = null)
        {
            WriteProperties(writer, value, options);
        }

        #endregion

        #region Read Methods

        internal static PropertiesTable ReadProperties(JsonElement element, bool convertValues)
        {
            var props = new Dictionary<string, object?>();
            
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (convertValues)
                    {
                       props.Add(property.Name, ReadPropertyValue(property.Value));
                    }
                    else
                    {
                        props.Add(property.Name, property.Value);
                    }
                }
            }
            return new PropertiesTable(props);
        }

        private static object? ReadPropertyValue(in JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Object:
                    var dictionary = new Dictionary<string, object?>();
                    foreach (JsonProperty jsonProperty in element.EnumerateObject())
                    {
                        dictionary.Add(jsonProperty.Name, ReadPropertyValue(jsonProperty.Value));
                    }
                    return dictionary;
                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        list.Add(ReadPropertyValue(item));
                    }
                    return list;
                default:
                    throw new NotSupportedException("Not supported value kind " + element.ValueKind);
            }
        }

        #endregion

        #region Write Methods

        internal static void WriteProperties(Utf8JsonWriter writer, IDictionary<string, object?>? properties, JsonSerializerOptions? options = null)
        {
            //Write properties
            writer.WriteStartObject();

            if (properties != null)
            {
                foreach (KeyValuePair<string, object?> pair in properties)
                {
                    if (!propertiesNotToWrite.Contains(pair.Key))
                    {
                        writer.WritePropertyName(pair.Key);
                        WritePropertyValue(writer, pair.Value, options);
                    }
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes the properties value to the GeoJSON object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException"></exception>
        private static void WritePropertyValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions? options = null)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case int i:
                    writer.WriteNumberValue(i);
                    break;
                case double d:
                    writer.WriteNumberValue(d);
                    break;
                case float f:
                    writer.WriteNumberValue(f);
                    break;
                case long l:
                    writer.WriteNumberValue(l);
                    break;
                case string s:
                    writer.WriteStringValue(s);
                    break;
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;
                case IEnumerable<KeyValuePair<string, object?>> enumerable:
                    writer.WriteStartObject();
                    foreach (KeyValuePair<string, object?> pair in enumerable)
                    {
                        writer.WritePropertyName(pair.Key);
                        WritePropertyValue(writer, pair.Value, options);
                    }
                    writer.WriteEndObject();
                    break;
                case IEnumerable<object?> objectEnumerable:
                    writer.WriteStartArray();
                    foreach (object? item in objectEnumerable)
                    {
                        WritePropertyValue(writer, item, options);
                    }
                    writer.WriteEndArray();
                    break;
                default:
                    //Try writing the object.
                    JsonSerializer.Serialize(writer, value, options ?? Constants.MapJsonSerializerOptions);
                    break;
            }
        }

        #endregion
    }
}
