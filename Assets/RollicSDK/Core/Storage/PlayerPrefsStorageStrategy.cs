using RollicSDK.Core.Interfaces;
using UnityEngine;

/// <summary>
/// Storage strategy using Unity's PlayerPrefs for simple key-value data persistence.
/// Suitable for small and lightweight data.
/// </summary>
public class PlayerPrefsStorageStrategy : IStorageStrategy
{
    /// <summary>
    /// Saves data to PlayerPrefs using the specified key.
    /// </summary>
    /// <param name="key">The key to associate with the data.</param>
    /// <param name="data">The data to store.</param>
    public void Save(string key, string data) => PlayerPrefs.SetString(key, data);

    /// <summary>
    /// Loads data from PlayerPrefs using the specified key.
    /// </summary>
    /// <param name="key">The key associated with the data.</param>
    /// <returns>The stored data, or an empty string if not found.</returns>
    public string Load(string key) => PlayerPrefs.GetString(key, "");

    /// <summary>
    /// Deletes the entry in PlayerPrefs associated with the specified key.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    public void Delete(string key) => PlayerPrefs.DeleteKey(key);

    /// <summary>
    /// Checks if a key exists in PlayerPrefs.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    public bool Exists(string key) => PlayerPrefs.HasKey(key);
}
