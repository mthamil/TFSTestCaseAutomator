using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public class TestCasesViewModel : ViewModelBase, ITestCases
    {
        /// <summary>
        /// Initializes a new <see cref="TestCasesViewModel"/>.
        /// </summary>
        /// <param name="explorer">>Provides team foundation services.</param>
        /// <param name="browserFactory">Creates test automation browser view-models.</param>
        /// <param name="testCaseFactory">Creates test case view-models.</param>
        public TestCasesViewModel(
            ITfsExplorer explorer,
            Func<ITestCaseViewModel, Lazy<TestBrowserViewModel>> browserFactory,
            Func<ITestCase, ITestCaseViewModel> testCaseFactory)
            : this()
        {
            _explorer = explorer;
            _browserFactory = browserFactory;
            _testCaseFactory = testCaseFactory;
        }

        private TestCasesViewModel()
        {
            _testBrowser = Property.New(this, p => p.TestBrowser, OnPropertyChanged);
            _selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
            _items = Property.New(this, p => p.Items, OnPropertyChanged);

            Items = new ObservableCollection<ITestCaseViewModel>();
        }

        public Lazy<TestBrowserViewModel> TestBrowser
        {
            get { return _testBrowser.Value; }
            private set { _testBrowser.Value = value; }
        }

        /// <see cref="ITestCases.SelectedTestCase"/>
        public ITestCaseViewModel SelectedTestCase
        {
            get { return _selectedTestCase.Value; }
            set
            {
                if (_selectedTestCase.TrySetValue(value) && value != null)
                {
                    TestBrowser = _browserFactory(SelectedTestCase);
                }
            }
        }

        /// <see cref="ITestCases.Items"/>
        public ICollection<ITestCaseViewModel> Items
        {
            get { return _items.Value; }
            private set { _items.Value = value; }
        }

        /// <see cref="ITestCases.LoadAsync"/>
        public async Task LoadAsync(string projectName)
        {
            Items.Clear();

            if (!String.IsNullOrWhiteSpace(projectName))
                await QueryTestCases(projectName);
        }

        private async Task QueryTestCases(string projectName)
        {
            (await _explorer.GetTestCasesAsync(
                        projectName,
                        new Progress<ITestCase>(testCase => Items.Add(_testCaseFactory(testCase))))).ToList();
        }

        private readonly Property<Lazy<TestBrowserViewModel>> _testBrowser;
        private readonly Property<ITestCaseViewModel> _selectedTestCase;
        private readonly Property<ICollection<ITestCaseViewModel>> _items;

        private readonly Func<ITestCaseViewModel, Lazy<TestBrowserViewModel>> _browserFactory;
        private readonly Func<ITestCase, ITestCaseViewModel> _testCaseFactory;
        private readonly ITfsExplorer _explorer;
    }
}