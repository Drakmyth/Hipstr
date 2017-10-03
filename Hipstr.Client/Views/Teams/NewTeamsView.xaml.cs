using Hipstr.Core.Models;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class NewTeamsView : Page
	{
		public NewTeamsViewModel ViewModel => (NewTeamsViewModel)DataContext;

		public NewTeamsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<NewTeamsViewModel>();
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

		public Visibility ShowEmptyCollectionMessage(int collectionCount)
		{
			return collectionCount > 0 ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}