using AzureMapsNativeControl;
using HybridWebView;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureMapsServiceCollectionExtension
    {
        internal static Action<AzureMapsConfiguration>? Configuration;

        public static void AddAzureMaps(this IServiceCollection services, Action<AzureMapsConfiguration> configuration)
        {
            Configuration = configuration;

            //Configure the HybridWebViewHandler for the HybridWebView
            services
                .ConfigureMauiHandlers(static handlers => handlers.AddHandler<HybridWebView.HybridWebView, HybridWebViewHandler>())
                .AddOptions<AzureMapsConfiguration>()
                .Configure(configuration);
        }
    }
}
