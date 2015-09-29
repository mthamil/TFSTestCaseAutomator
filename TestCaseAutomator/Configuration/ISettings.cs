using System;
using System.Collections.Generic;
using System.IO;

namespace TestCaseAutomator.Configuration
{
	/// <summary>
	/// Contains application settings.
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// Available TFS server URLs.
		/// </summary>
		IList<Uri> TfsServers { get; }

		/// <summary>
		/// The name of the current TFS project.
		/// </summary>
		string TfsProjectName { get; set; }

        /// <summary>
        /// Whether on start-up the application should automatically attempt to connect to the last server
        /// that was successfully connected to.
        /// </summary>
        bool AutoConnectOnStartup { get; set; }

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