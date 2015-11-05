using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using SharpEssentials.Concurrency;
using TestCaseAutomator.AutomationProviders.Abstractions;

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
	    public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
            => _childDiscoverers
                    .Select(d => d.DiscoverAutomatedTestsAsync(sources))
                    .Aggregate(Tasks.Empty<ITestAutomation>(), 
                              (tests, current) => tests.Concat(current));
             
	    private readonly IEnumerable<ITestAutomationDiscoverer> _childDiscoverers;
	}
}