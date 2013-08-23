using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using TestCaseAutomator.Utilities.Reflection;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that aids in creating a <see cref="ICommand"/> whose ability to execute
	/// is determined by a property.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	public class SimpleBoundCommandBuilder<TSource> : ICommandCompleter where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="SimpleBoundCommandBuilder{TSource}"/>.
		/// </summary>
		/// <param name="source">The object that declares the property the command is bound to</param>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		public SimpleBoundCommandBuilder(TSource source, Expression<Func<TSource, bool>> predicateProperty)
		{
			_source = source;
			_property = new Lazy<PropertyInfo>(() => Reflect.PropertyOf(predicateProperty));
			_canExecutePredicate = new Lazy<Func<bool>>(() =>
			{
				Func<TSource, bool> func = predicateProperty.Compile();
				return () => func(_source);
			});
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The parameterless operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action operation)
		{
			return Executes(_ => operation());
		}

		/// <summary>
		/// Sets the operation that a command will execute.
		/// </summary>
		/// <param name="operation">The operation to be executed</param>
		/// <returns>A new command</returns>
		public ICommand Executes(Action<object> operation)
		{
			if (operation == null)
				throw new ArgumentNullException("operation");

			return new BoundRelayCommand(_source, _property.Value.Name, _canExecutePredicate.Value, operation);
		}

		private readonly TSource _source;
		private readonly Lazy<PropertyInfo> _property;
		private readonly Lazy<Func<bool>> _canExecutePredicate;
	}
}