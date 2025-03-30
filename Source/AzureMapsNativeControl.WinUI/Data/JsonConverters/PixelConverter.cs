using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Data.JsonConverters
{
    /// <summary>
    /// A custom JSON converter for the <see cref="Pixel"/> class.
    /// </summary>
    public class PixelConverter : JsonConverter<Pixel>
    {
        #region Public Methods

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Pixel).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override Pixel Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            double[]? pixel;

            try
            {
                pixel = JsonSerializer.Deserialize<double[]>(ref reader, options);
            }
            catch (Exception e)
            {
                throw new JsonException("Error parsing pixel", e);
            }

            if (pixel != null && pixel.Length >= 2)
            {
                return new Pixel(pixel[0], pixel[1]);
            }

            throw new JsonException("Pixel cannot be null");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Pixel pixel, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            writer.WriteNumberValue(pixel.X);
            writer.WriteNumberValue(pixel.Y);

            writer.WriteEndArray();
        }

        #endregion
    }
}