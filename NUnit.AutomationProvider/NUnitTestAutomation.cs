using System;
using System.IO;
using System.Xml;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace NUnit.AutomationProvider
{
    /// <summary>
	/// Represents an NUnit automated test.
	/// </summary>
    public class NUnitTestAutomation : ITestAutomation
    {
        /// <summary>
	    /// Initializes a new <see cref="NUnitTestAutomation"/>.
	    /// </summary>
	    public NUnitTestAutomation(XmlNode testNode, string file)
        {
            Name = testNode.Attributes["fullname"].Value;
            Identifier = IdentifierFactory.CreateIdentifier(Name);
            Storage = Path.GetFileName(file);
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