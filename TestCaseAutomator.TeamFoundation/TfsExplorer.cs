using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TestCaseAutomator.Utilities;
using TestCaseAutomator.Utilities.Collections;

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
			_server = server;
			_workItemsFactory = workItemsFactory;
		}

		/// <summary>
		/// Provides access to the work items of a given project.
		/// </summary>
		/// <param name="projectName">The TFS project to access</param>
		/// <returns>An object providing access to a project's child objects</returns>
		public ITfsProjectWorkItemCollection WorkItems(string projectName)
		{
			var project = _server.TestManagement.GetTeamProject(projectName);
			return _workItemsFactory(project);
		}

		/// <summary>
		/// Retrieves the Team Projects for the given connection.
		/// </summary>
		public IEnumerable<ICatalogNode> TeamProjects()
		{
			return _server.CatalogRoot.QueryChildren(CatalogResourceTypes.TeamProject.ToEnumerable(),
													 false,
													 CatalogQueryOptions.None);
		}

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		public IEnumerable<TfsSolution> Solutions()
		{
			var solutions = _server.VersionControl.GetItems("$/*.sln", RecursionType.Full);
			return solutions.Select(item => new TfsSolution(item, _server.VersionControl));
		}

		/// <summary>
		/// The Team Foundation Server's URI.
		/// </summary>
		public Uri TfsServer { get { return _server.Uri; } }

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
			_server.Dispose();
		}

		private readonly Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> _workItemsFactory;
		private readonly ITfsServer _server;
	}
}