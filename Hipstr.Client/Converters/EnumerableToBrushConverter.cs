using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Hipstr.Client.Converters
{
	public class EnumerableToBrushConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty EmptyColorProperty = DependencyProperty.Register(nameof(EmptyColor), typeof(Brush), typeof(EnumerableToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));
		public static readonly DependencyProperty NotEmptyColorProperty = DependencyProperty.Register(nameof(NotEmptyColor), typeof(Brush), typeof(EnumerableToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.White)));

		public Brush EmptyColor
		{
			get { return (Brush)GetValue(EmptyColorProperty); }
			set { SetValue(EmptyColorProperty, value); }
		}

		public Brush NotEmptyColor
		{
			get { return (Brush)GetValue(NotEmptyColorProperty); }
			set { SetValue(NotEmptyColorProperty, value); }
		}

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var collection = (IEnumerable<object>)value;
			return collection.Any() ? NotEmptyColor : EmptyColor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}