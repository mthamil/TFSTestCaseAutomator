using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Base view-model that provides common node functionality.
	/// </summary>
	public abstract class NodeViewModel<TChild> : ViewModelBase, INodeViewModel<TChild> where TChild : class
	{
		/// <summary>
		/// Initializes a new NodeViewModel.
		/// </summary>
		protected NodeViewModel()
		{
			_isExpanded = Property.New(this, p => p.IsExpanded, OnPropertyChanged);
			_isSelected = Property.New(this, p => p.IsSelected, OnPropertyChanged);
			_isEnabled = Property.New(this, p => p.IsEnabled, OnPropertyChanged);
			_children = Property.New(this, p => p.Children, OnPropertyChanged);
			_children.Value = new ObservableCollection<TChild>();

			_isEnabled.Value = true;	// true by default
		}

		#region Implementation of INodeViewModel

		/// <see cref="INodeViewModel.Name"/>
		public abstract string Name { get; }

		/// <see cref="INodeViewModel.IsSelected"/>
		public bool IsSelected
		{
			get { return _isSelected.Value; }
			set { _isSelected.Value = value; }
		}

		/// <see cref="INodeViewModel.IsSelected"/>
		public bool IsExpanded
		{
			get { return _isExpanded.Value; }
			set { _isExpanded.Value = value; }
		}

		/// <see cref="INodeViewModel.IsSelected"/>
		public bool IsEnabled
		{
			get { return _isEnabled.Value; }
			set { _isEnabled.Value = value; }
		}

		/// <see cref="INodeViewModel.IsSelected"/>
		public virtual ICommand SelectedCommand { get; set; }

		/// <see cref="INodeViewModel.IsSelected"/>
		public virtual ICommand ExpandedCommand { get; set; }

        /// <see cref="INodeViewModel{TChild}.Children"/>
        public ICollection<TChild> Children => _children.Value;

	    /// <see cref="INodeViewModel.IconUri"/>
		public Uri IconUri { get; set; }

        #endregion

        private readonly Property<bool> _isSelected;
		private readonly Property<bool> _isExpanded;
		private readonly Property<bool> _isEnabled;
		private readonly Property<ICollection<TChild>> _children;
	}
}
