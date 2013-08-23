using TestCaseAutomator.Utilities.Mvvm.Commands.Builder;

namespace TestCaseAutomator.Utilities.Mvvm.Commands
{
	public static class AsyncCommandBuilderExtensions
	{
		/// <summary>
		/// Indicates that a command executes an asynchronous operation.
		/// </summary>
		public static IAsyncCommandCompleter Asynchronously(this ICommandCompleter completer)
		{
			return new AsyncCommandCompleterWrapper(completer);
		}
	}
}