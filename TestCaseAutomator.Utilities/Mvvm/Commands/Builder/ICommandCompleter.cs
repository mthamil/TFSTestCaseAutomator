using System;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Interface for a builder object that finishes constructing a command by specifying the
	/// actual operation that the command will execute.
	/// </summary>
	public interface ICommandCompleter
	{
		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes(Action operation);

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The operation to be executed</param>
		/// <returns>A new command</returns>
		ICommand Executes(Action<object> operation);
	}
}