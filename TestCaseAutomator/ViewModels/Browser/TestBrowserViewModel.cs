using System;
using System.Windows.Input;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels.Browser
{
    public class TestBrowserViewModel : ViewModelBase, IAutomationSelector
    {
        public TestBrowserViewModel(ITestCaseViewModel testCase, 
                                    ITestIdentifierFactory identifierFactory,
                                    FileSystemTestBrowserViewModel fileSystemBrowser,
                                    SourceControlTestBrowserViewModel sourceControlBrowser)
            : this()
        {
            _identifierFactory = identifierFactory;
            FileSystemBrowser = fileSystemBrowser;
            SourceControlBrowser = sourceControlBrowser;
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

        public ICommand SaveCommand { get; }

        public bool CanSave => !String.IsNullOrWhiteSpace(TestAutomation.Name) &&
                               !String.IsNullOrWhiteSpace(TestAutomation.TestType) &&
                               !String.IsNullOrWhiteSpace(TestAutomation.Storage);

        /// <summary>
        /// Saves a test case with the associated automation.
        /// </summary>
        public void Save()
        {
            OnAutomatedTestSelected(new TestAutomationUpdate
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
            set { _hasBeenSaved.Value = value; }
        }

        public FileSystemTestBrowserViewModel FileSystemBrowser { get; }
        public SourceControlTestBrowserViewModel SourceControlBrowser { get; }

        public event EventHandler<AutomatedTestSelectedEventArgs> AutomatedTestSelected;

        private void OnAutomatedTestSelected(ITestAutomation testAutomation)
        {
            AutomatedTestSelected?.Invoke(this, new AutomatedTestSelectedEventArgs(TestCase, testAutomation));
        }

        private readonly Property<bool?> _hasBeenSaved;
        private readonly Property<ITestAutomation> _testAutomation;

        private readonly ITestIdentifierFactory _identifierFactory;
    }
}