using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Interface for a builder object that finishes constructing a command by specifying the
	/// actual operation that the command will execute asynchronously.
	/// </summary>
	public interface IAsyncCommandCompleter
	{
		/// <summary>
		/// Sets the asynchronous operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless, asynchronous operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes(Func<Task> operation);

		/// <summary>
		/// Sets the asynchronous operation that a command will execute.
		/// </summary>
		/// <param name="operation">The asynchronous operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes(Func<object, Task> operation);
	}
}