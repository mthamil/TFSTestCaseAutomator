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
                             IServerManagement servers,
			                 ITestCases testCases) : this()
		{
			_explorer = explorer;
		    Servers = servers;
            Servers.PropertyChanged += Servers_PropertyChanged;
		    TestCases = testCases;
		}

        private MainViewModel()
	    {
            _isConnected = Property.New(this, p => p.IsConnected)
                                   .AlsoChanges(p => p.CanRefresh);

            _isConnecting = Property.New(this, p => p.IsConnecting)
                                    .AlsoChanges(p => p.CanConnect);

	        _projectName = Property.New(this, p => p.ProjectName)
	                               .AlsoChanges(p => p.CanRefresh);

            _status = Property.New(this, p => p.Status);

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
        /// Manages servers.
        /// </summary>
        public IServerManagement Servers { get; }

        /// <summary>
        /// Manages test cases.
        /// </summary>
        public ITestCases TestCases { get; }

        /// <summary>
        /// Whether connecting would refresh an existing connection or not.
        /// </summary>
        public bool CanRefresh => IsConnected && UriEqualityComparer.Instance.Equals(Servers.CurrentUri, _explorer.Server.Uri);

        /// <summary>
        /// Whether a connection can currently be attempted.
        /// </summary>
	    public bool CanConnect => !IsConnecting && !String.IsNullOrWhiteSpace(Servers.CurrentUri?.OriginalString);

        /// <summary>
        /// Whether a connection is currently being attempted.
        /// </summary>
	    public bool IsConnecting
	    {
	        get { return _isConnecting.Value; }
            set { _isConnecting.Value = value; }
	    }

        /// <summary>
        /// Command that forces a server refresh.
        /// </summary>
        public ICommand ConnectCommand { get; }

		/// <summary>
		/// Connects to the TFS server specified by <see cref="IServerManagement.CurrentUri"/>.
		/// </summary>
		public async Task ConnectAsync()
		{
		    await HandleServerError(async () =>
			{
                IsConnecting = true;
                try
			    {
                    var serverUrl = Servers.CurrentUri;
                    bool serverChanged = !UriEqualityComparer.Instance.Equals(serverUrl, _explorer.Server?.Uri);

                    if (_explorer.Server != null)
                        _explorer.Server.ConnectionStatusChanged -= Server_ConnectionStatusChanged;

                    await _explorer.ConnectAsync(serverUrl);
                    _explorer.Server.ConnectionStatusChanged += Server_ConnectionStatusChanged;

                    var currentProjectName = ProjectName;
                    ProjectNames.Clear();
                    ProjectNames.AddRange((await _explorer.GetTeamProjectsAsync()).Select(n => n.Name));

                    // Restore project name on reconnect.
                    if (ProjectNames.Contains(currentProjectName) && !serverChanged)
                        ProjectName = currentProjectName;

                    if (serverChanged)
                        ProjectName = null;

                    IsConnected = true;

                    Servers.Add(serverUrl);
                    OnConnectionSucceeded(serverUrl);

                    await Task.CompletedTask;
                }
			    finally
                {
                    IsConnecting = false;
                }
			});
		}

        /// <see cref="IApplication.ConnectionSucceeded"/>
        public event EventHandler<ConnectionSucceededEventArgs> ConnectionSucceeded;

	    private void OnConnectionSucceeded(Uri server)
	    {
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
			switch (propertyChangedEventArgs.PropertyName)
			{
                case nameof(ProjectName):
                    if (IsConnected)
				    {
					    await HandleServerError(async () => 
                            await LoadWorkItemsAsync());
				    }
			        break;
			}
		}

        private async Task LoadWorkItemsAsync()
        {
            if (String.IsNullOrWhiteSpace(ProjectName))
            {
                TestCases.WorkItems.TestCases.Clear();
                return;
            }

            await TestCases.WorkItems.LoadAsync(ProjectName);
        }

        private void Servers_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Propagate child changes.
            switch (e.PropertyName)
            {
                case nameof(Servers.CurrentUri):
                    OnPropertyChanged(nameof(CanRefresh));
                    OnPropertyChanged(nameof(CanConnect));
                    break;
            }
        }

        private void Server_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            IsConnected = !e.ConnectionFailed;
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
                errorMessage = tfsUnavailableEx.Message;
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

	    
		private readonly Property<string> _projectName;
		private readonly Property<string> _status;
	    private readonly Property<bool> _isConnected;
	    private readonly Property<bool> _isConnecting;

        private readonly ITfsExplorer _explorer;
	}
}