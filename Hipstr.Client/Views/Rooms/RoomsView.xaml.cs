using Hipstr.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Rooms
{
	public sealed partial class RoomsView : Page
	{
		public RoomsViewModel ViewModel => DataContext as RoomsViewModel;

		public RoomsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<RoomsViewModel>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.RoomGroupScrollToHeaderRequest += OnRoomGroupScrollToHeaderRequest;
			await ViewModel.LoadRoomsAsync();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.RoomGroupScrollToHeaderRequest -= OnRoomGroupScrollToHeaderRequest;
		}

		private void OnRoomGroupScrollToHeaderRequest(object sender, RoomGroup roomGroup)
		{
			RoomList.ScrollIntoView(roomGroup, ScrollIntoViewAlignment.Leading);
		}

		private void HeaderTextBlock_OnTapped(object sender, TappedRoutedEventArgs e)
		{
			if (ViewModel.JumpToHeaderCommand.CanExecute(null))
			{
				ViewModel.JumpToHeaderCommand.Execute(null);
			}
		}
	}
}