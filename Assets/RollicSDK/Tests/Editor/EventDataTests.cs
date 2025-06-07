using NUnit.Framework;
using RollicSDK.Data;
using System;

/// <summary>
/// Contains unit tests for the <see cref="EventData"/> class.
/// This suite focuses on verifying the correct creation and population of event data objects.
/// </summary>
[TestFixture]
public class EventDataTests
{
    /// <summary>
    /// Verifies that the <see cref="EventData.Create"/> factory method correctly populates all
    /// fields, including event name, session time, a unique ID, device information, and a recent timestamp.
    /// </summary>
    [Test]
    public void Create_WhenCalled_PopulatesAllFieldsCorrectly()
    {
        var mockProvider = new MockDeviceInfoProvider();
        string eventName = "test_creation";
        double sessionTime = 123.45;

        var eventData = EventData.Create(eventName, sessionTime, mockProvider);

        Assert.AreEqual(eventName, eventData.EventName, "EventName was not set correctly.");
        Assert.AreEqual(123, eventData.SessionTime, "SessionTime should be correctly cast to a long.");
        Assert.IsNotNull(eventData.EventId, "EventId should be generated and not empty.");
        Assert.AreEqual("TestPlatform", eventData.Platform, "Platform info is incorrect.");
        Assert.AreEqual("TestModel", eventData.DeviceModel, "DeviceModel info is incorrect.");

        long currentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        Assert.LessOrEqual(Math.Abs(currentTimestamp - eventData.TimestampUtc), 5, "Timestamp should be very recent.");
    }
}