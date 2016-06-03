using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SharpEssentials.Collections;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace MSTestV2.AutomationProvider
{
	/// <summary>
	/// Finds MSTest tests for test case automation.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class MSTestV2AutomationDiscoverer : ITestAutomationDiscoverer
	{
		/// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions => Extensions;

	    /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
		public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
		{
			if (sources == null)
				throw new ArgumentNullException(nameof(sources));

	        var tests = sources
                .Where(source => Extensions.Contains(Path.GetExtension(source)) && File.Exists(source))
	            .SelectMany(GetTests, (source, testCase) => new MSTestV2Automation(source, testCase));

            return Task.FromResult<IEnumerable<ITestAutomation>>(tests);
		}

		private IEnumerable<TestCase> GetTests(string source)
		{
		    var pluginPath = Path.GetDirectoryName(GetType().Assembly.Location);
		    var sourcePath = Path.GetDirectoryName(source);
            var paths = new[] { pluginPath, sourcePath };

            using (new AssemblyResolver(paths))
            {
                var sink = new DiscoverySink();
                var discoverer = new MSTestDiscoverer();
                discoverer.DiscoverTests(source.ToEnumerable(), null, new NullLogger(), sink);
                return sink.TestCases;
            }
        }

        private static readonly ICollection<string> Extensions = typeof(MSTestDiscoverer)
                                                                    .GetCustomAttributes<FileExtensionAttribute>()
                                                                    .Select(fe => fe.FileExtension)
                                                                    .ToList();

        class DiscoverySink : ITestCaseDiscoverySink
        {
            public ICollection<TestCase> TestCases { get; } = new List<TestCase>();

            public void SendTestCase(TestCase discoveredTest) => TestCases.Add(discoveredTest);
        }

        class NullLogger : IMessageLogger
        {
            public void SendMessage(TestMessageLevel testMessageLevel, string message) { }
        }
    }
}