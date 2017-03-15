using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Rooms
{
	public sealed partial class RoomsView : Page
	{
		public RoomsViewModel ViewModel => (RoomsViewModel)DataContext;

		public RoomsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<RoomsViewModel>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			RoomList.SelectedItem = null;
		}
	}
}