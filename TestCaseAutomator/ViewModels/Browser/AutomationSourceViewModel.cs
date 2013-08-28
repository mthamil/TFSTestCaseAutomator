using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Collections;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Represents a file that is a potential source of automated tests.
	/// </summary>
	public class AutomationSourceViewModel : VirtualizedNode<AutomatedTestViewModel>
	{
		/// <summary>
		/// Initializes a new <see cref="AutomationSourceViewModel"/>.
		/// </summary>
		/// <param name="file">A file that may contain automated tests</param>
		/// <param name="testFactory">Creates automated test view-models</param>
		/// <param name="testDiscoverer">Finds tests in files</param>
		public AutomationSourceViewModel(TfsFile file, Func<IAutomatedTest, AutomatedTestViewModel> testFactory,
		                                 IAutomatedTestDiscoverer testDiscoverer)
		{
			_file = file;
			_testFactory = testFactory;
			_testDiscoverer = testDiscoverer;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name
		{
			get { return _file.Name; }
		}

		/// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override AutomatedTestViewModel DummyNode
		{
			get { return _dummy; }
		}

		/// <see cref="VirtualizedNode{TChild}.LoadChildren"/>
		protected override IEnumerable<AutomatedTestViewModel> LoadChildren()
		{
			var localPath = _file.ServerPath.Replace("$/", string.Empty).Replace('/', '\\');
			var tempDir = Path.GetTempPath();
			var tempFile = Path.Combine(tempDir, localPath);
			_file.DownloadTo(tempFile);
			return _testDiscoverer.DiscoverAutomatedTests(tempFile.ToEnumerable()).Select(t => _testFactory(t));
		}

		private readonly TfsFile _file;
		private readonly Func<IAutomatedTest, AutomatedTestViewModel> _testFactory;
		private readonly IAutomatedTestDiscoverer _testDiscoverer;

		private static readonly DummyAutomatedTest _dummy = new DummyAutomatedTest();

		private class DummyAutomatedTest : AutomatedTestViewModel
		{
			public DummyAutomatedTest() : base(null) { }
			public override string Name { get { return "..."; } }
		}
	}
}