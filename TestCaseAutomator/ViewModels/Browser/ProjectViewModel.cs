using System;
using System.Collections.Generic;
using System.Linq;
using TestCaseAutomator.TeamFoundation;

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
		/// <param name="fileExtensions">Used to filter out undesired files in source control</param>
		public ProjectViewModel(TfsSolutionProject project, Func<TfsFile, AutomationSourceViewModel> sourceFactory,
		                        IReadOnlyCollection<string> fileExtensions)
		{
			_project = project;
			_sourceFactory = sourceFactory;
			_fileExtensions = fileExtensions;
		}

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

		/// <see cref="VirtualizedNode{TChild}.LoadChildren"/>
		protected override IEnumerable<AutomationSourceViewModel> LoadChildren()
		{
			return _project.Files(_fileExtensions).Select(f => _sourceFactory(f));
		}

		private readonly TfsSolutionProject _project;
		private readonly Func<TfsFile, AutomationSourceViewModel> _sourceFactory;
		private readonly IReadOnlyCollection<string> _fileExtensions;

		private static readonly DummySource _dummy = new DummySource();

		private class DummySource : AutomationSourceViewModel
		{
			public DummySource() : base(null, null, null) { }
			public override string Name { get { return "..."; } }
		}
	}
}