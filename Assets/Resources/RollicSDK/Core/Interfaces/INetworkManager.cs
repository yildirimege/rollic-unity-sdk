namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface defining the contract for network operations.
    /// </summary>
    public interface INetworkManager
    {
        /// <summary>
        /// Sends a POST request asynchronously to the given URL with the provided JSON payload.
        /// </summary>
        /// <param name="url">The target API endpoint.</param>
        /// <param name="jsonPayload">The JSON string to send in the body.</param>
        /// <returns>True if the request succeeded, false otherwise.</returns>
        System.Threading.Tasks.Task<bool> PostAsync(string url, string jsonPayload);
    }
}
