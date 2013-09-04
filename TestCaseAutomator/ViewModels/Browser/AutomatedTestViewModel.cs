using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Represents a test that can be associated with a test case.
	/// </summary>
	public class AutomatedTestViewModel : NodeViewModel<object>
	{
		/// <summary>
		/// Initializes a new <see cref="AutomatedTestViewModel"/>.
		/// </summary>
		/// <param name="automatedTest">An automated test</param>
		public AutomatedTestViewModel(IAutomatedTest automatedTest)
		{
			AutomatedTest = automatedTest;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name
		{
			get { return AutomatedTest.Name; }
		}

		/// <summary>
		/// The backing automated test.
		/// </summary>
		public IAutomatedTest AutomatedTest { get; private set; }
	}
}