using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents the work items of a TFS project.
	/// </summary>
	public class WorkItemsViewModel : ViewModelBase, IWorkItems
	{
		/// <summary>
		/// Initializes a new <see cref="WorkItemsViewModel"/>.
		/// </summary>
		/// <param name="workItems">Used for querying for test cases</param>
		/// <param name="scheduler">Used for scheduling background tasks</param>
		public WorkItemsViewModel(ITfsProjectWorkItemCollection workItems, TaskScheduler scheduler)
		{
			_workItems = workItems;
			_scheduler = scheduler;

			_testCases = Property.New(this, p => p.TestCases, OnPropertyChanged);
			TestCases = new ObservableCollection<TestCaseViewModel>();
		}

		/// <summary>
		/// Loads test cases.
		/// </summary>
		public async Task LoadAsync()
		{
			var progress = new Progress<TestCaseViewModel>(testCase => TestCases.Add(testCase));
			await QueryTestCases(progress);
		}

		private Task QueryTestCases(IProgress<TestCaseViewModel> progress)
		{
			return Task.Factory.StartNew(() =>
			{
				var testCases = _workItems.TestCases().AsTestCases().Select(tc => new TestCaseViewModel(tc));
				foreach (var testCase in testCases)
					progress.Report(testCase);
			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		public ICollection<TestCaseViewModel> TestCases 
		{
			get { return _testCases.Value; }
			private set { _testCases.Value = value; }
		}

		private readonly Property<ICollection<TestCaseViewModel>> _testCases;
		private readonly ITfsProjectWorkItemCollection _workItems;
		private readonly TaskScheduler _scheduler;
	}
}