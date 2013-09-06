﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;
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
		/// <param name="testCaseFactory">Creates test case view-models</param>
		/// <param name="scheduler">Used for scheduling background tasks</param>
		public WorkItemsViewModel(ITfsProjectWorkItemCollection workItems,
		                          Func<ITestCase, ITestCaseViewModel> testCaseFactory,
		                          TaskScheduler scheduler)
		{
			_workItems = workItems;
			_testCaseFactory = testCaseFactory;
			_scheduler = scheduler;

			_testCases = Property.New(this, p => p.TestCases, OnPropertyChanged);
			TestCases = new ObservableCollection<ITestCaseViewModel>();
		}

		/// <summary>
		/// Loads test cases.
		/// </summary>
		public async Task LoadAsync()
		{
			var progress = new Progress<ITestCaseViewModel>(testCase => TestCases.Add(testCase));
			await QueryTestCases(progress);
		}

		private Task QueryTestCases(IProgress<ITestCaseViewModel> progress)
		{
			return Task.Factory.StartNew(() =>
			{
				var testCases = _workItems.TestCases().AsTestCases().Select(tc => _testCaseFactory(tc));
				foreach (var testCase in testCases)
					progress.Report(testCase);
			}, CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		public ICollection<ITestCaseViewModel> TestCases 
		{
			get { return _testCases.Value; }
			private set { _testCases.Value = value; }
		}

		private readonly Property<ICollection<ITestCaseViewModel>> _testCases;
		private readonly ITfsProjectWorkItemCollection _workItems;
		private readonly Func<ITestCase, ITestCaseViewModel> _testCaseFactory;
		private readonly TaskScheduler _scheduler;
	}
}