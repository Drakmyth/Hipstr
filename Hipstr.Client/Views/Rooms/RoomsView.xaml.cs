﻿using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Rooms
{
	public sealed partial class RoomsView : Page
	{
		public RoomsViewModel ViewModel => (RoomsViewModel)DataContext;

		public RoomsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<RoomsViewModel>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.RoomGroupScrollToHeaderRequest += OnRoomGroupScrollToHeaderRequest;
			RoomList.SelectedItem = null;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.RoomGroupScrollToHeaderRequest -= OnRoomGroupScrollToHeaderRequest;
		}

		private void OnRoomGroupScrollToHeaderRequest(object sender, ObservableGroupedRoomsCollection observableGroupedCollection)
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
	}
}