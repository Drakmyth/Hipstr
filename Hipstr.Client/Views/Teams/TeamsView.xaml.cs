using Hipstr.Core.Models;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class TeamsView : Page
	{
		public TeamsViewModel ViewModel => (TeamsViewModel)DataContext;

		public TeamsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<TeamsViewModel>();
		}

		private void Grid_OnHolding(object sender, HoldingRoutedEventArgs e)
		{
			if (e.PointerDeviceType != PointerDeviceType.Mouse)
			{
				ShowFlyout((FrameworkElement)sender);
			}
		}

		private void Grid_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			if (e.PointerDeviceType == PointerDeviceType.Mouse)
			{
				ShowFlyout((FrameworkElement)sender);
			}
		}

		private void ShowFlyout(FrameworkElement listItem)
		{
			ViewModel.TappedTeam = (Team)listItem.DataContext;
			FlyoutBase.ShowAttachedFlyout(listItem);
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.RefreshTeamListAsync();
		}
	}
}