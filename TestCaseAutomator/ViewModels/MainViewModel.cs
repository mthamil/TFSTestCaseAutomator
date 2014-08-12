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
using TestCaseAutomator.Utilities.Collections;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// The main application window view model.
	/// </summary>
	public class MainViewModel : ViewModelBase, IApplication
	{
		public MainViewModel(
			Func<Uri, ITfsExplorer> explorerFactory, 
			IWorkItems workItems,
			TestSelectionViewModel testSelection)
                : this()
		{
			_explorerFactory = explorerFactory;
		    WorkItems = workItems;
            TestSelection = testSelection;

            // ugh, hacky
		    TestSelection.SolutionRetriever = () =>
		    {
		        IEnumerable<TfsSolution> solutions = null;
                HandleServerError(() => solutions = _explorer.Solutions());
		        return solutions;
		    };
		}

	    private MainViewModel()
	    {
            _serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged);
            _projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
            _status = Property.New(this, p => p.Status, OnPropertyChanged);

            RefreshCommand = new AsyncRelayCommand(Refresh);
            CloseCommand = new RelayCommand(Close);

            PropertyChanged += OnPropertyChanged;
	    }

		/// <see cref="IApplication.ServerUri"/>
		public Uri ServerUri
		{
			get { return _serverUri.Value; }
			set
			{
				if (_serverUri.TrySetValue(value))
				{
				   HandleServerError(() => 
                       ConnectToServer(value));
				}
			}
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
		public ICollection<string> ProjectNames
		{
			get { return _projectNames; }
		}

		/// <summary>
		/// Contains work items from the current server and project.
		/// </summary>
		public IWorkItems WorkItems { get; private set; }

        public TestSelectionViewModel TestSelection { get; private set; }

		/// <summary>
		/// Command that forces a server refresh.
		/// </summary>
		public ICommand RefreshCommand { get; private set; }

		/// <summary>
		/// Refreshes data from the server.
		/// </summary>
		public async Task Refresh()
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
		public ICommand CloseCommand { get; private set; }

		private void Close()
		{
			OnClosing();
		}

		/// <see cref="IApplication.Closing"/>
		public event EventHandler<EventArgs> Closing;

		private void OnClosing()
		{
			var localEvent = Closing;
			if (localEvent != null)
                localEvent(this, EventArgs.Empty);
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
			if (propertyChangedEventArgs.PropertyName == _projectName.Name)
			{
				if (_explorer != null)
				{
					await HandleServerError(async () => 
                        await LoadWorkItemsAsync());
				}
			}
		}

        private void ConnectToServer(Uri serverUrl)
        {
            _explorer = _explorerFactory(serverUrl);
            LoadProjectNames();
        }

        private void LoadProjectNames()
        {
            _projectNames.Clear();
            _projectNames.AddRange(_explorer.TeamProjects().Select(n => n.Name));
        }

        private async Task LoadWorkItemsAsync()
        {
            if (String.IsNullOrWhiteSpace(ProjectName))
                return;

            await WorkItems.LoadAsync(_explorer.WorkItems(ProjectName));
	    }

		private void HandleServerError(Action action)
		{
			try
			{
				action();
				Status = null;
			}
			catch (TeamFoundationServiceUnavailableException tfsUnavailableEx)
			{
				Status = tfsUnavailableEx.Message;
			}
			catch (TestObjectNotFoundException testObjectNotFoundEx)
			{
				Status = testObjectNotFoundEx.Message;
			}
		}

		private async Task HandleServerError(Func<Task> action)
		{
			try
			{
				await action();
				Status = null;
			}
			catch (TeamFoundationServiceUnavailableException tfsUnavailableEx)
			{
				Status = tfsUnavailableEx.Message;
			}
			catch (TestObjectNotFoundException testObjectNotFoundEx)
			{
				Status = testObjectNotFoundEx.Message;
			}
		}

		private ITfsExplorer _explorer;

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<string> _status;

		private readonly ICollection<string> _projectNames = new ObservableCollection<string>(); 

		private readonly Func<Uri, ITfsExplorer> _explorerFactory;
	}
}