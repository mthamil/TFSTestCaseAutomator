using System;
using System.Collections.Generic;
using SharpEssentials.Collections;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.ViewModels.Browser.Nodes;

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
	    /// 
	    public SourceControlTestBrowserViewModel(Func<SourceRootNodeViewModel> rootFactory)
            : this()
	    {
	        SourceTree = rootFactory().ToEnumerable();
	    }

	    private SourceControlTestBrowserViewModel()
	    {
            _selectedTest = Property.New(this, p => p.SelectedTest)
                                    .AlsoChanges(p => p.IsValid);
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
		public bool IsValid => SelectedTest != null && SelectedTest is TestAutomationNodeViewModel;

        public IEnumerable<INodeViewModel> SourceTree { get; }

	    private readonly Property<ViewModelBase> _selectedTest;
	}
}