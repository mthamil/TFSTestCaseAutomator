using System;
using System.Collections.Specialized;
using System.Linq;
using SharpEssentials.Testing;
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
		    _settings.TFSServerUrls = new StringCollection { "http://testserver/" };
			_settings.TFSProjectName = "project1";
			_settings["TestDiscoveryPluginLocation"] = @"C:\Plugins";

			// Act.
			var appSettings = new DotNetSettings(_settings);

			// Assert.
			AssertThat.SequenceEqual(new [] { new Uri("http://testserver/") }, appSettings.TfsServers);
			Assert.Equal("project1", appSettings.TfsProjectName);
			Assert.Equal(@"C:\Plugins", appSettings.TestDiscoveryPluginLocation.FullName);
		}

		[Fact]
		public void Test_Save()
		{
            // Arrange.
            _settings.TFSServerUrls = new StringCollection { "http://testserver/" };
            var appSettings = new DotNetSettings(_settings)
			{
				TfsProjectName = "project1"
			};

			appSettings.TfsServers.Add(new Uri("http://testserver2/"));
			appSettings.TfsProjectName = "project2";

			// Act.
			appSettings.Save();

			// Assert.
			AssertThat.SequenceEqual(new [] { "http://testserver/", "http://testserver2/" }, _settings.TFSServerUrls.Cast<string>());
			Assert.Equal("project2", _settings.TFSProjectName);
		}

		[Fact]
		public void Test_TfsProjectName_Changes()
		{
			// Arrange.
			_settings.TFSProjectName = "project1";

			var appSettings = new DotNetSettings(_settings);

			// Act/Assert.
			AssertThat.PropertyChanged(appSettings,
				s => s.TfsProjectName,
				() => appSettings.TfsProjectName = "project2");

			Assert.Equal("project2", appSettings.TfsProjectName);
		}

		private readonly Settings _settings = new Settings();
	}
}