using System;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a TFS server and the services it provides.
	/// </summary>
	public interface ITfsServer : IDisposable
	{
		/// <summary>
		/// Provides test management services.
		/// </summary>
		ITestManagementService TestManagement { get; }

		/// <summary>
		/// Provides access to source control.
		/// </summary>
		IVersionControl VersionControl { get; }

		/// <summary>
		/// Enables queries against a TFS connection's catalog.
		/// </summary>
		ICatalogNode CatalogRoot { get; }

        /// <summary>
        /// Event raised when a connection's status changes.
        /// </summary>
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

		/// <summary>
		/// The Team Foundation Server's URI.
		/// </summary>
		Uri Uri { get; }
	}
}