using Hipstr.Client.Commands.ViewCommands;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }

		private FrameworkElement _frameContent;
		public FrameworkElement FrameContent
		{
			get
			{
				return _frameContent;
			}
			set
			{
				_frameContent = value;
				OnPropertyChanged(nameof(FrameContent));
			}
		}

		private string _title;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

		public MainPageViewModel()
		{
			NavigateToTeamsViewCommand = new NavigateToTeamsViewCommand();
			NavigateToRoomsViewCommand = new NavigateToRoomsViewCommand();
			NavigateToUsersViewCommand = new NavigateToUsersViewCommand();
			PropertyChanged += OnFrameContentChanged;
		}

		private void OnFrameContentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(FrameContent)) return;

			Page page = FrameContent as Page;
			ITitled titled = page?.DataContext as ITitled;
			if (titled != null)
			{
				Title = titled.Title;
			}
		}
	}
}