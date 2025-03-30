using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Options for specifying how the map control should authenticate with the Azure Maps services.
    /// </summary>
    [JsonConverter(typeof(AzureMapsAuthConfigJsonConverter))]
    public sealed class AzureMapsConfiguration
    {
#if !MAUI
        /// <summary>
        /// Sets the global configuration for the Azure Maps control.
        /// </summary>
        /// <param name="configuration"></param>
        public static void Configure(AzureMapsConfiguration configuration)
        {
            _cachedConfig = configuration;
        }

#endif

        #region Service Options

        /// <summary>
        /// Disable telemetry collection.
        /// </summary>
        [JsonPropertyName("disableTelemetry")]
        public bool? DisableTelemetry { get; set; }

        /// <summary>
        /// The domain of the Azure Maps service to access. Normally "", but could be a different domain for government clouds such as "atlas.azure.us" for US gov cloud.
        /// </summary>
        public string Domain { get; set; } = "atlas.microsoft.com";


        /// <summary>
        /// Enable the accessibility feature to provide screen reader support for users who have difficulty visualizing the web application. 
        /// </summary>
        public bool? EnableAccessibility { get; set; }

        /// <summary>
        /// Enable the fallback to the REST API geocoder for detecting location accessibility if extracting location from vector data fails. 
        /// Disabling this option will prevent the generation of geocode API requests but may lead to a lack of location information for screen readers.
        /// </summary>
        public bool? EnableAccessibilityLocationFallback { get; set; }

        /// <summary>
        /// Sets the default session id used by the map and service modules unless the
        /// session id is explicitly specified when using those parts of the API.
        /// </summary>
        public string? SessionId { get; set; }

        #endregion

        #region Auth Options

        /// <summary>
        /// The Azure AD registered app ID. This is the app ID of an app registered in your Azure AD tenant.
        /// </summary>
        public string? AadAppId { get; set; }

        /// <summary>
        /// The AAD instance to use for logging in. Can be optionally specified when using the AAD authentication type. 
        /// By default the https://login.microsoftonline.com/ instance will be used.
        /// </summary>
        public string? AadInstance { get; set; }

        /// <summary>
        /// The AAD tenant that owns the registered app specified by `aadAppId`.
        /// </summary>
        public string? AadTenant { get; set; }

        /// <summary>
        /// The Azure Maps client ID, This is an unique identifier used to identify the maps account.
        /// Must be specified for AAD and anonymous authentication types.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Subscription key from your Azure Maps account.
        /// Must be specified for subscription key authentication type.
        /// </summary>
        public string? SubscriptionKey { get; set; }

        /// <summary>
        /// The Azure Active Directory domain to authenticate the map control with.
        /// </summary>
        public string? SasToken { get; set; }

        /// <summary>
        /// A callback to use with the anonymous/sas authentication mechanism.
        /// This callback will be responsible for resolving to a authentication token.
        /// Recieves the map instance and a the auth options as parameters.
        /// Returns a string that is the token to use for authentication.
        /// </summary>
        public Func<AzureMapsConfiguration, Task<string>>? GetTokenAsync { get; set; }

        #endregion

        #region Internal Methods

        internal string GetDomain()
        {
            return string.IsNullOrWhiteSpace(Domain) ? "atlas.microsoft.com" : Domain;
        }

        internal bool ValidateAuth() => !string.IsNullOrWhiteSpace(AuthType);

        internal string? AuthType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SubscriptionKey))
                {
                    return "subscriptionKey";
                }

                if(!string.IsNullOrWhiteSpace(SasToken))
                {
                    return "sas";
                }

                if (!string.IsNullOrWhiteSpace(ClientId))
                {
                    return !string.IsNullOrWhiteSpace(AadAppId)
                    && !string.IsNullOrWhiteSpace(AadTenant)
                    ? "aad"
                    : "anonymous";
                }

                return null;
            }
        }

        internal static AzureMapsConfiguration? _cachedConfig = null;

        internal static AzureMapsConfiguration GetInstance()
        {
            if(_cachedConfig != null)
            {
                return _cachedConfig;
            }

#if MAUI
            if (AzureMapsServiceCollectionExtension.Configuration != null)
            {
                _cachedConfig = new AzureMapsConfiguration();
                AzureMapsServiceCollectionExtension.Configuration?.Invoke(_cachedConfig);
                return _cachedConfig;
            }
#endif

            if (_cachedConfig == null || !_cachedConfig.ValidateAuth())
            {
                throw new Exception("Invalid Azure Maps Auth configuration provided.");
            }

            return _cachedConfig;
        }

        #endregion
    }

    internal sealed class AzureMapsAuthConfigJsonConverter : JsonConverter<AzureMapsConfiguration>
    {
        public override AzureMapsConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();
        
        public override void Write(Utf8JsonWriter writer, AzureMapsConfiguration value, JsonSerializerOptions options)
        {
            var authType = value.AuthType;
            
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(value.Domain))
            {
                writer.WriteString("domain", value.Domain);
            }

            if (value.DisableTelemetry != null)
            {
                writer.WriteBoolean("disableTelemetry", value.DisableTelemetry.Value);
            }

            if (value.EnableAccessibility != null)
            {
                writer.WriteBoolean("enableAccessibility", value.EnableAccessibility.Value);
            }

            if (value.EnableAccessibilityLocationFallback != null)
            {
                writer.WriteBoolean("enableAccessibilityLocationFallback", value.EnableAccessibilityLocationFallback.Value);
            }

            if (!string.IsNullOrWhiteSpace(value.SessionId))
            {
                writer.WriteString("sessionId", value.SessionId);                
            }

            writer.WriteStartObject("authOptions");
            writer.WriteString("authType", authType);

            if (authType == "subscriptionKey")
            {
                writer.WriteString("subscriptionKey", value.SubscriptionKey);
            }
            else if (authType == "aad")
            {
                writer.WriteString("aadAppId", value.AadAppId);
                writer.WriteString("aadTenant", value.AadTenant);
                writer.WriteString("clientId", value.ClientId);

                if (!string.IsNullOrWhiteSpace(value.AadInstance))
                {
                    writer.WriteString("aadInstance", value.AadInstance);
                }
            }
            else if(authType == "sas")
            {
                writer.WriteString("sasToken", value.SasToken);
            }
            else if (authType == "anonymous")
            {
                writer.WriteString("clientId", value.ClientId);
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}
