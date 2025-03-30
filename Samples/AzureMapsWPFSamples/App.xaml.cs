using AzureMapsNativeControl;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AzureMapsWPFSamples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AzureMapsConfiguration.Configure(new AzureMapsConfiguration
            {
                // Set the subscription key
                SubscriptionKey = "<Your_Azure_Maps_Key"
            });

            base.OnStartup(e);
        }
    }
}
