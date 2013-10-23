using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TestCaseAutomator.Utilities;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Wraps a TFS connection to provide simplified access to various services.
	/// </summary>
	public class TfsServer : DisposableBase, ITfsServer
	{
		/// <summary>
		/// Initializes a new <see cref="TfsServer"/>.
		/// </summary>
		/// <param name="connection">The TFS connection</param>
		public TfsServer(TfsTeamProjectCollection connection)
		{
			_connection = connection;
			TestManagement = _connection.GetService<ITestManagementService>();
			VersionControl = new VersionControlServerAdapter(_connection.GetService<VersionControlServer>());
			ProjectCollectionService = _connection.ConfigurationServer.GetService<ITeamProjectCollectionService>();
			CatalogRoot = new CatalogNodeWrapper(_connection.CatalogNode);
		}

		/// <see cref="ITfsServer.TestManagement"/>
		public ITestManagementService TestManagement { get; private set; }

		/// <see cref="ITfsServer.VersionControl"/>
		public IVersionControl VersionControl { get; private set; }

		/// <see cref="ITfsServer.ProjectCollectionService"/>
		public ITeamProjectCollectionService ProjectCollectionService { get; private set; }

		/// <see cref="ITfsServer.CatalogRoot"/>
		public ICatalogNode CatalogRoot { get; private set; }

		/// <see cref="ITfsServer.Uri"/>
		public Uri Uri
		{
			get { return _connection.Uri; }
		}

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
			VersionControl.Dispose();
			_connection.Dispose();
		}

		private readonly TfsTeamProjectCollection _connection;
	}
}