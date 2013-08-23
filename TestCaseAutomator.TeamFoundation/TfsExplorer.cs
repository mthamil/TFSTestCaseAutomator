using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Enables searching and browsing Team Foundation Server objects.
	/// </summary>
	public class TfsExplorer : IDisposable
	{
		/// <summary>
		/// Initializes a new <see cref="TfsExplorer"/>.
		/// </summary>
		/// <param name="tfsServer">The TFS server to connect to</param>
		public TfsExplorer(Uri tfsServer)
			: this(tfsServer, TfsTeamProjectCollectionFactory.GetTeamProjectCollection)
		{
		}

		/// <summary>
		/// Initializes a new <see cref="TfsExplorer"/>.
		/// </summary>
		/// <param name="tfsServer">The TFS server to connect to</param>
		/// <param name="tfsConnectionFactory">Factory that creates <see cref="TfsConnection"/>s</param>
		internal TfsExplorer(Uri tfsServer, Func<Uri, TfsConnection> tfsConnectionFactory)
		{
			_tfsConnection = tfsConnectionFactory(tfsServer);
		}

		/// <summary>
		/// Provides access to the work items of a given project.
		/// </summary>
		/// <param name="projectName">The TFS project to access</param>
		/// <returns>An object providing access to a project's child objects</returns>
		public TfsProjectWorkItems WorkItems(string projectName)
		{
			var testService = _tfsConnection.GetService<ITestManagementService>();
			var project = testService.GetTeamProject(projectName);
			return new TfsProjectWorkItems(project);
		}

		/// <summary>
		/// Provides access to Visual Studio solutions in source control.
		/// </summary>
		public IEnumerable<TfsSolution> Solutions()
		{
			var sourceControl = _tfsConnection.GetService<VersionControlServer>();
			var solutions = sourceControl.GetItems("$/*.sln", RecursionType.Full);
			return solutions.Items.Select(item => new TfsSolution(item));
		}

		/// <summary>
		/// The Team Foundation Server's URI.
		/// </summary>
		public Uri TfsServer { get { return _tfsConnection.Uri; } }

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			_tfsConnection.Dispose();
		}

		private readonly TfsConnection _tfsConnection;
	}
}