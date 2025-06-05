using RollicSDK.Core.Interfaces;

namespace RollicSDK.Core
{
    /// <summary>
    /// Main entry point facade for the Rollic SDK.
    /// </summary>
    public sealed class RollicSDK
    {
        private readonly IEventTracker _eventTracker;
        private readonly ISessionManager _sessionManager;
        private readonly RollicSDKConfig _config;

        private static RollicSDK _instance;

        private RollicSDK(RollicSDKConfig config, IEventTracker eventTracker, ISessionManager sessionManager)
        {
            _config = config;
            _eventTracker = eventTracker;
            _sessionManager = sessionManager;
        }

        /// <summary>
        /// Initializes the SDK singleton instance.
        /// </summary>
        /// <param name="config">Configuration object.</param>
        public static void Initialize(RollicSDKConfig config)
        {
            if (_instance != null)
            {
                if (config.EnableDebugLogging)
                    UnityEngine.Debug.LogWarning("RollicSDK is already initialized.");
                return;
            }

            _instance = new RollicSDK(config, new EventTracker(), new SessionManager());
            _instance._sessionManager.StartSession();

            if (config.EnableDebugLogging)
                UnityEngine.Debug.Log("RollicSDK initialized.");
        }

        /// <summary>
        /// Tracks an event through the event tracker.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public static void TrackEvent(string eventName)
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RollicSDK not initialized. Call Initialize() before tracking events.");
                return;
            }

            _instance._eventTracker.TrackEvent(eventName);
        }

        /// <summary>
        /// Ends the current session.
        /// </summary>
        public static void EndSession()
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RollicSDK not initialized.");
                return;
            }

            _instance._sessionManager.EndSession();
        }

        /// <summary>
        /// Gets current session duration in seconds.
        /// </summary>
        /// <returns>Session time in seconds.</returns>
        public static double GetSessionDuration()
        {
            if (_instance == null)
            {
                UnityEngine.Debug.LogError("RollicSDK not initialized.");
                return 0;
            }

            return _instance._sessionManager.GetSessionDuration();
        }
    }
}
