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
		/// <param name="versionControl">TFS source control</param>
		public TfsSolution(IVersionedItem solutionItem, IVersionControl versionControl)
			: base(solutionItem, versionControl)
		{
		}

		/// <summary>
		/// The projects in a solution. This does not include solution folders.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TfsSolutionProject> Projects()
		{
			var solutionDir = Path.GetDirectoryName(ServerPath);
			var contents = Item.DownloadFile();
			var solutionParser = new SolutionFileParser(contents);
			return solutionParser.GetProjects()
			                     .Select(p => Path.Combine(solutionDir, p))
			                     .Select(p => VersionControl.GetItem(p))
			                     .Where(p => p.ItemType == ItemType.File)
			                     .Select(p => new TfsSolutionProject(p, VersionControl));
		}
	}
}