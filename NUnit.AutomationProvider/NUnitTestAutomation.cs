using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace NUnit.AutomationProvider
{
    /// <summary>
	/// Represents an NUnit automated test.
	/// </summary>
	[DebuggerDisplay("{Name}")]
    public class NUnitTestAutomation : ITestAutomation
    {
        /// <summary>
	    /// Initializes a new <see cref="NUnitTestAutomation"/>.
	    /// </summary>
	    public NUnitTestAutomation(XElement testNode, XElement containerNode)
        {
            Name = testNode.Attribute("fullname").Value;
            Identifier = IdentifierFactory.CreateIdentifier(Name);
            Storage = Path.GetFileName(containerNode.Attribute("fullname").Value);
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