namespace RollicSDK.Core.Enums
{
    /// <summary>
    /// Type of the storage, used when choosing a storage strategy type.
    /// Implementations can include PlayerPrefs, file system, or encrypted storage.
    /// </summary>
    public enum StorageType
    {
        PlayerPrefs,
        FileSystem
        // Can be extented with Encrypted, Firebase, Cloud, Hybrid with fallback etc. in the future.
    }
}
