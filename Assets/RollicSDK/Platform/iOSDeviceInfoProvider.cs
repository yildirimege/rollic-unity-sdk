using RollicSDK.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace RollicSDK.Platform
{
    /// <summary>
    /// iOS-specific implementation of IDeviceInfoProvider.
    /// Uses UnityEngine.iOS APIs and shows where native code would be needed for more detail.
    /// </summary>
    internal class iOSDeviceInfoProvider : IDeviceInfoProvider
    {
        public Dictionary<string, object> GetDeviceInfo()
        {
            var deviceInfo = new Dictionary<string, object>
            {
                { "platform", "iOS" },
                { "device_model", SystemInfo.deviceModel },
                { "os_version", "iOS " + Device.systemVersion },
                { "device_id", Device.vendorIdentifier } 
            };
            
            // NOTE: To get the IDFA (Identifier for Advertisers), you would need to:
            // 1. Add the AppTrackingTransparency framework to your Xcode project.
            // 2. Request user permission via ATTrackingManager.RequestTrackingAuthorization.
            // 3. Call a native Objective-C/Swift method from C# (via [DllImport("__Internal")])
            //    to get the advertisingIdentifier.
            // deviceInfo["advertising_id"] = "CALL_TO_NATIVE_PLUGIN_FOR_IDFA";
            
            return deviceInfo;
        }
    }
}