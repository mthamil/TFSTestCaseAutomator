using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.TeamFoundation;
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
			Func<IEnumerable<TfsSolution>, TestCaseViewModel, SourceControlTestBrowserViewModel> sourceControlBrowserFactory,
			Func<TestCaseViewModel, FileSystemTestBrowserViewModel> fileSystemBrowserFactory)
		{
			_explorerFactory = explorerFactory;
			_workItemsFactory = workItemsFactory;
			_sourceControlBrowserFactory = sourceControlBrowserFactory;
			_fileSystemBrowserFactory = fileSystemBrowserFactory;

			_serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged);
			_projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
			_workItems = Property.New(this, p => p.WorkItems, OnPropertyChanged);
			_sourceControlBrowser = Property.New(this, p => p.SourceControlTestBrowser, OnPropertyChanged);
			_fileSystemBrowser = Property.New(this, p => p.FileSystemTestBrowser, OnPropertyChanged);
			_selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
			_status = Property.New(this, p => p.Status, OnPropertyChanged);

			RefreshCommand = new AsyncRelayCommand(Refresh);
			CloseCommand = new RelayCommand(Close);
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
			set
			{
				if (_projectName.TrySetValue(value))
				{
					if (_explorer != null)
					{
						HandleServerUnavailable(async () =>
						{
							WorkItems = _workItemsFactory(_explorer.WorkItems(value));
							await WorkItems.LoadAsync();
						});
					}
				}
			}
		}

		/// <summary>
		/// The currently selected test case.
		/// </summary>
		public TestCaseViewModel SelectedTestCase
		{
			get { return _selectedTestCase.Value; }
			set
			{
				if (_selectedTestCase.TrySetValue(value))
				{
					SourceControlTestBrowser = CreateSourceControlBrowser();
					FileSystemTestBrowser = CreateFileSystemBrowser();
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
			IEnumerable<TfsSolution> solutions = null;
			HandleServerUnavailable(() => solutions = _explorer.Solutions());

			return _sourceControlBrowserFactory(solutions ?? Enumerable.Empty<TfsSolution>(), SelectedTestCase);
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
			return _fileSystemBrowserFactory(SelectedTestCase);
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

		private void HandleServerUnavailable(Action action)
		{
			try
			{
				action();
				Status = null;
			}
			catch (TeamFoundationServiceUnavailableException e)
			{
				Status = e.Message;
			}
		}

		private async Task HandleServerUnavailable(Func<Task> action)
		{
			try
			{
				await action();
				Status = null;
			}
			catch (TeamFoundationServiceUnavailableException e)
			{
				Status = e.Message;
			}
		}

		private ITfsExplorer _explorer;

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<TestCaseViewModel> _selectedTestCase; 
		private readonly Property<IWorkItems> _workItems;
		private readonly Property<SourceControlTestBrowserViewModel> _sourceControlBrowser;
		private readonly Property<FileSystemTestBrowserViewModel> _fileSystemBrowser;
		private readonly Property<string> _status;
		private readonly Func<Uri, ITfsExplorer> _explorerFactory;
		private readonly Func<ITfsProjectWorkItemCollection, IWorkItems> _workItemsFactory;
		private readonly Func<IEnumerable<TfsSolution>, TestCaseViewModel, SourceControlTestBrowserViewModel> _sourceControlBrowserFactory;
		private readonly Func<TestCaseViewModel, FileSystemTestBrowserViewModel> _fileSystemBrowserFactory;
	}
}