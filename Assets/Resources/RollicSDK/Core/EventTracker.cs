// File: RollicSDK/Core/EventTracker.cs
using RollicSDK.Data;
using RollicSDK.Core.Interfaces;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of IEventTracker.
    /// It uses the EventData factory to create a complete event object and passes it to the persistent queue.
    /// </summary>
    internal class EventTracker : IEventTracker
    {
        private readonly EventQueue _eventQueue;
        private readonly IDeviceInfoProvider _deviceInfoProvider;
        private readonly RollicSDKConfig _config;

        public EventTracker(EventQueue eventQueue, IDeviceInfoProvider deviceInfoProvider, RollicSDKConfig config)
        {
            _eventQueue = eventQueue;
            _deviceInfoProvider = deviceInfoProvider;
            _config = config;
        }

        /// <summary>
        /// Creates a complete event data object and queues it for sending.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="sessionTime">The current session duration in seconds at the time of the event.</param>
        public void TrackEvent(string eventName, double sessionTime)
        {
            // The logic for populating data is now correctly encapsulated inside the EventData class itself.
            // The EventTracker's only job is to orchestrate the creation.
            var newEvent = EventData.Create(eventName, sessionTime, _deviceInfoProvider);

            _eventQueue.Enqueue(newEvent);

            if (_config.EnableDebugLogging)
            {
                Debug.Log($"[EventTracker] Queued event: '{eventName}'. Queue size: {_eventQueue.Count}");
            }
        }

        public int GetQueuedEventCount()
        {
            return _eventQueue.Count;
        }
    }
}