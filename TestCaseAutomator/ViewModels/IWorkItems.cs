using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents the work items of a TFS project.
	/// </summary>
	public interface IWorkItems
	{
		/// <summary>
		/// Loads work items for a given project.
		/// </summary>
		/// <param name="projectName">The project for which to load work items.</param>
		Task LoadAsync(string projectName);

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		ICollection<ITestCaseViewModel> TestCases { get; }
	}
}