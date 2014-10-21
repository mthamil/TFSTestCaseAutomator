using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Collections;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Represents a project belonging to a solution.
	/// </summary>
	public class ProjectViewModel : VirtualizedNode<AutomationSourceViewModel>
	{
		/// <summary>
		/// Initializes a new <see cref="ProjectViewModel"/>.
		/// </summary>
		/// <param name="project">A project belonging to a solution</param>
		/// <param name="sourceFactory">Creates test source view-models</param>
		/// <param name="scheduler">Used to schedule background tasks</param>
		public ProjectViewModel(TfsSolutionProject project, Func<TfsFile, AutomationSourceViewModel> sourceFactory,
								TaskScheduler scheduler)
		{
			_project = project;
			_sourceFactory = sourceFactory;
			_scheduler = scheduler;
		}

		/// <summary>
		/// Used to filter out undesired files in source control.
		/// </summary>
		public IReadOnlyCollection<string> FileExtensions { get; set; }

		/// <see cref="INodeViewModel.Name"/>
		public override string Name
		{
			get { return _project.Name; }
		}

		/// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override AutomationSourceViewModel DummyNode
		{
			get { return _dummy; }
		}

		/// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override Task<IReadOnlyCollection<AutomationSourceViewModel>> LoadChildrenAsync(IProgress<AutomationSourceViewModel> progress)
		{
			Invalidate();	// Reload on next query.
			return Task<IReadOnlyCollection<AutomationSourceViewModel>>.Factory.StartNew(() =>
				_project.Files(FileExtensions)
						.Select(f => _sourceFactory(f))
						.Tee(progress.Report)
						.ToList(),
					CancellationToken.None,
					TaskCreationOptions.None,
					_scheduler);
		}

		private readonly TfsSolutionProject _project;
		private readonly Func<TfsFile, AutomationSourceViewModel> _sourceFactory;
		
		private readonly TaskScheduler _scheduler;

		private static readonly DummySource _dummy = new DummySource();

		private class DummySource : AutomationSourceViewModel
		{
			public DummySource() : base(null, null, null, null) { }
			public override string Name { get { return "Loading..."; } }
		}
	}
}