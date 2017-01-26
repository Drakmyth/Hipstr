using Hipstr.Client.Services;
using Hipstr.Core.Utility;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Hipstr.Client.Views.Settings
{
	[UsedImplicitly]
	public class SettingsViewModel : ViewModelBase
	{
		private bool _isDarkTheme;

		public bool IsDarkTheme
		{
			get { return _isDarkTheme; }
			set
			{
				_isDarkTheme = value;
				OnPropertyChanged();
			}
		}

		private bool _isLightTheme;

		public bool IsLightTheme
		{
			get { return _isLightTheme; }
			set
			{
				_isLightTheme = value;
				OnPropertyChanged();
			}
		}

		private bool _isSystemTheme;

		public bool IsSystemTheme
		{
			get { return _isSystemTheme; }
			set
			{
				_isSystemTheme = value;
				OnPropertyChanged();
			}
		}

		public SettingsViewModel(IMainPageService mainPageService)
		{
			mainPageService.Title = "Settings";

			InitializeThemeSettings();
		}

		private void InitializeThemeSettings()
		{
			ElementTheme theme = CurrentTheme;

			_isDarkTheme = theme == ElementTheme.Dark;
			_isLightTheme = theme == ElementTheme.Light;
			_isSystemTheme = theme == ElementTheme.Default;

			//			_isDarkTheme = theme.HasValue && theme.Value == ApplicationTheme.Dark;
			//			_isLightTheme = theme.HasValue && theme.Value == ApplicationTheme.Light;
			//			_isSystemTheme = !theme.HasValue;
		}

		private void UpdateThemeSettings()
		{
			if (IsSystemTheme)
			{
				AppSettings.CurrentTheme = ElementTheme.Default;
			}
			else if (IsDarkTheme)
			{
				AppSettings.CurrentTheme = ElementTheme.Dark;
			}
			else if (IsLightTheme)
			{
				AppSettings.CurrentTheme = ElementTheme.Light;
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			switch (propertyName)
			{
				case nameof(IsDarkTheme):
				case nameof(IsLightTheme):
				case nameof(IsSystemTheme):
					UpdateThemeSettings();
					break;
			}

			base.OnPropertyChanged(propertyName);
		}
	}
}