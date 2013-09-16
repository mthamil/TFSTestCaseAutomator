﻿using System;
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
	[Export(typeof(IAutomatedTestDiscoverer))]
	public class XUnitAutomatedTestDiscoverer : IAutomatedTestDiscoverer
	{
		/// <summary>
		/// Initializes a new <see cref="XUnitAutomatedTestDiscoverer"/>.
		/// </summary>
		[ImportingConstructor]
		public XUnitAutomatedTestDiscoverer()
			: this(source => new ExecutorWrapper(source, null, true))
		{
		}

		/// <summary>
		/// Initializes a new <see cref="XUnitAutomatedTestDiscoverer"/>.
		/// </summary>
		/// <param name="discovererFactory">Creates objects that discover tests in assembly files</param>
		public XUnitAutomatedTestDiscoverer(Func<string, IExecutorWrapper> discovererFactory)
		{
			_discovererFactory = discovererFactory;
		}

		/// <see cref="IAutomatedTestDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions { get { return _extensions; } }

		/// <see cref="IAutomatedTestDiscoverer.DiscoverAutomatedTests"/>
		public IEnumerable<IAutomatedTest> DiscoverAutomatedTests(IEnumerable<string> sources)
		{
			if (sources == null)
				throw new ArgumentNullException("sources");

			foreach (string source in sources.Where(s => _extensions.Contains(Path.GetExtension(s))))
			{
				using (var executor = _discovererFactory(source))
				{
					foreach (var test in GetAutomatedTests(executor))
						yield return test;
				}
			}
		}

		private static IEnumerable<IAutomatedTest> GetAutomatedTests(IExecutorWrapper executor)
		{
			string assemblyFilename = executor.AssemblyFilename;
			return executor.EnumerateTests().SelectNodes("//method")
						   .Cast<XmlNode>()
			               .Select(methodNode =>
							   new XUnitAutomatedTest(
									Path.GetFileName(assemblyFilename), 
									methodNode.Attributes["type"].Value, 
									methodNode.Attributes["method"].Value));

		}

		private readonly Func<string, IExecutorWrapper> _discovererFactory;

		private static readonly ICollection<string> _extensions = new HashSet<string> { ".dll", ".exe" }; 
	}
}