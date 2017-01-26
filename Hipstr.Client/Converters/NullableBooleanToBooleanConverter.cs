using System;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class NullableBooleanToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var val = (bool?)value;
			return val.HasValue && val.Value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return (bool?)value;
		}
	}
}