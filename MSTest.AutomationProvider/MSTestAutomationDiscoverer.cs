using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;
using TestCaseAutomator.AutomationProviders.Interfaces;

namespace MSTest.AutomationProvider
{
	/// <summary>
	/// Finds MSTest tests for test case automation.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class MSTestAutomationDiscoverer : ITestAutomationDiscoverer
	{
		/// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions => _extensions;

	    /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
		public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
		{
			if (sources == null)
				throw new ArgumentNullException(nameof(sources));

			var warnings = new List<string>();
			var tests = sources
				.Where(source => _extensions.Contains(Path.GetExtension(source)) && File.Exists(source))
				.SelectMany(source => GetTests(source, warnings))
				.Select(testElement => new MSTestAutomation(testElement.TestMethod));

			return Task.FromResult<IEnumerable<ITestAutomation>>(tests);
		}

		private static IEnumerable<UnitTestElement> GetTests(string source, List<string> warnings)
		{
			ICollection<string> assemblyWarnings;
			var tests = new AssemblyEnumerator().EnumerateAssembly(source, out assemblyWarnings);
			warnings.AddRange(assemblyWarnings);
			return tests;
		}

		private static readonly ICollection<string> _extensions = new HashSet<string> { ".dll", ".exe" };
	}
}