using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TestCaseAutomator.Utilities.Mvvm.Commands;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Base class for a node that has virtualized children. That is, child nodes
	/// are not known or loaded until node expansion.
	/// </summary>
	public abstract class VirtualizedNode<TChild> : NodeViewModel<TChild>, IVirtualizedNode where TChild : class
	{
		/// <summary>
		/// Initializes a new node.
		/// </summary>
		protected VirtualizedNode()
		{
			RefreshCommand = new RelayCommand(Refresh, () => IsRealized);
			ExpandedCommand = new RelayCommand(Load);
			Reset();
		}

		#region Implementation of IVirtualizedNode

		/// <see cref="IVirtualizedNode.IsRealized"/>
		public bool IsRealized
		{
			get { return !(Children.Count == 1 && Children.Single() == DummyNode); }
		}

		/// <see cref="IVirtualizedNode.Reset"/>
		public void Reset()
		{
			Children.Clear();
			Children.Add(DummyNode);
		}

		/// <see cref="IVirtualizedNode.RefreshCommand"/>
		public ICommand RefreshCommand { get; private set; }

		#endregion

		private void Refresh()
		{
			Reset();
			IsExpanded = false;
		}

		private void Load()
		{
			Children.Clear();
			foreach (var project in LoadChildren())
				Children.Add(project);
		}

		/// <summary>
		/// Gets the dummy node.
		/// </summary>
		protected abstract TChild DummyNode { get; }

		protected abstract IEnumerable<TChild> LoadChildren();
	}
}