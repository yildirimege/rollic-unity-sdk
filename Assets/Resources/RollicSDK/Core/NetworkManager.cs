using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RollicSDK.Core.Interfaces;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;


namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of INetworkManager using HttpClient.
    /// It's recommended to use UnityWebRequest for better engine integration,
    /// but HttpClient works well for non-WebGL platforms.
    /// </summary>
    public class NetworkManager : INetworkManager
    {
        // A single static HttpClient is recommended for the application's lifetime.
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = System.TimeSpan.FromSeconds(10) };
        private readonly RollicSDKConfig _config;

        public NetworkManager(RollicSDKConfig config)
        {
            _config = config;
        }

        /// <inheritdoc />
        public async Task<bool> PostAsync(string url, string jsonPayload)
        {
            if (_config.EnableDebugLogging)
            {
                Debug.Log($"[NetworkManager] Sending POST to {url}\nPayload: {jsonPayload}");
            }

            try
            {
                // Using a 'using' statement ensures the content is disposed of correctly.
                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.LogWarning($"[NetworkManager] API request failed. Status: {response.StatusCode}. Response: {responseContent}");
                    return false;
                }

                if (_config.EnableDebugLogging)
                {
                    Debug.Log($"[NetworkManager] API request successful. Status: {response.StatusCode}");
                }
                return true;
            }
            catch (TaskCanceledException ex) // Catches timeouts
            {
                Debug.LogError($"[NetworkManager] HTTP request timed out: {ex.Message}");
                return false;
            }
            catch (HttpRequestException ex) // Catches DNS, connection, and other transport errors
            {
                Debug.LogError($"[NetworkManager] HTTP request error: {ex.Message}");
                return false;
            }
            catch (System.Exception ex) // Catch-all for other unexpected errors
            {
                Debug.LogError($"[NetworkManager] An unexpected error occurred: {ex.Message}");
                return false;
            }
        }
    }
}