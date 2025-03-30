using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// A JsonConverter for a collection of Features.
    /// </summary>
    public class FeatureEnumerableConverter: JsonConverter<IEnumerable<Feature>>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IEnumerable<Feature>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override IEnumerable<Feature> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            return Read(document.RootElement);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IEnumerable<Feature> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (Feature feature in value)
            {
                FeatureConverter.Write(writer, feature);
            }
            writer.WriteEndArray();
        }

        #endregion

        #region Private Methods

        internal static IList<Feature> Read(JsonElement element)
        {
            return FeatureCollectionConverter.Read(element).Features;
        }

        #endregion
    }
}
