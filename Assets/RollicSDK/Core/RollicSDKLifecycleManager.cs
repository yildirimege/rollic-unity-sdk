using RollicSDK.Core.Interfaces;
using RollicSDK.Core.Storage;
using RollicSDK.Platform;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// Internal MonoBehaviour that automatically manages the SDK's lifecycle.
    /// It ensures the SDK is initialized on game start and handles application pause/resume events.
    /// </summary>
    internal sealed class RollicSDKManager : MonoBehaviour
    {
        private static RollicSDKManager _instance;

        private RollicSDKConfig _config;
        private SessionManager _sessionManager;
        private EventProcessor _eventProcessor;

        /// <summary>
        /// This method is called by Unity when the game loads, before any scene is loaded.
        /// It creates a persistent GameObject to host the SDK manager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnApplicationStart()
        {
            if (_instance != null) return;

            var config = Resources.Load<RollicSDKConfig>("RollicSDKConfig");
            if (config == null)
            {
                Debug.LogError("[RollicSDK] Configuration not found! Please create a 'RollicSDKConfig' asset in a 'Resources' folder.");
                return;
            }

            GameObject sdkManagerObject = new GameObject("RollicSDKManager");
            _instance = sdkManagerObject.AddComponent<RollicSDKManager>();
            DontDestroyOnLoad(sdkManagerObject);

            _instance.Initialize(config);
        }

        /// <summary>
        /// Initializes all SDK components.
        /// </summary>
        private void Initialize(RollicSDKConfig config)
        {
            _config = config;

            IDeviceInfoProvider deviceInfoProvider = CreateDeviceInfoProvider();

            var storageStrategy = StorageStrategyFactory.Create(_config.StorageType);

            var eventQueue = new EventQueue(storageStrategy);
            var networkManager = new NetworkManager(_config);

            _sessionManager = new SessionManager(storageStrategy);
            var eventTracker = new EventTracker(eventQueue, deviceInfoProvider, _config);


            // Create new gameobject to run coroutines inside
            _eventProcessor = new GameObject("RollicSDKEventProcessor").AddComponent<EventProcessor>();
            _eventProcessor.transform.SetParent(this.transform);
            _eventProcessor.Initialize(eventQueue, networkManager, _config);

            // Auto initialize the main SDK
            RollicSDK.Initialize(_config, eventTracker, _sessionManager);
        }

        /// <summary>
        /// Handles the application being paused (i.e. user backgrounds the app).
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (_sessionManager == null) return;

            if (pauseStatus)
            {
                _sessionManager.PauseSession();
                if (_config.EnableDebugLogging) Debug.Log("[RollicSDK] Application paused. Session timer paused.");
            }
            else
            {
                _sessionManager.ResumeSession();
                if (_config.EnableDebugLogging) Debug.Log("[RollicSDK] Application resumed. Session timer resumed.");
            }
        }

        /// <summary>
        /// Handles the application quitting.
        /// </summary>
        private void OnApplicationQuit()
        {
            RollicSDK.Shutdown();
        }

        /// <summary>
        /// Factory method to create the appropriate device info provider based on the compilation platform.
        /// </summary>
        private IDeviceInfoProvider CreateDeviceInfoProvider()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidDeviceInfoProvider();
#elif UNITY_IOS && !UNITY_EDITOR
            return new iOSDeviceInfoProvider();
#else
            // Fallback for the Editor or other platforms
            return new DefaultDeviceInfoProvider();
#endif
        }
    }
}