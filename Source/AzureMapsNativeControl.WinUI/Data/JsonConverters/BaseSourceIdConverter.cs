using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    internal class BaseSourceIdConverter<T> : JsonConverter<T> where T : BaseSource
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Read the id.
            string? id = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.GetString() == "id")
                    {
                        reader.Read();
                        id = reader.GetString();
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }
            if (id != null)
            {
                //Create a new instance of the source with the id.
                var source = (T)Activator.CreateInstance(typeToConvert, null, null, id);
                return source;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            //Only write the id as a property of an object.
            writer.WriteStartObject();
            writer.WriteString("id", value.Id);
            writer.WriteEndObject();
        }
    }
}