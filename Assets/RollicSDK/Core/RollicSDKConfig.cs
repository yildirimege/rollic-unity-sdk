
namespace RollicSDK.Core
{
    /// <summary>
    /// Configuration settings for Rollic SDK.
    /// </summary>
    public sealed class RollicSDKConfig
    {
        /// <summary>
        /// API Endpoint URL.
        /// </summary>
        public string ApiEndpoint { get; }

        /// <summary>
        /// Enable or disable debug logs.
        /// </summary>
        public bool EnableDebugLogging { get; }

        /// <summary>
        /// Creates a new instance of the configuration.
        /// </summary>
        /// <param name="apiEndpoint">API URL string.</param>
        /// <param name="enableDebugLogging">Enable debug logs.</param>
        public RollicSDKConfig(string apiEndpoint, bool enableDebugLogging = false)
        {
            ApiEndpoint = apiEndpoint;
            EnableDebugLogging = enableDebugLogging;
        }
    }
}
