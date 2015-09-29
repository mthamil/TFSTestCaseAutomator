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
        /// <param name="workItemsFactory">Factory that creates <see cref="ITfsProjectWorkItemCollection"/></param>
        /// <param name="scheduler">Used to schedule background tasks</param>
        public TfsExplorer(Func<Uri, ITfsServer> serverFactory,
		                   Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> workItemsFactory,
                           TaskScheduler scheduler)
		{
		    _serverFactory = serverFactory;
		    _workItemsFactory = workItemsFactory;
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
        /// Provides access to the work items of a given project.
        /// </summary>
        /// <param name="projectName">The TFS project to access</param>
        /// <returns>An object providing access to a project's child objects</returns>
        public ITfsProjectWorkItemCollection WorkItems(string projectName)
		{
            ServerGuard();

            var project = Server.TestManagement.GetTeamProject(projectName);
			return _workItemsFactory(project);
		}

	    /// <summary>
	    /// Retrieves the Team Projects for the given connection.
	    /// </summary>
	    public IEnumerable<ICatalogNode> TeamProjects()
	    {
            ServerGuard();

            return Server.CatalogRoot.QueryChildren(CatalogResourceTypes.TeamProject.ToEnumerable(),
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
                                .GetItemsAsync("$/*.sln", RecursionType.Full).ConfigureAwait(false))
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
	    private readonly Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> _workItemsFactory;
	    private readonly TaskScheduler _scheduler;
	}
}