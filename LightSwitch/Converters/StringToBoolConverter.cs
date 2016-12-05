using System;
using System.Globalization;
using Xamarin.Forms;

namespace LightSwitch
{
	public class StringToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return false;

			if (((string)value).Trim() == string.Empty)
				return false;

			if (value .Equals("1") || value.Equals("true"))
				return true;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var boolValue = (bool)value;
			return boolValue ? "1" : "0";
		}
	}
}
