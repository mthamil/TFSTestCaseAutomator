using System;
using System.ComponentModel;
using System.Windows.Input;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;
using TestCaseAutomator.AutomationProviders.Abstractions;
using TestCaseAutomator.ViewModels.Browser.Nodes;

namespace TestCaseAutomator.ViewModels.Browser
{
    public class TestBrowserViewModel : ViewModelBase
    {
        public TestBrowserViewModel(ITestCaseViewModel testCase, 
                                    ITestIdentifierFactory identifierFactory,
                                    FileSystemTestBrowserViewModel fileSystemBrowser,
                                    SourceControlTestBrowserViewModel sourceControlBrowser) : this()
        {
            _identifierFactory = identifierFactory;

            FileSystemBrowser = fileSystemBrowser;
            FileSystemBrowser.PropertyChanged += Browser_PropertyChanged;

            SourceControlBrowser = sourceControlBrowser;
            SourceControlBrowser.PropertyChanged += Browser_PropertyChanged;

            TestCase = testCase;
            TestAutomation = TestCase.GetAutomation();
        }

        private TestBrowserViewModel()
        {
            _testAutomation = Property.New(this, p => p.TestAutomation, OnPropertyChanged)
                                      .AlsoChanges(p => p.CanSave);

            _hasBeenSaved = Property.New(this, p => p.HasBeenSaved, OnPropertyChanged);

            SaveCommand = Command.For(this)
                                 .DependsOn(p => p.CanSave)
                                 .Executes(Save);

            CancelCommand = new RelayCommand(Cancel);
        }

        public ITestCaseViewModel TestCase { get; }

        public ITestAutomation TestAutomation
        {
            get { return _testAutomation.Value; }
            set
            {
                _testAutomation.Value = new TestAutomationViewModel
                {
                    Identifier = value?.Identifier ?? default(Guid),
                    Name = value?.Name,
                    TestType = value?.TestType,
                    Storage = value?.Storage
                };
            }
        }

        public ICommand CancelCommand { get; }

        public void Cancel()
        {
            HasBeenSaved = false;
        }

        public ICommand SaveCommand { get; }

        public bool CanSave => !String.IsNullOrWhiteSpace(TestAutomation.Name) &&
                               !String.IsNullOrWhiteSpace(TestAutomation.TestType) &&
                               !String.IsNullOrWhiteSpace(TestAutomation.Storage);

        /// <summary>
        /// Saves a test case with the associated automation.
        /// </summary>
        public void Save()
        {
            TestCase.UpdateAutomation(new TestAutomationUpdate
            {
                Identifier = TestAutomation.Identifier == default(Guid)
                                ? _identifierFactory.CreateIdentifier(TestAutomation.Name)
                                : TestAutomation.Identifier,
                Name = TestAutomation.Name,
                TestType = TestAutomation.TestType,
                Storage = TestAutomation.Storage
            });

            HasBeenSaved = true;
        }

        /// <summary>
        /// Whether test automation has been chosen.
        /// </summary>
        public bool? HasBeenSaved
        {
            get { return _hasBeenSaved.Value; }
            set
            {
                if (_hasBeenSaved.TrySetValue(value))
                {
                    FileSystemBrowser.PropertyChanged -= Browser_PropertyChanged;
                    SourceControlBrowser.PropertyChanged -= Browser_PropertyChanged;
                }
            }
        }

        public FileSystemTestBrowserViewModel FileSystemBrowser { get; }
        public SourceControlTestBrowserViewModel SourceControlBrowser { get; }

        private void Browser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileSystemTestBrowserViewModel.SelectedTest) &&
                sender == FileSystemBrowser)
            {
                TestAutomation = FileSystemBrowser.SelectedTest?.TestAutomation;
            }
            else if (e.PropertyName == nameof(SourceControlTestBrowserViewModel.SelectedTest) &&
                     sender == SourceControlBrowser &&
                     SourceControlBrowser.IsValid)
            {
                TestAutomation = ((TestAutomationNodeViewModel)SourceControlBrowser.SelectedTest)?.TestAutomation;
            }
        }

        private readonly Property<bool?> _hasBeenSaved;
        private readonly Property<ITestAutomation> _testAutomation;

        private readonly ITestIdentifierFactory _identifierFactory;
    }
}