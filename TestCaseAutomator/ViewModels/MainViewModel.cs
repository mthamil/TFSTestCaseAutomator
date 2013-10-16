using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// The main application window view model.
	/// </summary>
	public class MainViewModel : ViewModelBase, IApplication
	{
		public MainViewModel(
			Func<Uri, ITfsExplorer> explorerFactory, 
			Func<ITfsProjectWorkItemCollection, IWorkItems> workItemsFactory,
			Func<IEnumerable<TfsSolution>, ITestCaseViewModel, SourceControlTestBrowserViewModel> sourceControlBrowserFactory,
			Func<ITestCaseViewModel, FileSystemTestBrowserViewModel> fileSystemBrowserFactory)
		{
			_explorerFactory = explorerFactory;
			_workItemsFactory = workItemsFactory;
			_sourceControlBrowserFactory = sourceControlBrowserFactory;
			_fileSystemBrowserFactory = fileSystemBrowserFactory;

			_serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged);
			_projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
			_workItems = Property.New(this, p => p.WorkItems, OnPropertyChanged);
			_selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
			_status = Property.New(this, p => p.Status, OnPropertyChanged);

			_sourceControlBrowser = Property.New(this, p => p.SourceControlTestBrowser, OnPropertyChanged);
			_fileSystemBrowser = Property.New(this, p => p.FileSystemTestBrowser, OnPropertyChanged);
			_manualEntry = Property.New(this, p => p.ManualEntry, OnPropertyChanged);
			_selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);

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
					_explorer = _explorerFactory(value);
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
		/// The currently selected test case.
		/// </summary>
		public ITestCaseViewModel SelectedTestCase
		{
			get { return _selectedTestCase.Value; }
			set 
			{
				if (_selectedTestCase.TrySetValue(value))
				{
					SourceControlTestBrowser = CreateSourceControlBrowser();
					FileSystemTestBrowser = CreateFileSystemBrowser();
					ManualEntry = CreateManualEntry();
				}
			}
		}

		/// <summary>
		/// Contains work items from the current server and project.
		/// </summary>
		public IWorkItems WorkItems
		{
			get { return _workItems.Value; }
			private set { _workItems.Value = value; }
		}

		/// <summary>
		/// Allows selection of an automated test from source control.
		/// </summary>
		public SourceControlTestBrowserViewModel SourceControlTestBrowser
		{
			get { return _sourceControlBrowser.Value; }
			private set { _sourceControlBrowser.Value = value; }
		}

		private SourceControlTestBrowserViewModel CreateSourceControlBrowser()
		{
			if (SourceControlTestBrowser != null)
				SourceControlTestBrowser.AutomatedTestSelected -= Browser_AutomatedTestSelected;

			IEnumerable<TfsSolution> solutions = null;
			HandleServerUnavailable(() => solutions = _explorer.Solutions());

			var sourceControlTestBrowser = _sourceControlBrowserFactory(solutions ?? Enumerable.Empty<TfsSolution>(), SelectedTestCase);
			sourceControlTestBrowser.AutomatedTestSelected += Browser_AutomatedTestSelected;
			return sourceControlTestBrowser;
		}

		/// <summary>
		/// Allows selection of an automated test from a file on the file system.
		/// </summary>
		public FileSystemTestBrowserViewModel FileSystemTestBrowser
		{
			get { return _fileSystemBrowser.Value; }
			private set { _fileSystemBrowser.Value = value; }
		}

		private FileSystemTestBrowserViewModel CreateFileSystemBrowser()
		{
			if (FileSystemTestBrowser != null)
				FileSystemTestBrowser.AutomatedTestSelected -= Browser_AutomatedTestSelected;

			var fileSystemTestBrowser = _fileSystemBrowserFactory(SelectedTestCase);
			fileSystemTestBrowser.AutomatedTestSelected += Browser_AutomatedTestSelected;
			return fileSystemTestBrowser;
		}

		/// <summary>
		/// Allows manually entering test case automation details.
		/// </summary>
		public ManualAutomationEntryViewModel ManualEntry
		{
			get { return _manualEntry.Value; }
			private set { _manualEntry.Value = value; }
		}

		private ManualAutomationEntryViewModel CreateManualEntry()
		{
			if (ManualEntry != null)
				ManualEntry.AutomatedTestSelected -= Browser_AutomatedTestSelected;

			var manualEntry = new ManualAutomationEntryViewModel(SelectedTestCase);
			manualEntry.AutomatedTestSelected += Browser_AutomatedTestSelected;
			return manualEntry;
		}

		private void Browser_AutomatedTestSelected(object sender, AutomatedTestSelectedEventArgs e)
		{
			e.TestCase.UpdateAutomation(e.TestAutomation);
			((IAutomationSelector)sender).AutomatedTestSelected -= Browser_AutomatedTestSelected;
		}

		/// <summary>
		/// Command that forces a server refresh.
		/// </summary>
		public ICommand RefreshCommand { get; private set; }

		/// <summary>
		/// Refreshes data from the server.
		/// </summary>
		public async Task Refresh()
		{
			await HandleServerUnavailable(async () =>
			{
				if (WorkItems != null)
					await WorkItems.LoadAsync();
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
				Closing(this, EventArgs.Empty);
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
					await HandleServerUnavailable(async () =>
					{
						WorkItems = _workItemsFactory(_explorer.WorkItems(ProjectName));
						await WorkItems.LoadAsync();
					});
				}
			}
		}

		private void HandleServerUnavailable(Action action)
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

		private async Task HandleServerUnavailable(Func<Task> action)
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
		private readonly Property<ITestCaseViewModel> _selectedTestCase; 
		private readonly Property<IWorkItems> _workItems;
		private readonly Property<string> _status;

		private readonly Property<SourceControlTestBrowserViewModel> _sourceControlBrowser;
		private readonly Property<FileSystemTestBrowserViewModel> _fileSystemBrowser;
		private readonly Property<ManualAutomationEntryViewModel> _manualEntry;

		private readonly Func<Uri, ITfsExplorer> _explorerFactory;
		private readonly Func<ITfsProjectWorkItemCollection, IWorkItems> _workItemsFactory;
		private readonly Func<IEnumerable<TfsSolution>, ITestCaseViewModel, SourceControlTestBrowserViewModel> _sourceControlBrowserFactory;
		private readonly Func<ITestCaseViewModel, FileSystemTestBrowserViewModel> _fileSystemBrowserFactory;
	}
}