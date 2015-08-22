using System.Collections.Generic;
using System.Linq;
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

	    /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<ITestAutomation> DiscoverAutomatedTests(IEnumerable<string> sources) 
            => _childDiscoverers.SelectMany(d => d.DiscoverAutomatedTests(sources));

	    private readonly IEnumerable<ITestAutomationDiscoverer> _childDiscoverers;
	}
}