using System;

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
		/// Stores the current settings. 
		/// </summary>
		void Save();
	}
}