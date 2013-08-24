using System.Collections.Generic;

namespace TestCaseAutomator.AutomationProviders.Interfaces
{
	/// <summary>
	/// Interface for classes that provide automated tests suitable for test case automation.
	/// This is similar to Visual Studio's <c>ITestDiscoverer</c> interface.
	/// </summary>
	public interface IAutomatedTestDiscoverer
	{
		/// <summary>
		/// Retrieves a sequence of automated tests suitable for association with test cases.
		/// </summary>
		/// <param name="sources">A collection of potential test sources. These are usually file paths.</param>
		/// <returns>Any tests found in the given sources</returns>
		IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources);
	}
}