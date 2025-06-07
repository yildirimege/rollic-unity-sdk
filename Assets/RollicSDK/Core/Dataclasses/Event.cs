using System.Collections.Generic;

namespace RollicSDK.Core.Dataclasses
{
    /// <summary>
    /// Represents an event to be tracked by the SDK.
    /// This is an internal class, not exposed to the SDK users.
    /// </summary>
    internal class Event
    {
        /// <summary>
        /// The mandatory name of the event.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Optional additional data about the event.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; }

        /// <summary>
        /// Constructor for creating an Event instance.
        /// </summary>
        /// <param name="eventName">Name of the event (mandatory).</param>
        /// <param name="additionalData">Optional additional event data.</param>
        public Event(string eventName, Dictionary<string, object> additionalData = null)
        {
            EventName = eventName;
            AdditionalData = additionalData ?? new Dictionary<string, object>();
        }
    }
}
