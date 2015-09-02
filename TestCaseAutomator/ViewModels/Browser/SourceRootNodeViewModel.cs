using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels.Browser
{
    /// <summary>
    /// Represents the visual root of the source control tree.
    /// </summary>
    public class SourceRootNodeViewModel : VirtualizedNode<SolutionViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceRootNodeViewModel"/> class.
        /// </summary>
        /// <param name="explorer">The current TFS explorer</param>
        /// <param name="solutionFactory">Creates solution view-models</param>
        /// <param name="scheduler">Used to schedule background tasks</param>
        public SourceRootNodeViewModel(ITfsExplorer explorer,
                                       Func<TfsSolution, SolutionViewModel> solutionFactory,
                                       TaskScheduler scheduler)
        {
            _explorer = explorer;
            _solutionFactory = solutionFactory;
            _scheduler = scheduler;
        }

        public override string Name => "$/";

        protected override SolutionViewModel DummyNode => DummySolution.Instance;

        protected override Task<IReadOnlyCollection<SolutionViewModel>> LoadChildrenAsync(IProgress<SolutionViewModel> progress)
        {
            return Task<IReadOnlyCollection<SolutionViewModel>>.Factory.StartNew(() =>
                _explorer.Solutions()
                         .Select(_solutionFactory)
                         .Tee(progress.Report)
                         .ToList(),
                    CancellationToken.None, TaskCreationOptions.None, _scheduler);
        }

        private readonly ITfsExplorer _explorer;
        private readonly Func<TfsSolution, SolutionViewModel> _solutionFactory;
        private readonly TaskScheduler _scheduler;

        private class DummySolution : SolutionViewModel
        {
            private DummySolution() : base(null, null, null) { }
            public override string Name => "Loading...";

            public static readonly DummySolution Instance = new DummySolution();
        }
    }
}