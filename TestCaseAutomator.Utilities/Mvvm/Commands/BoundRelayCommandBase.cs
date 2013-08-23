using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands
{
	/// <summary>
	/// Base class for commands whose ability to execute is bound to a property on some object.
	/// </summary>
	public abstract class BoundRelayCommandBase : ICommand
	{
		protected BoundRelayCommandBase(INotifyPropertyChanged propertyDeclarer, string propertyName, Func<bool> canExecute)
		{
			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");

			if (canExecute == null)
				throw new ArgumentNullException("canExecute");

			_propertyName = propertyName;
			_canExecute = canExecute;

			propertyDeclarer.PropertyChanged += propertyDeclarer_PropertyChanged;
		}

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public abstract void Execute(object parameter);

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
		/// </summary>
		protected void OnCanExecuteChanged()
		{
			var localEvent = CanExecuteChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		private void propertyDeclarer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _propertyName)
				OnCanExecuteChanged();
		}

		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;
	}
}