using System;
using System.Collections.Generic;
using TestCaseAutomator.Utilities.Reflection;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.TestCaseAutomator.Utilities.Reflection
{
	public class ReflectionExtensionsTests
	{
		[Fact]
		public void Test_GetDefaultValue_ReferenceType()
		{
			// Act.
			object defaultValue = typeof(string).GetDefaultValue();

			// Assert.
			Assert.Equal(null, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Integer()
		{
			// Act.
			object defaultValue = typeof(int).GetDefaultValue();

			// Assert.
			Assert.Equal(0, defaultValue);
		}

		[Fact]
		public void Test_GetDefaultValue_Boolean()
		{
			// Act.
			object defaultValue = typeof(bool).GetDefaultValue();

			// Assert.
			Assert.Equal(false, defaultValue);
		}

	    [Theory]
        [InlineData(typeof(IList<string>), typeof(IList<>), true)]
        [InlineData(typeof(List<string>), typeof(List<>), true)]
        [InlineData(typeof(string), typeof(List<>), false)]
        public void Test_IsClosedTypeOf(Type closed, Type open, bool expected)
	    {
	        // Act.
	        bool actual = closed.IsClosedTypeOf(open);

            // Assert.
            Assert.Equal(expected, actual);
	    }

        [Theory]
        [InlineData(typeof(string), typeof(IList<object>))]
        [InlineData(typeof(string), typeof(List<object>))]
        [InlineData(typeof(string), typeof(object))]
        public void Test_IsClosedTypeOf_With_NonOpen_Type(Type closed, Type open)
        {
            // Act/Assert.
            Assert.Throws<ArgumentException>(() => 
                closed.IsClosedTypeOf(open));
        }
	}
}
