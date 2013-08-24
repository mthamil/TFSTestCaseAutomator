using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.TeamFoundation.TestCaseAssociation
{
	/// <summary>
	/// Provides extension methods related to test case automation association.
	/// </summary>
	public static class TestCaseExtensions
	{
		/// <summary>
		/// Associates a test case with an automated test.
		/// </summary>
		public static void AssociateWithAutomation(this ITestCase testCase, IAutomatedTest automation)
		{
			// Create the associated automation.
			var implementation = testCase.Project.CreateTmiTestImplementation(
				automation.Name, automation.TestType, automation.Storage, automation.Identifier);

			// Save the test. If you are doing this for lots of test, you can consider
			// bulk saving too (outside of this method) for performance reason.
			testCase.WorkItem.Open();
			testCase.Implementation = implementation;
			testCase.Save();
		}
	}
}