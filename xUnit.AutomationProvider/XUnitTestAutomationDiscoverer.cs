using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using TestCaseAutomator.AutomationProviders.Interfaces;
using Xunit;

namespace xUnit.AutomationProvider
{
	/// <summary>
	/// Finds xUnit.net tests for test case automation.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class XUnitTestAutomationDiscoverer : ITestAutomationDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="XUnitTestAutomationDiscoverer"/>.
		/// </summary>
		[ImportingConstructor]
		public XUnitTestAutomationDiscoverer()
			: this(source => new ExecutorWrapper(source, null, true))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="XUnitTestAutomationDiscoverer"/>.
		/// </summary>
		/// <param name="discovererFactory">Creates objects that discover tests in assembly files</param>
		public XUnitTestAutomationDiscoverer(Func<string, IExecutorWrapper> discovererFactory)
		{
			_discovererFactory = discovererFactory;
		}

		/// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions { get { return _extensions; } }

		/// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<ITestAutomation> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			if (sources == null)
				throw new ArgumentNullException("sources");

			return sources.Where(IsTestAssembly)
						  .SelectMany(source =>
						  {
							  using (var executor = _discovererFactory(source))
								  return executor.EnumerateTests().SelectNodes("//method")
								                 .Cast<XmlNode>()
								                 .Select(methodNode =>
								                         new XUnitTestAutomation(
									                         Path.GetFileName(source),
									                         methodNode.Attributes["type"].Value,
									                         methodNode.Attributes["method"].Value));
						  });
		}

		private static bool IsTestAssembly(string source)
		{
			return _extensions.Contains(Path.GetExtension(source)) &&						// Easy check for .NET assembly file extensions.
			       File.Exists(Path.Combine(Path.GetDirectoryName(source), "xunit.dll"));	// Ensure there is an xunit.dll in the same directory, otherwise an exception will be thrown.
		}

		private readonly Func<string, IExecutorWrapper> _discovererFactory;

		private static readonly ICollection<string> _extensions = new HashSet<string> { ".dll", ".exe" }; 
	}
}