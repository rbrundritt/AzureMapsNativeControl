using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AzureMapsNativeControl.Data.JsonConverters;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// An enumeration used to specify the type of authentication mechanism to use.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum AuthenticationType
    {
        /// <summary>
        /// The subscription key authentication mechanism.
        /// Literal value `"subscriptionKey"`
        /// </summary>
        [EnumMember(Value = "subscriptionKey")]
        SubscriptionKey,

        /// <summary>
        /// The AAD implicit grant mechanism. Recommended for pages protected by a sign-in.
        /// By default the page will be redirected to the AAD login when the map control initializes.
        /// Specify a logged-in `PublicClientApplication` in the `AuthenticationOptions`
        /// for greater control over when/how the users signs in.
        /// Literal value `"aad"`
        /// </summary>
        [EnumMember(Value = "aad")]
        Aad,

        /// <summary>
        /// The anonymous authentication mechanism. Recommended for public pages.
        /// Allows a callback responsible for acquiring an authentication token to be provided.
        /// Literal value `"anonymous"`
        /// </summary>
        [EnumMember(Value = "anonymous")]
        Anonymous,

        /// <summary>
        /// The shared access signature authentication mechanism. Allows a callback responsible for acquiring a token to be provided on requests.
        /// Literal value `"sas"`.
        /// </summary>
        [EnumMember(Value = "sas")]
        Sas
    }
}
