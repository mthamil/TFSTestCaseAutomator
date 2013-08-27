using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents all TFS work items.
	/// </summary>
	public class WorkItemsViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new <see cref="WorkItemsViewModel"/>.
		/// </summary>
		/// <param name="workItems">Used for querying for test cases</param>
		public WorkItemsViewModel(ITfsProjectWorkItemCollection workItems)
		{
			_workItems = workItems;

			_testCases = Property.New(this, p => p.TestCases, OnPropertyChanged);
			RefreshCommand = new RelayCommand(Load);
		}

		/// <summary>
		/// Command that forces a refresh of test cases.
		/// </summary>
		public ICommand RefreshCommand { get; private set; }

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