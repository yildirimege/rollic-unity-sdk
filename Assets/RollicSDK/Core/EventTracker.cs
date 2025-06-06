using System.Collections.Generic;
using RollicSDK.Core.Dataclasses;
using RollicSDK.Core.Interfaces;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of the IEventTracker interface.
    /// Responsible for creating and managing event data.
    /// </summary>
    internal class EventTracker : IEventTracker
    {
        private readonly INetworkManager _networkManager;
        private readonly RollicSDKConfig _config;

        public EventTracker(INetworkManager networkManager, RollicSDKConfig config = null)
        {
            _networkManager = networkManager ?? throw new System.ArgumentNullException(nameof(networkManager));
            _config = config;
        }

        /// <inheritdoc />
        public void TrackEvent(string eventName, Dictionary<string, object> additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new System.ArgumentException("Event name must not be null or empty", nameof(eventName));
            }

            Dataclasses.Event newEvent = new Dataclasses.Event(eventName, additionalData);

            // TODO: Add event to queue and persist locally
            if (_config != null && _config.EnableDebugLogging)
            {
                Debug.Log($"[EventTracker] Tracking event: {newEvent.EventName}");
            }
        }
    }
}
