using Microsoft.UI.Xaml.Controls;
using System;

namespace AzureMapsWinUISamples
{
    internal class Helpers
    {
        public static Random Rand = new Random();

        /// <summary>
        /// Helper to get string values from dropdown pickers.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static string GetSelectedPickerString(object sender)
        {
            var picker = (ComboBox)sender;
            if (picker != null && picker.SelectedValue != null)
            {
                return ((ComboBoxItem)(picker.SelectedValue)).Content.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Helper to get a random color string.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomColorString()
        {
            return $"#{Rand.Next(0x1000000):X6}";
        }
    }
}
