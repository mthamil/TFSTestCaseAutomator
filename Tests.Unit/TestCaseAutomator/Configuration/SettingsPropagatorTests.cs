using System;
using System.ComponentModel;
using Moq;
using TestCaseAutomator.Configuration;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Configuration
{
	public class SettingsPropagatorTests
	{
		public SettingsPropagatorTests()
		{
			propagator = new SettingsPropagator(settings.Object, application.Object);
		}

		[Fact]
		public void Test_Application_Closing_Saves_Settings()
		{
			// Act.
			application.Raise(dm => dm.Closing += null, EventArgs.Empty);

			// Assert.
			settings.Verify(s => s.Save());
		}

		[Fact]
		public void Test_Application_ServerUri_Changes_Updates_Settings()
		{
			// Arrange.
			application.SetupGet(a => a.ServerUri).Returns(new Uri("http://test/"));

			// Act.
			application.Raise(dm => dm.PropertyChanged += null, new PropertyChangedEventArgs("ServerUri"));

			// Assert.
			settings.VerifySet(s => s.TfsServerLocation = new Uri("http://test/"));
		}

		[Fact]
		public void Test_Application_ProjectName_Changes_Updates_Settings()
		{
			// Arrange.
			application.SetupGet(a => a.ProjectName).Returns("TestProject");

			// Act.
			application.Raise(dm => dm.PropertyChanged += null, new PropertyChangedEventArgs("ProjectName"));

			// Assert.
			settings.VerifySet(s => s.TfsProjectName = "TestProject");
		}

		private readonly SettingsPropagator propagator;

 		private readonly Mock<ISettings> settings = new Mock<ISettings>();
		private readonly Mock<IApplication> application = new Mock<IApplication>();
	}
}