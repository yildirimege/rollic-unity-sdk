using RollicSDK.Core.Interfaces;
using System.IO;
using UnityEngine;

/// <summary>
/// Storage strategy that saves data to the local file system using JSON files.
/// Suitable for larger or structured persistent data.
/// </summary>
public class FileSystemStorageStrategy : IStorageStrategy
{
    private readonly string basePath;

    /// <summary>
    /// Initializes a new instance of FileSystemStorageStrategy.
    /// Creates a storage directory under persistentDataPath.
    /// </summary>
    /// <param name="directoryName">The name of the subdirectory to store files in.</param>
    public FileSystemStorageStrategy(string directoryName = "RollicSDK")
    {
        basePath = Path.Combine(Application.persistentDataPath, directoryName);
        Directory.CreateDirectory(basePath);
    }

    /// <summary>
    /// Saves data to a JSON file in the specified directory.
    /// </summary>
    /// <param name="key">The key used as the filename (without extension).</param>
    /// <param name="data">The JSON string to save.</param>
    public void Save(string key, string data)
    {
        string fullPath = Path.Combine(basePath, key + ".json");
        File.WriteAllText(fullPath, data);
    }

    /// <summary>
    /// Loads data from a JSON file.
    /// </summary>
    /// <param name="key">The key used as the filename (without extension).</param>
    /// <returns>The file contents if it exists, otherwise an empty string.</returns>
    public string Load(string key)
    {
        string fullPath = Path.Combine(basePath, key + ".json");
        return File.Exists(fullPath) ? File.ReadAllText(fullPath) : "";
    }

    /// <summary>
    /// Deletes the JSON file associated with the given key.
    /// </summary>
    /// <param name="key">The key used as the filename (without extension).</param>
    public void Delete(string key)
    {
        string fullPath = Path.Combine(basePath, key + ".json");
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    /// <summary>
    /// Checks if a JSON file exists for the given key.
    /// </summary>
    /// <param name="key">The key used as the filename (without extension).</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public bool Exists(string key)
    {
        return File.Exists(Path.Combine(basePath, key + ".json"));
    }
}
