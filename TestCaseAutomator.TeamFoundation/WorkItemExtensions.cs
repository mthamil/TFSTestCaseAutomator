using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Contains utility methods for work item related operations.
	/// </summary>
	public static class WorkItemExtensions
	{
		/// <summary>
		/// Converts a sequence of work items to test cases.
		/// </summary>
		/// <param name="workItems">The work items to convert</param>
		/// <returns>A sequence of test cases</returns>
		public static IEnumerable<ITestCase> AsTestCases(this IQueryable<WorkItem> workItems)
		{
			var project = ((TestCaseQueryable<WorkItem>)workItems).Project;
			return workItems.Select(wi => (ITestCase)project.CreateFromWorkItem(wi));
		}
	}
}