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
	    public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
            => _childDiscoverers
                    .Select(d => d.DiscoverAutomatedTestsAsync(sources))
                    .Aggregate(Task.FromResult(Enumerable.Empty<ITestAutomation>()), 
                            async (tests, current) => 
                                (await tests.ConfigureAwait(false)).Concat(
                                 await current.ConfigureAwait(false)));
             
	    private readonly IEnumerable<ITestAutomationDiscoverer> _childDiscoverers;
	}
}