using System;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public class TestSelectionViewModel : ViewModelBase
    {
        public TestSelectionViewModel(Func<ITestCaseViewModel, IAutomationSelector> browserFactory)
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
                    TestBrowser = CreateTestBrowser();
                }
            }
        }

        public Lazy<IAutomationSelector> TestBrowser
        {
            get { return _testBrowser.Value; }
            private set { _testBrowser.Value = value; }
        }

        private Lazy<IAutomationSelector> CreateTestBrowser()
        {
            if (TestBrowser != null && TestBrowser.IsValueCreated)
                TestBrowser.Value.AutomatedTestSelected -= Browser_AutomatedTestSelected;

            return new Lazy<IAutomationSelector>(() =>
            {
                var testBrowser = _browserFactory(SelectedTestCase);
                testBrowser.AutomatedTestSelected += Browser_AutomatedTestSelected;
                return testBrowser;
            });
        }

        private void Browser_AutomatedTestSelected(IAutomationSelector sender, AutomatedTestSelectedEventArgs e)
        {
            e.TestCase.UpdateAutomation(e.TestAutomation);
            sender.AutomatedTestSelected -= Browser_AutomatedTestSelected;
        }

        private readonly Property<ITestCaseViewModel> _selectedTestCase; 

        private readonly Property<Lazy<IAutomationSelector>> _testBrowser;
        private readonly Func<ITestCaseViewModel, IAutomationSelector> _browserFactory;
    }
}