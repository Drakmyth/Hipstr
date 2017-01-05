using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Users
{
	public sealed partial class UsersView : Page
	{
		public UsersViewModel ViewModel => (UsersViewModel)DataContext;

		public UsersView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<UsersViewModel>();
		}

		private void StackPanel_OnHolding(object sender, HoldingRoutedEventArgs e)
		{
			var listItem = (FrameworkElement)sender;
			ViewModel.TappedUser = (User)listItem.DataContext;
			FlyoutBase.ShowAttachedFlyout(listItem);
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.UserGroupScrollToHeaderRequest += OnUserGroupScrollToHeaderRequest;
			await ViewModel.RefreshUsersAsync(HipChatCacheBehavior.LoadFromCache);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.UserGroupScrollToHeaderRequest -= OnUserGroupScrollToHeaderRequest;
		}

		private void OnUserGroupScrollToHeaderRequest(object sender, ObservableGroupedUsersCollection userGroup)
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