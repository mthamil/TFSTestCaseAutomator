using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCaseAutomator.AutomationProviders.Interfaces
{
	/// <summary>
	/// Interface for classes that provide automated tests suitable for test case automation.
	/// This is similar to Visual Studio's <c>ITestDiscoverer</c> interface.
	/// </summary>
	public interface ITestAutomationDiscoverer
	{
		/// <summary>
		/// Reports the file extensions supported by a test discoverer.
		/// </summary>
		IEnumerable<string> SupportedFileExtensions { get; }

		/// <summary>
		/// Retrieves a sequence of automated tests suitable for association with test cases.
		/// </summary>
		/// <param name="sources">A collection of potential test sources. These are usually file paths.</param>
		/// <returns>Any tests found in the given sources</returns>
		Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources);
	}
}