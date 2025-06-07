using System;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

namespace RollicSDK.Data
{
    /// <summary>
    /// Represents the complete, serializable payload for a single tracked event.
    /// This class combines event-specific data, session data, and device context.
    /// </summary>
    [Serializable]
    public class EventData
    {
        // --- Core Event Info ---
        [JsonProperty("event_name")]
        public string EventName { get; private set; }

        [JsonProperty("event_id")]
        public string EventId { get; private set; }

        [JsonProperty("timestamp_utc")]
        public long TimestampUtc { get; private set; }

        // --- Session Info ---
        [JsonProperty("session_time")]
        public long SessionTime { get; private set; }

        // --- Device Context ---
        [JsonProperty("platform")]
        public string Platform { get; private set; }

        [JsonProperty("os_version")]
        public string OsVersion { get; private set; }

        [JsonProperty("device_model")]
        public string DeviceModel { get; private set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; private set; }

        /// <summary>
        /// Private constructor to be used by the factory method.
        /// </summary>
        private EventData() { }

        /// <summary>
        /// Factory method to create a new, fully populated EventData object.
        /// This is the only way to create an instance, ensuring all data is present.
        /// </summary>
        /// <param name="eventName">The name of the event being tracked.</param>
        /// <param name="sessionDuration">Current duration of the user session in seconds.</param>
        /// <param name="deviceInfoProvider">The platform-specific adapter for device info.</param>
        /// <returns>A new, complete EventData object.</returns>
        public static EventData Create(string eventName, double sessionDuration, Core.Interfaces.IDeviceInfoProvider deviceInfoProvider)
        {
            var data = new EventData
            {
                // Set event data
                EventName = eventName,
                EventId = Guid.NewGuid().ToString(),
                TimestampUtc = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),

                // Set session data
                SessionTime = (long)sessionDuration
            };

            // Populate device context using the provider
            var deviceInfo = deviceInfoProvider.GetDeviceInfo();
            data.Platform = deviceInfo.ContainsKey("platform") ? deviceInfo["platform"].ToString() : Application.platform.ToString();
            data.OsVersion = deviceInfo.ContainsKey("os_version") ? deviceInfo["os_version"].ToString() : SystemInfo.operatingSystem;
            data.DeviceModel = deviceInfo.ContainsKey("device_model") ? deviceInfo["device_model"].ToString() : SystemInfo.deviceModel;
            data.DeviceId = deviceInfo.ContainsKey("device_id") ? deviceInfo["device_id"].ToString() : SystemInfo.deviceUniqueIdentifier;

            return data;
        }
    }
}