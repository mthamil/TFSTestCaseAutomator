using System;

namespace TestCaseAutomator.AutomationProviders.Abstractions
{
	/// <summary>
	/// Represents the components of an automated test case.
	/// </summary>
	public interface ITestAutomation
	{
		/// <summary>
		/// A test's unique identifier.
		/// </summary>
		Guid Identifier { get; }

		/// <summary>
		/// The test name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// A string representation of the test's type.
		/// </summary>
		string TestType { get; }

		/// <summary>
		/// A string representation of where a test is located. For example,
		/// this may be the path of a DLL or source file.
		/// </summary>
		string Storage { get; }
	}
}