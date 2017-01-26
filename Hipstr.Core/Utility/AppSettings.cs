using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Hipstr.Core.Utility
{
	public static class AppSettings
	{
		public static event EventHandler<ElementTheme> CurrentThemeChanged;

		public static ElementTheme CurrentTheme
		{
			get { return LoadSetting(ElementTheme.Default); }
			set
			{
				SaveSetting(value);
				CurrentThemeChanged?.Invoke(nameof(AppSettings), value);
			}
		}

		private static T LoadSetting<T>(T defaultValue, [CallerMemberName] string setting = null)
		{
			var value = LoadSetting<T>(setting);
			bool isNullable = typeof(T).GetTypeInfo().IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);
			return value == null && !isNullable ? defaultValue : value;
		}

		private static T LoadSetting<T>([CallerMemberName] string setting = null)
		{
			Type type = typeof(T);

			bool isNullable = type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
			if (isNullable)
			{
				type = type.GetGenericArguments().First();
			}

			bool isEnum = type.GetTypeInfo().IsEnum;
			if (isEnum)
			{
				object value = ApplicationData.Current.LocalSettings.Values[setting];
				if (value == null)
				{
					return (T)value;
				}
				return (T)Enum.Parse(type, (string)value);
			}

			return (T)ApplicationData.Current.LocalSettings.Values[setting];
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

		private static void SaveSetting<T>(T value, [CallerMemberName] string setting = null)
		{
			Type type = typeof(T);

			bool isNullable = type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
			if (isNullable)
			{
				type = type.GetGenericArguments().First();
			}

			bool isEnum = type.GetTypeInfo().IsEnum;
			if (isEnum)
			{
				ApplicationData.Current.LocalSettings.Values[setting] = value?.ToString();
			}
			else
			{
				ApplicationData.Current.LocalSettings.Values[setting] = value;
			}
		}
	}
}