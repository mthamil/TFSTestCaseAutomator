using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace MSTest.AutomationProvider
{
	/// <summary>
	/// Represents an MSTest automated test.
	/// </summary>
	public class MSTestAutomatedTest : IAutomatedTest
	{
		/// <summary>
		/// Initializes a new <see cref="MSTestAutomatedTest"/>.
		/// </summary>
		public MSTestAutomatedTest(TestMethod testMethod)
		{
			Name = String.Format("{0}.{1}", testMethod.FullClassName, testMethod.Name);
			Identifier = _identifierFactory.CreateIdentifier(Name);
			Storage = Path.GetFileName(testMethod.AssemblyName);
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