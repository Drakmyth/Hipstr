using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class NewMainPageView : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public NewMainPageViewModel ViewModel => (NewMainPageViewModel)DataContext;

		public Frame Frame => AppFrame;

		private bool _commandBarIsOpen;

		private bool CommandBarIsOpen
		{
			get => _commandBarIsOpen;
			set
			{
				_commandBarIsOpen = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CommandButtonsAreCompact));
			}
		}

		private bool CommandButtonsAreCompact => !CommandBarIsOpen;

		public NewMainPageView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<NewMainPageViewModel>();
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}