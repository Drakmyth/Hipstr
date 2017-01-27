using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Hipstr.Core.Utility
{
	[UsedImplicitly]
	public class AppSettings : IAppSettings
	{
		public event EventHandler<ElementTheme> CurrentThemeChanged;

		public ElementTheme CurrentTheme
		{
			get { return LoadEnumSetting(ElementTheme.Default, typeof(ElementTheme)); }
			set
			{
				SaveEnumSetting(value);
				CurrentThemeChanged?.Invoke(nameof(AppSettings), value);
			}
		}

		private static T LoadEnumSetting<T>(T defaultValue, Type type, [CallerMemberName] string setting = null)
		{
			object value = ApplicationData.Current.LocalSettings.Values[setting];
			if (value == null)
			{
				return defaultValue;
			}

			return (T)Enum.Parse(type, (string)value);
		}

		private static T LoadSetting<T>(T defaultValue, [CallerMemberName] string setting = null)
		{
			object value = ApplicationData.Current.LocalSettings.Values[setting];
			if (value == null)
			{
				return defaultValue;
			}

			return (T)value;
		}

		/* Application Data supports the following types:
		 * 
		 * UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double
		 * Boolean
		 * Char16, String
		 * DateTime, TimeSpan
		 * GUID, Point, Size, Rect
		 * ApplicationDataCompositeValue
		 */

		private static void SaveEnumSetting(object value, [CallerMemberName] string setting = null)
		{
			ApplicationData.Current.LocalSettings.Values[setting] = value?.ToString();
		}

		private void SaveSetting(object value, [CallerMemberName] string setting = null)
		{
			ApplicationData.Current.LocalSettings.Values[setting] = value;
		}
	}
}