using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace MSTest.AutomationProvider
{
	/// <summary>
	/// Represents an MSTest automated test.
	/// </summary>
	public class MSTestAutomation : ITestAutomation
	{
		/// <summary>
		/// Initializes a new <see cref="MSTestAutomation"/>.
		/// </summary>
		public MSTestAutomation(TestMethod testMethod)
		{
			Name = String.Format("{0}.{1}", testMethod.FullClassName, testMethod.Name);
			Identifier = _identifierFactory.CreateIdentifier(Name);
			Storage = Path.GetFileName(testMethod.AssemblyName);
			TestType = "Unit Test";
		}

		/// <see cref="ITestAutomation.Identifier"/>
		public Guid Identifier { get; private set; }

		/// <see cref="ITestAutomation.Name"/>
		public string Name { get; private set; }

		/// <see cref="ITestAutomation.TestType"/>
		public string TestType { get; private set; }

		/// <see cref="ITestAutomation.Storage"/>
		public string Storage { get; private set; }

		private static readonly ITestIdentifierFactory _identifierFactory = new HashedIdentifierFactory();
	}
}