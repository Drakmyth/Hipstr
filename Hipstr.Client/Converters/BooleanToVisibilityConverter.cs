using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public bool InvertResult { get; set; } = false;

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var val = (bool)value;
			if (InvertResult)
			{
				val = !val;
			}
			return val ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			bool val = (Visibility)value == Visibility.Visible;
			if (InvertResult)
			{
				val = !val;
			}
			return val;
		}
	}
}