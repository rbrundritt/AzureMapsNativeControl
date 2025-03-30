using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// Converter to read and write an <see cref="Position" />, that is, the coordinates of a <see cref="Point" />.
    /// </summary>
    public class PositionConverter : JsonConverter<Position>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Position).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override Position? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return ReadPosition(ref reader, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Position position, JsonSerializerOptions options)
        {
            PositionConverter.WritePosition(writer, position);
        }

        #endregion 

        #region Internal Methods

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Utf8JsonReader" /> to read from.</param>
        /// <param name="type">Type of the object.</param>
        /// <param name="options"></param>
        /// <returns>
        /// The object value.
        /// </returns>
        internal static Position? ReadPosition(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();

                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        double longitude = reader.GetDouble();

                        reader.Read();

                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            double latitude = reader.GetDouble();

                            reader.Read();

                            if (reader.TokenType == JsonTokenType.Number)
                            {
                                double altitude = reader.GetDouble();

                                return new Position(longitude, latitude, altitude);
                            }

                            return new Position(longitude, latitude);
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore and return null.
            }

            return null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="position">Position to write.</param>
        /// <param name="sigDigits">Number of significant digits to write number values to. 6 ~= 10cm accuracy.</param>
        internal static void WritePosition(Utf8JsonWriter writer, Position position, int? sigDigits = null)
        {
            writer.WriteStartArray();

            if(sigDigits.HasValue)
            {
                writer.WriteNumberValue(Math.Round(position.Longitude, sigDigits.Value));
                writer.WriteNumberValue(Math.Round(position.Latitude, sigDigits.Value));

                if (position.Altitude.HasValue)
                {
                    writer.WriteNumberValue(Math.Round(position.Altitude.Value, sigDigits.Value));
                }
            }
            else
            {
                writer.WriteNumberValue(position.Longitude);
                writer.WriteNumberValue(position.Latitude);

                if (position.Altitude.HasValue)
                {
                    writer.WriteNumberValue(position.Altitude.Value);
                }
            }

            writer.WriteEndArray();
        }

        #endregion
    }
}