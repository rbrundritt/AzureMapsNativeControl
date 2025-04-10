using AzureMapsNativeControl;
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
                SubscriptionKey = " < Your_Azure_Maps_Key>"

                //Alternatively use anonymous authentication
                //ClientId = "<Your_Azure_Maps_Client_Id>",
                //GetTokenAsync = async (config) =>
                //{
                //    // Use the Azure Maps client ID to get a token
                //    var msg = await new HttpClient().GetAsync("https://example.com/gettoken");
                //    return await msg.Content.ReadAsStringAsync();
                //}
            });

            base.OnStartup(e);
        }
    }
}
