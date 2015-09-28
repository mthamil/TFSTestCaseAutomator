using System;
using System.Windows.Input;

namespace TestCaseAutomator.ViewModels
{
    /// <summary>
    /// Represents a server that can be connected to.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// A server's URI.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Command that removes a server URI from the server list.
        /// </summary>
        ICommand Forget { get; }
    }
}