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
			Name = $"{testMethod.FullClassName}.{testMethod.Name}";
			Identifier = IdentifierFactory.CreateIdentifier(Name);
			Storage = Path.GetFileName(testMethod.AssemblyName);
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