using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// An adapter for <see cref="VersionControlServer"/>.
	/// </summary>
	public class VersionControlServerAdapter : IVersionControl
	{
		/// <summary>
		/// Initializes a new <see cref="VersionControlServerAdapter"/>.
		/// </summary>
		/// <param name="versionControl">The wrapped version control instance</param>
		public VersionControlServerAdapter(VersionControlServer versionControl)
		{
			_versionControl = versionControl;
		}

		/// <see cref="IVersionControl.GetItem"/>
		public IVersionedItem GetItem(string path)
		{
			return new VersionedItem(_versionControl.GetItem(path));
		}

		/// <see cref="IVersionControl.GetItems(string)"/>
		public IReadOnlyList<IVersionedItem> GetItems(string path)
		{
			return _versionControl.GetItems(path).Items.Select(i => new VersionedItem(i)).ToList<IVersionedItem>();
		}

		/// <see cref="IVersionControl.GetItems(string,Microsoft.TeamFoundation.VersionControl.Client.RecursionType)"/>
		public IReadOnlyList<IVersionedItem> GetItems(string path, RecursionType recursion)
		{
			return _versionControl.GetItems(path, recursion).Items.Select(i => new VersionedItem(i)).ToList<IVersionedItem>();
		}

		private readonly VersionControlServer _versionControl;
	}
}