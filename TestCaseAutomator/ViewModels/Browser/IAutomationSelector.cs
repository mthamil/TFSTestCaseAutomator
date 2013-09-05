using System;

namespace TestCaseAutomator.ViewModels.Browser
{
	/// <summary>
	/// Interface for an automated test selector.
	/// </summary>
	public interface IAutomationSelector
	{
		/// <summary>
		/// Event raised when an automated test is selected.
		/// </summary>
		event EventHandler<AutomatedTestSelectedEventArgs> AutomatedTestSelected;

		/// <summary>
		/// Whether a browser has saved its test selection.
		/// </summary>
		bool? HasBeenSaved { get; set; }
	}
}