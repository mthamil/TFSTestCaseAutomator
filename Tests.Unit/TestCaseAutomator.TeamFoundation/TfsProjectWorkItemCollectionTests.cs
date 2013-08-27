using Microsoft.TeamFoundation.TestManagement.Client;
using Moq;
using TestCaseAutomator.TeamFoundation;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.TeamFoundation
{
	public class TfsProjectWorkItemCollectionTests
	{
		public TfsProjectWorkItemCollectionTests()
		{
			workItemCollection = new TfsProjectWorkItemCollection(testManagement.Object);
		}

		[Fact]
		public void Test_ProjectName()
		{
			// Arrange.
			testManagement.SetupGet(tm => tm.TeamProjectName).Returns("Tests Project");

			// Act.
			var name = workItemCollection.ProjectName;

			// Assert.
			Assert.Equal("Tests Project", name);
		}

		private readonly TfsProjectWorkItemCollection workItemCollection;

		private readonly Mock<ITestManagementTeamProject> testManagement = new Mock<ITestManagementTeamProject>();
	}
}