using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Linq;
using Windows.Devices.Input;
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
			if (e.PointerDeviceType != PointerDeviceType.Mouse)
			{
				ShowFlyout((FrameworkElement)sender);
			}
		}

		private void StackPanel_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			if (e.PointerDeviceType == PointerDeviceType.Mouse)
			{
				ShowFlyout((FrameworkElement)sender);
			}
		}

		private void ShowFlyout(FrameworkElement listItem)
		{
			ViewModel.TappedUser = (User)listItem.DataContext;
			FlyoutBase.ShowAttachedFlyout(listItem);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.UserGroupScrollToHeaderRequest += OnUserGroupScrollToHeaderRequest;
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