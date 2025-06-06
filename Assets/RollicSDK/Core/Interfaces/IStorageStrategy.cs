namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface for defining a data storage strategy.
    /// Implementations can include PlayerPrefs, file system, or encrypted storage.
    /// </summary>
    public interface IStorageStrategy
    {
        /// <summary>
        /// Saves data associated with a key.
        /// </summary>
        /// <param name="key">The unique identifier for the data.</param>
        /// <param name="data">The data to store.</param>
        void Save(string key, string data);

        /// <summary>
        /// Loads data associated with a key.
        /// </summary>
        /// <param name="key">The unique identifier for the data.</param>
        /// <returns>The stored data, or an empty string if not found.</returns>
        string Load(string key);

        /// <summary>
        /// Deletes data associated with a key.
        /// </summary>
        /// <param name="key">The unique identifier to delete.</param>
        void Delete(string key);

        /// <summary>
        /// Checks if data exists for the given key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool Exists(string key);
    }
}
