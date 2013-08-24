using System;
using System.IO;

namespace TestCaseAutomator.Configuration
{
	/// <summary>
	/// Contains application settings.
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// The current TFS server URL.
		/// </summary>
		Uri TfsServerLocation { get; set; }

		/// <summary>
		/// The directory where automated test discovery plugins are located.
		/// </summary>
		DirectoryInfo TestDiscoveryPluginLocation { get; }

		/// <summary>
		/// Stores the current settings. 
		/// </summary>
		void Save();
	}
}