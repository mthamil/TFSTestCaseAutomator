using System.Linq;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WiLinq.LinqProvider;

namespace TestCaseAutomator.TeamFoundation
{
	/// <summary>
	/// Represents a Team Foundation project's work items.
	/// </summary>
	public class TfsProjectWorkItems
	{
		/// <summary>
		/// Initializes a new <see cref="TfsProjectWorkItems"/>.
		/// </summary>
		/// <param name="testManagement">The test management project to use</param>
		public TfsProjectWorkItems(ITestManagementTeamProject testManagement)
		{
			_testManagement = testManagement;
		}

		/// <summary>
		/// Provides an <see cref="IQueryable"/> over the test cases in a project.
		/// </summary>
		public IQueryable<WorkItem> TestCases()
		{
			return new TestCaseQueryable<WorkItem>(_testManagement.WitProject.WorkItemSet(), _testManagement)
				.Where(wi => wi.Field<string>(SystemField.WorkItemType) == "Test Case");
		}

		/// <summary>
		/// The name of the Team Foundation project.
		/// </summary>
		public string ProjectName
		{
			get { return _testManagement.TeamProjectName; }
		}

		private readonly ITestManagementTeamProject _testManagement;
	}
}