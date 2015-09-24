using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Moq;
using SharpEssentials.Testing;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Configuration
{
	public class SettingsPropagatorTests
	{
		public SettingsPropagatorTests()
		{
		    _settings.SetupGet(s => s.TfsServers).Returns(new List<Uri>());
		    _application.SetupGet(s => s.ServerUris).Returns(new ObservableCollection<Uri>());

			_underTest = new SettingsPropagator(_settings.Object, _application.Object);
		}

		[Fact]
		public void Test_Application_Closing_Saves_Settings()
		{
			// Act.
			_application.Raise(dm => dm.Closing += null, EventArgs.Empty);

			// Assert.
			_settings.Verify(s => s.Save());
		}

		[Fact]
		public void Test_Application_ServerUris_Uri_Added()
		{
			// Act.
			_application.Object.ServerUris.Add(new Uri("http://testserver/"));

			// Assert.
			AssertThat.SequenceEqual(new[] { new Uri("http://testserver/") }, _settings.Object.TfsServers);
		}

        [Fact]
        public void Test_Application_ServerUris_Uri_Removed()
        {
            _application.Object.ServerUris.Add(new Uri("http://testserver/"));
            _application.Object.ServerUris.Add(new Uri("http://testserver2/"));

            // Act.
            _application.Object.ServerUris.Remove(new Uri("http://testserver/"));

            // Assert.
            AssertThat.SequenceEqual(new[] { new Uri("http://testserver2/") }, _settings.Object.TfsServers);
        }

        [Fact]
		public void Test_Application_ProjectName_Changes_Updates_Settings()
		{
			// Arrange.
			_application.SetupGet(a => a.ProjectName).Returns("TestProject");

			// Act.
			_application.Raise(dm => dm.PropertyChanged += null, new PropertyChangedEventArgs("ProjectName"));

			// Assert.
			_settings.VerifySet(s => s.TfsProjectName = "TestProject");
		}

		private readonly SettingsPropagator _underTest;

 		private readonly Mock<ISettings> _settings = new Mock<ISettings>();
		private readonly Mock<IApplication> _application = new Mock<IApplication>();
	}
}