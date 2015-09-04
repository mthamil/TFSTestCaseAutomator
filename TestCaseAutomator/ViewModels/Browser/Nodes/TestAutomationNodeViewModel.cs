using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
{
	/// <summary>
	/// Represents a tree node viewmodel that can be associated with test case automation.
	/// </summary>
	public class TestAutomationNodeViewModel : NodeViewModel<object>
	{
		/// <summary>
		/// Initializes a new <see cref="TestAutomationNodeViewModel"/>.
		/// </summary>
		/// <param name="testAutomation">An automated test</param>
		public TestAutomationNodeViewModel(ITestAutomation testAutomation)
		{
			TestAutomation = testAutomation;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name => TestAutomation.Name;

	    /// <summary>
		/// The backing automated test.
		/// </summary>
		public ITestAutomation TestAutomation { get; }
	}
}