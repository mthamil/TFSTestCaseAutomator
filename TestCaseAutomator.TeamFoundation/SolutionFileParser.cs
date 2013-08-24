using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Class that can parse Visual Studio solution files.
	/// </summary>
	public class SolutionFileParser
	{
		/// <summary>
		/// Initializes a new <see cref="SolutionFileParser"/>.
		/// </summary>
		/// <param name="solutionStream">A stream containing the contents of a solution file</param>
		public SolutionFileParser(Stream solutionStream)
		{
			_solutionStream = solutionStream;
		}

		/// <summary>
		/// Returns the projects in a solution.
		/// </summary>
		public IEnumerable<string> GetProjects()
		{
			return Lines(_solutionStream)
				.Where(l => l.StartsWith("Project(") && !l.Contains(solutionFolderGuidString))
				.Select(l => l.Split(',')[1].Trim('"', ' '));
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

		private readonly Stream _solutionStream;

		private static readonly string solutionFolderGuidString = WellKnownProjectTypes.SolutionFolder.ToString("B").ToUpperInvariant();
	}
}