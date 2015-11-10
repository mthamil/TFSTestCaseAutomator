using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using SharpEssentials.Observable;

namespace TestCaseAutomator.Configuration
{
	/// <summary>
	/// An adapter around a generated .NET Settings class.
	/// </summary>
	public class DotNetSettings : ObservableObject, ISettings
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DotNetSettings"/>.
		/// </summary>
		/// <param name="settings">The wrapped generated <see cref="Settings"/> object</param>
		internal DotNetSettings(Settings settings)
			: this()
		{
			_settings = settings;

            TfsServers = settings.TFSServerUrls?.Cast<string>().Select(url => new Uri(url)).ToList() ?? new List<Uri>();
			TfsProjectName = settings.TFSProjectName;
		    AutoConnectOnStartup = settings.AutoConnectOnStartup;
            TestDiscoveryPluginLocation = new DirectoryInfo(settings.TestDiscoveryPluginLocation);
		}

		private DotNetSettings()
		{
			_tfsProjectName = Property.New(this, p => p.TfsProjectName);
		}

		/// <see cref="ISettings.TfsServers"/>
		public IList<Uri> TfsServers { get; }

		/// <see cref="ISettings.TfsProjectName"/>
		public string TfsProjectName
		{
			get { return _tfsProjectName.Value; }
			set { _tfsProjectName.Value = value; }
		}

        /// <see cref="ISettings.AutoConnectOnStartup"/>
	    public bool AutoConnectOnStartup { get; set; }

	    /// <see cref="ISettings.TestDiscoveryPluginLocation"/>
		public DirectoryInfo TestDiscoveryPluginLocation { get; }

		/// <see cref="ISettings.Save"/>
		public void Save()
		{
            var newUrls = new StringCollection();
		    foreach (var uri in TfsServers)
		        newUrls.Add(uri.ToString());

			_settings.TFSServerUrls = newUrls;
			_settings.TFSProjectName = TfsProjectName;
		    _settings.AutoConnectOnStartup = AutoConnectOnStartup;

            _settings.Save();
		}

		private readonly Property<string> _tfsProjectName; 

		private readonly Settings _settings;
	}
}