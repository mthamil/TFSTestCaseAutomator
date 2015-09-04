using System.Windows.Input;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
{
	/// <summary>
	/// Interface for a node that doesn't load its children until it is expanded.
	/// </summary>
	public interface IVirtualizedNode
	{
		/// <summary>
		/// Whether a node has created and loaded its children.
		/// </summary>
		bool IsRealized { get; }

		/// <summary>
		/// Command to reset a node so that on the next expansion it will requery its children.
		/// </summary>
		ICommand RefreshCommand { get; }

		/// <summary>
		/// Collapses a node, removes its children, and resets it to reload its children
		/// on the next expansion.
		/// </summary>
		void Reset();
	}
}