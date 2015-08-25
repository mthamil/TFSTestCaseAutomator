using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using SharpEssentials;

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
            _connection.EnsureAuthenticated();
            _connection.ConnectivityFailureStatusChanged += connection_ConnectivityFailureStatusChanged;

			_testManagement = new Lazy<ITestManagementService>(() => _connection.GetService<ITestManagementService>());
			_versionControl = new Lazy<IVersionControl>(() => new VersionControlServerAdapter(_connection.GetService<VersionControlServer>()));
			_projectCollectionService = new Lazy<ITeamProjectCollectionService>(() => _connection.ConfigurationServer.GetService<ITeamProjectCollectionService>());
			_catalogRoot = new Lazy<ICatalogNode>(() => new CatalogNodeAdapter(_connection.CatalogNode));
		}

		/// <see cref="ITfsServer.TestManagement"/>
		public ITestManagementService TestManagement => _testManagement.Value;

	    /// <see cref="ITfsServer.VersionControl"/>
		public IVersionControl VersionControl => _versionControl.Value;

	    /// <see cref="ITfsServer.ProjectCollectionService"/>
		public ITeamProjectCollectionService ProjectCollectionService => _projectCollectionService.Value;

	    /// <see cref="ITfsServer.CatalogRoot"/>
		public ICatalogNode CatalogRoot => _catalogRoot.Value;

	    /// <see cref="ITfsServer.ConnectionStatusChanged"/>
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

	    private void OnConnectionStatusChanged(bool status)
	    {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(status));
	    }

        void connection_ConnectivityFailureStatusChanged(object sender, ConnectivityFailureStatusChangedEventArgs e)
        {
            OnConnectionStatusChanged(e.NewConnectivityFailureStatus);
        }

	    /// <see cref="ITfsServer.Uri"/>
		public Uri Uri => _connection.Uri;

	    /// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
            _versionControl.Dispose();
		    _connection.ConnectivityFailureStatusChanged -= connection_ConnectivityFailureStatusChanged;
			_connection.Dispose();
		}

		private readonly TfsTeamProjectCollection _connection;

	    private readonly Lazy<ITestManagementService> _testManagement;
	    private readonly Lazy<IVersionControl> _versionControl;
	    private readonly Lazy<ITeamProjectCollectionService> _projectCollectionService;
	    private readonly Lazy<ICatalogNode> _catalogRoot;
	}
}