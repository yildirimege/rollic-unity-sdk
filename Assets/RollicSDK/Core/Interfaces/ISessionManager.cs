namespace RollicSDK.Core.Interfaces
{
    /// <summary>
    /// Interface for managing user session tracking.
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Starts the session tracking.
        /// </summary>
        void StartSession();

        /// <summary>
        /// Ends the session tracking.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Returns the elapsed session time in seconds.
        /// </summary>
        /// <returns>Session duration in seconds.</returns>
        double GetSessionDuration();
    }
}
