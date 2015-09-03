using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels.Browser
{
    /// <summary>
    /// Represents the visual root of the source control tree.
    /// </summary>
    public class SourceRootNodeViewModel : VirtualizedNode<IVirtualizedNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceRootNodeViewModel"/> class.
        /// </summary>
        /// <param name="explorer">The current TFS explorer</param>
        /// <param name="fileFactory">Creates file view-models</param>
        /// <param name="directoryFactory">Creates directory view models</param>
        public SourceRootNodeViewModel(ITfsExplorer explorer,
                                       Func<TfsFile, AutomationSourceViewModel> fileFactory,
                                       Func<TfsDirectory, SourceDirectoryViewModel> directoryFactory)
        {
            _explorer = explorer;
            _fileFactory = fileFactory;
            _directoryFactory = directoryFactory;
        }

        public override string Name => "$/";

        protected override IVirtualizedNode DummyNode => Dummy.Instance;

        protected override async Task<IReadOnlyCollection<IVirtualizedNode>> LoadChildrenAsync(IProgress<IVirtualizedNode> progress)
        {
            return (await _explorer.GetSourceTreeAsync())
                                   .Select(item => item is TfsDirectory 
                                                       ? _directoryFactory((TfsDirectory)item) 
                                                       : (IVirtualizedNode)_fileFactory(item as TfsFile))
                                   .Tee(progress.Report)
                                   .ToList();
        }

        private readonly ITfsExplorer _explorer;
        private readonly Func<TfsFile, AutomationSourceViewModel> _fileFactory;
        private readonly Func<TfsDirectory, SourceDirectoryViewModel> _directoryFactory;

        private class Dummy : SourceRootNodeViewModel
        {
            private Dummy() : base(null, null, null) { }

            public override string Name => "Loading...";

            public static readonly Dummy Instance = new Dummy();
        }
    }
}