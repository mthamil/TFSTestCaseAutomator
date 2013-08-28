using System.IO;

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
		/// <param name="versionControl">TFS source control</param>
		public TfsFile(IVersionedItem fileItem, IVersionControl versionControl)
			: base(fileItem, versionControl)
		{
		}

		/// <summary>
		/// Downloads a file to a given file path.
		/// </summary>
		/// <param name="path">The local path to download to</param>
		public void DownloadTo(string path)
		{
			var downloadStream = Item.DownloadFile();

			var directoryPath = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

			using (var fileStream = File.OpenWrite(path))
				downloadStream.CopyTo(fileStream);
		}
	}
}