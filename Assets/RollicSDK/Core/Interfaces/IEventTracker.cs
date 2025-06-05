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
        /// <param name="additionalData">Optional dictionary of additional event data.</param>
        void TrackEvent(string eventName, Dictionary<string, object> additionalData = null);
    }
}
