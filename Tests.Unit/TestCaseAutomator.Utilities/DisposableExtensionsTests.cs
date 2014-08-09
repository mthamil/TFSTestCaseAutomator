using System;
using Moq;
using TestCaseAutomator.Utilities;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Utilities
{
	public class DisposableExtensionsTests
	{
		[Fact]
		public void Test_DisposeSafely()
		{
			// Arrange.
			var disposable = new Mock<IDisposable>();

			// Act.
			disposable.Object.DisposeSafely();

			// Assert.
			disposable.Verify(d => d.Dispose());
		}

		[Fact]
		public void Test_DisposeSafely_DoesNotThrow_ForNull()
		{
			// Arrange.
			IDisposable disposable = null;

			// Act/Assert.
			Assert.DoesNotThrow(() => disposable.DisposeSafely());
		}

	    [Fact]
	    public void Test_Lazy_Dispose_Does_Not_Call_Dispose_When_Value_Not_Created()
	    {
	        // Arrange.
	        var disposable = new Mock<IDisposable>();
	        var lazy = new Lazy<IDisposable>(() => disposable.Object);

	        // Act.
            lazy.Dispose();

	        // Assert.
            disposable.Verify(d => d.Dispose(), Times.Never);
	    }

        [Fact]
        public void Test_Lazy_Dispose_Calls_Dispose_When_Value_Created()
        {
            // Arrange.
            var disposable = new Mock<IDisposable>();
            var lazy = new Lazy<IDisposable>(() => disposable.Object);

            // Act.
            var value = lazy.Value; // Trigger value intialization.
            lazy.Dispose();

            // Assert.
            disposable.Verify(d => d.Dispose(), Times.Once);
        }
	}
}