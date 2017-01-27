using System;
using Windows.UI.Xaml;

namespace Hipstr.Core.Utility
{
	public interface IAppSettings
	{
		event EventHandler<ElementTheme> CurrentThemeChanged;

		ElementTheme CurrentTheme { get; set; }
	}
}