using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestCaseAutomator.Utilities.Mvvm.Commands;
using TestCaseAutomator.Utilities.PropertyNotification;

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
			_isLoading = Property.New(this, p => p.IsLoading, OnPropertyChanged);

			RefreshCommand = new RelayCommand(Refresh, () => IsRealized);
			ExpandedCommand = new AsyncRelayCommand(LoadAsync);
			Reset();
		}

		/// <summary>
		/// Indicates whether a node's children are loading or not.
		/// </summary>
		public bool IsLoading
		{
			get { return _isLoading.Value; }
			private set { _isLoading.Value = value; }
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

		private async Task LoadAsync()
		{
			if (!_isValid)
			{
				try
				{
					IsLoading = true;
					IsEnabled = false;
					Children.Clear();
					var progress = new Progress<TChild>(c => Children.Add(c));
					await LoadChildrenAsync(progress);
				}
				finally
				{
					IsLoading = false;
					IsEnabled = true;
				}
			}
		}

		/// <summary>
		/// Gets the dummy node.
		/// </summary>
		protected abstract TChild DummyNode { get; }

		protected abstract Task<IReadOnlyCollection<TChild>> LoadChildrenAsync(IProgress<TChild> progress);

		/// <summary>
		/// Notifies a node that it should reload its children instead of using a cached collection.
		/// </summary>
		protected void Invalidate()
		{
			_isValid = false;
		}

		private bool _isValid;

		private readonly Property<bool> _isLoading;
	}
}