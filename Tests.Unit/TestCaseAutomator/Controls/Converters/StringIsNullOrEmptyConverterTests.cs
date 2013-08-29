using System.Globalization;
using TestCaseAutomator.Controls.Converters;
using Xunit;
using Xunit.Extensions;

namespace Tests.Unit.TestCaseAutomator.Controls.Converters
{
	public class StringIsNullOrEmptyConverterTests
	{
		[Theory]
		[InlineData(true, null)]
		[InlineData(true, "")]
		[InlineData(false, "t")]
		[InlineData(false, "testing")]
		public void Test_Convert(bool expected, string input)
		{
			// Act.
			var value = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

			// Assert.
			var actual = Assert.IsType<bool>(value);
			Assert.Equal(expected, actual);
		}

		private readonly StringIsNullOrEmptyConverter converter = new StringIsNullOrEmptyConverter();
	}
}