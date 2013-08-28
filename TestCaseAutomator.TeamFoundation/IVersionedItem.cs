using System.IO;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a source controlled item.
	/// </summary>
	public interface IVersionedItem
	{
		/// <summary>
		/// Gets the path of the associated item on the server.
		/// </summary>
		/// <returns>
		/// The path of the associated item on the server.
		/// </returns>
		string ServerItem { get; }

		/// <summary>
		/// Gets the type of this item.
		/// </summary>
		/// <returns>
		/// The type of this item.
		/// </returns>
		ItemType ItemType { get; }

		/// <summary>
		/// Downloads the content of an item from source control.
		/// </summary>
		/// <returns>A stream that contains the contents of the item.</returns>
		Stream DownloadFile();
	}
}