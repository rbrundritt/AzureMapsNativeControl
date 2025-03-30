using AzureMapsNativeControl.Core;
using System.Globalization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Represents a padding around an area.
    /// </summary>
    public class Padding: IDeepCloneable<Padding>
    {
        #region Constructor

        /// <summary>
        /// Represents a padding around an area.
        /// </summary>
        /// <param name="pixelPadding">A constant padding on all four sides.</param>
        public Padding(int pixelPadding)
        {
            Left = pixelPadding;
            Right = pixelPadding;
            Top = pixelPadding;
            Bottom = pixelPadding;
        }

        /// <summary>
        /// Represents a padding around an area.
        /// </summary>
        /// <param name="left">Left padding.</param>
        /// <param name="right">Right padding.</param>
        /// <param name="top">Top padding.</param>
        /// <param name="bottom">Bottom padding.</param>
        public Padding(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// The left padding.
        /// </summary>
        [JsonPropertyName("left")]
        public int Left { get; }

        /// <summary>
        /// The right padding.
        /// </summary>
        [JsonPropertyName("right")]
        public int Right { get; }

        /// <summary>
        /// The top padding.
        /// </summary>
        [JsonPropertyName("top")]
        public int Top { get; }

        /// <summary>
        /// The bottom padding.
        /// </summary>
        [JsonPropertyName("bottom")]
        public int Bottom { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the padding.
        /// </summary>
        /// <returns></returns>
        public Padding DeepClone()
        {
           return new Padding(Left, Right, Top, Bottom);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{top:{0},bottom:{1},left:{2},right:{3}}},", Top, Bottom, Left, Right);
        }

        /// <summary>
        /// Parses a padding from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Padding? Parse(string? value)
        {
            if (value != null)
            {
                if (value.Contains("[") || value.Contains("]"))
                {
                    value = value.Replace("[", "").Replace("]", "");
                }

                var parts = value.Split(',');

                if (parts.Length == 1 && int.TryParse(parts[0].Trim(), out int val))
                {
                    return new Padding(val);
                }
                else if (parts.Length == 4 &&
                    int.TryParse(parts[0].Trim(), out int left) &&
                    int.TryParse(parts[1].Trim(), out int right) &&
                    int.TryParse(parts[2].Trim(), out int top) &&
                    int.TryParse(parts[3].Trim(), out int bottom))
                {
                    return new Padding(left, right, top, bottom);
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to parse a padding from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static bool TryParse(string? value, out Padding? padding)
        {
            padding = Parse(value);
            return padding != null;
        }

        #endregion
    }
}
