using System.IO;
using System.Threading.Tasks;

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
		public async Task DownloadToAsync(string path)
		{
			var directoryPath = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

            using (var downloadStream = Download())
			using (var fileStream = File.OpenWrite(path))
				await downloadStream.CopyToAsync(fileStream).ConfigureAwait(false);
		}
	}
}