﻿using Hipstr.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Users
{
	public sealed partial class UsersView : Page
	{
		public UsersViewModel ViewModel => DataContext as UsersViewModel;

		public UsersView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<UsersViewModel>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.UserGroupScrollToHeaderRequest += OnUserGroupScrollToHeaderRequest;
			await ViewModel.LoadUsersAsync();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.UserGroupScrollToHeaderRequest -= OnUserGroupScrollToHeaderRequest;
		}

		private void OnUserGroupScrollToHeaderRequest(object sender, UserGroup userGroup)
		{
			UserList.ScrollIntoView(userGroup, ScrollIntoViewAlignment.Leading);
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