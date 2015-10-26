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
        public async Task<IEnumerable<ITestCase>> GetTestCasesAsync(string projectName, IProgress<ITestCase> testCaseSink)
		{
            ServerGuard();

            var testProject = Server.TestManagement.GetTeamProject(projectName);
            return (await Server.WorkItemStore
                                .QueryAsync(@"SELECT * 
                                              FROM WorkItems 
                                              WHERE [System.TeamProject] = @project
                                              AND [System.WorkItemType] = 'Test Case' 
                                              ORDER BY [System.Id]",
                                              new Dictionary<string, string> { { "project", projectName }})
                                .ConfigureAwait(false))
                                .Select(wi => testProject.CreateFromWorkItem(wi))
                                .OfType<ITestCase>()
                                .Tee(tc => testCaseSink?.Report(tc));
		}

	    /// <summary>
	    /// Retrieves the Team Projects for the given connection.
	    /// </summary>
	    public Task<IEnumerable<ICatalogNode>> GetTeamProjectsAsync()
	    {
            ServerGuard();

            return Server.CatalogRoot
                         .QueryChildrenAsync(CatalogResourceTypes.TeamProject.ToEnumerable(),
	                                         false,
	                                         CatalogQueryOptions.None);
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
	    /// Retrieves the root of the source control tree.
	    /// </summary>
	    public TfsDirectory GetSourceTreeRoot()
	    {
            ServerGuard();

            return new TfsSourceTreeRoot(Server.VersionControl);
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