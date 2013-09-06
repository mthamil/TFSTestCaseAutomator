﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Moq;
using TestCaseAutomator.AutomationProviders.Interfaces;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.Utilities.Concurrency;
using TestCaseAutomator.ViewModels;
using TestCaseAutomator.ViewModels.Browser;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
	public class MainViewModelTests
	{
		public MainViewModelTests()
		{
			viewModel = new MainViewModel(CreateExplorer, CreateWorkItems,
			                              CreateSourceControlBrowser, CreateFileSystemBrowser);
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
			workItems.Verify(wi => wi.LoadAsync(), Times.Once());
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
			workItems.Verify(wi => wi.LoadAsync(), Times.Exactly(2));
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
			viewModel.SelectedTestCase = Mock.Of<ITestCaseViewModel>();

			// Assert.
			Assert.NotNull(viewModel.SourceControlTestBrowser);
			Assert.Equal(viewModel.SelectedTestCase, viewModel.SourceControlTestBrowser.TestCase);
			Assert.Equal(2, viewModel.SourceControlTestBrowser.Solutions.Count);
		}

		[Fact]
		public void Test_FileSystemBrowser()
		{
			// Arrange.
			viewModel.ServerUri = new Uri("http://test/");

			// Act.
			viewModel.SelectedTestCase = Mock.Of<ITestCaseViewModel>();

			// Assert.
			Assert.NotNull(viewModel.FileSystemTestBrowser);
			Assert.Equal(viewModel.SelectedTestCase, viewModel.FileSystemTestBrowser.TestCase);
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
			workItems.Setup(wi => wi.LoadAsync()).Returns(Task.FromResult<object>(null));
			return workItems.Object;
		}

		private SourceControlTestBrowserViewModel CreateSourceControlBrowser(IEnumerable<TfsSolution> solutions, ITestCaseViewModel testCase)
		{
			return new SourceControlTestBrowserViewModel(solutions, testCase, _ => null);
		}

		private FileSystemTestBrowserViewModel CreateFileSystemBrowser(ITestCaseViewModel testCase)
		{
			return new FileSystemTestBrowserViewModel(testCase, _ => null, Mock.Of<IAutomatedTestDiscoverer>(), new SynchronousTaskScheduler());
		}

		private readonly MainViewModel viewModel;

		private Mock<IWorkItems> workItems;
 		private Mock<ITfsExplorer> explorer;
		private readonly Mock<IVersionControl> versionControl = new Mock<IVersionControl>();
	}
}