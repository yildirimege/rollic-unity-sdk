using RollicSDK.Core.Interfaces;
using UnityEngine;
using RollicSDK.Core;

namespace RollicSDK
{
    /// <summary>
    /// Main public facade for the Rollic SDK. Provides static methods for tracking events and session info.
    /// </summary>
    public sealed class RollicSDK
    {
        private static RollicSDK _instance;
        private static readonly object _lock = new object();

        private readonly IEventTracker _eventTracker;
        private readonly ISessionManager _sessionManager;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private RollicSDK(IEventTracker eventTracker, ISessionManager sessionManager)
        {
            _eventTracker = eventTracker;
            _sessionManager = sessionManager;
        }

        /// <summary>
        /// Initializes the SDK. Called internally by RollicSDKLifeCycleManager.
        /// </summary>
        /// <param name="config">The configuration settings for the SDK.</param>
        /// <param name="eventProcessor">The processor responsible for sending events.</param>
        /// <param name="sessionManager">The manager for user sessions.</param>
        internal static void Initialize(RollicSDKConfig config, IEventTracker eventTracker, ISessionManager sessionManager)
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    if (config.EnableDebugLogging)
                        Debug.LogWarning("[RollicSDK] SDK is already initialized.");
                    return;
                }

                _instance = new RollicSDK(eventTracker, sessionManager);

                // Start the session
                _instance._sessionManager.StartSession();

                if (config.EnableDebugLogging)
                    Debug.Log("[RollicSDK] SDK Initialized successfully.");
            }
        }

        /// <summary>
        /// Tracks a custom event.
        /// The event is immediately saved to a persistent queue to prevent data loss.
        /// </summary>
        /// <param name="eventName">A string identifying the event, e.g., "level_start".</param>
        public static void TrackEvent(string eventName)
        {
            if (!IsInitialized()) return;

            if (string.IsNullOrWhiteSpace(eventName))
            {
                Debug.LogError("[RollicSDK] Event name cannot be null or empty.");
                return;
            }

            var sessionTime = _instance._sessionManager.GetSessionDuration();
            _instance._eventTracker.TrackEvent(eventName, sessionTime);
        }

        /// <summary>
        /// Gets the duration of the current user session in seconds.
        /// </summary>
        /// <returns>The total session time in seconds.</returns>
        public static double GetSessionDuration()
        {
            if (!IsInitialized()) return 0;
            return _instance._sessionManager.GetSessionDuration();
        }

        /// <summary>
        /// Gets the number of events currently stored in the persistent queue waiting to be sent.
        /// Useful for debugging.
        /// </summary>
        /// <returns>The number of events in the queue.</returns>
        public static int GetQueuedEventCount()
        {
            if (!IsInitialized()) return 0;
            return _instance._eventTracker.GetQueuedEventCount();
        }

        /// <summary>
        /// Checks if the SDK has been initialized.
        /// </summary>
        /// <returns>True if initialized, false otherwise.</returns>
        public static bool IsInitialized()
        {
            if (_instance == null)
            {
                Debug.LogError("[RollicSDK] SDK not initialized. Ensure a RollicSDKConfig asset exists in a 'Resources' folder.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Internal method to shut down the SDK.
        /// </summary>
        internal static void Shutdown()
        {
            lock (_lock)
            {
                if (_instance == null) return;

                _instance._sessionManager.EndSession();
                _instance = null;
                Debug.Log("[RollicSDK] SDK Shutdown.");
            }
        }
    }
}