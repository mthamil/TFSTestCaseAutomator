using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using SharpEssentials.Concurrency;
using TestCaseAutomator.AutomationProviders.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace xUnit.AutomationProvider
{
	/// <summary>
	/// Finds xUnit.net tests for test case automation.
	/// </summary>
	[Export(typeof(ITestAutomationDiscoverer))]
	public class XunitTestAutomationDiscoverer : ITestAutomationDiscoverer
	{
	    /// <summary>
		/// Initializes a new <see cref="XunitTestAutomationDiscoverer"/>.
		/// </summary>
		[ImportingConstructor]
		public XunitTestAutomationDiscoverer()
            : this(source => new XunitFrontController(AppDomainSupport.IfAvailable, source, sourceInformationProvider: new NullSourceInformationProvider()))
		{
		}

        /// <summary>
        /// Initializes a new <see cref="XunitTestAutomationDiscoverer"/>.
        /// </summary>
        public XunitTestAutomationDiscoverer(Func<string, ITestFrameworkDiscoverer> discovererFactory)
	    {
	        _discovererFactory = discovererFactory;
	    }

	    /// <see cref="ITestAutomationDiscoverer.SupportedFileExtensions"/>
		public IEnumerable<string> SupportedFileExtensions => Extensions;

	    /// <see cref="ITestAutomationDiscoverer.DiscoverAutomatedTestsAsync"/>
		public Task<IEnumerable<ITestAutomation>> DiscoverAutomatedTestsAsync(IEnumerable<string> sources)
		{
			if (sources == null)
				throw new ArgumentNullException(nameof(sources));

	        return sources.Where(IsTestAssembly)
                          .Select(src => FindAsync(() => _discovererFactory(src)))
                          .Aggregate(Tasks.Empty<ITestAutomation>(), 
                                    (tests, current) => tests.Concat(current));
		}

        private static async Task<IEnumerable<ITestAutomation>> FindAsync(Func<ITestFrameworkDiscoverer> discovererProvider)
        {
            var tests = new List<ITestAutomation>();

            using (var discoverer = discovererProvider())
            using (AssemblyHelper.SubscribeResolve())
            using (var sink = new DiscoveryMessageSink(message =>
                                    message.TestCases.Select(testCase => new XunitTestAutomation(testCase, message.TestAssembly))
                                                     .AddTo(tests)))
            {
                try
                {
                    discoverer.Find(false, sink, TestFrameworkOptions.ForDiscovery(new TestAssemblyConfiguration { AppDomain = AppDomainOption }));
                    await sink.Finished.AsTask();
                }
                catch (InvalidOperationException e)
                {
                    Debug.Write(e);
                }
            }

            return tests;
        }

        private static bool IsTestAssembly(string source)
		{
		    return Extensions.Contains(Path.GetExtension(source));  // Quick check for .NET assembly file extensions.
		}

        private readonly Func<string, ITestFrameworkDiscoverer> _discovererFactory;

	    private const AppDomainSupport AppDomainOption = AppDomainSupport.Denied;
	    private static readonly ICollection<string> Extensions = new HashSet<string> { ".dll", ".exe" };

	    private class DiscoveryMessageSink : TestMessageVisitor<IDiscoveryCompleteMessage>
	    {
	        public DiscoveryMessageSink(Action<ITestCaseDiscoveryMessage> testHandler)
	        {
	            _testHandler = testHandler;
	        }

	        protected override bool Visit(ITestCaseDiscoveryMessage testCaseDiscovered)
	        {
	            _testHandler(testCaseDiscovered);
	            return true;
	        }

            private readonly Action<ITestCaseDiscoveryMessage> _testHandler;
	    }
    }
}