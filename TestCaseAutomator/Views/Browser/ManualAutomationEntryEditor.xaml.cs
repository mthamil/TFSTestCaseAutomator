using System.Windows;

namespace TestCaseAutomator.Views.Browser
{
	/// <summary>
	/// Interaction logic for ManualAutomationEntryEditor.xaml
	/// </summary>
	public partial class ManualAutomationEntryEditor
	{
		public ManualAutomationEntryEditor()
		{
			InitializeComponent();
		}

        public string AutomationName
        {
            get { return (string)GetValue(AutomationNameProperty); }
            set { SetValue(AutomationNameProperty, value); }
        }

        public static readonly DependencyProperty AutomationNameProperty = 
            DependencyProperty.Register(
                nameof(AutomationName), 
                typeof(string), 
                typeof(ManualAutomationEntryEditor), 
                new PropertyMetadata(default(string)));

        public string AutomationStorage
        {
            get { return (string)GetValue(AutomationStorageProperty); }
            set { SetValue(AutomationStorageProperty, value); }
        }

        public static readonly DependencyProperty AutomationStorageProperty =
            DependencyProperty.Register(
                nameof(AutomationStorage),
                typeof(string),
                typeof(ManualAutomationEntryEditor),
                new PropertyMetadata(default(string)));

        public string AutomationTestType
        {
            get { return (string)GetValue(AutomationTestTypeProperty); }
            set { SetValue(AutomationTestTypeProperty, value); }
        }

        public static readonly DependencyProperty AutomationTestTypeProperty =
            DependencyProperty.Register(
                nameof(AutomationTestType),
                typeof(string),
                typeof(ManualAutomationEntryEditor),
                new PropertyMetadata(default(string)));
    }
}
