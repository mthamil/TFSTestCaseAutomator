using System;
using System.Collections.Generic;
using System.Linq;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.PropertyNotification;
using TestCaseAutomator.ViewModels.Browser;

namespace TestCaseAutomator.ViewModels
{
    public class TestSelectionViewModel : ViewModelBase
    {
        public TestSelectionViewModel(
                Func<IEnumerable<TfsSolution>, ITestCaseViewModel, SourceControlTestBrowserViewModel> sourceControlBrowserFactory,
                Func<ITestCaseViewModel, FileSystemTestBrowserViewModel> fileSystemBrowserFactory)
                : this()
        {
            _sourceControlBrowserFactory = sourceControlBrowserFactory;
            _fileSystemBrowserFactory = fileSystemBrowserFactory;
        }

        private TestSelectionViewModel()
        {
            _sourceControlBrowser = Property.New(this, p => p.SourceControlTestBrowser, OnPropertyChanged);
            _fileSystemBrowser = Property.New(this, p => p.FileSystemTestBrowser, OnPropertyChanged);
            _manualEntry = Property.New(this, p => p.ManualEntry, OnPropertyChanged);
            _selectedTestCase = Property.New(this, p => p.SelectedTestCase, OnPropertyChanged);
        }

        public Func<IEnumerable<TfsSolution>> SolutionRetriever { get; set; }

        /// <summary>
        /// The currently selected test case.
        /// </summary>
        public ITestCaseViewModel SelectedTestCase
        {
            get { return _selectedTestCase.Value; }
            set
            {
                if (_selectedTestCase.TrySetValue(value))
                {
                    SourceControlTestBrowser = CreateSourceControlBrowser();
                    FileSystemTestBrowser = CreateFileSystemBrowser();
                    ManualEntry = CreateManualEntry();
                }
            }
        }

        /// <summary>
        /// Allows selection of an automated test from source control.
        /// </summary>
        public Lazy<SourceControlTestBrowserViewModel> SourceControlTestBrowser
        {
            get { return _sourceControlBrowser.Value; }
            private set { _sourceControlBrowser.Value = value; }
        }

        private Lazy<SourceControlTestBrowserViewModel> CreateSourceControlBrowser()
        {
            if (SourceControlTestBrowser != null && SourceControlTestBrowser.IsValueCreated)
                SourceControlTestBrowser.Value.AutomatedTestSelected -= Browser_AutomatedTestSelected;

            return new Lazy<SourceControlTestBrowserViewModel>(() =>
            {
                var solutions = SolutionRetriever();

                var testBrowser = _sourceControlBrowserFactory(solutions ?? Enumerable.Empty<TfsSolution>(), SelectedTestCase);
                testBrowser.AutomatedTestSelected += Browser_AutomatedTestSelected;
                return testBrowser;
            });
        }

        /// <summary>
        /// Allows selection of an automated test from a file on the file system.
        /// </summary>
        public Lazy<FileSystemTestBrowserViewModel> FileSystemTestBrowser
        {
            get { return _fileSystemBrowser.Value; }
            private set { _fileSystemBrowser.Value = value; }
        }

        private Lazy<FileSystemTestBrowserViewModel> CreateFileSystemBrowser()
        {
            if (FileSystemTestBrowser != null && FileSystemTestBrowser.IsValueCreated)
                FileSystemTestBrowser.Value.AutomatedTestSelected -= Browser_AutomatedTestSelected;

            return new Lazy<FileSystemTestBrowserViewModel>(() =>
            {
                var fileSystemTestBrowser = _fileSystemBrowserFactory(SelectedTestCase);
                fileSystemTestBrowser.AutomatedTestSelected += Browser_AutomatedTestSelected;
                return fileSystemTestBrowser;
            });
        }

        /// <summary>
        /// Allows manually entering test case automation details.
        /// </summary>
        public Lazy<ManualAutomationEntryViewModel> ManualEntry
        {
            get { return _manualEntry.Value; }
            private set { _manualEntry.Value = value; }
        }

        private Lazy<ManualAutomationEntryViewModel> CreateManualEntry()
        {
            if (ManualEntry != null && ManualEntry.IsValueCreated)
                ManualEntry.Value.AutomatedTestSelected -= Browser_AutomatedTestSelected;

            return new Lazy<ManualAutomationEntryViewModel>(() =>
            {
                var manualEntry = new ManualAutomationEntryViewModel(SelectedTestCase);
                manualEntry.AutomatedTestSelected += Browser_AutomatedTestSelected;
                return manualEntry;
            });
        }

        private void Browser_AutomatedTestSelected(object sender, AutomatedTestSelectedEventArgs e)
        {
            e.TestCase.UpdateAutomation(e.TestAutomation);
            ((IAutomationSelector)sender).AutomatedTestSelected -= Browser_AutomatedTestSelected;
        }

        private readonly Property<ITestCaseViewModel> _selectedTestCase; 
        private readonly Property<Lazy<SourceControlTestBrowserViewModel>> _sourceControlBrowser;
        private readonly Property<Lazy<FileSystemTestBrowserViewModel>> _fileSystemBrowser;
        private readonly Property<Lazy<ManualAutomationEntryViewModel>> _manualEntry;


        private readonly Func<IEnumerable<TfsSolution>, ITestCaseViewModel, SourceControlTestBrowserViewModel> _sourceControlBrowserFactory;
        private readonly Func<ITestCaseViewModel, FileSystemTestBrowserViewModel> _fileSystemBrowserFactory;
    }
}