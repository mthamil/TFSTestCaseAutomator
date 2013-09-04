using System;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Event args for when an automated test is selected.
	/// </summary>
	public class AutomatedTestSelectedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new <see cref="AutomatedTestSelectedEventArgs"/>.
		/// </summary>
		/// <param name="testCase">The test case associated with the automated test</param>
		/// <param name="automatedTest">The selected automated test</param>
		public AutomatedTestSelectedEventArgs(TestCaseViewModel testCase, AutomatedTestViewModel automatedTest)
		{
			TestCase = testCase;
			AutomatedTest = automatedTest;
		}

		/// <summary>
		/// The test case associated with the automated test.
		/// </summary>
		public TestCaseViewModel TestCase { get; private set; }

		/// <summary>
		/// The selected automated test.
		/// </summary>
		public AutomatedTestViewModel AutomatedTest { get; private set; }
	}
}