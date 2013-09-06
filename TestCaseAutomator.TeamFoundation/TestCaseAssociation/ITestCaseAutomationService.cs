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
	}
}