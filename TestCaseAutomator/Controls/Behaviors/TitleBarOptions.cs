using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TestCaseAutomator.Controls.Behaviors
{
	/// <summary>
	/// Behavior that enables configuration of a Window's titlebar.
	/// </summary>
	public class TitleBarOptions : LoadDependentBehavior<Window>
	{
		/// <see cref="LoadDependentBehavior{T}.OnLoaded"/>
		protected override void OnLoaded()
		{
			SetButtonVisibility(AssociatedObject, ShowButtons);
		}

		/// <summary>
		/// Gets or sets whether a Window's titlebar buttons are visible.
		/// </summary>
		public bool ShowButtons
		{
			get { return (bool)GetValue(ShowButtonsProperty); }
			set { SetValue(ShowButtonsProperty, value); }
		}

		/// <summary>
		/// The ShowButtons dependency property.
		/// </summary>
		public static readonly DependencyProperty ShowButtonsProperty =
			DependencyProperty.Register(
				"ShowButtons",
				typeof(bool),
				typeof(TitleBarOptions),
				new UIPropertyMetadata(false, ShowButtonsChanged));

		private static void ShowButtonsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (TitleBarOptions)dependencyObject;
			if (behavior.AssociatedObject != null)
				SetButtonVisibility(behavior.AssociatedObject, (bool)e.NewValue);
		}

		private static void SetButtonVisibility(Window window, bool isVisible)
		{
			int visibilityFlag = isVisible ? WS_SYSMENU : ~WS_SYSMENU;

			var hwnd = new WindowInteropHelper(window).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & visibilityFlag);
		}

		#region Win32 imports

		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x00080000;

		[DllImport("user32", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		#endregion
	}
}