﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Common;
using NUnit.Engine;
using NUnit.Engine.Internal;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace NUnit.AutomationProvider
{
    /// <summary>
    /// Finds NUnit tests for test case automation.
    /// </summary>
    [Export(typeof(ITestAutomationDiscoverer))]
    public class NUnitAutomationDiscoverer : ITestAutomationDiscoverer
    {
        private readonly Func<ITestEngine> _engineFactory;

        /// <summary>
        /// Initializes a new <see cref="NUnitAutomationDiscoverer"/>.
        /// </summary>
        [ImportingConstructor]
        public NUnitAutomationDiscoverer()
            : this(() => new TestEngine()) { }

        /// <summary>
        /// Initializes a new <see cref="NUnitAutomationDiscoverer"/>.
        /// </summary>
        public NUnitAutomationDiscoverer(Func<ITestEngine> engineFactory)
        {
            _engineFactory = engineFactory;
        }

        /// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
        public IEnumerable<string> SupportedFileExtensions => Extensions;

        /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
        public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
        {
            var package = new TestPackage(sources.Where(IsTestAssembly).ToList());
            package.AddSetting(PackageSettings.DisposeRunners, true);
            package.AddSetting(PackageSettings.ShadowCopyFiles, true);
            package.AddSetting(PackageSettings.ProcessModel, ProcessModel.Single.ToString());
            package.AddSetting(PackageSettings.DomainUsage, DomainUsage.None.ToString());

            var engine = _engineFactory();
            using (var runner = engine.GetRunner(package))
            {
                var result = runner.Explore(TestFilter.Empty);

                var tests = (
                    from assemblyNode in result.SelectNodes("//test-suite[@type='Assembly']").Cast<XmlNode>()
                    from testNode in assemblyNode.SelectNodes("//test-case").Cast<XmlNode>()
                    select new NUnitTestAutomation(testNode, assemblyNode.Attributes["fullname"].Value)).ToList<ITestAutomation>();

                return Task.FromResult<IEnumerable<ITestAutomation>>(tests);
            }
        }

        private static bool IsTestAssembly(string source)
        {
            return Extensions.Contains(Path.GetExtension(source));  // Quick check for .NET assembly file extensions.
        }

        private static readonly ICollection<string> Extensions = new HashSet<string> { ".dll", ".exe" };
    }
}