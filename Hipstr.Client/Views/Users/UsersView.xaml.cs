using Hipstr.Core.Models;
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
			UserList.SelectedItem = null;
		}
	}
}