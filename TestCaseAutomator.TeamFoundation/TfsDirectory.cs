using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
    /// <summary>
    /// Represents a directory in TFS source control.
    /// </summary>
    public class TfsDirectory : TfsSourceControlledItem
    {
        /// <summary>
        /// Initializes a new <see cref="TfsDirectory"/>.
        /// </summary>
        /// <param name="item">The source controlled file</param>
        /// <param name="versionControl">TFS source control</param>
        public TfsDirectory(IVersionedItem item, IVersionControl versionControl) 
            : base(item, versionControl)
        {
        }

        /// <summary>
        /// Retrieves the contents of a directory.
        /// </summary>
        public async Task<IEnumerable<TfsSourceControlledItem>> GetItemsAsync()
        {
            return (await VersionControl.GetItemsAsync($"{Item.ServerItem}/*", RecursionType.None).ConfigureAwait(false))
                                        .Select(item => item.ItemType == ItemType.Folder
                                                            ? new TfsDirectory(item, VersionControl)
                                                            : (TfsSourceControlledItem)new TfsFile(item, VersionControl));
        }
    }
}