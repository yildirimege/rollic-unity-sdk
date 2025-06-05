using RollicSDK.Core.Interfaces;
using System;

namespace RollicSDK.Core
{
    /// <summary>
    /// Concrete implementation of ISessionManager.
    /// Tracks elapsed time between StartSession and EndSession.
    /// </summary>
    public sealed class SessionManager : ISessionManager
    {
        private DateTime? _sessionStartTime;
        private TimeSpan _accumulatedSessionTime = TimeSpan.Zero;
        private bool _isSessionActive = false;

        public void StartSession()
        {
            if (_isSessionActive)
            {
                UnityEngine.Debug.LogWarning("Session already started.");
                return;
            }

            _sessionStartTime = DateTime.UtcNow;
            _isSessionActive = true;
        }

        public void EndSession()
        {
            if (!_isSessionActive)
            {
                UnityEngine.Debug.LogWarning("No active session to end.");
                return;
            }

            if (_sessionStartTime.HasValue)
            {
                var sessionEndTime = DateTime.UtcNow;
                _accumulatedSessionTime += sessionEndTime - _sessionStartTime.Value;
                _sessionStartTime = null;
            }
            _isSessionActive = false;
        }

        public double GetSessionDuration()
        {
            if (_isSessionActive && _sessionStartTime.HasValue)
            {
                var now = DateTime.UtcNow;
                return (_accumulatedSessionTime + (now - _sessionStartTime.Value)).TotalSeconds;
            }
            return _accumulatedSessionTime.TotalSeconds;
        }
    }
}
