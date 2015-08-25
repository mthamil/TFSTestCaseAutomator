using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation;
using Moq;
using SharpEssentials.Testing;
using TestCaseAutomator.TeamFoundation;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
	public class MainViewModelTests
	{
		public MainViewModelTests()
		{
		    _workItems.Setup(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()))
		              .Returns(Task.FromResult<object>(null));

            _explorer.SetupGet(e => e.Server).Returns(_server.Object);
		    _explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
		             .Returns((string name) => Mock.Of<ITfsProjectWorkItemCollection>(wi => wi.ProjectName == name));

		    _underTest = new MainViewModel(_explorer.Object,
		                                   _workItems.Object,
		                                   new TestSelectionViewModel(null));
		}

		[Fact]
		public void Test_ServerUri_Updates_Explorer()
		{
			// Act.
			_underTest.ServerUri = new Uri("http://test/");

			// Assert.
            _explorer.Verify(e => e.Connect(new Uri("http://test/")));
		}

		[Fact]
		public void Test_ProjectName_Updates_WorkItems()
		{
			// Arrange.
			_underTest.ServerUri = new Uri("http://test/");

			// Act.
			_underTest.ProjectName = "TestProject";

			// Assert.
			Assert.NotNull(_workItems);
            _workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Once());
		}

		[Fact]
		public void Test_ProjectName_With_No_Server()
		{
            // Arrange.
		    _explorer.SetupGet(e => e.Server)
		             .Returns((ITfsServer)null);

			// Act.
			_underTest.ProjectName = "TestProject";

			// Assert.
            _workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Never);
		}

		[Fact]
		public void Test_ProjectName_Status_When_TFS_Unavailable()
		{
			// Arrange.
			_underTest.ServerUri = new Uri("http://test/");

		    _explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
		             .Throws(new TeamFoundationServiceUnavailableException(""));

			// Act.
			_underTest.ProjectName = "TestProject";

			// Assert.
			Assert.NotNull(_underTest.Status);
		}

		[Fact]
		public void Test_Refresh()
		{
			// Arrange.
			_underTest.ServerUri = new Uri("http://test/");
			_underTest.ProjectName = "TestProject";

			// Act.
			_underTest.Connect();

			// Assert.
            _workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Exactly(2));
		}

        [Fact]
		public void Test_CloseCommand_Raises_Closing_Event()
		{
			// Act/Assert.
			AssertThat.Raises<IApplication>(_underTest, 
				vm => vm.Closing += null, 
				() => _underTest.CloseCommand.Execute(null));
		}

		private readonly MainViewModel _underTest;

 		private readonly Mock<ITfsExplorer> _explorer = new Mock<ITfsExplorer>();
        private readonly Mock<ITfsServer> _server = new Mock<ITfsServer>();
        private readonly Mock<IWorkItems> _workItems = new Mock<IWorkItems>();
		private readonly Mock<IVersionControl> _versionControl = new Mock<IVersionControl>();
	}
}