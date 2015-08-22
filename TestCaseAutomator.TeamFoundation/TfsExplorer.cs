using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <param name="server">Enables access to a TFS server</param>
		/// <param name="workItemsFactory">Factory that creates <see cref="ITfsProjectWorkItemCollection"/></param>
		public TfsExplorer(ITfsServer server,
		                   Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> workItemsFactory)
		{
			Server = server;
			_workItemsFactory = workItemsFactory;
		}

		/// <summary>
		/// Provides access to the work items of a given project.
		/// </summary>
		/// <param name="projectName">The TFS project to access</param>
		/// <returns>An object providing access to a project's child objects</returns>
		public ITfsProjectWorkItemCollection WorkItems(string projectName)
		{
			var project = Server.TestManagement.GetTeamProject(projectName);
			return _workItemsFactory(project);
		}

	    /// <summary>
	    /// Retrieves the Team Projects for the given connection.
	    /// </summary>
	    public IEnumerable<ICatalogNode> TeamProjects()
	        => Server.CatalogRoot.QueryChildren(CatalogResourceTypes.TeamProject.ToEnumerable(),
	                                             false,
	                                             CatalogQueryOptions.None);

	    /// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		public IEnumerable<TfsSolution> Solutions() 
            => Server.VersionControl.GetItems("$/*.sln", RecursionType.Full)
                                     .Select(item => new TfsSolution(item, Server.VersionControl));

	    /// <summary>
		/// The Team Foundation Server.
		/// </summary>
		public ITfsServer Server { get; }

	    /// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing() => Server.Dispose();

	    private readonly Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> _workItemsFactory;
	}
}