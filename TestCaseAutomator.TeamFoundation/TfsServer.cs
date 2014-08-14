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
            _connection.ConnectivityFailureStatusChanged += connection_ConnectivityFailureStatusChanged;

			_testManagement = new Lazy<ITestManagementService>(() => _connection.GetService<ITestManagementService>());
			_versionControl = new Lazy<IVersionControl>(() => new VersionControlServerAdapter(_connection.GetService<VersionControlServer>()));
			_projectCollectionService = new Lazy<ITeamProjectCollectionService>(() => _connection.ConfigurationServer.GetService<ITeamProjectCollectionService>());
			_catalogRoot = new Lazy<ICatalogNode>(() => new CatalogNodeWrapper(_connection.CatalogNode));
		}

		/// <see cref="ITfsServer.TestManagement"/>
		public ITestManagementService TestManagement { get { return _testManagement.Value; } }

		/// <see cref="ITfsServer.VersionControl"/>
		public IVersionControl VersionControl { get { return _versionControl.Value; } }

		/// <see cref="ITfsServer.ProjectCollectionService"/>
		public ITeamProjectCollectionService ProjectCollectionService { get { return _projectCollectionService.Value; } }

		/// <see cref="ITfsServer.CatalogRoot"/>
		public ICatalogNode CatalogRoot { get { return _catalogRoot.Value; } }

        /// <see cref="ITfsServer.ConnectionStatusChanged"/>
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

	    private void OnConnectionStatusChanged(bool status)
	    {
	        var localEvent = ConnectionStatusChanged;
	        if (localEvent != null)
	            localEvent(this, new ConnectionStatusChangedEventArgs(status));
	    }

        void connection_ConnectivityFailureStatusChanged(object sender, ConnectivityFailureStatusChangedEventArgs e)
        {
            OnConnectionStatusChanged(e.NewConnectivityFailureStatus);
        }

	    /// <see cref="ITfsServer.Uri"/>
		public Uri Uri
		{
			get { return _connection.Uri; }
		}

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