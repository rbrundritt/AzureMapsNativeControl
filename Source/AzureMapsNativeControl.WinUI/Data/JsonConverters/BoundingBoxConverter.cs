using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Converts a BoundingBox object to and from JSON.
    /// </summary>
    public class BoundingBoxConverter : JsonConverter<BoundingBox>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(BoundingBox).IsAssignableFrom(typeToConvert);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="type">Type of the object.</param>
        /// <param name="options">The calling serializer options.</param>
        /// <returns>The object value.</returns>
        public override BoundingBox? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var coordinates = new List<double>();

                //Get all numbers if it is a sequence.
                do
                {
                    coordinates.Add(reader.GetDouble());
                } while (reader.Read() && reader.TokenType == JsonTokenType.Number);

                if (coordinates != null && coordinates.Count >= 4)
                {
                    return BoundingBox.FromArray(coordinates);
                }
            } 
            else
            {
                using var document = JsonDocument.ParseValue(ref reader);
                return Read(document.RootElement);
            }

            return null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="bounds">The value.</param>
        /// <param name="options">The calling serializer options.</param>
        public override void Write(Utf8JsonWriter writer, BoundingBox bounds, JsonSerializerOptions options)
        {
            Write(writer, bounds, false);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="bbox"></param>
        /// <param name="sigDigits"></param>
        internal static void Write(Utf8JsonWriter writer, BoundingBox? bbox, bool includePropertyName, int? sigDigits = null)
        {
            if (bbox != null)
            {
                if (includePropertyName)
                {
                    writer.WritePropertyName(Constants.BBoxProperty);
                }

                writer.WriteStartArray();

                if (sigDigits.HasValue)
                {
                    writer.WriteNumberValue(Math.Round(bbox.West, sigDigits.Value));
                    writer.WriteNumberValue(Math.Round(bbox.South, sigDigits.Value));

                    if (bbox.MinAltitude != null)
                    {
                        writer.WriteNumberValue(Math.Round(bbox.MinAltitude.Value, sigDigits.Value));
                    }

                    writer.WriteNumberValue(Math.Round(bbox.East, sigDigits.Value));
                    writer.WriteNumberValue(Math.Round(bbox.North, sigDigits.Value));

                    if (bbox.MaxAltitude != null)
                    {
                        writer.WriteNumberValue(Math.Round(bbox.MaxAltitude.Value, sigDigits.Value));
                    }
                }
                else
                {
                    writer.WriteNumberValue(bbox.West);
                    writer.WriteNumberValue(bbox.South);

                    if (bbox.MinAltitude != null)
                    {
                        writer.WriteNumberValue(bbox.MinAltitude.Value);
                    }

                    writer.WriteNumberValue(bbox.East);
                    writer.WriteNumberValue(bbox.North);

                    if (bbox.MaxAltitude != null)
                    {
                        writer.WriteNumberValue(bbox.MaxAltitude.Value);
                    }
                }

                writer.WriteEndArray();
            }
        }

        internal static BoundingBox? Read(JsonElement element)
        {
            List<double>? coordinates = new List<double>();

            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var coord in element.EnumerateArray())
                {
                    if (coord.ValueKind == JsonValueKind.Number)
                    {
                        coordinates.Add(coord.GetDouble());
                    }
                }

                return BoundingBox.FromArray(coordinates);
            }
            else if (element.ValueKind == JsonValueKind.String)
            {
                var parts = element.GetString()?.Split(',');

                if (parts != null)
                {
                    foreach (var part in parts)
                    {
                        if (double.TryParse(part, out double coord))
                        {
                            coordinates.Add(coord);
                        }
                    }
                }
            }

            if (coordinates != null && coordinates.Count >= 4)
            {
                return BoundingBox.FromArray(coordinates);
            }

            return null;
        }

        #endregion
    }
}