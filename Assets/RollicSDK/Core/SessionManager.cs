using RollicSDK.Core.Interfaces;
using System;
using UnityEngine;

namespace RollicSDK.Core
{
    /// <summary>
    /// Manages user session timing with support for start, end, pause, and resume.
    /// Persists session state using the provided storage strategy to survive app restarts.
    /// Provides events for session lifecycle changes.
    /// </summary>
    public sealed class SessionManager : ISessionManager
    {
        private const string StorageKey = "rollicsdk_session_data";

        private DateTime? _sessionStartTime;
        private TimeSpan _accumulatedSessionTime = TimeSpan.Zero;
        private bool _isSessionActive = false;
        private bool _isPaused = false;

        /// <summary>
        /// Event triggered when a session starts.
        /// </summary>
        public event Action OnSessionStarted;

        /// <summary>
        /// Event triggered when a session ends.
        /// </summary>
        public event Action OnSessionEnded;

        /// <summary>
        /// Event triggered when a session is paused.
        /// </summary>
        public event Action OnSessionPaused;

        /// <summary>
        /// Event triggered when a session is resumed.
        /// </summary>
        public event Action OnSessionResumed;

        private readonly IStorageStrategy storage;

        /// <summary>
        /// Creates a new instance of SessionManager.
        /// Attempts to restore previous session state from storage.
        /// </summary>
        /// <param name="storageStrategy">Storage strategy for persisting session data.</param>
        public SessionManager(IStorageStrategy storageStrategy)
        {
            storage = storageStrategy;
            LoadSession();
        }

        /// <summary>
        /// Starts a new session.
        /// If a session is already active, logs a warning and does nothing.
        /// Fires OnSessionStarted event.
        /// </summary>
        public void StartSession()
        {
            if (_isSessionActive)
            {
                Debug.LogWarning("Session already started.");
                return;
            }

            _sessionStartTime = DateTime.UtcNow;
            _accumulatedSessionTime = TimeSpan.Zero;
            _isSessionActive = true;
            _isPaused = false;

            SaveSession();
            OnSessionStarted?.Invoke();
        }

        /// <summary>
        /// Ends the current active session.
        /// If no session is active, logs a warning and does nothing.
        /// Fires OnSessionEnded event.
        /// </summary>
        public void EndSession()
        {
            if (!_isSessionActive)
            {
                Debug.LogWarning("No active session to end.");
                return;
            }

            if (_sessionStartTime.HasValue && !_isPaused)
            {
                var sessionEndTime = DateTime.UtcNow;
                _accumulatedSessionTime += sessionEndTime - _sessionStartTime.Value;
            }

            _sessionStartTime = null;
            _isSessionActive = false;
            _isPaused = false;

            SaveSession();
            OnSessionEnded?.Invoke();
        }

        /// <summary>
        /// Pauses the current session timing.
        /// If session is not active or already paused, does nothing.
        /// Fires OnSessionPaused event.
        /// </summary>
        public void PauseSession()
        {
            if (!_isSessionActive || _isPaused) return;

            if (_sessionStartTime.HasValue)
            {
                _accumulatedSessionTime += DateTime.UtcNow - _sessionStartTime.Value;
                _sessionStartTime = null;
            }

            _isPaused = true;
            SaveSession();
            OnSessionPaused?.Invoke();
        }

        /// <summary>
        /// Resumes the session if it was previously paused.
        /// If session is not active or not paused, does nothing.
        /// Fires OnSessionResumed event.
        /// </summary>
        public void ResumeSession()
        {
            if (!_isSessionActive || !_isPaused) return;

            _sessionStartTime = DateTime.UtcNow;
            _isPaused = false;
            SaveSession();
            OnSessionResumed?.Invoke();
        }

        /// <summary>
        /// Gets the total duration of the current session in seconds.
        /// Includes accumulated time plus active time since last start if session is active and not paused.
        /// </summary>
        /// <returns>Total session duration in seconds.</returns>
        public double GetSessionDuration()
        {
            if (_isSessionActive && _sessionStartTime.HasValue && !_isPaused)
            {
                return (_accumulatedSessionTime + (DateTime.UtcNow - _sessionStartTime.Value)).TotalSeconds;
            }

            return _accumulatedSessionTime.TotalSeconds;
        }

        /// <summary>
        /// Saves the current session state to persistent storage.
        /// </summary>
        private void SaveSession()
        {
            var data = new SessionData()
            {
                SessionStartTicks = _sessionStartTime?.Ticks ?? 0,
                AccumulatedTicks = _accumulatedSessionTime.Ticks,
                IsActive = _isSessionActive,
                IsPaused = _isPaused
            };

            string json = JsonUtility.ToJson(data);
            storage.Save(StorageKey, json);
        }

        /// <summary>
        /// Loads the session state from persistent storage, if available.
        /// Resets state if loading fails.
        /// </summary>
        private void LoadSession()
        {
            if (!storage.Exists(StorageKey)) return;

            try
            {
                string json = storage.Load(StorageKey);
                var data = JsonUtility.FromJson<SessionData>(json);

                _sessionStartTime = data.SessionStartTicks > 0 ? new DateTime(data.SessionStartTicks) : (DateTime?)null;
                _accumulatedSessionTime = new TimeSpan(data.AccumulatedTicks);
                _isSessionActive = data.IsActive;
                _isPaused = data.IsPaused;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load session: {e.Message}");
                _sessionStartTime = null;
                _accumulatedSessionTime = TimeSpan.Zero;
                _isSessionActive = false;
                _isPaused = false;
            }
        }

        /// <summary>
        /// Internal data class used for serializing session state to JSON.
        /// </summary>
        [Serializable]
        private class SessionData
        {
            /// <summary>Ticks for session start time (DateTime.Ticks).</summary>
            public long SessionStartTicks;

            /// <summary>Ticks for accumulated session time.</summary>
            public long AccumulatedTicks;

            /// <summary>Whether a session is active.</summary>
            public bool IsActive;

            /// <summary>Whether the session is paused.</summary>
            public bool IsPaused;
        }
    }
}
