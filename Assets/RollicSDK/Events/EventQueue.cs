using RollicSDK.Data;
using RollicSDK.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RollicSDK.Core
{
    /// <summary>
    /// Manages a persistent, thread-safe queue of events.
    /// </summary>
    public class EventQueue
    {
        private const string StorageKey = "rollic_sdk_event_queue";
        private readonly IStorageStrategy _storage;
        private readonly Queue<EventData> _eventQueue;
        private readonly object _queueLock = new object();
        private readonly bool _debugLogging;

        public int Count
        {
            get
            {
                lock (_queueLock)
                {
                    return _eventQueue.Count;
                }
            }
        }

        public EventQueue(IStorageStrategy storageStrategy, bool enableDebugLogging = false)
        {
            _storage = storageStrategy;
            _debugLogging = enableDebugLogging;
            _eventQueue = LoadQueueFromStorage();
            if (_debugLogging) Debug.Log($"[EventQueue] Loaded {_eventQueue.Count} events from previous session.");
        }

        public void Enqueue(EventData eventData)
        {
            lock (_queueLock)
            {
                _eventQueue.Enqueue(eventData);
                SaveQueueToStorage();
            }
        }

        /// <summary>
        /// Dequeues a batch of events up to the specified batch size.
        /// </summary>
        public List<EventData> DequeueBatch(int maxBatchSize)
        {
            var batch = new List<EventData>();
            lock (_queueLock)
            {
                int count = Math.Min(maxBatchSize, _eventQueue.Count);
                for (int i = 0; i < count; i++)
                {
                    batch.Add(_eventQueue.Dequeue());
                }

                if (batch.Count > 0)
                {
                    SaveQueueToStorage(); // Update storage after removing items
                }
            }
            return batch;
        }

        /// <summary>
        /// Adds a batch of events back to the front of the queue.
        /// Used when a network request fails and events need to be retried.
        /// </summary>
        public void RequeueBatch(List<EventData> batch)
        {
            if (batch == null || batch.Count == 0) return;

            lock (_queueLock)
            {
                var currentItems = new List<EventData>(_eventQueue);
                _eventQueue.Clear();

                // Add the failed batch to the front
                foreach (var item in batch) _eventQueue.Enqueue(item);
                // Add the rest of the items back
                foreach (var item in currentItems) _eventQueue.Enqueue(item);

                SaveQueueToStorage();
                if (_debugLogging) Debug.LogWarning($"[EventQueue] Re-queued {batch.Count} events after network failure.");
            }
        }

        // --- Private Save/Load Methods ---
        private void SaveQueueToStorage()
        {
            var serializableQueue = new SerializableEventQueue(_eventQueue);
            string json = JsonUtility.ToJson(serializableQueue);
            _storage.Save(StorageKey, json);
        }

        private Queue<EventData> LoadQueueFromStorage()
        {
            if (!_storage.Exists(StorageKey)) return new Queue<EventData>();

            string json = _storage.Load(StorageKey);
            if (string.IsNullOrEmpty(json)) return new Queue<EventData>();

            try
            {
                var wrapper = JsonUtility.FromJson<SerializableEventQueue>(json);
                return new Queue<EventData>(wrapper.Events ?? new List<EventData>());
            }
            catch (Exception e)
            {
                Debug.LogError($"[EventQueue] Failed to load and parse event queue. The queue will be cleared to prevent further errors. Reason: {e.Message}");
                _storage.Delete(StorageKey);
                return new Queue<EventData>();
            }
        }

        [Serializable]
        private class SerializableEventQueue
        {
            public List<EventData> Events;
            public SerializableEventQueue(Queue<EventData> queue) => Events = new List<EventData>(queue);
        }
    }
}