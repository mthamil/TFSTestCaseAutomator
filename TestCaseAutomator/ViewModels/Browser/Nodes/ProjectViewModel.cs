using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
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
		public ProjectViewModel(TfsSolutionProject project, 
                                Func<TfsFile, AutomationSourceViewModel> sourceFactory)
		{
			_project = project;
			_sourceFactory = sourceFactory;
		}

		/// <summary>
		/// Used to filter out undesired files in source control.
		/// </summary>
		public IReadOnlyCollection<string> FileExtensions { get; set; }

		/// <see cref="INodeViewModel.Name"/>
		public override string Name => _project.Name;

	    /// <see cref="VirtualizedNode{TChild}.DummyNode"/>
		protected override AutomationSourceViewModel DummyNode => Dummy.Instance;

	    /// <see cref="VirtualizedNode{TChild}.LoadChildrenAsync"/>
		protected override async Task<IReadOnlyCollection<AutomationSourceViewModel>> LoadChildrenAsync(IProgress<AutomationSourceViewModel> progress)
		{
			Invalidate();	// Reload on next query.
	        return (await _project.FilesAsync(FileExtensions))
	                              .Select(f => _sourceFactory(f))
	                              .Tee(progress.Report)
	                              .ToList();
		}

		private readonly TfsSolutionProject _project;
		private readonly Func<TfsFile, AutomationSourceViewModel> _sourceFactory;

		private class Dummy : AutomationSourceViewModel
		{
		    private Dummy() : base(null, null) { }
			public override string Name => "Loading...";

            public static readonly Dummy Instance = new Dummy();
        }
	}
}