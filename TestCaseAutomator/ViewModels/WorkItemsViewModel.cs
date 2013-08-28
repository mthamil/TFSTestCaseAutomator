using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		public WorkItemsViewModel(ITfsProjectWorkItemCollection workItems)
		{
			_workItems = workItems;

			_testCases = Property.New(this, p => p.TestCases, OnPropertyChanged);
		}

		/// <summary>
		/// Loads test cases.
		/// </summary>
		public void Load()
		{
			var testCases = _workItems.TestCases().AsTestCases().Select(tc => new TestCaseViewModel(tc));
			_testCases.Value = new ObservableCollection<TestCaseViewModel>(testCases);
		}

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		public ICollection<TestCaseViewModel> TestCases 
		{
			get { return _testCases.Value; }
		}

		private readonly Property<ICollection<TestCaseViewModel>> _testCases;
		private readonly ITfsProjectWorkItemCollection _workItems;
	}
}