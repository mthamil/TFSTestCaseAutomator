using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TestCaseAutomator.AutomationProviders.Interfaces;
using SharpEssentials.Collections;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;

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
		/// <param name="testDiscoverer">Finds tests in files</param>
		/// <param name="scheduler">Used to schedule background tasks</param>
		public FileSystemTestBrowserViewModel(ITestAutomationDiscoverer testDiscoverer, 
                                              TaskScheduler scheduler)
            : this()
		{
			_testDiscoverer = testDiscoverer;
			_scheduler = scheduler;
		}

	    private FileSystemTestBrowserViewModel()
	    {
	        _selectedFile = Property.New(this, p => p.SelectedFile, OnPropertyChanged)
	                                .UsingPathEquality();

            _tests = Property.New(this, p => p.Tests, OnPropertyChanged);

            _selectedTest = Property.New(this, p => p.SelectedTest, OnPropertyChanged)
                                    .AlsoChanges(p => p.CanSaveTestCase);

            _hasBeenSaved = Property.New(this, p => p.HasBeenSaved, OnPropertyChanged);

            _canBrowse = Property.New(this, p => p.CanBrowse, OnPropertyChanged);
            CanBrowse = true;

            Tests = new ObservableCollection<TestAutomationNodeViewModel>();
            SaveTestCaseCommand = Command.For(this)
                                         .DependsOn(p => p.CanSaveTestCase)
                                         .Executes(SaveTestCase);

            PropertyChanged += FileSystemTestBrowserViewModel_PropertyChanged;
        }

        /// <summary>
        /// The currently selected file.
        /// </summary>
        public FileInfo SelectedFile 
		{
			get { return _selectedFile.Value; }
			set 
			{
				if (_selectedFile.TrySetValue(value) && value != null)
					Tests.Clear();
			}
		}

        private async void FileSystemTestBrowserViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFile))
            {
                await DiscoverTestsAsync(new Progress<TestAutomationNodeViewModel>(test => Tests.Add(test)));
            }
        }

        private async Task DiscoverTestsAsync(IProgress<TestAutomationNodeViewModel> progress)
		{
			CanBrowse = false;
			try
			{
				await Task.Factory.StartNew(() =>
					_testDiscoverer.DiscoverAutomatedTests(SelectedFile.FullName.ToEnumerable())
								   .Select(test => new TestAutomationNodeViewModel(test))
								   .Tee(progress.Report)
								   .ToList(),
						CancellationToken.None,
						TaskCreationOptions.None,
						_scheduler);
			}
			finally
			{
				CanBrowse = true;
			}
		}

		/// <summary>
		/// The tests available in the currently selected file.
		/// </summary>
		public ICollection<TestAutomationNodeViewModel> Tests
		{
			get { return _tests.Value; }
			set { _tests.Value = value; }
		}

		/// <summary>
		/// The currently selected test.
		/// </summary>
		public TestAutomationNodeViewModel SelectedTest
		{
			get { return _selectedTest.Value; }
			set { _selectedTest.Value = value; }
		}

		/// <summary>
		/// Command that invokes <see cref="SaveTestCase"/>.
		/// </summary>
		public ICommand SaveTestCaseCommand { get; }

		/// <summary>
		/// Whether the current test case can be saved.
		/// </summary>
		public bool CanSaveTestCase => SelectedTest != null;

	    /// <summary>
		/// Saves a test case with the associated automation.
		/// </summary>
		public void SaveTestCase()
		{
			//OnAutomatedTestSelected();
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

		/// <summary>
		/// Whether a file can be selected.
		/// </summary>
		public bool CanBrowse
		{
			get { return _canBrowse.Value; }
			set { _canBrowse.Value = value; }
		}

		private readonly Property<FileInfo> _selectedFile;
		private readonly Property<ICollection<TestAutomationNodeViewModel>> _tests;
		private readonly Property<TestAutomationNodeViewModel> _selectedTest;
		private readonly Property<bool?> _hasBeenSaved;
		private readonly Property<bool> _canBrowse;

		private readonly ITestAutomationDiscoverer _testDiscoverer;
		private readonly TaskScheduler _scheduler;
	}
}