using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TestCaseAutomator.Controls;

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
			var message = new StringBuilder();
			FormatMessage(message, exception);
			var messageBox = new ScrollableMessageBox
			{
				Buttons = MessageBoxButton.YesNo,
				Title = "Application Error",
				Caption = 
@"An application error occurred. If this error occurs again there may be a more serious malfunction in the application, and it should be closed.

Do you want to exit the application?
(Warning: If you click Yes the application will close, if you click No the application will continue)",
				Message = message.ToString()
			};

			messageBox.ShowDialog();
			if (messageBox.DialogResult.HasValue && messageBox.DialogResult.Value)
				_application.Shutdown();
		}

		private static void FormatMessage(StringBuilder message, Exception exception)
		{
			message.AppendFormat("{1}:{0}{2}{0}{3}",
			                     Environment.NewLine,
			                     exception.GetType().Name,
			                     exception.Message,
			                     exception.StackTrace);

			if (exception.InnerException != null)
			{
				message.AppendLine();
				FormatMessage(message, exception.InnerException);
			}
		}

		private readonly Application _application;
		private readonly Dispatcher _dispatcher;
	}
}