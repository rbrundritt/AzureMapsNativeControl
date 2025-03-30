using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Internal.JsonConverters
{
    /// <summary>
    /// Converts a collection of IPopupContent objects to and from JSON.
    /// </summary>
    public class PopupContentEnumerableConverter : JsonConverter<IEnumerable<IPopupContent>>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IEnumerable<IPopupContent>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override IEnumerable<IPopupContent>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var list = new List<IPopupContent>();
           
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    list.Add(new PopupStringContent() { HtmlTemplate = reader.GetString() });
                }
                else
                {
                    var content = JsonSerializer.Deserialize<PopupPropertyInfoContent>(ref reader, options);

                    if (content != null)
                    {
                        list.Add(content);
                    }
                }
            }

            if (list.Count > 0)
            {
                return list;
            }

            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IEnumerable<IPopupContent> popupContent, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (popupContent != null)
            {
                foreach (var content in popupContent)
                {
                    if(content is PopupStringContent stringContent)
                    {
                        writer.WriteStringValue(stringContent.HtmlTemplate);
                    }
                    else if(content is PopupPropertyInfoContent propertyInfo)
                    {
                        JsonSerializer.Serialize(writer, propertyInfo, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, content, options);
                    }
                }
            }

            writer.WriteEndArray();
        }

        #endregion
    }
}
