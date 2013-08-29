using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TestCaseAutomator.Controls.Behaviors
{
	/// <summary>
	/// Since the TreeView's SelectedItem property is readonly, it is not bindable.
	/// This behavior provides a selected item property that is bindable.
	/// </summary>
	public class TreeViewBindableSelectedItem : Behavior<TreeView>
	{
		/// <see cref="Behavior.OnAttached"/>
		protected override void OnAttached()
		{
			AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
		}

		private void AssociatedObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			SelectedItem = e.NewValue;
		}

		/// <summary>
		/// The currently selected tree item.
		/// </summary>
		public object SelectedItem
		{
			get { return AssociatedObject.GetValue(SelectedItemProperty); }
			set { AssociatedObject.SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// The SelectedItem dependency property.
		/// </summary>
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register(
			"SelectedItem",
			typeof(object),
			typeof(TreeViewBindableSelectedItem),
			new FrameworkPropertyMetadata(null, 
				FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
				OnSelectedItemChanged));

		private static void OnSelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var treeView = dependencyObject as TreeView;
			if (treeView == null)
				return;

			treeView.FindContainerFromItem(e.NewValue).Apply(item =>
			{
				if (item != null && !item.IsSelected)
					item.IsSelected = true;
			});
		}
	}
}