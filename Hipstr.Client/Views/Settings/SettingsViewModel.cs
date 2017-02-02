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

		public SettingsViewModel() : base("Settings")
		{
			_isDarkTheme = false;
			_isLightTheme = false;
			_isSystemTheme = true;
		}

		public override void Initialize()
		{
			base.Initialize();

			ElementTheme theme = CurrentTheme;

			IsDarkTheme = theme == ElementTheme.Dark;
			IsLightTheme = theme == ElementTheme.Light;
			IsSystemTheme = theme == ElementTheme.Default;
		}

		private void UpdateThemeSettings()
		{
			if (IsSystemTheme)
			{
				App.Settings.CurrentTheme = ElementTheme.Default;
			}
			else if (IsDarkTheme)
			{
				App.Settings.CurrentTheme = ElementTheme.Dark;
			}
			else if (IsLightTheme)
			{
				App.Settings.CurrentTheme = ElementTheme.Light;
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