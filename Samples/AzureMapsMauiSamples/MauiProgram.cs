using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

#if IOS || ANDROID
using AzureMapsMauiSamples.Platforms;
#endif

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

            //Custom logic for iOS and Android to disable the swipe gesture of the shell navigation as it conflicts with the map gesture.
            builder.ConfigureMauiHandlers(handlers =>
            {
#if IOS
                handlers.AddHandler(typeof(Shell), typeof(CustomShellRenderer));
#elif ANDROID
                handlers.AddHandler(typeof(Shell), typeof(CustomShellRenderer));
#endif
            });

            return builder.Build();
        }
    }
}
