
namespace AzureMapsMauiSamples
{
    public static class Helpers
    {
        public static Random Rand = new Random();

        /// <summary>
        /// Helper to get string values from dropdown pickers.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static string GetSelectedPickerString(object sender)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            //Workaround for a bug in the current version of MAUI where the picker selected index is not set correctly.
            if (selectedIndex == -1)
            {
                picker.SelectedIndex = 0;
                selectedIndex = 0;
            }
            if (picker.ItemsSource[selectedIndex] is string value)
            {
                return value;
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
