using System.Collections.Generic;
using System.Linq;
using Moq;
using TestCaseAutomator.AutomationProviders;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.Utilities.Collections;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.AutomationProviders
{
	public class CompositeAutomatedTestDiscovererTests
	{
		public CompositeAutomatedTestDiscovererTests()
		{
			discoverer = new CompositeAutomatedTestDiscoverer(childDiscoverers);
		}

		[Fact]
		public void Test_SupportedFileExtensions()
		{
			// Arrange.
			childDiscoverers.AddRange(new[]
			{
				Mock.Of<IAutomatedTestDiscoverer>(d => d.SupportedFileExtensions == new[] { ".cs", ".ps1" }),
				Mock.Of<IAutomatedTestDiscoverer>(d => d.SupportedFileExtensions == new[] { ".js" }),
				Mock.Of<IAutomatedTestDiscoverer>(d => d.SupportedFileExtensions == new[] { ".ts", ".js" })
			});

			// Act.
			var extensions = discoverer.SupportedFileExtensions;

			// Assert.
			AssertThat.SequenceEqual(new[] { ".cs", ".ps1", ".js", ".ts" }, extensions);
		}

		[Fact]
		public void Test_DiscoverAutomatedTests()
		{
			// Arrange.
			childDiscoverers.AddRange(new[]
			{
				Mock.Of<IAutomatedTestDiscoverer>(d => d.DiscoverAutomatedTests(It.IsAny<IEnumerable<string>>()) == 
					Mock.Of<IAutomatedTest>(t => t.Name == "Test1").ToEnumerable()),
				Mock.Of<IAutomatedTestDiscoverer>(d => d.DiscoverAutomatedTests(It.IsAny<IEnumerable<string>>()) == 
					new[] { Mock.Of<IAutomatedTest>(t => t.Name == "Test2"), Mock.Of<IAutomatedTest>(t => t.Name == "Test3") }),
				Mock.Of<IAutomatedTestDiscoverer>(d => d.DiscoverAutomatedTests(It.IsAny<IEnumerable<string>>()) == 
					Mock.Of<IAutomatedTest>(t => t.Name == "Test4").ToEnumerable())
			});

			// Act.
			var tests = discoverer.DiscoverAutomatedTests(new[] { @"C:\sometests" }).ToList();

			// Assert.
			Assert.Equal(4, tests.Count);
			AssertThat.SequenceEqual(new[] { "Test1", "Test2", "Test3", "Test4" }, tests.Select(t => t.Name));
		}

		private readonly CompositeAutomatedTestDiscoverer discoverer;

		private readonly IList<IAutomatedTestDiscoverer> childDiscoverers = new List<IAutomatedTestDiscoverer>();
	}
}