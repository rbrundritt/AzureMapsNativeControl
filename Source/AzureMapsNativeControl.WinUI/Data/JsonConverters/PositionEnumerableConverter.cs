using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Converts a list of positions to and from json.
    /// </summary>
    public class PositionEnumerableConverter : JsonConverter<IList<Position>>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IList<Position>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override IList<Position>? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return ReadPositions(ref reader, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IList<Position> positions, JsonSerializerOptions options)
        {
            WritePositions(writer, positions);
        }

        #endregion

        #region Internal Methods

        internal static IList<Position>? ReadPositions(ref Utf8JsonReader reader, JsonSerializerOptions options)
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
                    return new List<Position>(result);
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

        internal static void WritePositions(Utf8JsonWriter writer, IList<Position> positions, int? sigDigits = null)
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