using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that enables a simpler syntax for defining asynchronous command operations.
	/// That is, async/await are not necessary and method references for Task-returning methods
	/// can be used.
	/// </summary>
	public class AsyncCommandCompleterWrapper : IAsyncCommandCompleter
	{
		public AsyncCommandCompleterWrapper(ICommandCompleter completer)
		{
			_completer = completer;
		}

		/// <see cref="IAsyncCommandCompleter.Executes(Func{Task})"/>
		public ICommand Executes(Func<Task> operation)
		{
			return _completer.Executes(async () => await operation());
		}

		/// <see cref="IAsyncCommandCompleter.Executes(Func{object,Task})"/>
		public ICommand Executes(Func<object, Task> operation)
		{
			return _completer.Executes(async parameter => await operation(parameter));
		}

		private readonly ICommandCompleter _completer;
	}
}