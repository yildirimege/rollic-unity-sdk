using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RollicSDK.Core.Interfaces;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of INetworkManager using HttpClient.
    /// </summary>
    public class NetworkManager : INetworkManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RollicSDKConfig _config;

        public NetworkManager(RollicSDKConfig config = null)
        {
            _config = config;
        }

        /// <inheritdoc />
        public async Task<bool> PostAsync(string url, string jsonPayload)
        {
            try
            {
                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogWarning($"NetworkManager: API responded with status {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"NetworkManager: HTTP request failed - {ex.Message}");
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"NetworkManager: Unexpected error - {ex.Message}");
                return false;
            }
        }
    }
}
