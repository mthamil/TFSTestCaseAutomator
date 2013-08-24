using System;
using System.IO;
using TestCaseAutomator.Utilities.PropertyNotification;

namespace TestCaseAutomator.Configuration
{
	/// <summary>
	/// An adapter around a generated .NET Settings class.
	/// </summary>
	public class DotNetSettings : PropertyChangedNotifier, ISettings
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DotNetSettings"/>.
		/// </summary>
		/// <param name="settings">The wrapped generated <see cref="Settings"/> object</param>
		internal DotNetSettings(Settings settings)
			: this()
		{
			_settings = settings;

			TfsServerLocation = settings.TFSServerUrl;
			TestDiscoveryPluginLocation = new DirectoryInfo(settings.TestDiscoveryPluginLocation);
		}

		private DotNetSettings()
		{
			_tfsServerLocation = Property.New(this, p => p.TfsServerLocation, OnPropertyChanged);
		}

		/// <see cref="ISettings.TfsServerLocation"/>
		public Uri TfsServerLocation 
		{
			get { return _tfsServerLocation.Value; }
			set { _tfsServerLocation.Value = value; }
		}

		/// <see cref="ISettings.TestDiscoveryPluginLocation"/>
		public DirectoryInfo TestDiscoveryPluginLocation { get; private set; }

		/// <see cref="ISettings.Save"/>
		public void Save()
		{
			_settings.TFSServerUrl = TfsServerLocation;

			_settings.Save();
		}

		private readonly Property<Uri> _tfsServerLocation; 

		private readonly Settings _settings;
	}
}