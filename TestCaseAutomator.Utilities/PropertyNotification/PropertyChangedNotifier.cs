using System;
using System.ComponentModel;
using System.Linq.Expressions;
using TestCaseAutomator.Utilities.Reflection;

namespace TestCaseAutomator.Utilities.PropertyNotification
{
	/// <summary>
	/// A base implementation of INotifyPropertyChanged.
	/// </summary>
	public abstract class PropertyChangedNotifier : INotifyPropertyChanged
	{
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// A base implementation of INotifyPropertyChanged that uses static polymorphism to
	/// safely reference property names.
	/// </summary>
	public abstract class PropertyChangedNotifier<T> : PropertyChangedNotifier
	{
		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="property">The property that changed</param>
		protected void OnPropertyChanged(Expression<Func<T, object>> property)
		{
			OnPropertyChanged(Reflect.PropertyOf(property).Name);
		}
	}
}