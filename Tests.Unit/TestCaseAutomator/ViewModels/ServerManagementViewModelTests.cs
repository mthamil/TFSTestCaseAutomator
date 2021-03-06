using System;
using System.Linq;
using SharpEssentials.Testing;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
    public class ServerManagementViewModelTests
    {
        [Fact]
        public void Test_Creation_With_Zero_Uris()
        {
            // Arrange.
            var serverLocations = Enumerable.Empty<Uri>();

            // Act.
            var underTest = new ServerManagementViewModel(serverLocations);

            // Assert.
            Assert.Equal(null, underTest.CurrentUri);
            AssertThat.SequenceEqual(serverLocations, underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Creation_With_One_Uri()
        {
            // Arrange.
            var serverLocations = new[] { new Uri("http://test") };

            // Act.
            var underTest = new ServerManagementViewModel(serverLocations);

            // Assert.
            Assert.Equal(new Uri("http://test"), underTest.CurrentUri);
            AssertThat.SequenceEqual(serverLocations, underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Creation_With_Multiple_Uris()
        {
            // Arrange.
            var serverLocations = new[] { new Uri("http://test"), new Uri("http://test2"), new Uri("https://test3") };

            // Act.
            var underTest = new ServerManagementViewModel(serverLocations);

            // Assert.
            Assert.Equal(new Uri("http://test"), underTest.CurrentUri);
            AssertThat.SequenceEqual(serverLocations, underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Add_With_No_Servers()
        {
            // Arrange.
            var underTest = new ServerManagementViewModel(Enumerable.Empty<Uri>());

            // Act.
            underTest.Add(new Uri("http://test"));

            // Assert.
            AssertThat.SequenceEqual(new[] { new Uri("http://test") }, underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Add_With_Multiple_Servers()
        {
            // Arrange.
            var underTest = new ServerManagementViewModel(new[] { new Uri("http://test"), new Uri("http://test2") });

            // Act.
            underTest.Add(new Uri("http://test3"));

            // Assert.
            AssertThat.SequenceEqual(new[] { new Uri("http://test3"), new Uri("http://test"), new Uri("http://test2") }, 
                                     underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Add_With_Existing_Server()
        {
            // Arrange.
            var underTest = new ServerManagementViewModel(new[] { new Uri("http://test"), new Uri("http://test2") });

            // Act.
            underTest.Add(new Uri("http://test2"));

            // Assert.
            AssertThat.SequenceEqual(new[] { new Uri("http://test2"), new Uri("http://test") },
                                     underTest.All.Select(s => s.Uri));
        }

        [Fact]
        public void Test_Server_Forget()
        {
            // Arrange.
            var underTest = new ServerManagementViewModel(new[] { new Uri("http://test"), new Uri("http://test2") });
            var server = underTest.All.Last();

            // Act.
            server.ForgetCommand.Execute(null);

            // Assert.
            AssertThat.SequenceEqual(new[] { new Uri("http://test") }, underTest.All.Select(s => s.Uri));
        }
    }
}
