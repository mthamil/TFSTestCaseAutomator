using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Interface that represents a Team Foundation project's work items.
	/// </summary>
	public interface ITfsProjectWorkItemCollection
	{
		/// <summary>
		/// Provides an <see cref="IQueryable"/> over the test cases in a project.
		/// </summary>
		IQueryable<WorkItem> TestCases();

		/// <summary>
		/// The name of the Team Foundation project.
		/// </summary>
		string ProjectName { get; }
	}
}