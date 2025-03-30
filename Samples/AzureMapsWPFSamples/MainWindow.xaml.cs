using System.Windows;
using System.Windows.Controls;

namespace AzureMapsWPFSamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavigateToSample("Welcome");
        }

        private void NavigateToSample(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item != null)
            {
                SampleTitle.Text = item.Header.ToString();
                NavigateToSample(item.Tag.ToString());
            }
        }

        private void NavigateToSample(string sampleName)
        {
            if (!string.IsNullOrEmpty(sampleName))
            {
                var sampleType = Type.GetType($"AzureMapsWPFSamples.Samples.{sampleName}");
                if (sampleType != null)
                {
                    var sampleInstance = Activator.CreateInstance(sampleType) as Page;
                    if (sampleInstance != null)
                    {
                        MainFrame.Navigate(sampleInstance);
                    }
                }
            }
        }
    }
}