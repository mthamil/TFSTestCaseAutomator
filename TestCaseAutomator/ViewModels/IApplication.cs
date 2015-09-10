using System;
using System.ComponentModel;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// The main application interface.
	/// </summary>
	public interface IApplication : INotifyPropertyChanged
	{
		/// <summary>
		/// The current TFS server URL.
		/// </summary>
		Uri ServerUri { get; }

		/// <summary>
		/// The name of the current TFS project.
		/// </summary>
		string ProjectName { get; }

        /// <summary>
        /// Event raised when a connection has been successfully established.
        /// </summary>
        event EventHandler<ConnectionSucceededEventArgs> ConnectionSucceeded;

		/// <summary>
		/// Event raised when the application is closing.
		/// This occurs before any final cleanup/save operations.
		/// </summary>
		event EventHandler<EventArgs> Closing;
	}
}