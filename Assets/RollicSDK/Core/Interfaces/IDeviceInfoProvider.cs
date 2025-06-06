using System.Collections.Generic;

namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface for an adapter that provides platform-specific device information.
    /// This allows the core SDK logic to remain platform-agnostic while still collecting rich contextual data.
    /// </summary>
    public interface IDeviceInfoProvider
    {
        /// <summary>
        /// Gets a dictionary of key-value pairs representing device-specific information.
        /// Common keys include 'os_version', 'device_model', 'device_id'.
        /// </summary>
        /// <returns>A dictionary containing device context information.</returns>
        Dictionary<string, object> GetDeviceInfo();
    }
}