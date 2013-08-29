using System.Windows.Controls;
using TestCaseAutomator.Utilities;

namespace TestCaseAutomator.Controls
{
	/// <summary>
	/// Contains extension methods for <see cref="TreeView"/>s.
	/// </summary>
	public static class TreeViewExtensions
	{
		/// <summary>
		/// Performs a depth-first search of the already generated items in a tree for a given item's
		/// corresponding <see cref="TreeViewItem"/>.
		/// </summary>
		/// <param name="treeView">The tree to search</param>
		/// <param name="item">The object to search for</param>
		/// <returns>The corresponding <see cref="TreeViewItem"/> or <see cref="Option{TreeViewItem}.None"/></returns>
		public static Option<TreeViewItem> FindContainerFromItem(this TreeView treeView, object item)
		{
			var found = FindGeneratedItem(treeView.ItemContainerGenerator, item);
			return Option<TreeViewItem>.From(found as TreeViewItem);
		}

		/// <summary>
		/// Attempts to find an item in a tree.
		/// </summary>
		private static object FindGeneratedItem(ItemContainerGenerator containerGenerator, object item)
		{
			var container = containerGenerator.ContainerFromItem(item);
			if (container != null)
				return container;

			foreach (var parent in containerGenerator.Items)
			{
				var c = containerGenerator.ContainerFromItem(parent) as TreeViewItem;
				if (c != null)
				{
					var found = FindGeneratedItem(c.ItemContainerGenerator, item);
					if (found != null)
						return found;
				}
			}

			return null;
		}
	}
}