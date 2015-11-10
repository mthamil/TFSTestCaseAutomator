using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;
using TestCaseAutomator.AutomationProviders.Abstractions;
using TestCaseAutomator.ViewModels.Browser.Nodes;

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
		public FileSystemTestBrowserViewModel(ITestAutomationDiscoverer testDiscoverer)
            : this()
		{
			_testDiscoverer = testDiscoverer;
		}

	    private FileSystemTestBrowserViewModel()
	    {
	        _selectedFile = Property.New(this, p => p.SelectedFile)
	                                .UsingPathEquality();

            _tests = Property.New(this, p => p.Tests);

            _selectedTest = Property.New(this, p => p.SelectedTest);

            _canBrowse = Property.New(this, p => p.CanBrowse);
            CanBrowse = true;

            Tests = new ObservableCollection<TestAutomationNodeViewModel>();

            PropertyChanged += OnPropertyChanged;
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

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFile) && SelectedFile != null)
            {
                await DiscoverTestsAsync(new Progress<TestAutomationNodeViewModel>(test => Tests.Add(test)));
            }
        }

        private async Task DiscoverTestsAsync(IProgress<TestAutomationNodeViewModel> progress)
		{
			CanBrowse = false;
			try
			{
				(await _testDiscoverer.DiscoverAutomatedTestsAsync(SelectedFile.FullName.ToEnumerable()))
								      .Select(test => new TestAutomationNodeViewModel(test))
								      .Tee(progress.Report)
								      .ToList();
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
		private readonly Property<bool> _canBrowse;

		private readonly ITestAutomationDiscoverer _testDiscoverer;
	}
}