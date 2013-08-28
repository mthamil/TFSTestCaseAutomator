using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a project belonging to a solution in TFS source control.
	/// </summary>
	public class TfsSolutionProject : TfsSourceControlledItem
	{
		/// <summary>
		/// Initializes a new <see cref="TfsSolutionProject"/>.
		/// </summary>
		/// <param name="projectItem">The source controlled project file</param>
		/// <param name="versionControl">TFS source control</param>
		public TfsSolutionProject(IVersionedItem projectItem, IVersionControl versionControl)
			: base(projectItem, versionControl)
		{
			_projectDocument = new Lazy<XDocument>(() =>
				XDocument.Load(new StreamReader(Item.DownloadFile())));

			_projectTypeGuids = new Lazy<IEnumerable<Guid>>(() =>
			    new HashSet<Guid>(_projectDocument.Value
					.Descendants(XName.Get("ProjectTypeGuids", ProjectNamespace))
					.SelectMany(e => e.Value.Split(';'))
					.Select(Guid.Parse)));
		}

		/// <summary>
		/// The GUIDs identifying the type of a project.
		/// </summary>
		public IEnumerable<Guid> ProjectTypeGuids
		{
			get { return _projectTypeGuids.Value; }
		}

		/// <summary>
		/// The files in a project.
		/// </summary>
		/// <param name="fileExtensionFilter">Any file extensions to filter out. If empty, all file extensions are returned.</param>
		/// <returns></returns>
		public IEnumerable<TfsFile> Files(IReadOnlyCollection<string> fileExtensionFilter)
		{
			var projectDir = Path.GetDirectoryName(Item.ServerItem);

			return _projectDocument.Value
				.Descendants(XName.Get("Compile", ProjectNamespace)).Concat(_projectDocument.Value
				.Descendants(XName.Get("None", ProjectNamespace)))
				.Select(e => Path.Combine(projectDir, e.Attribute(XName.Get("Include")).Value))
				.Where(p => fileExtensionFilter.Count == 0 || fileExtensionFilter.Contains(Path.GetExtension(p)))
				.Select(p => VersionControl.GetItem(p))
				.Select(i => new TfsFile(i, VersionControl));
		}

		private readonly Lazy<IEnumerable<Guid>> _projectTypeGuids; 
		private readonly Lazy<XDocument> _projectDocument; 

		private const string ProjectNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
	}
}