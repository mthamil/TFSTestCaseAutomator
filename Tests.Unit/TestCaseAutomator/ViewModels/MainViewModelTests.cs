using System;
using System.Collections.Generic;
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

		    _workItems.SetupGet(wi => wi.TestCases).Returns(new List<ITestCaseViewModel>());

            _explorer.SetupGet(e => e.Server).Returns(_server.Object);
		    _explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
		             .Returns((string name) => Mock.Of<ITfsProjectWorkItemCollection>(wi => wi.ProjectName == name));

		    _underTest = new MainViewModel(_explorer.Object,
		                                   _workItems.Object,
		                                   new TestSelectionViewModel(null));
		}

		[Fact]
		public async Task Test_ProjectName_Updates_WorkItems()
		{
			// Arrange.
			_underTest.ServerUri = new Uri("http://test/");
            await _underTest.ConnectAsync();

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
		public async Task Test_ProjectName_Status_When_TFS_Unavailable()
		{
			// Arrange.
		    _explorer.Setup(e => e.WorkItems(It.IsAny<string>()))
		             .Throws(new TeamFoundationServiceUnavailableException(""));

            _underTest.ServerUri = new Uri("http://test/");
		    await _underTest.ConnectAsync();

            // Act.
            _underTest.ProjectName = "TestProject";

			// Assert.
			Assert.NotNull(_underTest.Status);
		}

		[Fact]
		public async Task Test_Connect()
		{
			// Arrange.
			_underTest.ServerUri = new Uri("http://test/");
            _underTest.ProjectName = "TestProject";

            // Act.
            await _underTest.ConnectAsync();

			// Assert.
            Assert.True(_underTest.IsConnected);
            _workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Never);  // ProjectName will be null.
            Assert.Contains(new Uri("http://test/"), _underTest.ServerUris);
        }

        [Fact]
        public async Task Test_Reconnect()
        {
            // Arrange.
            _underTest.ServerUri = new Uri("http://test/");
            await _underTest.ConnectAsync();

            _underTest.ProjectName = "TestProject";

            // Act.
            await _underTest.ConnectAsync();

            // Assert.
            Assert.True(_underTest.IsConnected);
            _workItems.Verify(wi => wi.LoadAsync(It.IsAny<ITfsProjectWorkItemCollection>()), Times.Once);
        }

        [Theory]
        [InlineData(true, "http://test/")]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public void Test_CanConnect(bool expected, string serverUri)
        {
            // Arrange.
            _underTest.ServerUri = serverUri == null 
                ? null 
                : new Uri(serverUri, UriKind.RelativeOrAbsolute);

            // Act.
            bool actual = _underTest.CanConnect;

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true, "http://test/")]
        [InlineData(true, "http://test")]
        [InlineData(false, "http://test2/")]
        public async Task Test_CanRefresh(bool expected, string serverUri)
        {
            // Arrange.
            _underTest.ServerUri = new Uri(serverUri);
            _server.SetupGet(s => s.Uri).Returns(new Uri("http://test/"));
            await _underTest.ConnectAsync();

            // Act.
            bool actual = _underTest.CanRefresh;

            // Assert.
            Assert.Equal(expected, actual);
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