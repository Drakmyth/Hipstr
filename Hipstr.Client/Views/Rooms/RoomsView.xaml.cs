using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Rooms
{
	public sealed partial class RoomsView : Page
	{
		public RoomsViewModel ViewModel => DataContext as RoomsViewModel;

		public RoomsView()
		{
			InitializeComponent();
			DataContext = new RoomsViewModel();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.UpdateRoomsAsync();
			ViewModel.UpdateFilters();
		}
	}
}
