using System;
using System.Globalization;
using System.Windows.Data;

namespace TestCaseAutomator.Controls.Converters
{
	/// <summary>
	/// A converter that returns true if a string is null or empty.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(string))]
	public class StringIsNullOrEmptyConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <see cref="IValueConverter.Convert"/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var s = value as string;
			return String.IsNullOrEmpty(s);
		}

		/// <see cref="IValueConverter.ConvertBack"/>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Cannot convert back.");
		}

		#endregion
	}
}