﻿using Hipstr.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Users
{
	public sealed partial class UserProfileView : Page
	{
		public UserProfileViewModel ViewModel => (UserProfileViewModel)DataContext;

		public UserProfileView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<UserProfileViewModel>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.User = (User)e.Parameter;
		}
	}
}