using System;
using System.Collections.Generic;
using System.Linq;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// View-model for selection of automated tests from source control.
	/// </summary>
	public class SourceControlTestBrowserViewModel : ViewModelBase
	{
	    /// <summary>
	    /// Initializes a new <see cref="SourceControlTestBrowserViewModel"/>.
	    /// </summary>
	    /// <param name="explorer">The current TFS explorer</param>
	    /// <param name="solutionFactory">Creates solution view-models</param>
	    public SourceControlTestBrowserViewModel(ITfsExplorer explorer,
	                                             Func<TfsSolution, SolutionViewModel> solutionFactory)
            : this()
	    {
	        _explorer = explorer;
	        _solutionFactory = solutionFactory;
	    }

	    private SourceControlTestBrowserViewModel()
	    {
            _selectedTest = Property.New(this, p => p.SelectedTest, OnPropertyChanged)
                                    .AlsoChanges(p => p.CanSaveTestCase);
        }

		/// <summary>
		/// The currently selected test.
		/// </summary>
		public ViewModelBase SelectedTest
		{
			get { return _selectedTest.Value; }
			set { _selectedTest.Value = value; }
		}

		/// <summary>
		/// Whether the current test case can be saved.
		/// </summary>
		public bool CanSaveTestCase => SelectedTest != null && SelectedTest is TestAutomationNodeViewModel;

		/// <summary>
		/// The available solutions in source control.
		/// </summary>
		public IEnumerable<SolutionViewModel> Solutions => _explorer.Solutions().Select(_solutionFactory);

	    private readonly Property<ViewModelBase> _selectedTest;

        private readonly ITfsExplorer _explorer;
	    private readonly Func<TfsSolution, SolutionViewModel> _solutionFactory;
	}
}