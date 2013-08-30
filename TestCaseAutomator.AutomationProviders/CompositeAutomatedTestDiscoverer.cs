using System.Collections.Generic;
using System.Linq;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.AutomationProviders
{
	/// <summary>
	/// An <see cref="IAutomatedTestDiscoverer"/> composed of all other discoverers.
	/// </summary>
	public class CompositeAutomatedTestDiscoverer : IAutomatedTestDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="CompositeAutomatedTestDiscoverer"/>.
		/// </summary>
		/// <param name="childDiscoverers">Any child test discoverers</param>
		public CompositeAutomatedTestDiscoverer(IEnumerable<IAutomatedTestDiscoverer> childDiscoverers)
		{
			_childDiscoverers = childDiscoverers;
		}

		/// <see cref="IAutomatedTestDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions
		{
			get { return _childDiscoverers.SelectMany(d => d.SupportedFileExtensions).Distinct(); }
		}

		/// <see cref="IAutomatedTestDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			return _childDiscoverers.SelectMany(d => d.DiscoverAutomatedTests(sources));
		}

		private readonly IEnumerable<IAutomatedTestDiscoverer> _childDiscoverers;
	}
}