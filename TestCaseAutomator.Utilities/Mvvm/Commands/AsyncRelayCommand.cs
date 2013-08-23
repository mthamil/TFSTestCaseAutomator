using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking asynchronous functions. The default return value for the CanExecute
	/// method is 'true'.  This command does not take any parameters.
	/// </summary>
	public class AsyncRelayCommand : CommandBase
	{
		/// <summary>
		/// Initializes a new asynchronous command.
		/// </summary>
		/// <param name="execute">The asynchronous operation to execute</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public override bool CanExecute(object parameter)
		{
			if (_canExecute == null)
				return true;

			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public async override void Execute(object parameter)
		{
			await _execute();
		}

		#endregion

		readonly Func<Task> _execute;
		readonly Func<bool> _canExecute;
	}

	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking asynchronous functions.  In order for CanExecute to return
	/// true, the command parameter must of type T.
	/// </summary>
	/// <typeparam name="T">The type of parameter to be passed to the command</typeparam>
	public class AsyncRelayCommand<T> : CommandBase
	{
		/// <summary>
		/// Initializes a new asynchronous command.
		/// </summary>
		/// <param name="execute">The asynchronous operation to execute</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public override bool CanExecute(object parameter)
		{
			if (_canExecute == null)
				return true;

			if (parameter is T)
				return _canExecute((T)parameter);

			return false;
		}

		/// <see cref="ICommand.Execute"/>
		public async override void Execute(object parameter)
		{
			await _execute((T)parameter);
		}

		#endregion

		readonly Func<T, Task> _execute;
		readonly Predicate<T> _canExecute;
	}
}
