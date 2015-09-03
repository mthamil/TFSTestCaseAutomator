using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestCaseAutomator.AutomationProviders.Interfaces;
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
            : this(source => new XunitFrontController(false, source, sourceInformationProvider: new NullSourceInformationProvider()))
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
                          .Select(src => FindAsync(_discovererFactory(src)))
                          .Aggregate(Task.FromResult(Enumerable.Empty<ITestAutomation>()), 
                                async (tests, current) => 
                                    (await tests.ConfigureAwait(false)).Concat(
                                     await current.ConfigureAwait(false)));
		}

        private static Task<IEnumerable<ITestAutomation>> FindAsync(ITestFrameworkDiscoverer discoverer)
        {
            var tcs = new TaskCompletionSource<IEnumerable<ITestAutomation>>();
            var tests = new List<ITestAutomation>();
            discoverer.Find(false, new DiscoveryMessageSink(message =>
            {
                tests.AddRange(
                    message.TestCases.Select(testCase =>
                        new XunitTestAutomation(testCase, message.TestAssembly)));
            }, () => tcs.SetResult(tests)), TestFrameworkOptions.ForDiscovery(new TestAssemblyConfiguration { UseAppDomain = false }));

            return tcs.Task;
        }

        private static bool IsTestAssembly(string source)
		{
		    return Extensions.Contains(Path.GetExtension(source));  // Quick check for .NET assembly file extensions.
		}

        private readonly Func<string, ITestFrameworkDiscoverer> _discovererFactory;

        private static readonly ICollection<string> Extensions = new HashSet<string> { ".dll", ".exe" };

	    private class DiscoveryMessageSink : LongLivedMarshalByRefObject, IMessageSink
	    {
	        public DiscoveryMessageSink(Action<ITestCaseDiscoveryMessage> testHandler, 
                                        Action completionHandler)
	        {
	            _testHandler = testHandler;
	            _completionHandler = completionHandler;
	        }

	        public bool OnMessage(IMessageSinkMessage message)
	        {
	            var testMessage = message as ITestCaseDiscoveryMessage;
	            if (testMessage != null)
	            {
	                _testHandler(testMessage);
	            }
	            else
	            {
	                var completionMessage = message as IDiscoveryCompleteMessage;
	                if (completionMessage != null)
	                {
	                    _completionHandler();
	                }
	            }

	            return true;
	        }

            private readonly Action<ITestCaseDiscoveryMessage> _testHandler;
	        private readonly Action _completionHandler;
	    }
    }
}