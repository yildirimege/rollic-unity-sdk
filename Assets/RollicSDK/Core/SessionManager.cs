using RollicSDK.Core.Interfaces;
using System;
using UnityEngine;

namespace RollicSDK.Core
{
    public sealed class SessionManager : ISessionManager
    {
        private const string StorageKey = "rollicsdk_session_data";

        private DateTime? _sessionStartTime;
        private TimeSpan _accumulatedSessionTime = TimeSpan.Zero;
        private bool _isSessionActive = false;
        private bool _isPaused = false;

        public event Action OnSessionStarted;
        public event Action OnSessionEnded;
        public event Action OnSessionPaused;
        public event Action OnSessionResumed;

        private readonly IStorageStrategy storage;

        public SessionManager(IStorageStrategy storageStrategy)
        {
            storage = storageStrategy;
            LoadSession();
        }

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

        public void ResumeSession()
        {
            if (!_isSessionActive || !_isPaused) return;

            _sessionStartTime = DateTime.UtcNow;
            _isPaused = false;
            SaveSession();
            OnSessionResumed?.Invoke();
        }

        public double GetSessionDuration()
        {
            if (_isSessionActive && _sessionStartTime.HasValue && !_isPaused)
            {
                return (_accumulatedSessionTime + (DateTime.UtcNow - _sessionStartTime.Value)).TotalSeconds;
            }

            return _accumulatedSessionTime.TotalSeconds;
        }

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

        [Serializable]
        private class SessionData
        {
            public long SessionStartTicks;
            public long AccumulatedTicks;
            public bool IsActive;
            public bool IsPaused;
        }
    }
}
