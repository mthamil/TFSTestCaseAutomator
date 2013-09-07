﻿using System;

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
		/// The test case to associate with automation.
		/// </summary>
		ITestCaseViewModel TestCase { get; }
	}
}