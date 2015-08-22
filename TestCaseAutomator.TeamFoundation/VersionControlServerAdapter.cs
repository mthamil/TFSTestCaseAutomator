using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using SharpEssentials;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// An adapter for <see cref="VersionControlServer"/>.
	/// </summary>
	public class VersionControlServerAdapter : DisposableBase, IVersionControl
	{
		/// <summary>
		/// Initializes a new <see cref="VersionControlServerAdapter"/>.
		/// </summary>
		/// <param name="versionControl">The wrapped version control instance</param>
		public VersionControlServerAdapter(VersionControlServer versionControl)
		{
			_versionControl = versionControl;

			_versionControl.CommitCheckin += versionControl_CommitCheckin;
		}

		/// <see cref="IVersionControl.GetItem"/>
		public IVersionedItem GetItem(string path) => new VersionedItem(_versionControl.GetItem(path));

	    /// <see cref="IVersionControl.GetItems(string)"/>
		public IReadOnlyList<IVersionedItem> GetItems(string path) 
            => _versionControl.GetItems(path)
                              .Items
                              .Select(i => new VersionedItem(i))
                              .ToList<IVersionedItem>();

	    /// <see cref="IVersionControl.GetItems(string,Microsoft.TeamFoundation.VersionControl.Client.RecursionType)"/>
		public IReadOnlyList<IVersionedItem> GetItems(string path, RecursionType recursion) 
            => _versionControl.GetItems(path, recursion)
                              .Items
                              .Select(i => new VersionedItem(i))
                              .ToList<IVersionedItem>();

	    /// <see cref="IVersionControl.ChangeCommitted"/>
		public event EventHandler<CommitCheckinEventArgs> ChangeCommitted;

		private void versionControl_CommitCheckin(object sender, CommitCheckinEventArgs e)
		{
            ChangeCommitted?.Invoke(this, e);
		}

		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing()
		{
			_versionControl.CommitCheckin -= versionControl_CommitCheckin;
		}

		private readonly VersionControlServer _versionControl;
	}
}