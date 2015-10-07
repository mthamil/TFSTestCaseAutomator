using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;

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
	    /// <param name="explorer">Provides team foundtion services</param>
	    /// <param name="testCaseFactory">Creates test case view-models</param>
	    public WorkItemsViewModel(ITfsExplorer explorer,
                                  Func<ITestCase, ITestCaseViewModel> testCaseFactory)
		{
	        _explorer = explorer;
	        _testCaseFactory = testCaseFactory;

			_testCases = Property.New(this, p => p.TestCases, OnPropertyChanged);
			TestCases = new ObservableCollection<ITestCaseViewModel>();
		}

	    /// <summary>
	    /// Loads work items for a given project.
	    /// </summary>
	    /// <param name="projectName">The project for which to load work items.</param>
	    public async Task LoadAsync(string projectName)
		{
			TestCases.Clear();
            await QueryTestCases(projectName);
		}

		private async Task QueryTestCases(string projectName)
		{
            (await _explorer.GetTestCasesAsync(
                        projectName, 
                        new Progress<ITestCase>(testCase => TestCases.Add(_testCaseFactory(testCase))))).ToList();
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
	    private readonly ITfsExplorer _explorer;
	    private readonly Func<ITestCase, ITestCaseViewModel> _testCaseFactory;
	}
}