using System;
using System.Windows.Input;
using TestCaseAutomator.TeamFoundation;
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
		public MainViewModel(Func<Uri, ITfsExplorer> explorerFactory)
		{
			_explorerFactory = explorerFactory;
			_serverUri = Property.New(this, p => p.ServerUri, OnPropertyChanged);
			_projectName = Property.New(this, p => p.ProjectName, OnPropertyChanged);
			_workItems = Property.New(this, p => p.WorkItems, OnPropertyChanged);

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
						_workItems.Value = new WorkItemsViewModel(_explorer.WorkItems(value));
						_workItems.Value.Load();
					}
				}
			}
		}

		/// <summary>
		/// Contains work items from the current server and project.
		/// </summary>
		public WorkItemsViewModel WorkItems
		{
			get { return _workItems.Value; }
			private set { _workItems.Value = value; }
		}

		/// <summary>
		/// Commands invoked when the applicaiton is closing.
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

		private ITfsExplorer _explorer;

		private readonly Property<Uri> _serverUri;
		private readonly Property<string> _projectName;
		private readonly Property<WorkItemsViewModel> _workItems;
		private readonly Func<Uri, ITfsExplorer> _explorerFactory;
	}
}