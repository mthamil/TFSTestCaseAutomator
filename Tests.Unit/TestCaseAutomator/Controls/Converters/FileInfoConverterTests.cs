using System.Globalization;
using System.IO;
using TestCaseAutomator.Controls.Converters;
using Xunit;

namespace Tests.Unit.TestCaseAutomator.Controls.Converters
{
	public class FileInfoConverterTests
	{
		[Fact]
		public void Test_Convert()
		{
			// Arrange.
			var file = new FileInfo(@"C:\testfile.txt");

			// Act.
			var value = converter.Convert(file, typeof(FileInfo), null, CultureInfo.InvariantCulture);

			// Assert.
			var path = Assert.IsType<string>(value);
			Assert.Equal(@"C:\testfile.txt", path);
		}

		[Fact]
		public void Test_Convert_When_Value_Is_Null()
		{
			// Act.
			var value = converter.Convert(null, typeof(FileInfo), null, CultureInfo.InvariantCulture);

			// Assert.
			var path = Assert.IsType<string>(value);
			Assert.Equal(string.Empty, path);
		}

		[Fact]
		public void Test_ConvertBack()
		{
			// Arrange.
			var path = @"C:\testfile.txt";

			// Act.
			var value = converter.ConvertBack(path, typeof(string), null, CultureInfo.InvariantCulture);

			// Assert.
			var file = Assert.IsType<FileInfo>(value);
			Assert.Equal(@"C:\testfile.txt", file.FullName);
		}

		[Fact]
		public void Test_ConvertBack_When_Value_Is_Null()
		{
			// Act.
			var value = converter.ConvertBack(null, typeof(string), null, CultureInfo.InvariantCulture);

			// Assert.
			Assert.Equal(null, value);
		}

		private readonly FileInfoConverter converter = new FileInfoConverter();
	}
}