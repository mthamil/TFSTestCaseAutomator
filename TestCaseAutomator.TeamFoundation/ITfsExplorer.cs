using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Interface for searching and browsing Team Foundation Server objects.
	/// </summary>
	public interface ITfsExplorer
	{
	    /// <summary>
	    /// Connects to a TFS server.
	    /// </summary>
	    Task ConnectAsync(Uri serverUri);

        /// <summary>
        /// The Team Foundation Server.
        /// </summary>
        ITfsServer Server { get; }

        /// <summary>
        /// Retrieves the test cases of a given project.
        /// </summary>
        /// <param name="projectName">The TFS project to access</param>
        /// <param name="testCaseSink">An optional progress sink handler for each test case</param>
        /// <returns>The test cases in the given project</returns>
        Task<IEnumerable<ITestCase>> GetTestCasesAsync(string projectName, IProgress<ITestCase> testCaseSink = null);

		/// <summary>
		/// Retrieves the Team Projects for the given connection.
		/// </summary>
		Task<IEnumerable<ICatalogNode>> GetTeamProjectsAsync();

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		Task<IEnumerable<TfsSolution>> SolutionsAsync();

        /// <summary>
        /// Retrieves the root of the source control tree.
        /// </summary>
        TfsDirectory GetSourceTreeRoot();
	}
}