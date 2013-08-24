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
			settings["TestDiscoveryPluginLocation"] = @"C:\Plugins";

			// Act.
			var appSettings = new DotNetSettings(settings);

			// Assert.
			Assert.Equal("http://testserver/", appSettings.TfsServerLocation.ToString());
			Assert.Equal(@"C:\Plugins", appSettings.TestDiscoveryPluginLocation.FullName);
		}

		[Fact]
		public void Test_Save()
		{
			// Arrange.
			var appSettings = new DotNetSettings(settings)
			{
				TfsServerLocation = new Uri("http://testserver/"),
			};

			appSettings.TfsServerLocation = new Uri("http://testserver2/");

			// Act.
			appSettings.Save();

			// Assert.
			Assert.Equal("http://testserver2/", settings.TFSServerUrl.ToString());
		}

		[Fact]
		public void Test_TfsServerLocationChanges_Changes()
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

		private readonly Settings settings = new Settings();
	}
}