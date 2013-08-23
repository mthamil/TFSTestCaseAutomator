using System;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Contains GUID constants for well-known project types.
	/// </summary>
	public static class WellKnownProjectTypes
	{
		/// <summary>
		/// The Windows C# project type.
		/// </summary>
		public static readonly Guid CSharp = Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");

		/// <summary>
		/// The Test project type.
		/// </summary>
		public static readonly Guid Test = Guid.Parse("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

		/// <summary>
		/// The project type of solution folders.
		/// </summary>
		public static readonly Guid SolutionFolder = Guid.Parse("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
	
	}
}