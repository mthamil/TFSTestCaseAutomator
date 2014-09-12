using System;
using System.ComponentModel;
using System.Linq.Expressions;
using TestCaseAutomator.Utilities.Reflection;

namespace TestCaseAutomator.Utilities.Observable
{
    /// <summary>
    /// Base class for objects that notify observers about changes to their state.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
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
    /// Base class for objects that notify observers about changes to their state that uses static polymorphism to
    /// safely reference property names.
    /// </summary>
    public abstract class ObservableObject<T> : ObservableObject
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