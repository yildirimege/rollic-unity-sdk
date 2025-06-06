using System;

/// <summary>
/// Represents a single trackable event with a name and optional session time.
/// </summary>
[Serializable]
public class EventData
{
    public string EventName;
    public long SessionTime; // Unix timestamp

    public EventData(string eventName, long sessionTime)
    {
        EventName = eventName;
        SessionTime = sessionTime;
    }
}
