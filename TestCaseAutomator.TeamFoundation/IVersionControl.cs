using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents TFS source control.
	/// </summary>
	public interface IVersionControl
	{
		/// <summary>
		/// Gets the <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> at the specified path.
		/// </summary>
		/// <returns>
		/// The specified <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/>. Throws an exception if an item is not found.
		/// </returns>
		/// <param name="path">The path to the item, may be server or local.</param>
		Item GetItem(string path);

		/// <summary>
		/// Gets an array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects from repository that match the specified <paramref name="path"/>.
		/// </summary>
		/// <returns>An array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects matching the path.</returns>
		/// <param name="path">The path to the item(s), may be server or local.</param>
		IReadOnlyList<Item> GetItems(string path);

		/// <summary>
		/// Gets an array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects from repository that match the specified <paramref name="path"/>.
		/// </summary>
		/// <returns>An array of <see cref="T:Microsoft.TeamFoundation.VersionControl.Client.Item"/> objects matching the path.</returns>
		/// <param name="path">The path to the item(s), may be server or local.</param><param name="recursion">A flag describing whether the items should be listed from subfolders.</param>
		IReadOnlyList<Item> GetItems(string path, RecursionType recursion);

		/// <summary>
		/// Downloads the content of an item from source control.
		/// </summary>
		/// <param name="item">The source controlled item to download</param>
		/// <returns>A stream that contains the contents of the item.</returns>
		Stream DownloadFile(Item item);
	}
}