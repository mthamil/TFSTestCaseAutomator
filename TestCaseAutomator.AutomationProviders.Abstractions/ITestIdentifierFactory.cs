using System;

namespace TestCaseAutomator.AutomationProviders.Abstractions
{
	/// <summary>
	/// Interface for an object thaat creates automated test identifiers.
	/// </summary>
	public interface ITestIdentifierFactory
	{
		/// <summary>
		/// Creates a test identifier from the given input.
		/// </summary>
		/// <param name="identifyingInput">A string representing some aspect of a test's identity, such as its fully qualified name</param>
		/// <returns>A test identifier</returns>
		Guid CreateIdentifier(string identifyingInput);
	}
}