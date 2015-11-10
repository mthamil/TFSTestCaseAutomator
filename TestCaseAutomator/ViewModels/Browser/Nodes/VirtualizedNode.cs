using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SharpEssentials.Controls.Mvvm.Commands;
using SharpEssentials.Observable;

namespace TestCaseAutomator.ViewModels.Browser.Nodes
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
			_isLoading = Property.New(this, p => p.IsLoading);

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
		public bool IsRealized => !(Children.Count == 1 && Children.Single() == DummyNode);

	    /// <see cref="IVirtualizedNode.Reset"/>
		public void Reset()
		{
			Children.Clear();
			Children.Add(DummyNode);
		}

		/// <see cref="IVirtualizedNode.RefreshCommand"/>
		public ICommand RefreshCommand { get; }

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
			        _isValid = true;

			        var progress = new Progress<TChild>(c => Children.Add(c));
			        await LoadChildrenAsync(progress);

			        IsExpanded = true;
			    }
			    catch (Exception)
			    {
			        _isValid = false;   // Don't consider realized in the event of an error.
			        throw;
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