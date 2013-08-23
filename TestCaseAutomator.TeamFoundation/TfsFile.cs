using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a TFS source controlled file.
	/// </summary>
	public class TfsFile : TfsSourceControlledItem
	{
		/// <summary>
		/// Initializes a new <see cref="TfsFile"/>.
		/// </summary>
		/// <param name="fileItem">The source controlled file</param>
		public TfsFile(Item fileItem)
			: base(fileItem)
		{
		}

		/// <summary>
		/// Downloads a file to a given file path.
		/// </summary>
		/// <param name="path">The local path to download to</param>
		public void DownloadTo(string path)
		{
			Item.DownloadFile(path);
		}
	}
}