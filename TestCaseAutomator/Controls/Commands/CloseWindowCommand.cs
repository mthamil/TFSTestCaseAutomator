using System.Windows;
using TestCaseAutomator.Utilities.Mvvm.Commands;

namespace TestCaseAutomator.Controls.Commands
{
	/// <summary>
	/// Closes a window.
	/// </summary>
	public class CloseWindowCommand : RelayCommand<Window>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		public CloseWindowCommand() 
			: base(w => w.Close()) { }
	}
}