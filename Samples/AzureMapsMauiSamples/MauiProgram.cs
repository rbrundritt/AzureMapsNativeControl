using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AzureMapsMauiSamples
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            //Attach Azure Maps to services. Pass the auth information into the configuration callback.
            builder.Services.AddAzureMaps(configuration => {
                configuration.SubscriptionKey = "<Your_Azure_Maps_Key>";

                //Alternatively use anonymous authentication
                //configuration.ClientId = "<Your_Azure_Maps_Client_Id>",
                //configuration.GetTokenAsync = async (config) =>
                //{
                //    // Use the Azure Maps client ID to get a token
                //    var msg = await new HttpClient().GetAsync("https://example.com/gettoken");
                //    return await msg.Content.ReadAsStringAsync();
                //}
            });

            return builder.Build();
        }
    }
}
