using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="scheduler">Used to schedule background tasks</param>
        public VersionControlServerAdapter(VersionControlServer versionControl, TaskScheduler scheduler)
		{
			_versionControl = versionControl;
	        _scheduler = scheduler;

	        _versionControl.CommitCheckin += versionControl_CommitCheckin;
		}

        /// <see cref="IVersionControl.GetItemAsync"/>
        public Task<IVersionedItem> GetItemAsync(string path) 
            => Task.Factory.StartNew<IVersionedItem>(() =>
                    new VersionedItem(_versionControl.GetItem(path)), 
                CancellationToken.None, TaskCreationOptions.None, _scheduler);

        /// <see cref="IVersionControl.GetItemsAsync(string)"/>
        public Task<IReadOnlyList<IVersionedItem>> GetItemsAsync(string path) 
            => Task.Factory.StartNew<IReadOnlyList<IVersionedItem>>(() =>
                    _versionControl.GetItems(path)
                                   .Items
                                   .Select(i => new VersionedItem(i))
                                   .ToList<IVersionedItem>(), 
                CancellationToken.None, TaskCreationOptions.None, _scheduler);

	    /// <see cref="IVersionControl.GetItemsAsync(string,Microsoft.TeamFoundation.VersionControl.Client.RecursionType)"/>
		public Task<IReadOnlyList<IVersionedItem>> GetItemsAsync(string path, RecursionType recursion) 
            => Task.Factory.StartNew<IReadOnlyList<IVersionedItem>>(() =>
                    _versionControl.GetItems(path, recursion)
                                   .Items
                                   .Select(i => new VersionedItem(i))
                                   .ToList<IVersionedItem>(), 
                CancellationToken.None, TaskCreationOptions.None, _scheduler);

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
	    private readonly TaskScheduler _scheduler;
	}
}