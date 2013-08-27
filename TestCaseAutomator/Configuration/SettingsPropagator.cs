using System.ComponentModel;
using TestCaseAutomator.Utilities.Reflection;
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
		}

		void application_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == serverUriPropName)
				_settings.TfsServerLocation = _application.ServerUri;
			else if (e.PropertyName == projectNamePropName)
				_settings.TfsProjectName = _application.ProjectName;
		}

		private void application_Closing(object sender, System.EventArgs e)
		{
			_settings.Save();	// Save before the app shuts down.
		}

		private readonly IApplication _application;
		private readonly ISettings _settings;

		private static readonly string serverUriPropName = Reflect.PropertyOf<IApplication>(a => a.ServerUri).Name;
		private static readonly string projectNamePropName = Reflect.PropertyOf<IApplication>(a => a.ProjectName).Name;
	}
}