using System.Collections.Generic;
using System.ComponentModel.Composition;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.AutomationProviders
{
	/// <summary>
	/// An <see cref="IAutomatedTestDiscoverer"/> composed of all other discoverers.
	/// </summary>
	[Export(typeof(CompositeAutomatedTestDiscoverer))]
	public class CompositeAutomatedTestDiscoverer : IAutomatedTestDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="CompositeAutomatedTestDiscoverer"/>.
		/// </summary>
		/// <param name="childDiscoverers">Any child test discoverers</param>
		[ImportingConstructor]
		public CompositeAutomatedTestDiscoverer([ImportMany]IEnumerable<IAutomatedTestDiscoverer> childDiscoverers)
		{
			_childDiscoverers = childDiscoverers;
		}

		/// <see cref="IAutomatedTestDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			throw new System.NotImplementedException();
		}

		private readonly IEnumerable<IAutomatedTestDiscoverer> _childDiscoverers;
	}
}