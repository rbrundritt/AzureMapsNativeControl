using AzureMapsNativeControl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzureMapsWinUISamples
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            NavigateToSample("Welcome");
        }

        private void NavigateToSample(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            if (item != null)
            {
                SampleTitle.Text = item.Text;
                NavigateToSample(item.Tag.ToString());
            }
        }

        private void NavigateToSample(string sampleName)
        {
            if (!string.IsNullOrEmpty(sampleName))
            {
                var sampleType = Type.GetType($"AzureMapsWinUISamples.Samples.{sampleName}");
                if (sampleType != null)
                {
                    MainFrame.Navigate(sampleType);
                }
            }
        }
    }
}
