using System.Windows;
using System.Windows.Interactivity;

namespace TestCaseAutomator.Controls.Behaviors
{
	/// <summary>
	/// Provides a base class for a <see cref="T:System.Windows.Interactivity.Behavior`1"/> that requires attachment to be
	/// performed once its <c>AssociatedObject</c> is loaded.
	/// </summary>
	/// <typeparam name="T">The type the <see cref="T:System.Windows.Interactivity.Behavior`1"/> can be attached to.</typeparam>
	public abstract class LoadDependentBehavior<T> : Behavior<T> where T : FrameworkElement
	{
		protected override void OnAttached()
		{
			if (AssociatedObject.IsLoaded)
				OnLoaded();
			else
				AssociatedObject.Loaded += AssociatedObject_Loaded;
		}

		/// <summary>
		/// This method will be called once a <see cref="T:System.Windows.Interactivity.Behavior`1"/>'s <c>AssociatedObject</c> is loaded.
		/// </summary>
		protected abstract void OnLoaded();

		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			OnLoaded();
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}
	}
}