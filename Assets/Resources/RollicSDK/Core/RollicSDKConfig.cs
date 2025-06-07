// File: RollicSDK/Core/RollicSDKConfig.cs
using RollicSDK.Core.Enums;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// ScriptableObject for configuring the Rollic SDK.
    /// To create an instance, right-click in the Project window and select 'Create/Rollic SDK/Configuration'.
    /// </summary>
    [CreateAssetMenu(fileName = "RollicSDKConfig", menuName = "Rollic SDK/Configuration", order = 1)]
    public sealed class RollicSDKConfig : ScriptableObject
    {
        [Header("API Settings")]
        [Tooltip("The API endpoint URL for sending events.")]
        public string ApiEndpoint = "https://exampleapi.rollic.gs/event";

        [Header("Event Processing")]
        [Tooltip("The maximum number of events to send in a single batch.")]
        public int MaxBatchSize = 20;

        [Tooltip("The interval in seconds to check the queue and send events.")]
        public float SendInterval = 15f;

        [Header("Retry Logic")]
        [Tooltip("The initial delay in seconds before the first retry attempt.")]
        public float InitialRetryDelay = 2f;

        [Tooltip("The maximum delay in seconds for exponential backoff.")]
        public float MaxRetryDelay = 60f;

        [Header("Storage")]
        [Tooltip("The storage strategy to use for persisting events. 'FileSystem' is recommended for mobile.")]
        public StorageType StorageType = StorageType.FileSystem;

        [Header("Debugging")]
        [Tooltip("Enable detailed SDK logs in the console.")]
        public bool EnableDebugLogging = false;

        [Tooltip("If checked, the SDK will simulate being offline, forcing events to queue up even with an internet connection.")]
        public bool MockOfflineMode = false;
    }
}