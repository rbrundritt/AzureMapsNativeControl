using AzureMapsNativeControl;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzureMapsWinUISamples
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        public static Window? Window { get; private set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            AzureMapsConfiguration.Configure(new AzureMapsConfiguration
            {
                // Set the subscription key
                SubscriptionKey = "<Your_Azure_Maps_Key>"

                //Alternatively use anonymous authentication
                //ClientId = "<Your_Azure_Maps_Client_Id>",
                //GetTokenAsync = async (config) =>
                //{
                //    // Use the Azure Maps client ID to get a token
                //    var msg = await new HttpClient().GetAsync("https://example.com/gettoken");
                //    return await msg.Content.ReadAsStringAsync();
                //}
            });

            Window = new MainWindow();
            Window.Activate();
        }
    }
}
