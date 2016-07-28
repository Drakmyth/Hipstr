using Windows.UI.Xaml.Controls;

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
	}
}
