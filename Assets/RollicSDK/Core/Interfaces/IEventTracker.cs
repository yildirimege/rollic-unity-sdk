namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface for tracking user events in the SDK.
    /// </summary>
    public interface IEventTracker
    {
        /// <summary>
        /// Tracks an arbitrary user event.
        /// </summary>
        /// <param name="eventName">The name of the event (e.g., "button_click").</param>
        void TrackEvent(string eventName);
    }
}

