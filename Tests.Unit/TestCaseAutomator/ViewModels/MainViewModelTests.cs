using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Moq;
using SharpEssentials.Testing;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation;
using SharpEssentials.Concurrency;
using TestCaseAutomator.ViewModels;
using TestCaseAutomator.ViewModels.Browser;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
	public class MainViewModelTests
	{
		public MainViewModelTests()
		{
            workItems = new Mock<IWorkItems>();
		    workItems.Setup(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()))
		             .Returns(Task.FromResult<object>(null));

			viewModel = new MainViewModel(CreateExplorer, workItems.Object,
			                              new TestSelectionViewModel(CreateSourceControlBrowser, CreateFileSystemBrowser));
		}

		[Fact]
		public void Test_ServerUri_Updates_Explorer()
		{
			// Act.
			viewModel.ServerUri = new Uri("http://test/");

			// Assert.
			Assert.NotNull(explorer);
			Assert.Equal(new Uri("http://test/"), serverUri);
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
            workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Once());
		}

		[Fact]
		public void Test_ProjectName_With_No_Server()
		{
			// Act.
			viewModel.ProjectName = "TestProject";

			// Assert.
            workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Never);
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
            workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Exactly(2));
		}

		[Fact]
		public void Test_SourceControlBrowser()
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
			viewModel.TestSelection.SelectedTestCase = Mock.Of<ITestCaseViewModel>();

			// Assert.
            Assert.NotNull(viewModel.TestSelection.SourceControlTestBrowser);
            Assert.Equal(viewModel.TestSelection.SelectedTestCase, viewModel.TestSelection.SourceControlTestBrowser.Value.TestCase);
            Assert.Equal(2, viewModel.TestSelection.SourceControlTestBrowser.Value.Solutions.Count);
		}

		[Fact]
		public void Test_FileSystemBrowser()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");

			// Act.
            viewModel.TestSelection.SelectedTestCase = Mock.Of<ITestCaseViewModel>();

			// Assert.
            Assert.NotNull(viewModel.TestSelection.FileSystemTestBrowser);
            Assert.Equal(viewModel.TestSelection.SelectedTestCase, viewModel.TestSelection.FileSystemTestBrowser.Value.TestCase);
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
		    serverUri = uri;
			explorer = new Mock<ITfsExplorer>();
			explorer.SetupGet(e => e.Server).Returns(server.Object);
			explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
			        .Returns((string name) => Mock.Of<ITfsProjectWorkItemCollection>(wi => wi.ProjectName == name));
			return explorer.Object;
		}

		private SourceControlTestBrowserViewModel CreateSourceControlBrowser(IEnumerable<TfsSolution> solutions, ITestCaseViewModel testCase)
		{
			return new SourceControlTestBrowserViewModel(solutions, testCase, _ => null);
		}

		private FileSystemTestBrowserViewModel CreateFileSystemBrowser(ITestCaseViewModel testCase)
		{
			return new FileSystemTestBrowserViewModel(testCase, _ => null, Mock.Of<ITestAutomationDiscoverer>(), new SynchronousTaskScheduler());
		}

		private readonly MainViewModel viewModel;

	    private Uri serverUri;
 		private Mock<ITfsExplorer> explorer;
        private readonly Mock<ITfsServer> server = new Mock<ITfsServer>();
        private readonly Mock<IWorkItems> workItems;
		private readonly Mock<IVersionControl> versionControl = new Mock<IVersionControl>();
	}
}