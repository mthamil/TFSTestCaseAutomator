using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpEssentials.Collections;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
{
    public class SourceDirectoryViewModel : VirtualizedNode<IVirtualizedNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDirectoryViewModel"/> class.
        /// </summary>
        /// <param name="directory">A source control directory</param>
        /// <param name="fileFactory">Creates file view-models</param>
        /// <param name="directoryFactory">Creates directory view models</param>
        public SourceDirectoryViewModel(TfsDirectory directory,
                                        Func<TfsFile, AutomationSourceViewModel> fileFactory,
                                        Func<TfsDirectory, SourceDirectoryViewModel> directoryFactory)
        {
            _directory = directory;
            _fileFactory = fileFactory;
            _directoryFactory = directoryFactory;
        }

        public override string Name => _directory.Name;

        protected override IVirtualizedNode DummyNode => Dummy.Instance;

        protected async override Task<IReadOnlyCollection<IVirtualizedNode>> LoadChildrenAsync(IProgress<IVirtualizedNode> progress)
        {
            return (await _directory.GetItemsAsync())
                                    .Select(item => item is TfsDirectory
                                                        ? _directoryFactory((TfsDirectory)item)
                                                        : _fileFactory(item as TfsFile))
                                    .Tee(progress.Report)
                                    .ToList();
        }

        private readonly TfsDirectory _directory;
        private readonly Func<TfsFile, IVirtualizedNode> _fileFactory;
        private readonly Func<TfsDirectory, IVirtualizedNode> _directoryFactory;

        private class Dummy : SourceDirectoryViewModel
        {
            private Dummy() : base(null, null, null) { }
            public override string Name => "Loading...";
            public static readonly Dummy Instance = new Dummy();
        }
    }
}