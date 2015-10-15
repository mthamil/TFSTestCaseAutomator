using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace NUnit.AutomationProvider
{
    /// <summary>
    /// Finds NUnit tests for test case automation.
    /// </summary>
    [Export(typeof(ITestAutomationDiscoverer))]
    public class NUnitAutomationDiscoverer : ITestAutomationDiscoverer
    {
        /// <summary>
        /// Initializes a new <see cref="NUnitAutomationDiscoverer"/>.
        /// </summary>
        [ImportingConstructor]
        public NUnitAutomationDiscoverer()
        {
            
        }

        /// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
        public IEnumerable<string> SupportedFileExtensions => Extensions;

        /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
        public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
        {
            throw new System.NotImplementedException();
        }

        private static readonly ICollection<string> Extensions = new HashSet<string> { ".dll", ".exe" };
    }
}