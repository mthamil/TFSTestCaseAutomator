using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.Utilities.Mvvm;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents a test case.
	/// </summary>
	public class TestCaseViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new <see cref="TestCaseViewModel"/>.
		/// </summary>
		/// <param name="testCase">The backing TFS test case</param>
		public TestCaseViewModel(ITestCase testCase)
		{
			_testCase = testCase;
		}

		/// <summary>
		/// A test case's work item ID.
		/// </summary>
		public int Id
		{
			get { return _testCase.Id; }
		}

		/// <summary>
		/// A test case's title.
		/// </summary>
		public string Title 
		{ 
			get { return _testCase.Title; } 
		}

		/// <summary>
		/// A test case's associated automation if any exists.
		/// </summary>
		public string AssociatedAutomation
		{
			get { return _testCase.IsAutomated ? _testCase.Implementation.DisplayText : string.Empty; }
		}

		private readonly ITestCase _testCase;
	}
}