using System;
using System.ComponentModel;
using TestCaseAutomator.Utilities.Mvvm.Commands.Builder;

namespace TestCaseAutomator.Utilities.Mvvm.Commands
{
	/// <summary>
	/// Contains factory methods for commands.
	/// </summary>
	public static class Command
	{
		/// <summary>
		/// Begins creating a command for a given object.
		/// </summary>
		/// <typeparam name="TSource">The type of object that owns the command</typeparam>
		/// <param name="source">The object that owns the command</param>
		/// <returns>A new command builder</returns>
		public static ICommandBuilder<TSource> For<TSource>(TSource source) where TSource : INotifyPropertyChanged
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return new CommandBuilder<TSource>(source);
		}
	}
}