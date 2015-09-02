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
	                             .AlsoChanges(p => p.CanRefresh);
            _projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
            _status = Property.New(this, p => p.Status, OnPropertyChanged);

            ConnectCommand = new AsyncRelayCommand(ConnectAsync);
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

		/// <see cref="IApplication.ProjectName"/>
		public string ProjectName
		{
			get { return _projectName.Value; }
			set { _projectName.Value = value; }
		}

		/// <summary>
		/// Available project names.
		/// </summary>
		public ICollection<string> ProjectNames => _projectNames;

	    /// <summary>
		/// Contains work items from the current server and project.
		/// </summary>
		public IWorkItems WorkItems { get; }

        public TestSelectionViewModel TestSelection { get; }

        /// <summary>
        /// Whether connecting would refresh an existing connection or not.
        /// </summary>
        public bool CanRefresh => IsConnected && UriEqualityComparer.Instance.Equals(ServerUri, _explorer.Server.Uri);

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
                ConnectToServer(ServerUri);
			    await LoadWorkItemsAsync();
			});
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

        private void ConnectToServer(Uri serverUrl)
        {
            bool serverChanged = !UriEqualityComparer.Instance.Equals(serverUrl, _explorer.Server?.Uri);

            if (_explorer.Server != null)
                _explorer.Server.ConnectionStatusChanged -= Server_ConnectionStatusChanged;

            _explorer.Connect(serverUrl);
            _explorer.Server.ConnectionStatusChanged += Server_ConnectionStatusChanged;

            _projectNames.Clear();
            _projectNames.AddRange(_explorer.TeamProjects().Select(n => n.Name));

            if (serverChanged)
                ProjectName = null;

            IsConnected = true;
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
                    _projectNames.Clear();
                }
            }
		}

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<string> _status;
	    private readonly Property<bool> _isConnected;

		private readonly ICollection<string> _projectNames = new ObservableCollection<string>(); 

		private readonly ITfsExplorer _explorer;
	}
}