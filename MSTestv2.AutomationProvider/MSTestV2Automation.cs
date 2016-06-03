using System;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace MSTestV2.AutomationProvider
{
	/// <summary>
	/// Represents an MSTest automated test.
	/// </summary>
	public class MSTestV2Automation : ITestAutomation
	{
		/// <summary>
		/// Initializes a new <see cref="MSTestV2Automation"/>.
		/// </summary>
		public MSTestV2Automation(string source, TestCase testCase)
		{
			Name = testCase.FullyQualifiedName;
			Identifier = testCase.Id;
		    Storage = Path.GetFileName(source);
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
	}
}