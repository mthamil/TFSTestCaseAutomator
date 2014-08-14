using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TestCaseAutomator.Controls
{
	/// <summary>
    /// Wraps the standard Windows message box and exposes an MVVM friendly API.
    /// </summary>
    [DesignTimeVisible(false)]  // Set to false so it doesn't show up in the designer
    public class MessageBox : Control
    {
		/// <summary>
        /// Initializes a new message box.
        /// </summary>
		public MessageBox()
        {
            // The control doesn't have any specific rendering of its own.
            Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Property to trigger the display of the message box. Whenever this (implicitly the variable it is bound to) is set a true, the message box will display
        /// </summary>
        public bool Trigger
        {
            get { return (bool)GetValue(TriggerProperty);  }
            set  { SetValue(TriggerProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "Trigger". This also overrides the PropertyChangedCallback to trigger the message box display.
		/// </summary>
		public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger", typeof(bool), typeof(MessageBox),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTriggerChange)));

        /// <summary>
        /// Type of message box. Can take values "OK", "OKCancel", "YesNo", "YesNoCancel". 
        /// The appropriate dependency property should be set as:
        /// If "OK", no action property is required, but one may be provided to execute when OK is selected. 
        /// For "OKCancel", OkAction and CancelAction is required.
        /// If "YesNo" and "YesNoCancel", YesAction and NoAction are required.
        /// For "YesNoCancel" in addition, CancelAction should be specified.
        /// </summary>
		public MessageBoxButton Type
        {
            get { return (MessageBoxButton)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "Type".
		/// </summary>
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(MessageBoxButton), typeof(MessageBox));

        /// <summary>
        /// Command executed for Ok/Yes options.
        /// </summary>
		public ICommand AffirmativeAction
        {
            get { return (ICommand)GetValue(AffirmativeActionProperty); }
            set  { SetValue(AffirmativeActionProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "AffirmativeAction".
		/// </summary>
		public static readonly DependencyProperty AffirmativeActionProperty = DependencyProperty.Register("AffirmativeAction", typeof(ICommand), typeof(MessageBox));

        /// <summary>
        /// Command executed for No options.
        /// </summary>
		public ICommand NegativeAction
        {
            get { return (ICommand)GetValue(NegativeActionProperty); }
            set { SetValue(NegativeActionProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "NegativeAction".
		/// </summary>
		public static readonly DependencyProperty NegativeActionProperty = DependencyProperty.Register("NoAction", typeof(ICommand), typeof(MessageBox));

        /// <summary>
        /// On a Yes/No/Cancel or Ok/Cancel dialog, this will Execute the bound DelegateCommand on the user clicking "Cancel".
        /// </summary>
		public ICommand CancelAction
        {
            get { return (ICommand)GetValue(NegativeActionProperty); }
            set { SetValue(NegativeActionProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "CancelAction".
		/// </summary>
		public static readonly DependencyProperty CancelActionProperty = DependencyProperty.Register("CancelAction", typeof(ICommand), typeof(MessageBox));

        /// <summary>
        /// The message to show the user.
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "Message".
		/// </summary>
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageBox));

        /// <summary>
        /// The message box caption/title to show the user.
        /// </summary>
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for "Caption".
		/// </summary>
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(MessageBox));

        /// <summary>
        /// The "Trigger" propery changed override. Whenever the "Trigger" property changes to true or false this will be executed.
        /// When the property changes to true, the message box will be shown.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void OnTriggerChange(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
			var messageBox = (MessageBox)dependencyObject;
            if (!messageBox.Trigger) return;

            switch (messageBox.Type)
            {
                case MessageBoxButton.OK:
                    messageBox.ShowInfo();
                    break;
                case MessageBoxButton.OKCancel:
                    messageBox.ShowOkCancel();
                    break;
                case MessageBoxButton.YesNo:
                    messageBox.ShowYesNo();
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageBox.ShowYesNoCancel();
                    break;
            }
        }

        /// <summary>
        /// Displays the Info message box.
        /// </summary>
        private void ShowInfo()
        {
            System.Windows.MessageBox.Show(Message, Caption, MessageBoxButton.OK, MessageBoxImage.Information);

			if (AffirmativeAction != null)
				AffirmativeAction.Execute(null);
        }

        /// <summary>
        /// Displays the Info message box.
        /// </summary>
        private void ShowOkCancel()
        {
			ICommand action = System.Windows.MessageBox.Show(Message, Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.OK 
				? AffirmativeAction 
				: CancelAction;

			if (action != null)
        		action.Execute(null);
        }

		/// <summary>
        /// Displays the Ok/Cancel message box and based on user action executes the appropriate command.
        /// </summary>
        private void ShowYesNo()
        {
			ICommand action = System.Windows.MessageBox.Show(Message, Caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes 
				? AffirmativeAction 
				: NegativeAction;

			if (action != null)
        		action.Execute(null);
        }

		/// <summary>
        /// Displays the Yes/No/Cancel message box and based on user action executes the appropriate command.
        /// </summary>
        private void ShowYesNoCancel()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(Message, Caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

			ICommand action;
            switch (result)
            {
                case MessageBoxResult.Yes:
					action = AffirmativeAction;
                    break;
                case MessageBoxResult.No:
                    action = NegativeAction;
                    break;
            	default:
                    action = CancelAction;
                    break;
            }

			if (action != null)
				action.Execute(null);
        }
    }
}
