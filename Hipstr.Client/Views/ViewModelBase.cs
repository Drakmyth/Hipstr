using Hipstr.Client.Properties;
using Hipstr.Core.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Hipstr.Client.Views
{
	public class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ElementTheme _currentTheme;

		public ElementTheme CurrentTheme
		{
			get { return _currentTheme; }
			private set
			{
				_currentTheme = value;
				OnPropertyChanged();
			}
		}

		protected ViewModelBase()
		{
			AppSettings.CurrentThemeChanged += OnCurrentThemeChange;
			CurrentTheme = AppSettings.CurrentTheme;
		}

		private void OnCurrentThemeChange(object sender, ElementTheme theme)
		{
			CurrentTheme = theme;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Dispose()
		{
			AppSettings.CurrentThemeChanged -= OnCurrentThemeChange;
		}
	}
}