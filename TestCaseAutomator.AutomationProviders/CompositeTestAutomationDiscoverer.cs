using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.AutomationProviders
{
	/// <summary>
	/// An <see cref="ITestAutomationDiscoverer"/> composed of all other discoverers.
	/// </summary>
	public class CompositeTestAutomationDiscoverer : ITestAutomationDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="CompositeTestAutomationDiscoverer"/>.
		/// </summary>
		/// <param name="childDiscoverers">Any child test discoverers</param>
		public CompositeTestAutomationDiscoverer(IEnumerable<ITestAutomationDiscoverer> childDiscoverers)
		{
			_childDiscoverers = childDiscoverers;
		}

		/// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions
            => _childDiscoverers.SelectMany(d => d.SupportedFileExtensions).Distinct();

	    /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
	    public async Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
	    {
	        var tests = Enumerable.Empty<ITestAutomation>();
	        foreach (var discoverer in _childDiscoverers)
	            tests = tests.Concat(await discoverer.DiscoverAutomatedTestsAsync(sources).ConfigureAwait(false));
	        return tests;
	    }
             
	    private readonly IEnumerable<ITestAutomationDiscoverer> _childDiscoverers;
	}
}