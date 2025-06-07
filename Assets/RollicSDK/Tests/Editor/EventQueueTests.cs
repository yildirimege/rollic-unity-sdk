using NUnit.Framework;
using RollicSDK.Core;
using RollicSDK.Data;
using System.Linq;

/// <summary>
/// Contains unit tests for the <see cref="EventQueue"/> class.
/// This suite verifies the core logic of queuing, batching, re-queuing, and persistence
/// to ensure no event data is ever lost.
/// </summary>
[TestFixture]
public class EventQueueTests
{
    private MockStorageStrategy mockStorage;
    private MockDeviceInfoProvider mockProvider;
    private EventQueue eventQueue;

    /// <summary>
    /// This method runs before each test, ensuring a clean and isolated
    /// test environment with fresh instances of the queue and its dependencies.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        mockStorage = new MockStorageStrategy();
        mockProvider = new MockDeviceInfoProvider();
        eventQueue = new EventQueue(mockStorage, enableDebugLogging: false);
    }

    /// <summary>
    /// Verifies that enqueuing a single event increases the total count of the queue by one.
    /// </summary>
    [Test]
    public void Enqueue_WhenCalled_IncreasesQueueCount()
    {
        var testEvent = EventData.Create("test_event", 10.0, mockProvider);

        eventQueue.Enqueue(testEvent);

        Assert.AreEqual(1, eventQueue.Count);
    }

    /// <summary>
    /// Verifies that dequeuing a batch returns the correct number of items and
    /// correctly reduces the remaining count in the queue when there are enough items available.
    /// </summary>
    [Test]
    public void DequeueBatch_WhenQueueHasEnoughItems_ReturnsCorrectBatchAndUpdatesCount()
    {
        for (int i = 0; i < 5; i++)
            eventQueue.Enqueue(EventData.Create($"event_{i}", i, mockProvider));

        var batch = eventQueue.DequeueBatch(3);

        Assert.AreEqual(3, batch.Count, "Dequeued batch should contain 3 events.");
        Assert.AreEqual(2, eventQueue.Count, "Remaining queue count should be 2.");
    }

    /// <summary>
    /// Verifies that attempting to dequeue a batch larger than the number of items
    /// in the queue returns all available items and leaves the queue empty.
    /// </summary>
    [Test]
    public void DequeueBatch_WhenQueueHasFewerItems_ReturnsAllItems()
    {
        for (int i = 0; i < 2; i++)
            eventQueue.Enqueue(EventData.Create($"event_{i}", i, mockProvider));

        var batch = eventQueue.DequeueBatch(5);

        Assert.AreEqual(2, batch.Count, "Dequeued batch should contain all 2 events.");
        Assert.AreEqual(0, eventQueue.Count, "Queue should be empty after dequeuing all items.");
    }

    /// <summary>
    /// Verifies that a batch of events that failed to send can be added back to the
    /// front of the queue, preserving the original order of events.
    /// </summary>
    [Test]
    public void RequeueBatch_AfterFailure_AddsItemsToFrontOfQueue()
    {
        eventQueue.Enqueue(EventData.Create("first_event", 1.0, mockProvider));
        eventQueue.Enqueue(EventData.Create("second_event", 2.0, mockProvider));

        var failedBatch = eventQueue.DequeueBatch(1);
        eventQueue.RequeueBatch(failedBatch);

        var finalBatch = eventQueue.DequeueBatch(2);
        Assert.AreEqual(2, finalBatch.Count, "Final batch count is incorrect.");
        Assert.AreEqual("first_event", finalBatch[0].EventName, "First event was not correctly re-queued to the front.");
        Assert.AreEqual("second_event", finalBatch[1].EventName, "Second event is not in the correct order.");
    }

    /// <summary>
    /// Verifies that a new <see cref="EventQueue"/> instance correctly loads and
    /// deserializes pre-existing event data from its storage strategy upon creation.
    /// </summary>
    [Test]
    public void Constructor_WhenStorageHasData_LoadsExistingQueue()
    {
        var initialQueue = new EventQueue(mockStorage, false);
        initialQueue.Enqueue(EventData.Create("saved_event", 1.0, mockProvider));

        var newQueue = new EventQueue(mockStorage, false);

        Assert.AreEqual(1, newQueue.Count, "New queue did not load the saved event.");
        Assert.AreEqual("saved_event", newQueue.DequeueBatch(1).First().EventName, "The loaded event has incorrect data.");
    }
}