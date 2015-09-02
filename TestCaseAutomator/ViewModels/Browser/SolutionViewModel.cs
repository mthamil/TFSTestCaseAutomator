using System;
using System.Collections.Generic;
using System.Linq;
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
		public SolutionViewModel(TfsSolution solution, 
                                 Func<TfsSolutionProject, ProjectViewModel> projectFactory)
		{
			_solution = solution;
			_projectFactory = projectFactory;
		}

		/// <see cref="INodeViewModel.Name"/>
		public override string Name => _solution.Name;

	    /// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override ProjectViewModel DummyNode => DummyProject.Instance;

	    /// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override async Task<IReadOnlyCollection<ProjectViewModel>> LoadChildrenAsync(IProgress<ProjectViewModel> progress)
		{
			Invalidate();	// Reload on next query.
			return (await _solution.ProjectsAsync())
						           .Select(p => _projectFactory(p))
						           .Tee(progress.Report)
						           .ToList();
		}

		private readonly TfsSolution _solution;
		private readonly Func<TfsSolutionProject, ProjectViewModel> _projectFactory;

		private class DummyProject : ProjectViewModel
		{
		    private DummyProject() : base(null, null) { }
			public override string Name => "Loading...";

            public static readonly DummyProject Instance = new DummyProject();
        }
	}
}