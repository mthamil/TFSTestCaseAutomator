using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Utilities.Collections;
using TestCaseAutomator.Utilities.InputOutput;
using TestCaseAutomator.Utilities.Mvvm;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// View-model for selection of automated tests from a file on the file system.
	/// </summary>
	public class FileSystemTestBrowserViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new <see cref="FileSystemTestBrowserViewModel"/>.
		/// </summary>
		/// <param name="testCase">The current test case</param>
		/// <param name="testFactory">Creates automated test view-models</param>
		/// <param name="testDiscoverer">Finds tests in files</param>
		/// <param name="scheduler">Used to schedule background tasks</param>
		public FileSystemTestBrowserViewModel(TestCaseViewModel testCase,
		                                      Func<IAutomatedTest, AutomatedTestViewModel> testFactory,
		                                      IAutomatedTestDiscoverer testDiscoverer, TaskScheduler scheduler)
		{
			_testFactory = testFactory;
			_testDiscoverer = testDiscoverer;
			_scheduler = scheduler;
			TestCase = testCase;

			_selectedFile = Property.New(this, p => p.SelectedFile, OnPropertyChanged)
			                        .EqualWhen((f1, f2) => FileInfoPathEqualityComparer.Instance.Equals(f1, f2));
			_tests = Property.New(this, p => p.Tests, OnPropertyChanged);
			_selectedTest = Property.New(this, p => p.SelectedTest, OnPropertyChanged)
									.AlsoChanges(p => p.CanSaveTestCase);

			Tests = new ObservableCollection<AutomatedTestViewModel>();
			SaveTestCaseCommand = Command.For(this)
										 .DependsOn(p => p.CanSaveTestCase)
										 .Executes(SaveTestCase);
		}

		/// <summary>
		/// The test case to associate with automation.
		/// </summary>
		public TestCaseViewModel TestCase { get; private set; }

		/// <summary>
		/// The currently selected file.
		/// </summary>
		public FileInfo SelectedFile 
		{
			get { return _selectedFile.Value; }
			set 
			{
				if (_selectedFile.TrySetValue(value) && value != null)
				{
					Tests.Clear();
					DiscoverTests(new Progress<AutomatedTestViewModel>(test => Tests.Add(test)));
				}
			}
		}

		private Task<IReadOnlyCollection<AutomatedTestViewModel>> DiscoverTests(IProgress<AutomatedTestViewModel> progress)
		{
			return Task<IReadOnlyCollection<AutomatedTestViewModel>>.Factory.StartNew(() =>
			    _testDiscoverer.DiscoverAutomatedTests(SelectedFile.FullName.ToEnumerable())
			                   .Select(test => _testFactory(test))
							   .Tee(progress.Report)
			                   .ToList(),
					CancellationToken.None,
					TaskCreationOptions.None, 
					_scheduler);
		}

		/// <summary>
		/// The tests available in the currently selected file.
		/// </summary>
		public ICollection<AutomatedTestViewModel> Tests
		{
			get { return _tests.Value; }
			set { _tests.Value = value; }
		}

		/// <summary>
		/// The currently selected test.
		/// </summary>
		public AutomatedTestViewModel SelectedTest
		{
			get { return _selectedTest.Value; }
			set { _selectedTest.Value = value; }
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
			get { return SelectedTest != null; }
		}

		/// <summary>
		/// Saves a test case with the associated automation.
		/// </summary>
		public void SaveTestCase()
		{

		}

		private readonly Property<FileInfo> _selectedFile;
		private readonly Property<ICollection<AutomatedTestViewModel>> _tests;
		private readonly Property<AutomatedTestViewModel> _selectedTest;

		private readonly Func<IAutomatedTest, AutomatedTestViewModel> _testFactory;
		private readonly IAutomatedTestDiscoverer _testDiscoverer;
		private readonly TaskScheduler _scheduler;
	}
}