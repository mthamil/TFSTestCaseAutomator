using System;

namespace TestCaseAutomator.TeamFoundation
{
    /// <summary>
    /// Provides information about when a connection's status changes.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        
        /// <summary>
        /// Initializes a new instance of <see cref="ConnectionStatusChangedEventArgs"/>.
        /// </summary>
        /// <param name="connectionFailed">Whether the connection has failed</param>
        public ConnectionStatusChangedEventArgs(bool connectionFailed)
        {
            ConnectionFailed = connectionFailed;
        }

        /// <summary>
        /// Whether the connection has failed.
        /// </summary>
        public bool ConnectionFailed { get; private set; }
    }
}