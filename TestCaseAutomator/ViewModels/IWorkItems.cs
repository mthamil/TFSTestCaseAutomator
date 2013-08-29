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
		/// Loads test cases.
		/// </summary>
		Task LoadAsync();

		/// <summary>
		/// The current collection of test cases.
		/// </summary>
		ICollection<TestCaseViewModel> TestCases { get; }
	}
}