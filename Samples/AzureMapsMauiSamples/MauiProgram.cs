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
                configuration.SubscriptionKey = "<Your_Azure_Maps_Key>";// builder.Configuration["AzureMaps:SubscriptionKey"];
            });

            return builder.Build();
        }
    }
}
