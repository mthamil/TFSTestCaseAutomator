using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Common;
using NUnit.Engine;
using NUnit.Engine.Internal;
using TestCaseAutomator.AutomationProviders.Abstractions;

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
            var validSources = sources.Where(IsTestAssembly).ToList();
            if (validSources.Count == 0)
                return Task.FromResult(Enumerable.Empty<ITestAutomation>());

            var package = new TestPackage(validSources);
            package.AddSetting(PackageSettings.DisposeRunners, true);
            package.AddSetting(PackageSettings.ShadowCopyFiles, true);
            package.AddSetting(PackageSettings.ProcessModel, ProcessModel.Single.ToString());
            package.AddSetting(PackageSettings.DomainUsage, DomainUsage.None.ToString());

            var engine = _engineFactory();
            using (var runner = engine.GetRunner(package))
            {
                var result = runner.Explore(TestFilter.Empty);
                var xml = XElement.Load(result.CreateNavigator().ReadSubtree());

                var tests = (
                    from assembly in xml.DescendantsAndSelf("test-suite")
                    where assembly.Attribute("type").Value == "Assembly"
                    from test in assembly.Descendants("test-case")
                    select new NUnitTestAutomation(test, assembly)).ToList();

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