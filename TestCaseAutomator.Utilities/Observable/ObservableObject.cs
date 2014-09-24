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

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="property">A lambda expression referencing the property that changed</param>
        protected void OnPropertyChanged<T>(Expression<Func<T, object>> property)
        {
            var localEvent = PropertyChanged;
            if (localEvent != null)
                localEvent(this, new PropertyChangedEventArgs(Reflect.PropertyOf(property).Name));
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="instance">An instance of the type the property is declared on (used for inference)</param>
        /// <param name="property">A lambda expression referencing the property that changed</param>
        protected void OnPropertyChanged<T>(T instance, Expression<Func<T, object>> property)
        {
            OnPropertyChanged(property);
        }
    }
}