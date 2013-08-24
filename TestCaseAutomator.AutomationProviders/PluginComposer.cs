using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace TestCaseAutomator.AutomationProviders
{
	/// <summary>
	/// Class responsible for finding and composing all plugins.
	/// </summary>
	public class PluginComposer
	{
		/// <summary>
		/// The directory where plugins should be located.
		/// </summary>
		public DirectoryInfo PluginLocation { get; set; }

		/// <summary>
		/// Finds all test discovery plugins and composes them into the <see cref="RootDiscoverer"/>.
		/// </summary>
		public void Compose()
		{
			var rootCatalog = new AggregateCatalog();
			rootCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(GetType())));
			rootCatalog.Catalogs.Add(new DirectoryCatalog(PluginLocation.FullName));

			var container = new CompositionContainer(rootCatalog);
			container.ComposeParts(this);
		}

		/// <summary>
		/// The test discoverer containing all plugin provided discoverers.
		/// </summary>
		[Import]
		public CompositeAutomatedTestDiscoverer RootDiscoverer { get; set; }
	}
}