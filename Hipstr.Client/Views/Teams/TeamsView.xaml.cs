using Hipstr.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class TeamsView : Page
	{
		public TeamsViewModel ViewModel => DataContext as TeamsViewModel;

		public TeamsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<TeamsViewModel>();
		}

		private void OnTeamRightTapped(object sender, HoldingRoutedEventArgs e)
		{
			var listItem = (FrameworkElement)sender;
			ViewModel.TappedTeam = listItem.DataContext as Team;
			FlyoutBase.ShowAttachedFlyout(listItem);
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.RefreshTeamListAsync();
		}
	}
}