using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace TestCaseAutomator.TeamFoundation.TestCaseAssociation
{
	/// <summary>
	/// Provides operations related to test case automation association.
	/// </summary>
	public interface ITestCaseAutomationService
	{
		/// <summary>
		/// Associates a test case with an automated test.
		/// </summary>
		void AssociateWithAutomation(ITestCase testCase, ITestAutomation automation);

		/// <summary>
		/// Removes automation from a test case.
		/// </summary>
		void RemoveAutomation(ITestCase testCase);

		/// <summary>
		/// Returns an <see cref="ITestAutomation"/> representing a <see cref="ITestCase"/>'s
		/// existing automation if it has any.
		/// </summary>
		ITestAutomation GetExistingAutomation(ITestCase testCase);
	}
}