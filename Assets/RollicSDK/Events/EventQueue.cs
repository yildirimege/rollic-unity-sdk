using System.Collections.Generic;
using RollicSDK.Core.Interfaces;
using UnityEngine;
using System;

/// <summary>
/// Manages a persistent queue of events, storing and retrieving them using the configured storage strategy.
/// </summary>
public class EventQueue
{
    private const string StorageKey = "rollicsdk_event_queue";

    private readonly IStorageStrategy storage;
    private readonly Queue<EventData> eventQueue;

    public EventQueue(IStorageStrategy storageStrategy)
    {
        storage = storageStrategy;
        eventQueue = LoadQueueFromStorage();
    }

    /// <summary>
    /// Adds a new event to the queue and saves it to storage.
    /// </summary>
    public void Enqueue(EventData eventData)
    {
        eventQueue.Enqueue(eventData);
        SaveQueueToStorage();
    }

    /// <summary>
    /// Removes and returns the next event in the queue.
    /// </summary>
    public EventData Dequeue()
    {
        if (eventQueue.Count == 0) return null;

        var ev = eventQueue.Dequeue();
        SaveQueueToStorage();
        return ev;
    }

    /// <summary>
    /// Peeks at the next event without removing it.
    /// </summary>
    public EventData Peek()
    {
        return eventQueue.Count > 0 ? eventQueue.Peek() : null;
    }

    /// <summary>
    /// Returns the number of events currently in the queue.
    /// </summary>
    public int Count => eventQueue.Count;

    /// <summary>
    /// Clears the entire queue and storage.
    /// </summary>
    public void Clear()
    {
        eventQueue.Clear();
        storage.Delete(StorageKey);
    }

    private void SaveQueueToStorage()
    {
        string json = JsonUtility.ToJson(new SerializableEventQueue(eventQueue));
        storage.Save(StorageKey, json);
    }

    private Queue<EventData> LoadQueueFromStorage()
    {
        if (!storage.Exists(StorageKey)) return new Queue<EventData>();

        string json = storage.Load(StorageKey);
        try
        {
            var wrapper = JsonUtility.FromJson<SerializableEventQueue>(json);
            return new Queue<EventData>(wrapper.Events);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[EventQueue] Failed to load queue: {e.Message}");
            return new Queue<EventData>();
        }
    }

    [Serializable]
    private class SerializableEventQueue
    {
        public List<EventData> Events = new();

        public SerializableEventQueue(Queue<EventData> queue)
        {
            Events = new List<EventData>(queue);
        }
    }
}

