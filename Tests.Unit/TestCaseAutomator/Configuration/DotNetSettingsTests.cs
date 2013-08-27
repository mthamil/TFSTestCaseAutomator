using System;
using TestCaseAutomator.Configuration;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Configuration
{
	public class DotNetSettingsTests
	{
		[Fact]
		public void Test_Initialization()
		{
			// Arrange.
			settings.TFSServerUrl = new Uri("http://testserver/");
			settings.TFSProjectName = "project1";
			settings["TestDiscoveryPluginLocation"] = @"C:\Plugins";

			// Act.
			var appSettings = new DotNetSettings(settings);

			// Assert.
			Assert.Equal("http://testserver/", appSettings.TfsServerLocation.ToString());
			Assert.Equal("project1", appSettings.TfsProjectName);
			Assert.Equal(@"C:\Plugins", appSettings.TestDiscoveryPluginLocation.FullName);
		}

		[Fact]
		public void Test_Save()
		{
			// Arrange.
			var appSettings = new DotNetSettings(settings)
			{
				TfsServerLocation = new Uri("http://testserver/"),
				TfsProjectName = "project1"
			};

			appSettings.TfsServerLocation = new Uri("http://testserver2/");
			appSettings.TfsProjectName = "project2";

			// Act.
			appSettings.Save();

			// Assert.
			Assert.Equal("http://testserver2/", settings.TFSServerUrl.ToString());
			Assert.Equal("project2", settings.TFSProjectName);
		}

		[Fact]
		public void Test_TfsServerLocation_Changes()
		{
			// Arrange.
			settings.TFSServerUrl = new Uri("http://testserver/");

			var appSettings = new DotNetSettings(settings);

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.TfsServerLocation,
				() => appSettings.TfsServerLocation = new Uri("http://testserver2/"));

			Assert.Equal("http://testserver2/", appSettings.TfsServerLocation.ToString());
		}

		[Fact]
		public void Test_TfsProjectName_Changes()
		{
			// Arrange.
			settings.TFSProjectName = "project1";

			var appSettings = new DotNetSettings(settings);

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.TfsProjectName,
				() => appSettings.TfsProjectName = "project2");

			Assert.Equal("project2", appSettings.TfsProjectName);
		}

		private readonly Settings settings = new Settings();
	}
}