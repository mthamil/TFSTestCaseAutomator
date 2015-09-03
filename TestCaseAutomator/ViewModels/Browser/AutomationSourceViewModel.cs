﻿using System;
using System.Collections.Generic;
using System.Linq;
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
		public AutomationSourceViewModel(TfsFile file,
		                                 ITestAutomationDiscoverer testDiscoverer)
		{
			_file = file;
			_testDiscoverer = testDiscoverer;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name => _file.Name;

	    /// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override TestAutomationNodeViewModel DummyNode => Dummy.Instance;

	    /// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override Task<IReadOnlyCollection<TestAutomationNodeViewModel>> LoadChildrenAsync(IProgress<TestAutomationNodeViewModel> progress)
		{
			Invalidate();	// Reload on next query.
	        return DiscoverTests(progress);
		}

		private async Task<IReadOnlyCollection<TestAutomationNodeViewModel>> DiscoverTests(IProgress<TestAutomationNodeViewModel> progress)
		{
			var localPath = _file.ServerPath.Replace("$/", string.Empty).Replace('/', '\\');
			using (var tempFile = new TemporaryFile(localPath))
			{
				await _file.DownloadToAsync(tempFile.File.FullName).ConfigureAwait(false);
				return (await _testDiscoverer.DiscoverAutomatedTestsAsync(tempFile.File.FullName.ToEnumerable()))
				                             .Select(t => new TestAutomationNodeViewModel(t))
				                             .Tee(progress.Report)
				                             .ToList();
			}
		}

		private readonly TfsFile _file;
		private readonly ITestAutomationDiscoverer _testDiscoverer;

		private class Dummy : TestAutomationNodeViewModel
		{
		    private Dummy() : base(null) { }
			public override string Name => "Loading...";

            public static readonly Dummy Instance = new Dummy();
		}
	}
}