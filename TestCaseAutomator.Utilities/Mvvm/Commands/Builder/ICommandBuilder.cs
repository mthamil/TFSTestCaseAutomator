using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Interface for an object that aids in creating complex <see cref="ICommand"/>s.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	public interface ICommandBuilder<TSource> where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates that a command is bound to the given boolean property's value to determine
		/// whether it can execute.
		/// </summary>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		/// <returns>A builder for a command bound to a property</returns>
		ICommandCompleter DependsOn(Expression<Func<TSource, bool>> predicateProperty);

		/// <summary>
		/// Indicates that a command is bound to a property of each element of a collection to determine
		/// whether it can execute.
		/// </summary>
		/// <typeparam name="TChild">The type of child object</typeparam>
		/// <param name="collection">The collection whose elements determine whether a command can execute</param>
		/// <returns>A builder for a command bound to a child property</returns>
		ChildBoundCommandBuilder<TSource, TChild> DependsOnCollection<TChild>(Expression<Func<TSource, IEnumerable<TChild>>> collection)
			where TChild : INotifyPropertyChanged;
	}
}