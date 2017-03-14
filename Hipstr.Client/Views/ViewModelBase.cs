using Hipstr.Client.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

		public virtual void Initialize()
		{
			App.Settings.CurrentThemeChanged += OnCurrentThemeChange;
			CurrentTheme = App.Settings.CurrentTheme;
		}

		public virtual Task InitializeAsync()
		{
			return Task.CompletedTask;
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

		public virtual void Dispose()
		{
			App.Settings.CurrentThemeChanged -= OnCurrentThemeChange;
		}
	}
}