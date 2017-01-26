using Hipstr.Client.Properties;
using Hipstr.Client.Services;
using Hipstr.Core.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Hipstr.Client.Views
{
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
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

		private readonly string _title;
		private readonly IMainPageService _mainPageService;

		protected ViewModelBase(string title)
		{
			AppSettings.CurrentThemeChanged += OnCurrentThemeChange;
			CurrentTheme = AppSettings.CurrentTheme;

			_title = title;
			_mainPageService = IoCContainer.Resolve<IMainPageService>();
			RefreshTitle();
		}

		public void RefreshTitle()
		{
			_mainPageService.Title = _title;
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