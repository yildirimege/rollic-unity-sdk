using System.Collections.Generic;
using RollicSDK.Core.Dataclasses;
using RollicSDK.Core.Interfaces;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of the IEventTracker interface.
    /// Responsible for creating and managing event data.
    /// </summary>
    internal class EventTracker : IEventTracker
    {
        /// <inheritdoc />
        public void TrackEvent(string eventName, Dictionary<string, object> additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new System.ArgumentException("Event name must not be null or empty", nameof(eventName));
            }

            Event newEvent = new Event(eventName, additionalData);

            // TODO: Add event to que and persist locally
            UnityEngine.Debug.Log($"[EventTracker] Tracking event: {newEvent.EventName}");
        }
    }
}
