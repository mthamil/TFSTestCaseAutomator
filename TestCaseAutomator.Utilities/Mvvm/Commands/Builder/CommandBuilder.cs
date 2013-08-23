using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace TestCaseAutomator.Utilities.Mvvm.Commands.Builder
{
	/// <summary>
	/// Class that aids in creating complex <see cref="ICommand"/>s.
	/// </summary>
	/// <typeparam name="TSource">The type of object for which a command is being built</typeparam>
	public class CommandBuilder<TSource> : ICommandBuilder<TSource> where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new <see cref="CommandBuilder{TSource}"/>.
		/// </summary>
		/// <param name="source">The object that the command is for</param>
		public CommandBuilder(TSource source)
		{
			_source = source;
		}

		/// <summary>
		/// Indicates that a command is bound to the given boolean property's value to determine
		/// whether it can execute.
		/// </summary>
		/// <param name="predicateProperty">The boolean property that determines whether a command can execute</param>
		/// <returns>A builder for a command bound to a property</returns>
		public ICommandCompleter DependsOn(Expression<Func<TSource, bool>> predicateProperty)
		{
			if (predicateProperty == null)
				throw new ArgumentNullException("predicateProperty");

			return new SimpleBoundCommandBuilder<TSource>(_source, predicateProperty);
		}

		/// <summary>
		/// Indicates that a command is bound to a property of each object of a child collection to determine
		/// whether it can execute.
		/// </summary>
		/// <typeparam name="TChild">The type of child object</typeparam>
		/// <param name="collection">The collection whose items determine whether a command can execute</param>
		/// <returns>A builder for a command bound to a child property</returns>
		public ChildBoundCommandBuilder<TSource, TChild> DependsOnCollection<TChild>(Expression<Func<TSource, IEnumerable<TChild>>> collection)
			where TChild : INotifyPropertyChanged
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			return new ChildBoundCommandBuilder<TSource, TChild>(_source, collection);
		}

		private readonly TSource _source;
	}
}