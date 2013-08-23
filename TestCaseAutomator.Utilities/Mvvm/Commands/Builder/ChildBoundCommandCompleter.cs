using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that completes construction of a command that depends on a child collection.
	/// </summary>
	/// <typeparam name="TParent">The type of parent object</typeparam>
	/// <typeparam name="TChild">The type of child object the parent depends on</typeparam>
	public class ChildBoundCommandCompleter<TParent, TChild> : ICommandCompleter where TChild : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="ChildBoundCommandCompleter{TParent,TChild}"/>.
		/// </summary>
		/// <param name="parent">The parent object</param>
		/// <param name="collectionGetter">Function that retrieves the collection whose items determine whether a command can execute</param>
		/// <param name="childProperty">A child property that the parent is somehow dependent upon for determining whether a command can execute</param>
		/// <param name="canExecute">The actual predicate that determines whether a command can execute</param>
		public ChildBoundCommandCompleter(
			TParent parent, 
			Func<IEnumerable<TChild>> collectionGetter, 
			Expression<Func<TChild, bool>> childProperty,
			Func<bool> canExecute)
		{
			_parent = parent;
			_collectionGetter = collectionGetter;
			_childProperty = childProperty;
			_canExecute = canExecute;
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
			return new ChildPropertyBoundCommand<TParent, TChild>(
				_parent, _collectionGetter, _childProperty, _canExecute, operation);
		}

		private readonly TParent _parent;
		private readonly Func<IEnumerable<TChild>> _collectionGetter;
		private readonly Expression<Func<TChild, bool>> _childProperty;
		private readonly Func<bool> _canExecute;
	}
}