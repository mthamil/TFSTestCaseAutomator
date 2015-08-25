using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Collections;
using SharpEssentials.InputOutput;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Represents a file that is a potential source of automated tests.
	/// </summary>
	public class AutomationSourceViewModel : VirtualizedNode<TestAutomationNodeViewModel>
	{
		/// <summary>
		/// Initializes a new <see cref="AutomationSourceViewModel"/>.
		/// </summary>
		/// <param name="file">A file that may contain automated tests</param>
		/// <param name="testDiscoverer">Finds tests in files</param>
		/// <param name="scheduler">Used to schedule background tasks</param>
		public AutomationSourceViewModel(TfsFile file,
		                                 ITestAutomationDiscoverer testDiscoverer, 
                                         TaskScheduler scheduler)
		{
			_file = file;
			_testDiscoverer = testDiscoverer;
			_scheduler = scheduler;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name => _file.Name;

	    /// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override TestAutomationNodeViewModel DummyNode => Dummy;

	    /// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override Task<IReadOnlyCollection<TestAutomationNodeViewModel>> LoadChildrenAsync(IProgress<TestAutomationNodeViewModel> progress)
		{
			Invalidate();	// Reload on next query.
			return Task.Factory.StartNew(() => 
				DiscoverTests(progress), 
					CancellationToken.None, 
					TaskCreationOptions.None, 
					_scheduler);
		}

		private IReadOnlyCollection<TestAutomationNodeViewModel> DiscoverTests(IProgress<TestAutomationNodeViewModel> progress)
		{
			var localPath = _file.ServerPath.Replace("$/", string.Empty).Replace('/', '\\');
			using (var tempFile = new TemporaryFile(localPath))
			{
				_file.DownloadTo(tempFile.File.FullName);
				return _testDiscoverer.DiscoverAutomatedTests(tempFile.File.FullName.ToEnumerable())
				                      .Select(t => new TestAutomationNodeViewModel(t))
				                      .Tee(progress.Report)
				                      .ToList();
			}
		}

		private readonly TfsFile _file;
		private readonly ITestAutomationDiscoverer _testDiscoverer;
		private readonly TaskScheduler _scheduler;

		private static readonly DummyTestAutomationNode Dummy = new DummyTestAutomationNode();

		private class DummyTestAutomationNode : TestAutomationNodeViewModel
		{
			public DummyTestAutomationNode() : base(null) { }
			public override string Name => "Loading...";
		}
	}
}