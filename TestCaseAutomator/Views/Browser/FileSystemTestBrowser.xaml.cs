using System.Windows;
using System.Windows.Markup;

namespace TestCaseAutomator.Views.Browser
{
	/// <summary>
	/// Interaction logic for FileSystemTestBrowser.xaml
	/// </summary>
	public partial class FileSystemTestBrowser
	{
		public FileSystemTestBrowser()
		{
			InitializeComponent();
		}

        [Ambient]
        public string AutomationName
        {
            get { return (string)GetValue(AutomationNameProperty); }
            set { SetValue(AutomationNameProperty, value); }
        }

        public static readonly DependencyProperty AutomationNameProperty =
            DependencyProperty.Register(
                nameof(AutomationName),
                typeof(string),
                typeof(FileSystemTestBrowser),
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
                typeof(FileSystemTestBrowser),
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
                typeof(FileSystemTestBrowser),
                new PropertyMetadata(default(string)));
    }
}
