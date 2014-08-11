using System.Windows;
using Autofac;
using TestCaseAutomator.Container;

namespace TestCaseAutomator
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			_errorHandler = new UnhandledErrorHandler(Current, Dispatcher);

			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule<BootstrapModule>();
			_container = containerBuilder.Build();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (_container != null)
				_container.Dispose();
		}

		/// <summary>
		/// The application's IoC container.
		/// </summary>
		public static IContainer Container { get { return _container; } }
		private static IContainer _container;

		private static UnhandledErrorHandler _errorHandler;
	}
}
