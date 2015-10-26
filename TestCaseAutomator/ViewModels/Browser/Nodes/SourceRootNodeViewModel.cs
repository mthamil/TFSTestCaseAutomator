using System;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
{
    /// <summary>
    /// Represents the visual root of the source control tree.
    /// </summary>
    public class SourceRootNodeViewModel : SourceDirectoryViewModel
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
            : base(explorer.GetSourceTreeRoot(), fileFactory, directoryFactory) { }

        public override string Name => "$/";
    }
}