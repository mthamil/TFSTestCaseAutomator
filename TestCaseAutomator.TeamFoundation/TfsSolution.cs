using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a solution in TFS source control.
	/// </summary>
	public class TfsSolution : TfsSourceControlledItem
	{
		/// <summary>
		/// Initializes a new <see cref="TfsSolution"/>.
		/// </summary>
		/// <param name="solutionItem">The source contorlled solution file</param>
		public TfsSolution(Item solutionItem)
			: base(solutionItem)
		{
		}

		/// <summary>
		/// The projects in a solution. This does not include solution folders.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TfsSolutionProject> Projects()
		{
			var solutionDir = Path.GetDirectoryName(Item.ServerItem);
			var contents = Item.DownloadFile();
			var solutionFolderGuidString = WellKnownProjectTypes.SolutionFolder.ToString("B").ToUpperInvariant();
			return Lines(contents)
				.Where(l => l.StartsWith("Project(") && !l.Contains(solutionFolderGuidString))
				.Select(l => l.Split(',')[1].Trim('"', ' '))
				.Select(p => Path.Combine(solutionDir, p))
				.Select(p => Item.VersionControlServer.GetItem(p))
				.Where(p => p.ItemType == ItemType.File)
				.Select(p => new TfsSolutionProject(p));
		}

		/// <summary>
		/// Returns a stream as a sequence of lines.
		/// </summary>
		/// <param name="stream">The stream to iterate over</param>
		/// <returns>A sequence of lines from the stream</returns>
		private static IEnumerable<string> Lines(Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
					yield return reader.ReadLine();
			}
		}
	}
}