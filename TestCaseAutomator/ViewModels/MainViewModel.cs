using System;
using System.Collections.Generic;
using System.Linq;
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
			Func<IEnumerable<TfsSolution>, TestCaseViewModel, TestBrowserViewModel> testBrowserFactory)
		{
			_explorerFactory = explorerFactory;
			_workItemsFactory = workItemsFactory;
			_testBrowserFactory = testBrowserFactory;

			_serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged);
			_projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
			_workItems = Property.New(this, p => p.WorkItems, OnPropertyChanged);
			_testBrowser = Property.New(this, p => p.TestBrowser, OnPropertyChanged);
			_selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
			_status = Property.New(this, p => p.Status, OnPropertyChanged);

			RefreshCommand = new RelayCommand(Refresh);
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
						HandleServerUnavailable(() =>
						{
							_workItems.Value = _workItemsFactory(_explorer.WorkItems(value));
							_workItems.Value.Load();
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
					TestBrowser = CreateTestBrowser();
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
		/// Allows selection of an automated test.
		/// </summary>
		public TestBrowserViewModel TestBrowser
		{
			get { return _testBrowser.Value; }
			private set { _testBrowser.Value = value; }
		}

		private TestBrowserViewModel CreateTestBrowser()
		{
			IEnumerable<TfsSolution> solutions = null;
			HandleServerUnavailable(() => solutions = _explorer.Solutions());

			return _testBrowserFactory(solutions ?? Enumerable.Empty<TfsSolution>(), SelectedTestCase);
		}

		/// <summary>
		/// Command that forces a server refresh.
		/// </summary>
		public ICommand RefreshCommand { get; private set; }

		/// <summary>
		/// Refreshes data from the server.
		/// </summary>
		public void Refresh()
		{
			HandleServerUnavailable(() =>
			{
				if (WorkItems != null)
					WorkItems.Load();
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

		private ITfsExplorer _explorer;

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<TestCaseViewModel> _selectedTestCase; 
		private readonly Property<IWorkItems> _workItems;
		private readonly Property<TestBrowserViewModel> _testBrowser;
		private readonly Property<string> _status;
		private readonly Func<Uri, ITfsExplorer> _explorerFactory;
		private readonly Func<ITfsProjectWorkItemCollection, IWorkItems> _workItemsFactory;
		private readonly Func<IEnumerable<TfsSolution>, TestCaseViewModel, TestBrowserViewModel> _testBrowserFactory;
	}
}