using System;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace xUnit.AutomationProvider
{
	/// <summary>
	/// Represents an xUnit.net automated test.
	/// </summary>
	public class XUnitTestAutomation : ITestAutomation
	{
		/// <summary>
		/// Initializes a new <see cref="XUnitTestAutomation"/>.
		/// </summary>
		/// <param name="assemblyName">The file name of the assembly containing the test</param>
		/// <param name="typeName">The full name of the type containing the test</param>
		/// <param name="methodName">The method name of the test</param>
		public XUnitTestAutomation(string assemblyName, string typeName, string methodName)
		{
			Name = $"{typeName}.{methodName}";
			Identifier = IdentifierFactory.CreateIdentifier(Name);
			Storage = assemblyName;
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