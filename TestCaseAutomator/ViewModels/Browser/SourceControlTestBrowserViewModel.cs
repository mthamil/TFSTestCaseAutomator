using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// View-model for selection of automated tests from source control.
	/// </summary>
	public class SourceControlTestBrowserViewModel : ViewModelBase, IAutomationSelector
	{
	    /// <summary>
	    /// Initializes a new <see cref="SourceControlTestBrowserViewModel"/>.
	    /// </summary>
	    /// <param name="solutions">Existing solutions in source control</param>
	    /// <param name="testCase">The test case to associate with automation</param>
	    /// <param name="solutionFactory">Creates solution view-models</param>
	    public SourceControlTestBrowserViewModel(IEnumerable<TfsSolution> solutions,
	                                             ITestCaseViewModel testCase,
	                                             Func<TfsSolution, SolutionViewModel> solutionFactory)
	    {
	        TestCase = testCase;

	        _solutions = Property.New(this, p => p.Solutions, OnPropertyChanged);
	        _selectedTest = Property.New(this, p => p.SelectedTest, OnPropertyChanged)
	                                .AlsoChanges(p => p.CanSaveTestCase);
	        _hasBeenSaved = Property.New(this, p => p.HasBeenSaved, OnPropertyChanged);

	        Solutions = new ObservableCollection<SolutionViewModel>(solutions.Select(solutionFactory));

	        SaveTestCaseCommand = Command.For(this)
	                                     .DependsOn(p => p.CanSaveTestCase)
	                                     .Executes(SaveTestCase);
	    }

	    /// <see cref="IAutomationSelector.TestCase"/>
		public ITestCaseViewModel TestCase { get; private set; }

		/// <summary>
		/// The currently selected test.
		/// </summary>
		public ViewModelBase SelectedTest
		{
			get { return _selectedTest.Value; }
			set { _selectedTest.Value = value; }
		}

		/// <see cref="IAutomationSelector.AutomatedTestSelected"/>
		public event EventHandler<AutomatedTestSelectedEventArgs> AutomatedTestSelected;

		private void OnAutomatedTestSelected()
		{
			var localEvent = AutomatedTestSelected;
			if (localEvent != null)
				localEvent(this, new AutomatedTestSelectedEventArgs(TestCase, ((TestAutomationViewModel)SelectedTest).TestAutomation));
		}

		/// <summary>
		/// Command that invokes <see cref="SaveTestCase"/>.
		/// </summary>
		public ICommand SaveTestCaseCommand { get; private set; }

		/// <summary>
		/// Whether the current test case can be saved.
		/// </summary>
		public bool CanSaveTestCase
		{
			get { return SelectedTest != null && SelectedTest is TestAutomationViewModel; }
		}

		/// <summary>
		/// Saves a test case with the associated automation.
		/// </summary>
		public void SaveTestCase()
		{
			OnAutomatedTestSelected();
			HasBeenSaved = true;
		}

		/// <summary>
		/// Whether test automation has been chosen.
		/// </summary>
		public bool? HasBeenSaved
		{
			get { return _hasBeenSaved.Value; }
			set { _hasBeenSaved.Value = value; }
		}

		/// <summary>
		/// The available solutions in source control.
		/// </summary>
		public ICollection<SolutionViewModel> Solutions
		{
			get { return _solutions.Value; }
			private set { _solutions.Value = value; }
		}

		private readonly Property<ViewModelBase> _selectedTest;
		private readonly Property<bool?> _hasBeenSaved;
		private readonly Property<ICollection<SolutionViewModel>> _solutions;
	}
}