using System;
using System.Collections.Generic;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Interface for searching and browsing Team Foundation Server objects.
	/// </summary>
	public interface ITfsExplorer
	{
	    /// <summary>
	    /// Connects to a TFS server..
	    /// </summary>
	    void Connect(Uri serverUri);

        /// <summary>
        /// The Team Foundation Server.
        /// </summary>
        ITfsServer Server { get; }

        /// <summary>
        /// Provides access to the work items of a given project.
        /// </summary>
        /// <param name="projectName">The TFS project to access</param>
        /// <returns>An object providing access to a project's child objects</returns>
        ITfsProjectWorkItemCollection WorkItems(string projectName);

		/// <summary>
		/// Retrieves the Team Projects for the given connection.
		/// </summary>
		IEnumerable<ICatalogNode> TeamProjects();

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		IEnumerable<TfsSolution> Solutions();
	}
}