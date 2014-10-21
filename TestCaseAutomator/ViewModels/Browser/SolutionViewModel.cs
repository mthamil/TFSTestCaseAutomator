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
	/// Represents a Visual Studio solution in source control.
	/// </summary>
	public class SolutionViewModel : VirtualizedNode<ProjectViewModel>
	{
		/// <summary>
		/// Initializes a new <see cref="SolutionViewModel"/>.
		/// </summary>
		/// <param name="solution">The actual solution object</param>
		/// <param name="projectFactory">Creates project view-models</param>
		/// <param name="scheduler">Used to schedule background tasks</param>
		public SolutionViewModel(TfsSolution solution, Func<TfsSolutionProject, ProjectViewModel> projectFactory,
		                         TaskScheduler scheduler)
		{
			_solution = solution;
			_projectFactory = projectFactory;
			_scheduler = scheduler;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name
		{
			get { return _solution.Name; }
		}

		/// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override ProjectViewModel DummyNode
		{
			get { return _dummy; }
		}

		/// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override Task<IReadOnlyCollection<ProjectViewModel>> LoadChildrenAsync(IProgress<ProjectViewModel> progress)
		{
			Invalidate();	// Reload on next query.
			return Task<IReadOnlyCollection<ProjectViewModel>>.Factory.StartNew(() =>
				_solution.Projects()
						.Select(p => _projectFactory(p))
						.Tee(progress.Report)
						.ToList(),
					CancellationToken.None, TaskCreationOptions.None, _scheduler);
		}

		private readonly TfsSolution _solution;
		private readonly Func<TfsSolutionProject, ProjectViewModel> _projectFactory;
		private readonly TaskScheduler _scheduler;

		private static readonly DummyProject _dummy = new DummyProject();

		private class DummyProject : ProjectViewModel
		{
			public DummyProject() : base(null, null, null) { }
			public override string Name { get { return "Loading..."; } }
		}
	}
}