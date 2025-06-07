using System.Collections.Generic;

namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface for tracking user events in the SDK.
    /// </summary>
    public interface IEventTracker
    {
        /// <summary>
        /// Tracks an arbitrary user event with the specified name and optional additional data.
        /// </summary>
        /// <param name="eventName">The name of the event (mandatory).</param>
        /// <param name="sessionTime">Duration of current session in seconds</param>
        void TrackEvent(string eventName, double sessionTime);

        /// <summary>
        /// Gets the number of events currently waiting in the queue.
        /// </summary>
        /// <returns>The count of queued events.</returns>
        int GetQueuedEventCount();
    }
}
