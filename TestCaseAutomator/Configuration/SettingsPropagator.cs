using System.ComponentModel;
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
            _application.ConnectionSucceeded += application_ConnectionSucceeded;
		}

        private void application_ConnectionSucceeded(object sender, ConnectionSucceededEventArgs e)
        {
            if (!_settings.TfsServers.Contains(e.Server))
                _settings.TfsServers.Add(e.Server);
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

	    private void application_Closing(object sender, System.EventArgs e)
		{
			_settings.Save();	// Save before the app shuts down.
		}

		private readonly IApplication _application;
		private readonly ISettings _settings;
	}
}