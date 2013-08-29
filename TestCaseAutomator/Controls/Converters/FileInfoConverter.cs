using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TestCaseAutomator.Controls.Converters
{
	/// <summary>
	/// Converts between strings and <see cref="FileInfo"/>.
	/// </summary>
	[ValueConversion(typeof(FileInfo), typeof(string))]
	public class FileInfoConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var file = value as FileInfo;
			if (file == null)
				return string.Empty;

			return file.FullName;
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string path = value as string;
			if (String.IsNullOrEmpty(path))
				return null;

			return new FileInfo(path);
		}

		#endregion
	}
}