using System;
using System.Collections.Generic;
using System.Linq;
using TestCaseAutomator.TeamFoundation;

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
		public override string Name
		{
			get { return _solution.Name; }
		}

		/// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override ProjectViewModel DummyNode
		{
			get { return _dummy; }
		}

		/// <see cref="VirtualizedNode{TChild}.LoadChildren"/>
		protected override IEnumerable<ProjectViewModel> LoadChildren()
		{
			return _solution.Projects().Select(p => _projectFactory(p));
		}

		private readonly TfsSolution _solution;
		private readonly Func<TfsSolutionProject, ProjectViewModel> _projectFactory;

		private static readonly DummyProject _dummy = new DummyProject();

		private class DummyProject : ProjectViewModel
		{
			public DummyProject() : base(null, null, new string[0]) { }
			public override string Name { get { return "..."; } }
		}
	}
}