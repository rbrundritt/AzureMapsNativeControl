using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#if WINUI
using Microsoft.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Options for specifying how the map control should authenticate with the Azure Maps services.
    /// </summary>
#if MAUI
    public class MapAuthOptions : BindableObject
#else
    public class MapAuthOptions
#endif
    {
        // Excluded options: authContext (consider exposing in the future if needed).

        #region Constructor

        /// <summary>
        /// Options for specifying how the map control should authenticate with the Azure Maps services.
        /// </summary>
        public MapAuthOptions()
        {
            AuthType = AuthenticationType.SubscriptionKey;
        }

        /// <summary>
        /// Options for specifying how the map control should authenticate with the Azure Maps services.
        /// </summary>
        /// <param name="azureKeyCredential">Shared key authenication</param>
        public MapAuthOptions(Azure.AzureKeyCredential azureKeyCredential)
        {
            AuthType = AuthenticationType.SubscriptionKey;
            SubscriptionKey = azureKeyCredential.Key;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The authentication mechanism to be used.
        /// </summary>
        [JsonPropertyName("authType")]
        public AuthenticationType? AuthType { get; set; }

        /// <summary>
        /// Subscription key from your Azure Maps account.
        /// Must be specified for subscription key authentication type.
        /// </summary>
        [JsonPropertyName("subscriptionKey")]
        public string? SubscriptionKey { get; set; }

        /// <summary>
        /// The Azure Maps client ID, This is an unique identifier used to identify the maps account.
        /// Preferred to always be specified, but must be specified for AAD and anonymous authentication types.
        /// </summary>
        [JsonPropertyName("clientId")]
        public string? ClientId { get; set; }

        /// <summary>
        /// The Azure AD registered app ID. This is the app ID of an app registered in your Azure AD tenant.
        /// Must be specified for AAD authentication type.
        /// </summary>
        [JsonPropertyName("aadAppId")]
        public string? AadAppId { get; set; }

        /// <summary>
        /// The AAD tenant that owns the registered app specified by `aadAppId`.
        /// Must be specified for AAD authentication type.
        /// </summary>
        [JsonPropertyName("aadTenant")]
        public string? AadTenant { get; set; }

        /// <summary>
        /// The AAD instance to use for logging in.
        /// Can be optionally specified when using the AAD authentication type.
        /// By default the `https://login.microsoftonline.com/` instance will be used.
        /// </summary>
        [JsonPropertyName("aadInstance")]
        public string? AadInstance { get; set; }

        /// <summary>
        /// Optionally provide an initial token for sas authentication.
        /// </summary>
        [JsonPropertyName("sasToken")]
        public string? SasToken { get; set; }

        /// <summary>
        /// A callback to use with the anonymous/sas authentication mechanism.
        /// This callback will be responsible for resolving to a authentication token.
        /// Recieves the map instance and a the auth options as parameters.
        /// Returns a string that is the token to use for authentication.
        /// </summary>
        [JsonIgnore]
        public Func<Map, MapAuthOptions, Task<string>>? GetTokenAsync { get; set; }

        #endregion
    }
}
