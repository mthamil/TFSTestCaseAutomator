using System;
using System.IO;
using TestCaseAutomator.AutomationProviders.Abstractions;
using Xunit.Abstractions;

namespace xUnit.AutomationProvider
{
	/// <summary>
	/// Represents an xUnit.net automated test.
	/// </summary>
	public class XunitTestAutomation : ITestAutomation
	{
	    /// <summary>
	    /// Initializes a new <see cref="XunitTestAutomation"/>.
	    /// </summary>
	    /// <param name="testCase">An xUnit test case</param>
	    /// <param name="testAssembly">The assembly containing the test case</param>
	    public XunitTestAutomation(ITestCase testCase, ITestAssembly testAssembly)
		{
			Name = testCase.DisplayName;
			Identifier = IdentifierFactory.CreateIdentifier(Name);
			Storage = Path.GetFileName(testAssembly.Assembly.AssemblyPath);
			TestType = "Unit Test";
		}

		/// <see cref="ITestAutomation.Identifier"/>
		public Guid Identifier { get; }

		/// <see cref="ITestAutomation.Name"/>
		public string Name { get; }

		/// <see cref="ITestAutomation.TestType"/>
		public string TestType { get; }

		/// <see cref="ITestAutomation.Storage"/>
		public string Storage { get; }

		private static readonly ITestIdentifierFactory IdentifierFactory = new HashedIdentifierFactory();
	}
}