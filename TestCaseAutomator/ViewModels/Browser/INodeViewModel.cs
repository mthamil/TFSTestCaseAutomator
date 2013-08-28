using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Interface for tree nodes.
	/// </summary>
	public interface INodeViewModel
	{
		/// <summary>
		/// The node name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Whether the node is selected.
		/// </summary>
		bool IsSelected { get; set; }

		/// <summary>
		/// Whether the node is expanded.
		/// </summary>
		bool IsExpanded { get; set; }

		/// <summary>
		/// Whether a node is currently available.
		/// </summary>
		bool IsEnabled { get; set; }

		/// <summary>
		/// Command executed when a node is selected.
		/// </summary>
		ICommand SelectedCommand { get; }

		/// <summary>
		/// Command executed when a node is expanded.
		/// </summary>
		ICommand ExpandedCommand { get; }

		/// <summary>
		/// An optional node icon location.
		/// </summary>
		Uri IconUri { get; set; }
	}

	/// <summary>
	/// Interface for tree nodes with children.
	/// </summary>
	/// <typeparam name="TChild">The type of children this node has</typeparam>
	public interface INodeViewModel<TChild> : INodeViewModel where TChild : class
	{
		/// <summary>
		/// All child nodes.
		/// </summary>
		ICollection<TChild> Children { get; }
	}
}