using System;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class IdentityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}