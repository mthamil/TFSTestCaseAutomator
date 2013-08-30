using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Collections;
using TestCaseAutomator.Utilities.InputOutput;

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
		/// <param name="scheduler">Used to schedule background tasks</param>
		public AutomationSourceViewModel(TfsFile file, Func<IAutomatedTest, AutomatedTestViewModel> testFactory,
		                                 IAutomatedTestDiscoverer testDiscoverer, TaskScheduler scheduler)
		{
			_file = file;
			_testFactory = testFactory;
			_testDiscoverer = testDiscoverer;
			_scheduler = scheduler;
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

		/// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override Task<IReadOnlyCollection<AutomatedTestViewModel>> LoadChildrenAsync(IProgress<AutomatedTestViewModel> progress)
		{
			Invalidate();	// Reload on next query.
			return Task.Factory.StartNew(() => 
				DiscoverTests(progress), 
					CancellationToken.None, 
					TaskCreationOptions.None, 
					_scheduler);
		}

		private IReadOnlyCollection<AutomatedTestViewModel> DiscoverTests(IProgress<AutomatedTestViewModel> progress)
		{
			var localPath = _file.ServerPath.Replace("$/", string.Empty).Replace('/', '\\');
			using (var tempFile = new TemporaryFile(localPath))
			{
				_file.DownloadTo(tempFile.File.FullName);
				return _testDiscoverer.DiscoverAutomatedTests(tempFile.File.FullName.ToEnumerable())
				                      .Select(t => _testFactory(t))
				                      .Tee(progress.Report)
				                      .ToList();
			}
		}

		private readonly TfsFile _file;
		private readonly Func<IAutomatedTest, AutomatedTestViewModel> _testFactory;
		private readonly IAutomatedTestDiscoverer _testDiscoverer;
		private readonly TaskScheduler _scheduler;

		private static readonly DummyAutomatedTest _dummy = new DummyAutomatedTest();

		private class DummyAutomatedTest : AutomatedTestViewModel
		{
			public DummyAutomatedTest() : base(null) { }
			public override string Name { get { return "Loading..."; } }
		}
	}
}