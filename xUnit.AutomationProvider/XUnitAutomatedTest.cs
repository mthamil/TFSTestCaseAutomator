using System;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace xUnit.AutomationProvider
{
	/// <summary>
	/// Represents an xUnit.net automated test.
	/// </summary>
	public class XUnitAutomatedTest : IAutomatedTest
	{
		/// <summary>
		/// Initializes a new <see cref="XUnitAutomatedTest"/>.
		/// </summary>
		/// <param name="assemblyName">The file name of the assembly containing the test</param>
		/// <param name="typeName">The full name of the type containing the test</param>
		/// <param name="methodName">The method name of the test</param>
		public XUnitAutomatedTest(string assemblyName, string typeName, string methodName)
		{
			Name =  String.Format("{0}.{1}", typeName, methodName);
			Identifier = _identifierFactory.CreateIdentifier(Name);
			Storage = assemblyName;
			TestType = "Unit Test";
		}

		/// <see cref="IAutomatedTest.Identifier"/>
		public Guid Identifier { get; private set; }

		/// <see cref="IAutomatedTest.Name"/>
		public string Name { get; private set; }

		/// <see cref="IAutomatedTest.TestType"/>
		public string TestType { get; private set; }

		/// <see cref="IAutomatedTest.Storage"/>
		public string Storage { get; private set; }

		private static readonly ITestIdentifierFactory _identifierFactory = new HashedIdentifierFactory();
	}
}