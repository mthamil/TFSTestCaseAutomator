using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using SharpEssentials;
using SharpEssentials.Collections;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Enables searching and browsing Team Foundation Server objects.
	/// </summary>
	public class TfsExplorer : DisposableBase, ITfsExplorer
	{
        /// <summary>
        /// Initializes a new <see cref="TfsExplorer"/>.
        /// </summary>
        /// <param name="serverFactory">Enables access to a TFS server</param>
        /// <param name="scheduler">Used to schedule background tasks</param>
        public TfsExplorer(Func<Uri, ITfsServer> serverFactory,
                           TaskScheduler scheduler)
		{
		    _serverFactory = serverFactory;
            _scheduler = scheduler;
		}

	    /// <summary>
	    /// Connects to a TFS server..
	    /// </summary>
	    public async Task ConnectAsync(Uri serverUri)
        {
            Server?.Dispose();

	        var server = await Task.Factory.StartNew(() => 
                _serverFactory(serverUri), 
                    CancellationToken.None, TaskCreationOptions.None, _scheduler).ConfigureAwait(false);

	        Server = server;
        }

        /// <summary>
        /// The Team Foundation Server.
        /// </summary>
        public ITfsServer Server { get; private set; }

        /// <summary>
        /// Retrieves the test cases of a given project.
        /// </summary>
        /// <param name="projectName">The TFS project to access</param>
        /// <param name="testCaseSink">An optional progress sink handler for each test case</param>
        /// <returns>The test cases in the given project</returns>
        public Task<IEnumerable<ITestCase>> GetTestCasesAsync(string projectName, IProgress<ITestCase> testCaseSink)
		{
            ServerGuard();

            return Task.Factory.StartNew(() => 
                        Server.TestManagement
                              .GetTeamProject(projectName)
                              .TestCases.Query(@"SELECT * 
                                                 FROM WorkItems 
                                                 WHERE [System.TeamProject] = @project
                                                   AND [System.WorkItemType] = 'Test Case' 
                                                 ORDER BY [System.Id]")
                              .Tee(tc => testCaseSink?.Report(tc)),
                    CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

	    /// <summary>
	    /// Retrieves the Team Projects for the given connection.
	    /// </summary>
	    public async Task<IEnumerable<ICatalogNode>> GetTeamProjectsAsync()
	    {
            ServerGuard();

            return (await Server.CatalogRoot
                                .QueryChildrenAsync(CatalogResourceTypes.TeamProject.ToEnumerable(),
	                                                false,
	                                                CatalogQueryOptions.None)
                                .ConfigureAwait(false));
	    }

	    /// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		public async Task<IEnumerable<TfsSolution>> SolutionsAsync()
	    {
	        ServerGuard();

	        return (await Server.VersionControl
                                .GetItemsAsync("$/*.sln", RecursionType.Full)
                                .ConfigureAwait(false))
	                            .Select(item => new TfsSolution(item, Server.VersionControl));
	    }

	    /// <summary>
	    /// Retrieves the source control tree.
	    /// </summary>
	    public Task<IEnumerable<TfsSourceControlledItem>> GetSourceTreeAsync()
	    {
            ServerGuard();

            return new TfsSourceTreeRoot(Server.VersionControl).GetItemsAsync();
	    }

        private void ServerGuard()
        {
            if (Server == null)
                throw new InvalidOperationException("Not connected to a server.");
        }

        /// <see cref="DisposableBase.OnDisposing"/>
        protected override void OnDisposing() => Server?.Dispose();

	    private readonly Func<Uri, ITfsServer> _serverFactory;
	    private readonly TaskScheduler _scheduler;
	}
}