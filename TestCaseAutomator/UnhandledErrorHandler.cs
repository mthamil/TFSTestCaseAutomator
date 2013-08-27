using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TestCaseAutomator
{
	/// <summary>
	/// Handles unhandled application exceptions.
	/// </summary>
	internal class UnhandledErrorHandler
	{
		public UnhandledErrorHandler(Application application, Dispatcher dispatcher)
		{
			_application = application;
			_dispatcher = dispatcher;
			application.DispatcherUnhandledException += Application_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			_dispatcher.Invoke(() => ShowMessageBox(e.Exception));
		}

		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			_dispatcher.BeginInvoke(new Action(() => ShowMessageBox(e.Exception)));
		}

		private void ShowMessageBox(Exception exception)
		{
			var result = MessageBox.Show(FormatMessage(exception), "Application Error", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
				_application.Shutdown();
		}

		private static string FormatMessage(Exception exception)
		{
			const string caption =
				@"An application error occurred. If this error occurs again there may be a more serious malfunction in the application, and it should be closed.

Do you want to exit the application?
(Warning: If you click Yes the application will close, if you click No the application will continue)";

			return String.Format("{1}{0}{2}{0}{3}{0}{4}{5}",
			                     Environment.NewLine,
			                     caption,
			                     exception.GetType().Name,
			                     exception.Message,
			                     exception.StackTrace,
			                     exception.InnerException != null
				                     ? String.Format("{1}{0}{2}{0}{3}",
				                                     Environment.NewLine,
				                                     exception.InnerException.GetType().Name,
				                                     exception.InnerException.Message,
				                                     exception.InnerException.StackTrace)
				                     : string.Empty);
		}

		private readonly Application _application;
		private readonly Dispatcher _dispatcher;
	}
}