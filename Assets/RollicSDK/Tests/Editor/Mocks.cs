using System.Collections.Generic;
using RollicSDK.Core.Interfaces;

// This file contains "mock" or "fake" implementations of our interfaces.
// This allows us to test classes in isolation without depending on real files or networks.

public class MockStorageStrategy : IStorageStrategy
{
    private readonly Dictionary<string, string> memory = new Dictionary<string, string>();
    public void Delete(string key) => memory.Remove(key);
    public bool Exists(string key) => memory.ContainsKey(key);
    public string Load(string key) => memory.TryGetValue(key, out var value) ? value : "";
    public void Save(string key, string data) => memory[key] = data;
    public void Clear() => memory.Clear();
}

public class MockDeviceInfoProvider : IDeviceInfoProvider
{
    public Dictionary<string, object> GetDeviceInfo() => new Dictionary<string, object>
    {
        { "platform", "TestPlatform" },
        { "os_version", "TestOS 1.0" },
        { "device_model", "TestModel" },
        { "device_id", "test-device-id" }
    };
}