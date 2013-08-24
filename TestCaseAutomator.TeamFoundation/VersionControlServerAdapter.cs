using System.Collections.Generic;
using System.IO;
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
		public Item GetItem(string path)
		{
			return _versionControl.GetItem(path);
		}

		/// <see cref="IVersionControl.GetItems(string)"/>
		public IReadOnlyList<Item> GetItems(string path)
		{
			return _versionControl.GetItems(path).Items;
		}

		/// <see cref="IVersionControl.GetItems(string,Microsoft.TeamFoundation.VersionControl.Client.RecursionType)"/>
		public IReadOnlyList<Item> GetItems(string path, RecursionType recursion)
		{
			return _versionControl.GetItems(path, recursion).Items;
		}

		/// <see cref="IVersionControl.DownloadFile"/>
		public Stream DownloadFile(Item item)
		{
			return _versionControl.DownloadFileByUrl(item.DownloadUrl);
		}

		private readonly VersionControlServer _versionControl;
	}
}