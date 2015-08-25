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
                                    ITestIdentifierFactory identifierFactory)
            : this()
        {
            _identifierFactory = identifierFactory;
            TestCase = testCase;
            var existingAutomation = TestCase.GetAutomation();
            AutomationIdentifier = existingAutomation?.Identifier;
            AutomationName = existingAutomation?.Name;
            AutomationTestType = existingAutomation?.TestType;
            AutomationStorage = existingAutomation?.Storage;
        }

        private TestBrowserViewModel()
        {
            _automationIdentifier = Property.New(this, p => p.AutomationIdentifier, OnPropertyChanged);

            _automationName = Property.New(this, p => p.AutomationName, OnPropertyChanged)
                                      .AlsoChanges(p => p.CanSave);

            _automationTestType = Property.New(this, p => p.AutomationTestType, OnPropertyChanged)
                                          .AlsoChanges(p => p.CanSave);

            _automationStorage = Property.New(this, p => p.AutomationStorage, OnPropertyChanged)
                                         .AlsoChanges(p => p.CanSave);

            _hasBeenSaved = Property.New(this, p => p.HasBeenSaved, OnPropertyChanged);

            SaveCommand = Command.For(this)
                                 .DependsOn(p => p.CanSave)
                                 .Executes(Save);
        }

        public ITestCaseViewModel TestCase { get; }

        /// <summary>
        /// A test's unique identifier.
        /// </summary>
        public Guid? AutomationIdentifier
        {
            get { return _automationIdentifier.Value; }
            set { _automationIdentifier.Value = value; }
        }

        /// <summary>
        /// The test name.
        /// </summary>
        public string AutomationName
        {
            get { return _automationName.Value; }
            set { _automationName.Value = value; }
        }

        /// <summary>
        /// A string representation of the test's type.
        /// </summary>
        public string AutomationTestType
        {
            get { return _automationTestType.Value; }
            set { _automationTestType.Value = value; }
        }

        /// <summary>
        /// A string representation of where a test is located. For example,
        /// this may be the path of a DLL or source file.
        /// </summary>
        public string AutomationStorage
        {
            get { return _automationStorage.Value; }
            set { _automationStorage.Value = value; }
        }

        public ICommand SaveCommand { get; }

        public bool CanSave => !String.IsNullOrWhiteSpace(AutomationName) &&
                               !String.IsNullOrWhiteSpace(AutomationStorage) &&
                               !String.IsNullOrWhiteSpace(AutomationTestType);

        /// <summary>
        /// Saves a test case with the associated automation.
        /// </summary>
        public void Save()
        {
            OnAutomatedTestSelected(new TestAutomationUpdate
            {
                Identifier = AutomationIdentifier ?? _identifierFactory.CreateIdentifier(AutomationName),
                Name = AutomationName,
                TestType = AutomationTestType,
                Storage = AutomationStorage
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

        public event EventHandler<AutomatedTestSelectedEventArgs> AutomatedTestSelected;

        private void OnAutomatedTestSelected(ITestAutomation testAutomation)
        {
            AutomatedTestSelected?.Invoke(this, new AutomatedTestSelectedEventArgs(TestCase, testAutomation));
        }

        private readonly Property<bool?> _hasBeenSaved;

        private readonly Property<Guid?> _automationIdentifier;
        private readonly Property<string> _automationName;
        private readonly Property<string> _automationTestType;
        private readonly Property<string> _automationStorage;

        private readonly ITestIdentifierFactory _identifierFactory;
    }
}