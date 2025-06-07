using RollicSDK.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace RollicSDK.Platform
{
    /// <summary>
    /// Android-specific implementation of IDeviceInfoProvider.
    /// Uses AndroidJavaClass to access native Android APIs for richer device information.
    /// </summary>
    internal class AndroidDeviceInfoProvider : IDeviceInfoProvider
    {
        public Dictionary<string, object> GetDeviceInfo()
        {
            var deviceInfo = new Dictionary<string, object>
            {
                { "platform", "Android" },
                { "device_id", SystemInfo.deviceUniqueIdentifier }
            };

            try
            {
                // Natve Android Build Props
                var buildClass = new AndroidJavaClass("android.os.Build");
                var versionClass = new AndroidJavaClass("android.os.Build$VERSION");

                deviceInfo["device_model"] = buildClass.GetStatic<string>("MODEL");
                deviceInfo["os_version"] = $"Android {versionClass.GetStatic<string>("RELEASE")} (API {versionClass.GetStatic<int>("SDK_INT")})";

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AndroidDeviceInfoProvider] Failed to get native info: {ex.Message}. Using fallbacks.");
                deviceInfo["device_model"] = SystemInfo.deviceModel;
                deviceInfo["os_version"] = SystemInfo.operatingSystem;
            }

            return deviceInfo;
        }
    }
}