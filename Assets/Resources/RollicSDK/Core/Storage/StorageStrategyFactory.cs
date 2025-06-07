using RollicSDK.Core.Enums;
using RollicSDK.Core.Interfaces;
using UnityEngine;

namespace RollicSDK.Core.Storage
{
    /// <summary>
    /// Factory class to create appropriate IStorageStrategy instances.
    /// </summary>
    public static class StorageStrategyFactory
    {
        /// <summary>
        /// Creates a storage strategy based on the provided StorageType.
        /// </summary>
        /// <param name="type">The desired storage type (PlayerPrefs or FileSystem).</param>
        /// <returns>An instance of IStorageStrategy.</returns>
        public static IStorageStrategy Create(StorageType type)
        {
            switch (type)
            {
                case StorageType.PlayerPrefs:
                    return new PlayerPrefsStorageStrategy();

                case StorageType.FileSystem:
                    return new FileSystemStorageStrategy();

                default:
                    Debug.LogWarning($"Storage type {type} is not supported. Falling back to PlayerPrefs.");
                    return new PlayerPrefsStorageStrategy();
            }
        }

        /// <summary>
        /// Creates a storage strategy based on platform defaults or internal logic.
        /// </summary>
        /// <returns>An instance of IStorageStrategy using internal logic (platform, config, etc.).</returns>
        public static IStorageStrategy CreateFromDefaults()
        {
#if UNITY_ANDROID || UNITY_IOS // This cases are randomly passed pased on platform. Can be vise versa, or hybrid in both.
            return new FileSystemStorageStrategy();
#else
            // Use PlayerPrefs on other platforms
            return new PlayerPrefsStorageStrategy();
#endif
        }
    }
}

