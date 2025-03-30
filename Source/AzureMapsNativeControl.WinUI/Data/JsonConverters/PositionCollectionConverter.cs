using AzureMapsNativeControl.Data;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Converts a PositionCollection object to and from JSON.
    /// </summary>
    public class PositionCollectionConverter : JsonConverter<PositionCollection>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(PositionCollection).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override PositionCollection? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return ReadPositions(ref reader, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PositionCollection positions, JsonSerializerOptions options)
        {
            WritePositions(writer, positions);
        }

        #endregion

        #region Internal Methods

        internal static PositionCollection? ReadPositions(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartArray:
                    break;
                default:
                    throw new InvalidOperationException("Incorrect json type");
            }

            var startDepth = reader.CurrentDepth;
            var result = new List<Position>();

            while (reader.Read())
            {
                if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
                {
                    return new PositionCollection(result);
                }

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    var p = PositionConverter.ReadPosition(ref reader, options);

                    if (p != null)
                    {
                        result.Add(p);
                    }
                }
            }

            return null;
        }

        internal static void WritePositions(Utf8JsonWriter writer, PositionCollection positions, int? sigDigits = null)
        {
            writer.WriteStartArray();
            foreach (var position in positions)
            {
                PositionConverter.WritePosition(writer, position, sigDigits);
            }
            writer.WriteEndArray();
        }

        #endregion
    }
}