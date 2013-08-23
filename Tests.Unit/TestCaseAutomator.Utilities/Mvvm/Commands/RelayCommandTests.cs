using TestCaseAutomator.Utilities.Mvvm.Commands;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Utilities.Mvvm.Commands
{
	public class RelayCommandTests
	{
		[Fact]
		public void Test_WithParameter_CanExecute_NoPredicate()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { });

			// Act.
			bool actual = command.CanExecute(false);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithParameter_CanExecute_WrongType()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { }, b => b);

			// Act.
			bool actual = command.CanExecute("parameter");

			// Assert.
			Assert.False(actual);
		}

		[Fact]
		public void Test_WithParameter_CanExecute()
		{
			// Arrange.
			var command = new RelayCommand<bool>(b => { }, b => b);

			// Act.
			bool actual = command.CanExecute(true);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithParameter_Execute()
		{
			// Arrange.
			bool executed = false;
			var command = new RelayCommand<bool>(b => { executed = true; });

			// Act.
			command.Execute(true);

			// Assert.
			Assert.True(executed);
		}

		[Fact]
		public void Test_WithoutParameter_CanExecute_NoPredicate()
		{
			// Arrange.
			var command = new RelayCommand(() => { });

			// Act.
			bool actual = command.CanExecute(false);

			// Assert.
			Assert.True(actual);
		}

		[Fact]
		public void Test_WithoutParameter_CanExecute()
		{
			// Arrange.
			var command = new RelayCommand(() => { }, () => false);

			// Act.
			bool actual = command.CanExecute(true);

			// Assert.
			Assert.False(actual);
		}

		[Fact]
		public void Test_WithoutParameter_Execute()
		{
			// Arrange.
			bool executed = false;
			var command = new RelayCommand(() => { executed = true; });

			// Act.
			command.Execute(null);

			// Assert.
			Assert.True(executed);
		}
	}
}