using System;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public class TestSelectionViewModel : ViewModelBase
    {
        public TestSelectionViewModel(Func<ITestCaseViewModel, TestBrowserViewModel> browserFactory)
            : this()
        {
            _browserFactory = browserFactory;
        }

        private TestSelectionViewModel()
        {
            _testBrowser = Property.New(this, p => p.TestBrowser, OnPropertyChanged);
            _selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
        }

        /// <summary>
        /// The currently selected test case.
        /// </summary>
        public ITestCaseViewModel SelectedTestCase
        {
            get { return _selectedTestCase.Value; }
            set
            {
                if (_selectedTestCase.TrySetValue(value) && value != null)
                {
                    TestBrowser = new Lazy<TestBrowserViewModel>(() => _browserFactory(SelectedTestCase));
                }
            }
        }

        public Lazy<TestBrowserViewModel> TestBrowser
        {
            get { return _testBrowser.Value; }
            private set { _testBrowser.Value = value; }
        }

        private readonly Property<ITestCaseViewModel> _selectedTestCase; 

        private readonly Property<Lazy<TestBrowserViewModel>> _testBrowser;
        private readonly Func<ITestCaseViewModel, TestBrowserViewModel> _browserFactory;
    }
}