using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Collections;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Net;
using SharpEssentials.Observable;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// The main application window view model.
	/// </summary>
	public class MainViewModel : ViewModelBase, IApplication
	{
		public MainViewModel(ITfsExplorer explorer, 
			                 IWorkItems workItems,
			                 TestSelectionViewModel testSelection) : this()
		{
			_explorer = explorer;
		    WorkItems = workItems;
            TestSelection = testSelection;
		}

	    private MainViewModel()
	    {
            _isConnected = Property.New(this, p => p.IsConnected, OnPropertyChanged)
                                   .AlsoChanges(p => p.CanRefresh);
            _serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged)
	                             .AlsoChanges(p => p.CanRefresh)
                                 .AlsoChanges(p => p.CanConnect);
	        _projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged)
	                               .AlsoChanges(p => p.CanRefresh);
            _status = Property.New(this, p => p.Status, OnPropertyChanged);

	        ConnectCommand = Command.For(this)
	                                .DependsOn(p => p.CanConnect)
	                                .Asynchronously()
	                                .Executes(ConnectAsync);
            CloseCommand = new RelayCommand(OnClosing);

            PropertyChanged += OnPropertyChanged;
	    }

        /// <summary>
        /// Whether the connection is known to be up.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected.Value; }
            set { _isConnected.Value = value; }
        }

	    /// <see cref="IApplication.ServerUri"/>
		public Uri ServerUri
		{
			get { return _serverUri.Value; }
			set { _serverUri.Value = value; }
		}

	    /// <summary>
	    /// Available server URIs.
	    /// </summary>
	    public ICollection<Uri> ServerUris => _serverUris;

	    /// <see cref="IApplication.ProjectName"/>
	    public string ProjectName
		{
			get { return _projectName.Value; }
			set { _projectName.Value = value; }
		}

	    /// <summary>
		/// Available project names.
		/// </summary>
		public ICollection<string> ProjectNames { get; } = new ObservableCollection<string>();

	    /// <summary>
		/// Contains work items from the current server and project.
		/// </summary>
		public IWorkItems WorkItems { get; }

        public TestSelectionViewModel TestSelection { get; }

        /// <summary>
        /// Whether connecting would refresh an existing connection or not.
        /// </summary>
        public bool CanRefresh => IsConnected && UriEqualityComparer.Instance.Equals(ServerUri, _explorer.Server.Uri);

	    public bool CanConnect => !String.IsNullOrWhiteSpace(ServerUri?.OriginalString);

        /// <summary>
        /// Command that forces a server refresh.
        /// </summary>
        public ICommand ConnectCommand { get; }

		/// <summary>
		/// Connects to the TFS server specified by <see cref="ServerUri"/>.
		/// </summary>
		public async Task ConnectAsync()
		{
			await HandleServerError(async () =>
			{
			    var serverUrl = ServerUri;
                bool serverChanged = !UriEqualityComparer.Instance.Equals(serverUrl, _explorer.Server?.Uri);

                if (_explorer.Server != null)
                    _explorer.Server.ConnectionStatusChanged -= Server_ConnectionStatusChanged;

                _explorer.Connect(serverUrl);
                _explorer.Server.ConnectionStatusChanged += Server_ConnectionStatusChanged;

                var currentProjectName = ProjectName;
                ProjectNames.Clear();
                ProjectNames.AddRange(_explorer.TeamProjects().Select(n => n.Name));

                // Restore project name on reconnect.
                if (ProjectNames.Contains(currentProjectName) && !serverChanged)
                    ProjectName = currentProjectName;

                if (serverChanged)
                    ProjectName = null;

                IsConnected = true;

                OnConnectionSucceeded(serverUrl);

                await Task.CompletedTask;
			});
		}

        /// <see cref="IApplication.ConnectionSucceeded"/>
        public event EventHandler<ConnectionSucceededEventArgs> ConnectionSucceeded;

	    private void OnConnectionSucceeded(Uri server)
	    {
            if (!_serverUris.Contains(server))
                _serverUris.Insert(0, server);

	        ConnectionSucceeded?.Invoke(this, new ConnectionSucceededEventArgs(server));
	    }

        /// <summary>
        /// Command invoked when the application is closing.
        /// </summary>
        public ICommand CloseCommand { get; }

		/// <see cref="IApplication.Closing"/>
		public event EventHandler<EventArgs> Closing;

		private void OnClosing()
		{
            Closing?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// The current application status.
		/// </summary>
		public string Status
		{
			get { return _status.Value; }
			private set { _status.Value = value; }
		}

	    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName == nameof(ProjectName))
			{
				if (IsConnected)
				{
					await HandleServerError(async () => 
                        await LoadWorkItemsAsync());
				}
			}
		}

        private void Server_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            IsConnected = !e.ConnectionFailed;
        }

        private async Task LoadWorkItemsAsync()
        {
            if (String.IsNullOrWhiteSpace(ProjectName))
            {
                WorkItems.TestCases.Clear();
                return;
            }

            await WorkItems.LoadAsync(_explorer.WorkItems(ProjectName));
	    }

		private async Task HandleServerError(Func<Task> action)
		{
		    string errorMessage = null;
		    try
		    {
		        await action();
		        Status = null;
		    }
		    catch (TestObjectNotFoundException testObjectNotFoundEx)
		    {
		        errorMessage = testObjectNotFoundEx.Message;
		    }
            catch (TeamFoundationServiceUnavailableException tfsUnavailableEx)
            {
                errorMessage = tfsUnavailableEx.Message; ;
            }
            catch (Exception e) when (e.InnerException != null &&
		                              e.InnerException.GetType() == typeof(TeamFoundationServiceUnavailableException))
		    {
		        errorMessage = e.InnerException.Message;
		    }
		    finally
		    {
                if (errorMessage != null)
                {
                    Status = errorMessage;
                    IsConnected = false;
                    ProjectNames.Clear();
                }
            }
		}

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<string> _status;
	    private readonly Property<bool> _isConnected;

        private readonly ObservableCollection<Uri> _serverUris = new ObservableCollection<Uri>();

        private readonly ITfsExplorer _explorer;
	}
}