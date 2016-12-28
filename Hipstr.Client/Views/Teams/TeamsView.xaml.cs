using Hipstr.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
			ViewModel.TappedTeam = ((FrameworkElement)e.OriginalSource).DataContext as Team;
			var listView = (ListView)sender;
			DeleteFlyout.ShowAt(listView, e.GetPosition(listView));
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.RefreshTeamListAsync();
		}
	}
}