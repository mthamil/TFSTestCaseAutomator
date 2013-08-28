using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.TestManagement.Client;
using Moq;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.ViewModels;
using TestCaseAutomator.ViewModels.Browser;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
	public class MainViewModelTests
	{
		public MainViewModelTests()
		{
			viewModel = new MainViewModel(CreateExplorer, CreateWorkItems, CreateTestBrowser);
		}

		[Fact]
		public void Test_ServerUri_Updates_Explorer()
		{
			// Act.
			viewModel.ServerUri = new Uri("http://test/");

			// Assert.
			Assert.NotNull(explorer);
			Assert.Equal(new Uri("http://test/"), explorer.Object.TfsServer);
		}

		[Fact]
		public void Test_ProjectName_Updates_WorkItems()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");

			// Act.
			viewModel.ProjectName = "TestProject";

			// Assert.
			Assert.NotNull(workItems);
			workItems.Verify(wi => wi.Load(), Times.Once());
		}

		[Fact]
		public void Test_ProjectName_With_No_Server()
		{
			// Act.
			viewModel.ProjectName = "TestProject";

			// Assert.
			Assert.Null(workItems);
		}

		[Fact]
		public void Test_ProjectName_Status_When_TFS_Unavailable()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");

			explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
			        .Throws(new TeamFoundationServiceUnavailableException(""));

			// Act.
			viewModel.ProjectName = "TestProject";

			// Assert.
			Assert.NotNull(viewModel.Status);
		}

		[Fact]
		public void Test_Refresh()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");
			viewModel.ProjectName = "TestProject";

			// Act.
			viewModel.Refresh();

			// Assert.
			workItems.Verify(wi => wi.Load(), Times.Exactly(2));
		}

		[Fact]
		public void Test_TestBrowser()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");

			explorer.Setup(e => e.Solutions())
			        .Returns(new[]
			        {
				        new TfsSolution(Mock.Of<IVersionedItem>(), versionControl.Object),
				        new TfsSolution(Mock.Of<IVersionedItem>(), versionControl.Object)
			        });

			// Act.
			viewModel.SelectedTestCase = new TestCaseViewModel(Mock.Of<ITestCase>());

			// Assert.
			Assert.NotNull(viewModel.TestBrowser);
			Assert.Equal(2, viewModel.TestBrowser.Solutions.Count);
		}

		[Fact]
		public void Test_CloseCommand_Raises_Closing_Event()
		{
			// Act/Assert.
			AssertThat.Raises<IApplication>(viewModel, 
				vm => vm.Closing += null, 
				() => viewModel.CloseCommand.Execute(null));
		}

		private ITfsExplorer CreateExplorer(Uri uri)
		{
			explorer = new Mock<ITfsExplorer>();
			explorer.SetupGet(e => e.TfsServer).Returns(uri);
			explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
			        .Returns((string name) => Mock.Of<ITfsProjectWorkItemCollection>(wi => wi.ProjectName == name));
			return explorer.Object;
		}

		private IWorkItems CreateWorkItems(ITfsProjectWorkItemCollection items)
		{
			workItems = new Mock<IWorkItems>();
			return workItems.Object;
		}

		private TestBrowserViewModel CreateTestBrowser(IEnumerable<TfsSolution> solutions, TestCaseViewModel testCase)
		{
			return new TestBrowserViewModel(solutions, testCase, _ => null);
		}

		private readonly MainViewModel viewModel;

		private Mock<IWorkItems> workItems;
 		private Mock<ITfsExplorer> explorer;
		private readonly Mock<IVersionControl> versionControl = new Mock<IVersionControl>();
	}
}