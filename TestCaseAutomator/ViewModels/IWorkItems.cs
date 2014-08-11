using System.Collections.Generic;
using System.Threading.Tasks;
using TestCaseAutomator.TeamFoundation;

namespace TestCaseAutomator.ViewModels
{
	/// <summary>
	/// Represents the work items of a TFS project.
	/// </summary>
	public interface IWorkItems
	{
		/// <summary>
		/// Loads work items.
		/// </summary>
		Task LoadAsync(ITfsProjectWorkItemCollection projectworkItemCollection);

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		ICollection<ITestCaseViewModel> TestCases { get; }
	}
}