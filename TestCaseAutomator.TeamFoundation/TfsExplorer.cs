using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TestCaseAutomator.Utilities;

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
		/// <param name="tfsConnection">Enables access to a TFS server</param>
		/// <param name="workItemsFactory">Factory that creates <see cref="ITfsProjectWorkItemCollection"/></param>
		/// <param name="versionControlFactory">Factory that creates <see cref="IVersionControl"/>s</param>
		public TfsExplorer(
			TfsConnection tfsConnection,
			Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> workItemsFactory,
			Func<TfsConnection, IVersionControl> versionControlFactory)
		{
			_tfsConnection = tfsConnection;
			_workItemsFactory = workItemsFactory;
			_versionControlFactory = versionControlFactory;
		}

		/// <summary>
		/// Provides access to the work items of a given project.
		/// </summary>
		/// <param name="projectName">The TFS project to access</param>
		/// <returns>An object providing access to a project's child objects</returns>
		public ITfsProjectWorkItemCollection WorkItems(string projectName)
		{
			var testService = _tfsConnection.GetService<ITestManagementService>();
			var project = testService.GetTeamProject(projectName);
			return _workItemsFactory(project);
		}

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		public IEnumerable<TfsSolution> Solutions()
		{
			var sourceControl = _versionControlFactory(_tfsConnection);
			var solutions = sourceControl.GetItems("$/*.sln", RecursionType.Full);
			return solutions.Select(item => new TfsSolution(item, sourceControl));
		}

		/// <summary>
		/// The Team Foundation Server's URI.
		/// </summary>
		public Uri TfsServer { get { return _tfsConnection.Uri; } }

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
			_tfsConnection.Dispose();
		}

		private readonly Func<ITestManagementTeamProject, ITfsProjectWorkItemCollection> _workItemsFactory;
		private readonly Func<TfsConnection, IVersionControl> _versionControlFactory;
		private readonly TfsConnection _tfsConnection;
	}
}