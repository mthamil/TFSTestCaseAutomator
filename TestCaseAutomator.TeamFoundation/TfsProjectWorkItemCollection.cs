using System.Linq;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WiLinq.LinqProvider;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a Team Foundation project's work items.
	/// </summary>
	public class TfsProjectWorkItemCollection : ITfsProjectWorkItemCollection
	{
		/// <summary>
		/// Initializes a new <see cref="TfsProjectWorkItemCollection"/>.
		/// </summary>
		/// <param name="testManagement">The test management project to use</param>
		public TfsProjectWorkItemCollection(ITestManagementTeamProject testManagement)
		{
			_testManagement = testManagement;
		}

		/// <summary>
		/// Provides an <see cref="IQueryable"/> over the test cases in a project.
		/// </summary>
		public IQueryable<WorkItem> TestCases() 
            => new TestCaseQueryable<WorkItem>(_testManagement.WitProject.WorkItemSet(), 
                                               _testManagement)
		            .Where(wi => wi.Field<string>(SystemField.WorkItemType) == "Test Case");

	    /// <summary>
		/// The name of the Team Foundation project.
		/// </summary>
		public string ProjectName => _testManagement.TeamProjectName;

	    private readonly ITestManagementTeamProject _testManagement;
	}
}