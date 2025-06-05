
using RollicSDK.Core.Interfaces;
using System;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of IEventTracker.
    /// Currently logs events to console, will extend to network layer later.
    /// </summary>
    public sealed class EventTracker : IEventTracker
    {
        public void TrackEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));
            }

            UnityEngine.Debug.Log($"[EventTracker] Event tracked: {eventName}");
        }
    }
}
