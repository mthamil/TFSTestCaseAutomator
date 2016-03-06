using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration;
using Microsoft.VisualStudio.TestPlatform.MSTestFramework;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using SharpEssentials;
using SharpEssentials.Collections;
using TestCaseAutomator.AutomationProviders.Abstractions;

namespace MSTest.AutomationProvider
{
	/// <summary>
	/// Finds MSTest tests for test case automation.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class MSTestAutomationDiscoverer : ITestAutomationDiscoverer
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
                .SelectMany(GetTests)
                .Select(testElement => new MSTestAutomation(testElement.TestMethod));

            return Task.FromResult<IEnumerable<ITestAutomation>>(tests);
		}

		private IEnumerable<UnitTestElement> GetTests(string source)
		{
		    var pluginPath = Path.GetDirectoryName(GetType().Assembly.Location);
		    var sourcePath = Path.GetDirectoryName(source);
            var paths = new[] { pluginPath, sourcePath };

            // Set up a separate app domain to avoid locking files.
            using (new AssemblyResolver(paths))
            using (var testDomain = Dispose.Of(AppDomain.CreateDomain($"{nameof(MSTestAutomationDiscoverer)}: {Path.GetFileName(source)}",
                                                                      AppDomain.CurrentDomain.Evidence,
                                                                      new AppDomainSetup
                                                                      {
                                                                          ApplicationBase = pluginPath,
                                                                          ShadowCopyFiles = "true",
                                                                          ShadowCopyDirectories = sourcePath,
                                                                          AppDomainInitializer = InitializeDomain,
                                                                          AppDomainInitializerArguments = paths,
                                                                          LoaderOptimization = LoaderOptimization.MultiDomain,
                                                                          DisallowBindingRedirects = false
                                                                          
                                                                      }), AppDomain.Unload))
            {
                var testSink = (ITestSink)testDomain.Value.CreateObject<TestSink>();
                testDomain.Value.SetData("source", source);
                testDomain.Value.SetData("testSink", testSink);
                testDomain.Value.DoCallBack(DiscoverTests);
                return testSink.Tests;
            }
		}

        private static void InitializeDomain(string[] args)
	    {
            AppDomain.CurrentDomain.SetData("resolver", new AssemblyResolver(args));
	    }

	    private static void DiscoverTests()
	    {
            var source = AppDomain.CurrentDomain.GetData<string>("source");
	        var testSink = AppDomain.CurrentDomain.GetData<ITestSink>("testSink");
            ICollection<string> warnings;
            new AssemblyEnumerator().EnumerateAssembly(source, out warnings)
                                    .ToSink(testSink.Tests);
	        AppDomain.CurrentDomain.GetData<AssemblyResolver>("resolver").Dispose();
	    }

        private static readonly ICollection<string> Extensions = typeof(MSTestDiscoverer)
                                                                    .GetCustomAttributes<FileExtensionAttribute>()
                                                                    .Select(fe => fe.FileExtension)
                                                                    .ToList();
	}
}