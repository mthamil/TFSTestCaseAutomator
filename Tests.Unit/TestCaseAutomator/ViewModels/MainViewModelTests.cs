﻿using System;
using System.Collections.Generic;
using System.Linq;
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
		    _testCases.Setup(t => t.LoadAsync(It.IsAny<string>()))
		              .Returns(Task.FromResult<object>(null));

		    _testCases.SetupGet(t => t.Items).Returns(new List<ITestCaseViewModel>());

            _explorer.SetupGet(e => e.Server).Returns(_server.Object);

		    _underTest = new MainViewModel(_explorer.Object,
                                           new ServerManagementViewModel(Enumerable.Empty<Uri>()), 
		                                   _testCases.Object);
		}

		[Fact]
		public async Task Test_ProjectName_Updates_WorkItems()
		{
			// Arrange.
			_underTest.Servers.CurrentUri = new Uri("http://test/");
            await _underTest.ConnectAsync();

            // Act.
            _underTest.ProjectName = "TestProject";

			// Assert.
            _testCases.Verify(t => t.LoadAsync("TestProject"), Times.Once());
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
            _testCases.Verify(t => t.LoadAsync("TestProject"), Times.Never);
		}

		[Fact]
		public async Task Test_ProjectName_Status_When_TFS_Unavailable()
		{
			// Arrange.
		    _testCases.Setup(t => t.LoadAsync(It.IsAny<string>()))
		              .Throws(new TeamFoundationServiceUnavailableException(""));

            _underTest.Servers.CurrentUri = new Uri("http://test/");
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
			_underTest.Servers.CurrentUri = new Uri("http://test/");
            _underTest.ProjectName = "TestProject";

            // Act.
            await _underTest.ConnectAsync();

			// Assert.
            Assert.True(_underTest.IsConnected);
            _testCases.Verify(t => t.LoadAsync(It.IsAny<string>()), Times.Never);  // ProjectName will be null.
            Assert.Contains(new Uri("http://test/"), _underTest.Servers.All.Select(s => s.Uri));
        }

        [Fact]
        public async Task Test_Reconnect()
        {
            // Arrange.
            _underTest.Servers.CurrentUri = new Uri("http://test/");
            await _underTest.ConnectAsync();

            _underTest.ProjectName = "TestProject";

            // Act.
            await _underTest.ConnectAsync();

            // Assert.
            Assert.True(_underTest.IsConnected);
            _testCases.Verify(t => t.LoadAsync("TestProject"), Times.Once);
        }

        [Theory]
        [InlineData(true, "http://test/")]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public void Test_CanConnect(bool expected, string serverUri)
        {
            // Arrange.
            _underTest.Servers.CurrentUri = serverUri == null
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
            _underTest.Servers.CurrentUri = new Uri(serverUri);
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
        private readonly Mock<ITestCases> _testCases = new Mock<ITestCases>();
		private readonly Mock<IVersionControl> _versionControl = new Mock<IVersionControl>();
	}
}