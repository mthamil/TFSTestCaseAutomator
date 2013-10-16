using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Allows manual entry of test automation details.
	/// </summary>
	public class ManualAutomationEntryViewModel : ViewModelBase, IAutomationSelector
	{
		/// <summary>
		/// Initializes a new <see cref="ManualAutomationEntryViewModel"/>.
		/// </summary>
		/// <param name="testCase">The test case to associate with automation</param>
		public ManualAutomationEntryViewModel(ITestCaseViewModel testCase)
		{
			TestCase = testCase;

			_name = Property.New(this, p => p.Name, OnPropertyChanged)
			                .AlsoChanges(p => p.CanSaveTestCase);
			_storageLocation = Property.New(this, p => p.StorageLocation, OnPropertyChanged)
			                           .AlsoChanges(p => p.CanSaveTestCase);
			_testType = Property.New(this, p => p.TestType, OnPropertyChanged)
			                    .AlsoChanges(p => p.CanSaveTestCase);
			_hasBeenSaved = Property.New(this, p => p.HasBeenSaved, OnPropertyChanged);

			SaveTestCaseCommand = Command.For(this)
										 .DependsOn(p => p.CanSaveTestCase)
										 .Executes(SaveTestCase);

			var existingAutomation = TestCase.GetAutomation();
			if (existingAutomation != null)
			{
				Name = existingAutomation.Name;
				StorageLocation = existingAutomation.Storage;
				TestType = existingAutomation.TestType;
			}
		}

		/// <see cref="IAutomationSelector.TestCase"/>
		public ITestCaseViewModel TestCase { get; private set; }

		/// <summary>
		/// An automated test's name.
		/// </summary>
		public string Name
		{
			get { return _name.Value; }
			set { _name.Value = value; }
		}

		/// <summary>
		/// Where an automated test can be found.
		/// </summary>
		public string StorageLocation
		{
			get { return _storageLocation.Value; }
			set { _storageLocation.Value = value; }
		}

		/// <summary>
		/// The type of automated test.
		/// </summary>
		public string TestType
		{
			get { return _testType.Value; }
			set { _testType.Value = value; }
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
			get
			{
				return !String.IsNullOrWhiteSpace(Name) &&
				       !String.IsNullOrWhiteSpace(StorageLocation) &&
				       !String.IsNullOrWhiteSpace(TestType);
			}
		}

		/// <summary>
		/// Saves a test case with the associated automation.
		/// </summary>
		public void SaveTestCase()
		{
			var test = new ManuallyCreatedTestAutomation
			{
				Name = Name,
				Storage = StorageLocation,
				TestType = TestType
			};

			OnAutomatedTestSelected(test);
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

		/// <see cref="IAutomationSelector.AutomatedTestSelected"/>
		public event EventHandler<AutomatedTestSelectedEventArgs> AutomatedTestSelected;

		private void OnAutomatedTestSelected(ITestAutomation testAutomation)
		{
			var localEvent = AutomatedTestSelected;
			if (localEvent != null)
				localEvent(this, new AutomatedTestSelectedEventArgs(TestCase, testAutomation));
		}

		private readonly Property<string> _name;
		private readonly Property<string> _storageLocation;
		private readonly Property<string> _testType;
		private readonly Property<bool?> _hasBeenSaved;

		private class ManuallyCreatedTestAutomation : ITestAutomation
		{
			public ManuallyCreatedTestAutomation()
			{
				_identifier = new Lazy<Guid>(() => new HashedIdentifierFactory().CreateIdentifier(Name));
			}

			public Guid Identifier { get { return _identifier.Value; } }
			public string Name { get; set; }
			public string TestType { get; set; }
			public string Storage { get; set; }

			private readonly Lazy<Guid> _identifier;
		}
	}
}