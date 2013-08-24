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
		/// Provides access to the work items of a given project.
		/// </summary>
		/// <param name="projectName">The TFS project to access</param>
		/// <returns>An object providing access to a project's child objects</returns>
		ITfsProjectWorkItemCollection WorkItems(string projectName);

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		IEnumerable<TfsSolution> Solutions();

		/// <summary>
		/// The Team Foundation Server's URI.
		/// </summary>
		Uri TfsServer { get; }
	}
}