using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Represents a test that can be associated with a test case.
	/// </summary>
	public class TestAutomationViewModel : NodeViewModel<object>
	{
		/// <summary>
		/// Initializes a new <see cref="TestAutomationViewModel"/>.
		/// </summary>
		/// <param name="testAutomation">An automated test</param>
		public TestAutomationViewModel(ITestAutomation testAutomation)
		{
			TestAutomation = testAutomation;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name
		{
			get { return TestAutomation.Name; }
		}

		/// <summary>
		/// The backing automated test.
		/// </summary>
		public ITestAutomation TestAutomation { get; private set; }
	}
}