using System;
using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
    /// <summary>
    /// Represents the root of TFS source control.
    /// </summary>
    public class TfsSourceTreeRoot : TfsDirectory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TfsSourceTreeRoot"/> class.
        /// </summary>
        /// <param name="versionControl">Provides access to TFS source control</param>
        public TfsSourceTreeRoot(IVersionControl versionControl)
            : base(new TfsRootItem(), versionControl) { }

        private class TfsRootItem : IVersionedItem
        {
            public string ServerItem => "$";

            public ItemType ItemType => ItemType.Folder;

            public Stream DownloadFile()
            {
                throw new NotSupportedException("Cannot download directory.");
            }
        }
    }
}