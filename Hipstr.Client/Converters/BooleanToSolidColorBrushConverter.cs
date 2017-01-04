using System;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Hipstr.Client.Converters
{
	public class BooleanToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			string[] parameters = (parameter as string)?.Split('|') ?? new[] {nameof(Colors.White), nameof(Colors.DimGray)};

			PropertyInfo trueColorProp = typeof(Colors).GetProperty(parameters[0]);
			PropertyInfo falseColorProp = typeof(Colors).GetProperty(parameters[1]);

			var trueColor = (Color)(trueColorProp?.GetValue(null) ?? Colors.White);
			var falseColor = (Color)(falseColorProp?.GetValue(null) ?? Colors.DimGray);

			return value is bool && (bool)value ? new SolidColorBrush(trueColor) : new SolidColorBrush(falseColor);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var brush = value as SolidColorBrush;
			return brush?.Color == Colors.White;
		}
	}
}