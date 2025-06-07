using RollicSDK.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace RollicSDK.Platform
{
    /// <summary>
    /// A default implementation of IDeviceInfoProvider for the Unity Editor and standalone platforms.
    /// It uses cross-platform Unity APIs.
    /// </summary>
    internal class DefaultDeviceInfoProvider : IDeviceInfoProvider
    {
        public Dictionary<string, object> GetDeviceInfo()
        {
            return new Dictionary<string, object>
            {
                { "platform", Application.platform.ToString() },
                { "os_version", SystemInfo.operatingSystem },
                { "device_model", SystemInfo.deviceModel },
                { "device_id", SystemInfo.deviceUniqueIdentifier } // NOTE: for future devs: This id may change on reinstallation, so best not use as a ground truth while querying user data.
            };
        }
    }
}