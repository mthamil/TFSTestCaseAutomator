using System.ComponentModel;
using Microsoft.TeamFoundation.TestManagement.Client;
using Moq;
using SharpEssentials.Testing;
using TestCaseAutomator.TeamFoundation.TestCaseAssociation;
using TestCaseAutomator.ViewModels;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.ViewModels
{
	public class TestCaseViewModelTests
	{
		[Fact]
		public void Test_Id()
		{
			// Arrange.
			var testCase = Mock.Of<ITestCase>(tc => tc.Id == 112);
			var vm = new TestCaseViewModel(testCase, automationService.Object);

			// Act.
			var id = vm.Id;

			// Assert.
			Assert.Equal(112, id);
		}

		[Fact]
		public void Test_Title()
		{
			// Arrange.
			var testCase = Mock.Of<ITestCase>(tc => tc.Title == "A test case");
			var vm = new TestCaseViewModel(testCase, automationService.Object);

			// Act.
			var title = vm.Title;

			// Assert.
			Assert.Equal("A test case", title);
		}

		[Fact]
		public void Test_AssociatedAutomation_With_Implementation()
		{
			// Arrange.
			var testCase = Mock.Of<ITestCase>(tc => 
				tc.IsAutomated &&
				tc.Implementation == Mock.Of<ITestImplementation>(ti => 
				ti.DisplayText == "test123"));

			var vm = new TestCaseViewModel(testCase, automationService.Object);

			// Act.
			var implementation = vm.AssociatedAutomation;

			// Assert.
			Assert.Equal("test123", implementation);
		}

		[Fact]
		public void Test_AssociatedAutomation_Without_Implementation()
		{
			// Arrange.
			var testCase = Mock.Of<ITestCase>(tc =>
				tc.IsAutomated == false);

			var vm = new TestCaseViewModel(testCase, automationService.Object);

			// Act.
			var implementation = vm.AssociatedAutomation;

			// Assert.
			Assert.Equal(string.Empty, implementation);
		}

		[Fact]
		public void Test_Title_PropertyChange_Propagates()
		{
			// Arrange.
			var testCase = new Mock<ITestCase>();
			var vm = new TestCaseViewModel(testCase.Object, automationService.Object);

			// Act/Assert.
			AssertThat.PropertyChanged(vm,
				p => p.Title, 
				() => testCase.Raise(tc => tc.PropertyChanged += null, new PropertyChangedEventArgs("Title")));
		}

		[Fact]
		public void Test_Implementation_PropertyChange_Propagates_To_CanRemoveAutomation()
		{
			// Arrange.
			var testCase = new Mock<ITestCase>();
			var vm = new TestCaseViewModel(testCase.Object, automationService.Object);

			// Act/Assert.
			AssertThat.PropertyChanged(vm,
				p => p.CanRemoveAutomation,
				() => testCase.Raise(tc => tc.PropertyChanged += null, new PropertyChangedEventArgs("Implementation")));
		}

		[Fact]
		public void Test_Implementation_PropertyChange_Propagates_To_AssociatedAutomation()
		{
			// Arrange.
			var testCase = new Mock<ITestCase>();
			var vm = new TestCaseViewModel(testCase.Object, automationService.Object);

			// Act/Assert.
			AssertThat.PropertyChanged(vm,
				p => p.AssociatedAutomation,
				() => testCase.Raise(tc => tc.PropertyChanged += null, new PropertyChangedEventArgs("Implementation")));
		}

		private readonly Mock<ITestCaseAutomationService> automationService = new Mock<ITestCaseAutomationService>();
	}
}