using System;

namespace TestCaseAutomator.ViewModels
{
    public class ConnectionSucceededEventArgs : EventArgs
    {
        public ConnectionSucceededEventArgs(Uri server)
        {
            Server = server;
        }

        public Uri Server { get; }
    }
}