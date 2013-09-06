using TestCaseAutomator.AutomationProviders.Interfaces;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents a test case view-model.
	/// </summary>
	public interface ITestCaseViewModel
	{
		/// <summary>
		/// A test case's work item ID.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// A test case's title.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// A test case's associated automation if any exists.
		/// </summary>
		string AssociatedAutomation { get; }

		/// <summary>
		/// Whether automation can be removed.
		/// </summary>
		bool CanRemoveAutomation { get; }

		/// <summary>
		/// Updates the automation a test case is associated with.
		/// </summary>
		/// <param name="automatedTest">An automated test</param>
		void UpdateAutomation(IAutomatedTest automatedTest);

		/// <summary>
		/// Removes the automation from a test case.
		/// </summary>
		void RemoveAutomation();
	}
}