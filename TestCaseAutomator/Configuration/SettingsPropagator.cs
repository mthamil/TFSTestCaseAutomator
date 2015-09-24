using System;
using System.ComponentModel;
using SharpEssentials.Collections;
using TestCaseAutomator.ViewModels;

namespace TestCaseAutomator.Configuration
{
	/// <summary>
	/// Responsible for updating objects in response to settings changes or
	/// capturing events that should change settings.
	/// </summary>
	public class SettingsPropagator
	{
		/// <summary>
		/// Initializes a new <see cref="SettingsPropagator"/>.
		/// </summary>
		/// <param name="settings">The application settings</param>
		/// <param name="application">The application object</param>
		public SettingsPropagator(ISettings settings, IApplication application)
		{
			_settings = settings;
			_application = application;

			_application.Closing += application_Closing;
			_application.PropertyChanged += application_PropertyChanged;

            _serverUris = new CollectionMirror<Uri>(_application.ServerUris, _settings.TfsServers);
		}

        private void application_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		    switch (e.PropertyName)
		    {
		        case nameof(IApplication.ProjectName):
		            _settings.TfsProjectName = _application.ProjectName;
		            break;
		    }
		}

	    private void application_Closing(object sender, EventArgs e)
		{
			_settings.Save();	// Save before the app shuts down.
		}

	    private readonly CollectionMirror<Uri> _serverUris;

		private readonly IApplication _application;
		private readonly ISettings _settings;
	}
}