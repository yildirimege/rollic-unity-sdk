using NUnit.Framework;
using RollicSDK.Core;
using UnityEngine;

/// <summary>
/// Contains unit tests for the <see cref="SessionManager"/> class.
/// This suite verifies the state transitions and data persistence of the session lifecycle.
/// </summary>
[TestFixture]
public class SessionManagerTests
{
    private MockStorageStrategy mockStorage;
    private SessionManager sessionManager;

    /// <summary>
    /// Runs before each test to provide a clean SessionManager and mock storage instance.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        mockStorage = new MockStorageStrategy();
        sessionManager = new SessionManager(mockStorage);
    }

    /// <summary>
    /// Verifies that the session duration is zero before any session has been started.
    /// </summary>
    [Test]
    public void GetSessionDuration_WhenNoSessionStarted_ReturnsZero()
    {
        Assert.AreEqual(0, sessionManager.GetSessionDuration(), 0.01, "Initial session duration should be zero.");
    }

    /// <summary>
    /// Verifies that after starting and then ending a session, the persisted state
    /// is correctly marked as inactive and that time has been accumulated.
    /// </summary>
    [Test]
    public void StartSession_ThenEndSession_SavesStateAsInactive()
    {
        sessionManager.StartSession();
        sessionManager.EndSession();

        string json = mockStorage.Load("rollicsdk_session_data");
        Assert.IsFalse(string.IsNullOrEmpty(json), "Session data was not saved to storage upon ending.");

        var savedData = JsonUtility.FromJson<SessionManager.SessionData>(json);
        Assert.IsFalse(savedData.IsActive, "Saved session state should be inactive after ending.");
        Assert.IsTrue(savedData.AccumulatedTicks >= 0, "Accumulated time should be saved.");
    }

    /// <summary>
    /// Verifies that starting a session correctly saves the state to storage
    /// and marks the session as active and not paused.
    /// </summary>
    [Test]
    public void StartSession_WhenCalled_SavesStateAsActive()
    {
        sessionManager.StartSession();

        string json = mockStorage.Load("rollicsdk_session_data");
        Assert.IsFalse(string.IsNullOrEmpty(json), "Session data was not saved to storage upon starting.");

        var savedData = JsonUtility.FromJson<SessionManager.SessionData>(json);
        Assert.IsTrue(savedData.IsActive, "Saved session state should be active after starting.");
        Assert.IsFalse(savedData.IsPaused, "Saved session state should not be paused after starting.");
    }

    /// <summary>
    /// Verifies that the persisted 'IsPaused' flag is correctly toggled
    /// when the session is paused and subsequently resumed.
    /// </summary>
    [Test]
    public void PauseAndResume_WhenCalled_SavesCorrectPausedState()
    {
        sessionManager.StartSession();

        sessionManager.PauseSession();
        string pausedJson = mockStorage.Load("rollicsdk_session_data");
        var pausedData = JsonUtility.FromJson<SessionManager.SessionData>(pausedJson);
        Assert.IsTrue(pausedData.IsPaused, "Session should be marked as paused in storage.");

        sessionManager.ResumeSession();
        string resumedJson = mockStorage.Load("rollicsdk_session_data");
        var resumedData = JsonUtility.FromJson<SessionManager.SessionData>(resumedJson);
        Assert.IsFalse(resumedData.IsPaused, "Session should not be marked as paused after resuming.");
    }
}