using Hipstr.Core.Models;
using Hipstr.Core.Models.Collections;
using Hipstr.Core.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
			await ViewModel.RefreshRoomsAsync(HipChatCacheBehavior.LoadFromCache);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.RoomGroupScrollToHeaderRequest -= OnRoomGroupScrollToHeaderRequest;
		}

		private void OnRoomGroupScrollToHeaderRequest(object sender, ObservableGroupedCollection<Room> observableGroupedCollection)
		{
			RoomList.ScrollIntoView(observableGroupedCollection, ScrollIntoViewAlignment.Leading);
		}

		private void HeaderTextBlock_OnTapped(object sender, TappedRoutedEventArgs e)
		{
			if (ViewModel.JumpToHeaderCommand.CanExecute(null))
			{
				ViewModel.JumpToHeaderCommand.Execute(null);
			}
		}

		private void OnRoomRightTapped(object sender, HoldingRoutedEventArgs e)
		{
			var listItem = (FrameworkElement)sender;
			ViewModel.TappedRoom = listItem.DataContext as Room;
			FlyoutBase.ShowAttachedFlyout(listItem);
		}
	}
}