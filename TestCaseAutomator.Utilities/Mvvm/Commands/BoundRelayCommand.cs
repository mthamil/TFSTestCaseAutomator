using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute and CanExecuteChanged are bound to 
	/// a property of another object.
	/// </summary>
	public class BoundRelayCommand : BoundRelayCommandBase
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		/// <param name="propertyName">The name of the bound property</param>
		/// <param name="canExecute">Function that determines whether a command can be executed</param>
		/// <param name="execute">The operation to execute</param>
		public BoundRelayCommand(INotifyPropertyChanged propertyDeclarer, string propertyName, Func<bool> canExecute, Action<object> execute)
			: base(propertyDeclarer, propertyName, canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
		}

		#region ICommand Members

		/// <see cref="ICommand.Execute"/>
		public override void Execute(object parameter)
		{
			_execute(parameter);
		}

		#endregion

		private readonly Action<object> _execute;
	}
}