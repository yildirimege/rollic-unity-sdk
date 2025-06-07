using RollicSDK.Data;
using RollicSDK.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;
namespace RollicSDK.Core
{
    /// <summary>
    /// A MonoBehaviour responsible for processing the event queue in the background.
    /// It periodically batches events and sends them to the API, with retry logic.
    /// </summary>
    public class EventProcessor : MonoBehaviour
    {
        private EventQueue _eventQueue;
        private INetworkManager _networkManager;
        private RollicSDKConfig _config;

        private Coroutine _processingCoroutine;
        private float _currentWaitTime;
        private bool _isSending = false;

        /// <summary>
        /// Initializes the processor with its dependencies.
        /// </summary>
        public void Initialize(EventQueue queue, INetworkManager networkManager, RollicSDKConfig config)
        {
            _eventQueue = queue;
            _networkManager = networkManager;
            _config = config;
            _currentWaitTime = _config.SendInterval;

            _processingCoroutine = StartCoroutine(ProcessQueue());
        }

        private void OnDestroy()
        {
            if (_processingCoroutine != null)
            {
                StopCoroutine(_processingCoroutine);
            }
        }

        /// <summary>
        /// The main processing loop that runs as a coroutine.
        /// It waits, checks conditions, and then attempts to send a batch.
        /// </summary>
        private IEnumerator ProcessQueue()
        {
            // Wait a moment on startup before the first send attempt.
            yield return new WaitForSeconds(_config.InitialRetryDelay);

            while (true)
            {
                // Avoid sending an empty or already sending batch of events
                if (!_isSending && _eventQueue.Count > 0)
                {
                    // Check for internet connection or mock offline mode
                    if (Application.internetReachability == NetworkReachability.NotReachable || _config.MockOfflineMode)
                    {

                        if (_config.EnableDebugLogging)
                        {
                            string reason = _config.MockOfflineMode ? "(Mock Offline Mode is Active)" : "(No Internet Connection)";
                            Debug.LogWarning($"[EventProcessor] Offline mode active {reason}. Waiting...");
                        }
                        // If there's no connection, we use exponential wait times to avoid spamming.
                        _currentWaitTime = Mathf.Min(_currentWaitTime * 2, _config.MaxRetryDelay);
                    }
                    else
                    {   // If we are fit to send (both event data and internet connection are present)
                        _isSending = true;

                        var eventBatch = _eventQueue.DequeueBatch(_config.MaxBatchSize);

                        // Internal safeguard check. Should always be true
                        if (eventBatch.Count > 0)
                        {
                            var payload = new { events = eventBatch };
                            string jsonPayload = JsonConvert.SerializeObject(payload);

                            // 1. Start the async Task without awaiting it.
                            var postTask = _networkManager.PostAsync(_config.ApiEndpoint, jsonPayload);

                            // 2. Yield control back to Unity, telling the coroutine to resume only when the task is complete.
                            //    This is the non-blocking way to wait for a Task in a coroutine.
                            yield return new WaitUntil(() => postTask.IsCompleted);

                            // 3. Once the task is complete, we can safely get its result.
                            bool success = postTask.Result;

                            if (success)
                            {
                                if (_config.EnableDebugLogging) Debug.Log($"[EventProcessor] Successfully sent a batch of {eventBatch.Count} events.");
                                // On success, reset wait time to the normal configured interval.
                                _currentWaitTime = _config.SendInterval;
                            }
                            else
                            {
                                if (_config.EnableDebugLogging) Debug.LogError($"[EventProcessor] Failed to send event batch. Re-queuing {eventBatch.Count} events.");
                                // IMPORTANT: Re-queue the batch if sending failed to prevent data loss.
                                _eventQueue.RequeueBatch(eventBatch);

                                // On failure, apply exponential backoff for the next attempt's wait time.
                                _currentWaitTime = Mathf.Min(_currentWaitTime * 2, _config.MaxRetryDelay);
                                if (_config.EnableDebugLogging) Debug.LogWarning($"[EventProcessor] Next retry attempt will be in {_currentWaitTime} seconds.");
                            }
                        }
                        _isSending = false;
                    }
                }
                else
                {
                    // If queue is empty, just wait for the normal interval.
                    _currentWaitTime = _config.SendInterval;
                }

                yield return new WaitForSeconds(_currentWaitTime);
            }
        }
    }
}