using Hipstr.Client.Dialogs;
using System;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class ValidationResultToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var validationResult = value as ValidationResult;
			return validationResult?.Reason ?? "Error converting ValidationResult to String";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var reason = value as string;
			return string.IsNullOrEmpty(reason) ? ValidationResult.Valid() : ValidationResult.Invalid(reason);
		}
	}
}