using Microsoft.TeamFoundation.TestManagement.Client;
using TestCaseAutomator.AutomationProviders.Interfaces;

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
		void AssociateWithAutomation(ITestCase testCase, IAutomatedTest automation);

		/// <summary>
		/// Removes automation from a test case.
		/// </summary>
		void RemoveAutomation(ITestCase testCase);

		/// <summary>
		/// Returns an <see cref="IAutomatedTest"/> representing a <see cref="ITestCase"/>'s
		/// existing automation if it has any.
		/// </summary>
		IAutomatedTest GetExistingAutomation(ITestCase testCase);
	}
}