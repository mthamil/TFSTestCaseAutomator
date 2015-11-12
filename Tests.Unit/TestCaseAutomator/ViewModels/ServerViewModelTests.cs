using System;
using System.Windows.Input;
using Moq;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
    public class ServerViewModelTests
    {
        [Theory]
        [InlineData(true,  @"http://example.com", @"http://example.com")]
        [InlineData(true,  @"http://example.com", @"http://EXAMPLE.com")]
        [InlineData(true,  @"http://example.com", @"http://example.com/")]
        [InlineData(false, @"http://example.com", @"https://example.com")]
        [InlineData(false, @"http://example.com", @"http://example")]
        public void Test_Uri_Equals(bool expected, string firstUri, string secondUri)
        {
            // Arrange.
            var firstServer = new ServerViewModel(new Uri(firstUri), Mock.Of<ICommand>());
            var secondServer = new ServerViewModel(new Uri(secondUri), Mock.Of<ICommand>());

            // Act.
            bool actual = firstServer.Equals(secondServer);

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true,  @"http://example.com/", @"http://example.com/")]
        [InlineData(true,  @"http://example.com/", @"http://EXAMPLE.com")]
        [InlineData(true,  @"http://example.com",  @"http://example.com")]
        [InlineData(false, @"https://example.com", @"http://example.com")]
        [InlineData(false, @"https://example.com", @"http://example.net")]
        public void Test_GetHashCode(bool expected, string testUri, string controlUri)
        {
            // Arrange.
            var underTest = new ServerViewModel(new Uri(testUri), Mock.Of<ICommand>());
            var control = new ServerViewModel(new Uri(controlUri), Mock.Of<ICommand>()).GetHashCode();

            // Act.
            int actual = underTest.GetHashCode();

            // Assert.
            if (expected)
                Assert.Equal(control, actual);
            else
                Assert.NotEqual(control, actual);
        }
    }
}