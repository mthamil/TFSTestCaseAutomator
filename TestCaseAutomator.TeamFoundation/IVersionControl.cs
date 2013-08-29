using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents TFS source control.
	/// </summary>
	public interface IVersionControl : IDisposable
	{
		/// <summary>
		/// Gets the <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> at the specified path.
		/// </summary>
		/// <returns>
		/// The specified <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/>. Throws an exception if an item is not found.
		/// </returns>
		/// <param name="path">The path to the item, may be server or local.</param>
		IVersionedItem GetItem(string path);

		/// <summary>
		/// Gets an array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects from repository that match the specified <paramref name="path"/>.
		/// </summary>
		/// <returns>An array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects matching the path.</returns>
		/// <param name="path">The path to the item(s), may be server or local.</param>
		IReadOnlyList<IVersionedItem> GetItems(string path);

		/// <summary>
		/// Gets an array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects from repository that match the specified <paramref name="path"/>.
		/// </summary>
		/// <returns>An array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects matching the path.</returns>
		/// <param name="path">The path to the item(s), may be server or local.</param><param name="recursion">A flag describing whether the items should be listed from subfolders.</param>
		IReadOnlyList<IVersionedItem> GetItems(string path, RecursionType recursion);

		/// <summary>
		/// Event raised on the commit of a new check-in.
		/// </summary>
		event EventHandler<CommitCheckinEventArgs> ChangeCommitted;
	}
}